using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XNode;
using Random = UnityEngine.Random;

public class EventDirector : MonoBehaviour
{
    static readonly WaitForSeconds PauseProcessingWait = new WaitForSeconds(0.3f);
    static readonly WaitForSeconds DelayStartProcessing = new WaitForSeconds(0.12f);

    [Header("Event Setting")]
    [SerializeField]
    EventGraph getSalaryScenario;

    [Header("Event Emiiter Setting")]
    [SerializeField]
    int normalEventPerMonth = 3;

    [SerializeField]
    int optionalEventPerMonth = 2;

    [SerializeField]
    int normalEventAfterDay = 10;

    [SerializeField]
    int optionalEventAfterDay = 5;

    [SerializeField]
    int optinalEventDecayAfter = 3;

    [SerializeField]
    EventGraph[] normalScenarios;

    [SerializeField]
    EventGraph[] optionalScenarios;

    [Header("InnerTime")]
    [SerializeField]
    InnerTime innerTime;

    [Header("UI Setting")]
    [SerializeField]
    AlertBoxController alertbox;

    [SerializeField]
    UIInGameController ingameUI;

    [Header("Sound Setting")]
    [SerializeField]
    SoundManager soundManager;

    [Header("Dependencies")]
    [SerializeField]
    UIInGameStatusController ingameStatusUI;

    [Header("Debug")]
    [SerializeField]
    bool isInPlayground = false;

    public struct Cache
    {
        public StartNode startNode;
        public EndNode endNode;
        public MessageNode messageNode;
        public ResultNode resultNode;
        public LateResultNode lateResultNode;
        public RandomNode randomNode;
        public MultiRandomNode multiRandomNode;
        public PromptNode promptNode;
        public TimeSkipNode timeSkipNode;
        public ShopNode shopNode;
        public RelationshipNode relationshipNode;
        public ConditionNode conditionNode;
    }

    public Action<EventType> OnStartScenario;
    public Action<EventType> OnFinishScenario;

    public bool IsStartScenario => isStartScenario;

    bool isStartScenario;
    bool isStartOptionalScenario;

    bool shouldEndGame;

    bool previousPauseProcessing;
    bool shouldPauseProcessing;

    Cache cache;
    EventType currentEventType;

    EventGraph optionalScenario;
    EventGraph[] normalEvents;

    Dictionary<DateTime, Queue<DialogNode>> lateEventQueue; // <- key = day of year
    Dictionary<DateTime, EventGraph> keyEvents; // <- key = day of year

    DialogNode currentNode;

    EventGraph currentScenario;
    EventGraph currentLateScenario;

    Coroutine processNodeCoroutine;
    WaitUntil shouldWaitForUnpauseProcessing;

    // Event emiiter flag
    Queue<EventGraph> queueOfDay;
    Queue<DialogNode> queueOfLateEvent;

    int dayPass;
    int monthPass;

    int normalEventOffset = 1;
    int optionalEventOffset = 1;
    int optionalDecayExpectDay = 0;

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        SubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void Initialize()
    {
        cache = new Cache();
        lateEventQueue = new Dictionary<DateTime, Queue<DialogNode>>();
        shouldWaitForUnpauseProcessing = new WaitUntil(() => { return (!shouldPauseProcessing); });
        queueOfDay = new Queue<EventGraph>();
    }

    void SubscribeEvent()
    {
        GameController.Instance.OnGameStateChange += OnGameStateChange;
        alertbox.OnSelectButton += AlertBox_MessageSelectChoiceCallback;

        innerTime.OnDayPass += OnDayPass;
        innerTime.OnMonthPass += OnMonthPass;
        innerTime.OnYearPass += OnYearPass;

        innerTime.OnFinishAdvanceTime += OnFinishAdvanceTime;

    }

    void UnsubscribeEvent()
    {
        GameController.Instance.OnGameStateChange -= OnGameStateChange;
        alertbox.OnSelectButton -= AlertBox_MessageSelectChoiceCallback;

        innerTime.OnDayPass -= OnDayPass;
        innerTime.OnMonthPass -= OnMonthPass;
        innerTime.OnYearPass -= OnYearPass;

        innerTime.OnFinishAdvanceTime -= OnFinishAdvanceTime;
    }

    public void BeginPlay()
    {
        if (isInPlayground)
        {
            ingameUI.ShowIngameUI();
            StartScenario(normalScenarios[0]);
        }
        else
        {
            GameController.Instance?.BeginPlay();

            innerTime.StartClock();
            ingameUI.ShowIngameUI();

            soundManager.Play();

            // logic to start the event emiiter here
        }
    }

    void OnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.End:
                {
                    Debug.Log("Game Over...");
                }
                break;

            default:
                break;
        }
    }

    void AlertBox_MessageSelectChoiceCallback(int id)
    {
        var node = cache.messageNode;

        if (node == null)
        {
            Debug.LogError("Cannot find the last message node...");
            return;
        }

        bool haveCustomChoice = (node.choices.Length > 0);

        if (haveCustomChoice && (id >= node.choices.Length))
        {
            Debug.Log("Unknown choices...");
            return;
        }

        string portName = "output";

        if (haveCustomChoice)
        {
            portName = ("choices " + id);
        }

        alertbox.Show(false);

        currentNode = currentScenario.GetNextNode(currentNode, portName);
        SetPauseProcessingState(false);
    }

    void OnFinishAdvanceTime()
    {
        currentNode = currentScenario.GetNextNode(currentNode);
        SetPauseProcessingState(false);
    }

    // Hacks, Emmit event here
    void OnDayPass(DateTime date)
    {
        // Status every day
        GameController.Instance?.RemoveSalaryPerDay();
        GameController.Instance?.GainHappiness();

        // Emit event
        dayPass += 1;

        bool shouldStartLateEventHere = true;

        var nodes = new List<DialogNode>();

        // Late event
        foreach (var key in lateEventQueue.Keys)
        {
            if (date >= key) {
                queueOfLateEvent = lateEventQueue[key];
            }
            else {
                break;
            }
        }

        // Optional event
        bool shouldDecayOptionalEvent = (dayPass >= optionalDecayExpectDay);

        if (shouldDecayOptionalEvent) {
            optionalScenario = null;
            ingameStatusUI.NotifyNotification(false);
        }

        bool shouldEmitOptionalEvent = (optionalScenario == null) && (dayPass >= (optionalEventAfterDay * optionalEventOffset));

        if (shouldEmitOptionalEvent) {
            Random.InitState(Random.Range(0, 1000));

            int index = Random.Range(0, optionalScenarios.Length);
            optionalScenario = optionalScenarios[index];

            ingameStatusUI.NotifyNotification();
            optionalEventOffset += 1;

            optionalDecayExpectDay = (dayPass + optinalEventDecayAfter);
            shouldStartLateEventHere = false;
        }

        // Normal event
        bool shouldEmitNormalEvent = dayPass >= (normalEventAfterDay * normalEventOffset);

        if (shouldEmitNormalEvent) {
            Random.InitState(Random.Range(0, 1000));

            int index = Random.Range(0, normalScenarios.Length);
            var result = normalScenarios[index];

            shouldStartLateEventHere = false;

            normalEventOffset += 1;
            StartScenario(result);
        }

        if (shouldStartLateEventHere) {
            if (queueOfLateEvent != null && queueOfLateEvent.Count > 0) {
                var node = queueOfLateEvent.Dequeue();
                StartLateScenario(node);
            }
        }
    }

    void OnMonthPass(DateTime date)
    {
        monthPass += 1;
        dayPass = 0;

        normalEventOffset = 1;
        optionalEventOffset = 1;

        GameController.Instance?.GetSalary();
        StartScenario(getSalaryScenario);
    }

    void OnYearPass(DateTime date)
    {
        shouldEndGame = true;
    }

    void StartScenario(EventGraph scenario)
    {
        if (isStartScenario)
        {
            return;
        }

        currentScenario = scenario;
        currentNode = scenario.GetStartNode();

        if (currentNode == null)
        {
            Debug.LogError("Cannot find the scenario start node...");
            Debug.LogError("Scenario will not process...");
            return;
        }

        isStartScenario = true;
        OnStartScenario?.Invoke(currentEventType);

        innerTime.Pause();
        StartProcessNode();

        Debug.Log("Scenario has started..");
    }

    void StartLateScenario(DialogNode node)
    {
        if (isStartScenario)
        {
            return;
        }

        currentScenario = (EventGraph)node.graph;
        currentNode = node;

        if (currentNode == null)
        {
            Debug.LogError("Cannot find the scenario of late event...");
            Debug.LogError("Late event will not resume ...");
            return;
        }

        isStartScenario = true;
        OnStartScenario?.Invoke(currentEventType);

        innerTime.Pause();
        StartProcessNode();

        Debug.Log("Late Scenario has started..");
    }

    void EndScenario()
    {
        if (!isStartScenario)
        {
            return;
        }

        ResetScenario();
        OnFinishScenario?.Invoke(currentEventType);

        alertbox.Show(false);

        if (!isInPlayground)
        {
            innerTime.Pause(false);
        }

        Debug.Log("Scenario has finished..");

        // Hacks, optional scenario
        if (isStartOptionalScenario)
        {
            optionalScenario = null;
            isStartOptionalScenario = false;
        }

        // Hacks, recursive start late event until done
        if (queueOfLateEvent != null && queueOfLateEvent.Count > 0) {
            var node = queueOfLateEvent.Dequeue();
            StartLateScenario(node);
        }

        // Hacks, game over here
        if (shouldEndGame)
        {
            GameController.Instance?.GameOver();
            innerTime.ResetClock();
        }
    }

    void ResetScenario()
    {
        isStartScenario = false;
        currentNode = null;
        previousPauseProcessing = false;
        shouldPauseProcessing = false;
    }

    void StartProcessNode()
    {
        if (!isStartScenario)
        {
            Debug.LogError("Attempt to start processing node without starting scenario...");
            return;
        }

        if (processNodeCoroutine != null)
        {
            StopCoroutine(processNodeCoroutine);
        }

        processNodeCoroutine = StartCoroutine(ProcessNode());
    }

    IEnumerator ProcessNode()
    {
        while (isStartScenario)
        {
            bool noNodeLeft = (currentNode == null);

            if (noNodeLeft)
            {
                EndScenario();
                yield break;
            }

            switch (currentNode.DialogType)
            {
                case DialogNode.Dialog.Start:
                    {
                        cache.startNode = (currentNode as StartNode);
                        ProcessStartNode(cache.startNode);
                    }
                    break;

                case DialogNode.Dialog.Message:
                    {
                        if (!shouldPauseProcessing)
                        {
                            SetPauseProcessingState(true);
                        }

                        cache.messageNode = (currentNode as MessageNode);
                        ProcessMessageNode(cache.messageNode);
                    }
                    break;

                case DialogNode.Dialog.Prompt:
                    {
                        //TODO : skip for now
                        /* if (!shouldPauseProcessing) { */
                        /*     SetPauseProcessingState(true); */
                        /* } */

                        cache.promptNode = (currentNode as PromptNode);
                        ProcessPromptNode(cache.promptNode);
                    }
                    break;

                case DialogNode.Dialog.TimeSkip:
                    {
                        if (!shouldPauseProcessing)
                        {
                            SetPauseProcessingState(true);
                        }

                        cache.timeSkipNode = (currentNode as TimeSkipNode);
                        ProcessTimeSkipNode(cache.timeSkipNode);
                    }
                    break;

                case DialogNode.Dialog.Result:
                    {
                        cache.resultNode = (currentNode as ResultNode);
                        ProcessResultNode(cache.resultNode);
                    }
                    break;

                case DialogNode.Dialog.Random:
                    {
                        currentNode = currentScenario.GetNextNodeByRandom(currentNode);
                    }
                    break;

                case DialogNode.Dialog.MultiRandom:
                    {
                        currentNode = currentScenario.GetNextNodeByMultiRandom(currentNode);
                    }
                    break;

                case DialogNode.Dialog.LateResult:
                    {
                        // it need to add all the events that it point to, to the late event queue
                        // then : check the output port
                        // if there is no connection, end?

                        cache.lateResultNode = (currentNode as LateResultNode);
                        ProcessLateResultNode(cache.lateResultNode);
                    }
                    break;

                case DialogNode.Dialog.Shop:
                    {
                        cache.shopNode = (currentNode as ShopNode);
                        ProcessShopNode(cache.shopNode);
                    }
                    break;

                case DialogNode.Dialog.Relationship:
                    {
                        cache.relationshipNode = (currentNode as RelationshipNode);
                        ProcessRelationshipNode(cache.relationshipNode);
                    }
                    break;

                case DialogNode.Dialog.Conditional:
                    {
                        cache.conditionNode = (currentNode as ConditionNode);
                        ProcessConditionNode(cache.conditionNode);
                    }
                    break;

                case DialogNode.Dialog.End:
                    {
                        EndScenario();
                        yield break;
                    }
                    break;

                default:
                    break;
            }

            if (shouldPauseProcessing)
            {
                yield return shouldWaitForUnpauseProcessing;
                yield return DelayStartProcessing;
            }
            else
            {
                yield return null;
            }
        }
    }

    private void ProcessConditionNode(ConditionNode node)
    {
        var player = GameController.Instance.Player;
        bool conditiontrue = false;
        for (int i = 0; i < node.actions.Length; i++)
        {
            ConditionAction action = node.actions[i];
            bool currentCondition = false;
            switch (action.condition)
            {
                case ConditionAction.ConditionType.Health:
                    currentCondition = player.ProcessHealthCondition(action);
                    break;
                case ConditionAction.ConditionType.Money:
                    currentCondition = player.ProcessMoneyCondition(action);
                    break;
                case ConditionAction.ConditionType.BeggarRelationship:
                case ConditionAction.ConditionType.CatRelationship:
                case ConditionAction.ConditionType.SalesmanRelationship:
                case ConditionAction.ConditionType.GirlRelationship:
                    currentCondition = player.ProcessRelationshipCondition(action);
                    break;
                default:
                    break;
            }
            if (currentCondition == false)
            {
                break; // Not compatible with all condition
            }
            if (i == node.actions.Length - 1)
            {
                conditiontrue = currentCondition;
            }
        }
        currentNode = currentScenario.GetNextNodeCondition(currentNode, conditiontrue);
    }

    private void ProcessRelationshipNode(RelationshipNode node)
    {
        var player = GameController.Instance.Player;
        player.EditResource(node.actions);
        currentNode = currentScenario.GetNextNode(currentNode);
    }

    void SetPauseProcessingState(bool value)
    {
        previousPauseProcessing = shouldPauseProcessing;
        shouldPauseProcessing = value;
    }

    void ProcessStartNode(StartNode node)
    {
        currentEventType = node.eventType;
        currentNode = currentScenario.GetNextNode(node, "start");
    }

    void ProcessMessageNode(MessageNode node)
    {
        alertbox.SetMessageInfo(currentEventType, node.eventImage, node.message, node.choices, node.npc);
        alertbox.Show();
    }

    void ProcessResultNode(ResultNode node)
    {
        var player = GameController.Instance.Player;
        player.EditResource(node.actions);
        currentNode = currentScenario.GetNextNode(currentNode);
    }

    void ProcessTimeSkipNode(TimeSkipNode node)
    {
        var duration = node.duration;
        var amount = node.amount;

        TimeSpan timeSpan;

        switch (duration)
        {
            case TimeSkipNode.SkipDuration.Day:
                {
                    var date = innerTime.Calendar;
                    var nextDate = date.AddDays(amount);
                    timeSpan = (nextDate - innerTime.Calendar);
                }
                break;

            case TimeSkipNode.SkipDuration.Week:
                {
                    var date = innerTime.Calendar;
                    var nextDate = date.AddDays(amount * 7);
                    timeSpan = (nextDate - innerTime.Calendar);
                }
                break;

            case TimeSkipNode.SkipDuration.Month:
                {
                    var date = innerTime.Calendar;
                    var nextDate = date.AddMonths(amount);
                    timeSpan = (nextDate - innerTime.Calendar);
                }
                break;

            default:
                timeSpan = new TimeSpan(0, 0, 0, 0);
                break;
        }

        innerTime.AdvanceTime(timeSpan);
    }

    void ProcessLateResultNode(LateResultNode node)
    {
        var date = innerTime.Calendar;
        var queue = new Queue<DialogNode>();

        for (int i = 0; i < node.infos.Length; ++i)
        {
            var info = node.infos[i];
            var nodeToAdd = currentScenario.GetNextNode(currentNode, "infos " + i);

            Debug.Log("Duration : " + info.duration);
            Debug.Log("Amount : " + info.amount);

            if (nodeToAdd == null)
            {
                Debug.Log("no node to add to queue");
            }
            else
            {
                queue.Enqueue(nodeToAdd);
                Debug.Log("Node to add : " + nodeToAdd.ToString());
            }
        }

        if (queue.Count > 0) {
            if (lateEventQueue.ContainsKey(date)) {
                foreach (var item in queue)
                {
                    lateEventQueue[date].Enqueue(item);
                }
            }
            else {
                lateEventQueue.Add(date, queue);
            }
        }

        //then
        currentNode = currentScenario.GetNextNode(currentNode);
    }

    void ProcessShopNode(ShopNode node)
    {
        var player = GameController.Instance.Player;

        for (int i = 0; i < node.shopCarts.Length; ++i)
        {
            var item = node.shopCarts[i];

            if (ShopNode.ShopActionType.Buy == item.actionType)
            {
                player.BuyItem(item.item);
                Debug.Log($"Buy {item.item.itemName}");
            }
            else if (ShopNode.ShopActionType.Sell == item.actionType)
            {
                player.SellItem(item.item);
                Debug.Log($"Sell : {item.item.itemName}");
            }
            else
            {
                player.TakeItem(item.item);
                Debug.Log($"Give : {item.item.itemName}");
            }
        }

        //then
        currentNode = currentScenario.GetNextNode(currentNode);
    }

    // TODO
    void ProcessPromptNode(PromptNode node)
    {
        /* Skip */
        /* ------------------------------ */
        /* promptbox.SetPromptInfo() */
        /* promptbox.Show(); */

        /* var id = 0; */
        /* var portName = ("choices " + id); */

        /* if (node.choices.Length <= 0) { */
        /*     currentNode = null; */
        /*     return; */
        /* } */

        /* currentNode = currentScenario.GetNextNode(currentNode, portName); */
        /* ------------------------------ */

        Debug.LogError("You cannot use prompt node...");
        currentNode = currentScenario.GetNextNode(currentNode);
    }

    public void StartCurrentOptionalScenario()
    {
        if (optionalScenario == null)
            return;

        if (isStartScenario)
            return;

        if (isStartOptionalScenario)
            return;

        ingameStatusUI.NotifyNotification(false);

        isStartOptionalScenario = true;
        StartScenario(optionalScenario);
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class EventDirector : MonoBehaviour
{
    static readonly WaitForSeconds PauseProcessingWait = new WaitForSeconds(0.3f);
    static readonly WaitForSeconds DelayStartProcessing = new WaitForSeconds(0.12f);

    [Header("Game Event Setting")]
    [SerializeField]
    EventGraph[] normalScenarios;

    [Header("UI Setting")]
    [SerializeField]
    AlertBoxController alertbox;

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
    }

    public Action<EventType> OnStartScenario;
    public Action<EventType> OnFinishScenario;

    public bool IsStartScenario => isStartScenario;

    bool isStartScenario;
    bool previousPauseProcessing;
    bool shouldPauseProcessing;

    Cache cache;
    EventType currentEventType;

    /* Queue<EventGraph> eventQueue; */
    Dictionary<int, EventGraph> keyEvents; // <- key = day of year
    Dictionary<int, Queue<DialogNode>> lateEventQueue; // <- key = day of year

    DialogNode currentNode;
    EventGraph currentScenario;

    Coroutine processNodeCoroutine;
    WaitUntil shouldWaitForUnpauseProcessing;

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        SubscribeEvent();
        BeginPlay();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void Initialize()
    {
        cache = new Cache();
        lateEventQueue = new Dictionary<int, Queue<DialogNode>>();
        shouldWaitForUnpauseProcessing = new WaitUntil(() => { return (!shouldPauseProcessing); });
    }

    void SubscribeEvent()
    {
        GameController.Instance.OnGameStateChange += OnGameStateChange;
        alertbox.OnSelectButton += AlertBox_MessageSelectChoiceCallback;
    }

    void UnsubscribeEvent()
    {
        GameController.Instance.OnGameStateChange -= OnGameStateChange;
        alertbox.OnSelectButton -= AlertBox_MessageSelectChoiceCallback;
    }

    void BeginPlay()
    {
        /* GameController.Instance?.ShowProfile(); */
        //Test
        GameController.Instance?.BeginPlay();
    }

    void OnGameStateChange(GameState state)
    {
        // Start event as the begin
        if (GameState.Normal == state)
        {
            // Begin Start Scenario
            // TODO : start game event emiiter here
            // Test, start first scenario
            currentEventType = EventType.Normal;
            StartScenario(normalScenarios[0]);
        }
    }

    void AlertBox_MessageSelectChoiceCallback(int id)
    {
        var node = cache.messageNode;

        if (node == null) {
            Debug.LogError("Cannot find the last message node...");
            return;
        }

        bool haveCustomChoice = (node.choices.Length > 0);

        if (haveCustomChoice && (id >= node.choices.Length)) {
            Debug.Log("Unknown choices...");
            return;
        }

        string portName = "output";

        if (haveCustomChoice) {
            portName = ("choices " + id);
        }

        alertbox.Show(false);

        currentNode = currentScenario.GetNextNode(currentNode, portName);
        SetPauseProcessingState(false);
    }

    void StartScenario(EventGraph scenario)
    {
        if (isStartScenario) {
            return;
        }

        currentScenario = scenario;
        currentNode = scenario.GetStartNode();

        if (currentNode == null) {
            Debug.LogError("Cannot find the scenario start node...");
            Debug.LogError("Scenario will not process...");
            return;
        }

        isStartScenario = true;
        StartProcessNode();

        Debug.Log("Scenario has started..");
    }

    void EndScenario()
    {
        if (!isStartScenario) {
            return;
        }

        ResetScenario();
        OnFinishScenario?.Invoke(currentEventType);

        alertbox.Show(false);
        Debug.Log("Scenario has finished..");
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
        if (!isStartScenario) {
            Debug.LogError("Attempt to start processing node without starting scenario...");
            return;
        }

        if (processNodeCoroutine != null) {
            StopCoroutine(processNodeCoroutine);
        }

        processNodeCoroutine = StartCoroutine(ProcessNode());
    }

    IEnumerator ProcessNode()
    {
        while (isStartScenario)
        {
            bool noNodeLeft = (currentNode == null);

            if (noNodeLeft) {
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
                    if (!shouldPauseProcessing) {
                        SetPauseProcessingState(true);
                    }

                    cache.messageNode = (currentNode as MessageNode);
                    ProcessMessageNode(cache.messageNode);
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
                    Debug.Log("At the random node.");
                    currentNode = currentScenario.GetNextNodeByRandom(currentNode);
                }
                break;

                case DialogNode.Dialog.TimeSkip:
                {
                    cache.timeSkipNode = (currentNode as TimeSkipNode);

                    // Skip the game time,

                    // then
                     currentNode = currentScenario.GetNextNode(currentNode);
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
                    currentNode = currentScenario.GetNextNode(currentNode);
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

            if (shouldPauseProcessing) {
                yield return shouldWaitForUnpauseProcessing;
                yield return DelayStartProcessing;
            }
            else {
                yield return null;
            }
        }
    }

    void SetPauseProcessingState(bool value)
    {
        previousPauseProcessing = shouldPauseProcessing;
        shouldPauseProcessing = value;
    }

    void ProcessStartNode(StartNode node)
    {
        currentNode = currentScenario.GetNextNode(node, "start");
    }

    void ProcessMessageNode(MessageNode node)
    {
        // raise flag to pause the in-game time (not the engine time)
        // here

        alertbox.SetMessageInfo(node.message, node.choices, node.npc);
        alertbox.Show();
    }

    void ProcessResultNode(ResultNode node)
    {
        var player = GameController.Instance.Player;
        player.EditResource(node.actions);
        currentNode = currentScenario.GetNextNode(currentNode);
    }
}


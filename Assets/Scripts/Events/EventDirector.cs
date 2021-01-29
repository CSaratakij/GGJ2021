using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class EventDirector : MonoBehaviour
{
    static readonly WaitForSeconds PauseProcessingWait = new WaitForSeconds(0.3f);

    [Header("Game Event Setting")]
    [SerializeField]
    EventGraph[] normalScenarios;

    // TODO (Hook alertbox click button id callback)
    /* [Header("UI Setting")] */
    /* [SerializeField] */
    /* Transform alertbox */

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

    bool isStartScenario;
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
        SubscribeEvent();
    }

    void Start()
    {
        // Test : start event here
        currentEventType = EventType.Normal;
        StartScenario(normalScenarios[0]);
    }

    void Update()
    {
        // Test
        if (Input.GetKeyDown(KeyCode.F)) {
            AlertBox_MessageSelectChoiceCallback(0);
        }
        else if (Input.GetKeyDown(KeyCode.G)) {
            AlertBox_MessageSelectChoiceCallback(1);
        }
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
        // TODO : subscribe press callback from alert box
    }

    void UnsubscribeEvent()
    {

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

        Debug.Log($"You select : {id}");

        //advance to next node here..
        string portName = "output";

        if (haveCustomChoice) {
            portName = ("choices " + id);
        }

        currentNode = currentScenario.GetNextNode(currentNode, portName);
        shouldPauseProcessing = false;
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

        Debug.Log("Scenario has finished..");
    }

    void ResetScenario()
    {
        isStartScenario = false;
        currentNode = null;
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
                        shouldPauseProcessing = true;
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

                case DialogNode.Dialog.TimeSkip:
                {
                    cache.timeSkipNode = (currentNode as TimeSkipNode);

                    // Skip the game time,

                    // then
                     currentNode = currentScenario.GetNextNode(currentNode);
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
            }
            else {
                yield return null;
            }
        }
    }

    void ProcessStartNode(StartNode node)
    {
        currentNode = currentScenario.GetNextNode(node, "start");
    }

    void ProcessMessageNode(MessageNode node)
    {
        // TODO : if npc is not null, show npc name and set its image to alertbox
        Debug.Log("Message Node:");
        Debug.Log($"message : {node.message}");
        Debug.Log($"npc : {node.npc}");

        if (node.choices.Length <= 0) {
            Debug.Log("NO choise, will select next node on output port");
        }
        else {
            foreach (var name in node.choices) {
                Debug.Log($"Choice : {name}");
            }
        }

        // TODO
        // raise flag to pause the in-game time (not the engine time)
        // raise alertbox
    }

    void ProcessResultNode(ResultNode node)
    {
        //Adjst share resource here
        //------------------------------

        //then
        currentNode = currentScenario.GetNextNode(currentNode);
    }
}


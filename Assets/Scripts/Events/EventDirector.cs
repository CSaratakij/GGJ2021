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

    public Action<EventType> OnStartScenario;
    public Action<EventType> OnFinishScenario;

    bool isStartScenario;
    bool shouldPauseProcessing;

    EventType currentEventType;

    /* Queue<EventGraph> eventQueue; */
    Dictionary<int, EventGraph> keyEvents; // <- key = day of year
    Dictionary<int, Queue<DialogNode>> lateEventQueue; // <- key = day of year

    DialogNode currentNode;
    EventGraph currentScenario;

    Coroutine processNodeCoroutine;

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        // Test : start event here
        currentEventType = EventType.Normal;
        StartScenario(normalScenarios[0]);
    }

    void Initialize()
    {
        lateEventQueue = new Dictionary<int, Queue<DialogNode>>();
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
    }

    void EndScenario()
    {
        if (!isStartScenario) {
            return;
        }

        ResetScenario();
        OnFinishScenario?.Invoke(currentEventType);
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
            if (shouldPauseProcessing) {
                yield return PauseProcessingWait;
            }

            bool noNodeLeft = (currentNode == null);

            if (noNodeLeft) {
                EndScenario();
                yield break;
            }

            switch (currentNode.DialogType)
            {
                case DialogNode.Dialog.Start:
                {
                     currentNode = currentScenario.GetNextNode(currentNode);
                }
                break;
                
                case DialogNode.Dialog.End:
                {
                    EndScenario();
                    yield break;
                }
                break;

                case DialogNode.Dialog.Message:
                {
                    if (!shouldPauseProcessing) {
                        shouldPauseProcessing = true;
                    }

                    /* Debug.Log("Reach here.."); */

                    // get message box info
                    // and show to alert properly
                    // raise flag via alertbox callback to get which button player choose
                    // raise flag and pause the game time (not engine game time)
                    //------------------------------
                    // next node before pause false

                    /* var messageNode = currentNode as MessageNode; */
                    /* ProcessMessageNode(messageNode); */
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

                case DialogNode.Dialog.Result:
                {
                    //Call for share resource here
                    //------------------------------
                    /* var resultNode = currentNode as ResultNode; */
                    /* ProcessResultNode(resultNode); */
                    //then
                    currentNode = currentScenario.GetNextNode(currentNode);
                }
                break;
                
                /* case DialogNode.Dialog.LateResult: */
                /* { */
                /*     //TODO */
                /*     // it need to add all the events that it point to, to the late event queue */
                /*     // then end? */
                /* } */
                /* break; */
                
                default:
                break;
            }

            yield return null;
        }
    }
}


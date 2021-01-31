using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "E_NewEvent")]
public class EventGraph : NodeGraph
{
    public EventType eventType;

    public DialogNode GetStartNode()
    {
        foreach (DialogNode node in nodes)
        {
            if (DialogNode.Dialog.Start == node.DialogType)
                return node;
        }

        return null;
    }

    public DialogNode GetNextNodeCondition(DialogNode currentNode, bool conditiontrue)
    {
        string outputPortName = conditiontrue ? "resulttrue" : "resultfalse";
        if (currentNode == null)
        {
            return null;
        }

        NodePort port = currentNode.GetPort(outputPortName);

        if (port != null)
        {
            NodePort otherPort = port.Connection;

            if (otherPort != null)
            {
                return (otherPort.node as DialogNode);
            }
        }

        return null;
    }

    public DialogNode GetNextNode(DialogNode currentNode, string outputPortName = "output")
    {
        if (currentNode == null)
        {
            return null;
        }

        NodePort port = currentNode.GetPort(outputPortName);

        if (port != null)
        {
            NodePort otherPort = port.Connection;

            if (otherPort != null)
            {
                return (otherPort.node as DialogNode);
            }
        }

        return null;
    }

    public DialogNode GetNextNodeByRandom(DialogNode currentNode, string outputPortName = "output")
    {
        if (currentNode == null)
        {
            return null;
        }

        Random.InitState(Random.Range(0, 1000));

        var otherPorts = currentNode.GetOutputPort(outputPortName).GetConnections();

        if (otherPorts.Count <= 0)
        {
            return null;
        }

        var i = Random.Range(0, otherPorts.Count);
        return (otherPorts[i].node as DialogNode);
    }

    public DialogNode GetNextNodeByMultiRandom(DialogNode currentNode)
    {
        if (currentNode == null)
        {
            return null;
        }

        var node = (currentNode as MultiRandomNode);

        if (node.chances.Length <= 0)
        {
            return null;
        }

        var chanceTable = new Dictionary<int, NodePort>();

        int weightSum = 0;
        int offset = 10;

        // make chance unique
        for (int i = 0; i < node.chances.Length; ++i)
        {
            int amount = node.chances[i];
            NodePort port = node.GetPort("chances " + i);

            if (chanceTable.ContainsKey(amount))
            {
                amount += offset;
                offset += 10;
            }

            weightSum += amount;
            chanceTable.Add(amount, port);
        }

        // rand result
        Random.InitState(Random.Range(0, 1000));
        int randResult = Random.Range(0, weightSum);

        // check which port to return
        int cumulativeWeight = 0;
        NodePort selectPort = null;

        foreach (var pair in chanceTable)
        {
            cumulativeWeight += pair.Key;

            if (randResult < cumulativeWeight)
            {
                selectPort = pair.Value;
                break;
            }
        }

        // get node from select port here
        if (selectPort != null)
        {
            NodePort otherPort = selectPort.Connection;

            if (otherPort != null)
            {
                return (otherPort.node as DialogNode);
            }
        }

        return null;
    }
}


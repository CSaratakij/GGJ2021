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
        foreach (DialogNode node in nodes) {
            if (DialogNode.Dialog.Start == node.DialogType)
                return node;
        }

        return null;
    }

    public DialogNode GetNextNode(DialogNode currentNode, string outputPortName = "output")
    {
        if (currentNode == null) {
            return null;
        }

        NodePort port = currentNode.GetPort(outputPortName);

        if (port != null) {
            NodePort otherPort = port.Connection;

            if (otherPort != null) {
                return (otherPort.node as DialogNode);
            }
        }

        return null;
    }

    public DialogNode GetNextNodeByRandom(DialogNode currentNode, string outputPortName = "output")
    {
        if (currentNode == null) {
            return null;
        }

        Random.InitState(Random.Range(0, 1000));

        var otherPorts = currentNode.GetOutputPort(outputPortName).GetConnections();

        if (otherPorts.Count <= 0) {
            return null;
        }

        var i = Random.Range(0, otherPorts.Count);
        Debug.Log("All posssible resut:" + otherPorts.Count);
        return (otherPorts[i].node as DialogNode);
    }

    // TODO : don't forget to sort the distribution
    public DialogNode GetNextNodeByMultiRandom(DialogNode currentNode)
    {
        if (currentNode == null) {
            return null;
        }

        if (DialogNode.Dialog.MultiRandom != currentNode.DialogType) {
            return null;
        }

        // get all choices
        var node = currentNode as MultiRandomNode;

        if (node.chances.Length <= 0) {
            return null;
        }

        // TODO : get all chances sort, random with distribution and return node
        return null;
    }
}


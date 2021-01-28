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

    public DialogNode GetNextNode(DialogNode currentNode)
    {
        switch (currentNode.DialogType)
        {
            case DialogNode.Dialog.Start:
            {
                var otherPort = currentNode.GetOutputPort("start").Connection;
                if (otherPort != null) {
                    return (otherPort.node as DialogNode);
                }
            }
            break;
        }

        return null;
    }

    // TODO
    public DialogNode GetNextNodeByRandom(DialogNode currentNode)
    {
        return null;
    }

    // TODO : don't forget to sort the distribution
    public DialogNode GetNextNodeByMultiRandom(DialogNode currentNode)
    {
        return null;
    }
}


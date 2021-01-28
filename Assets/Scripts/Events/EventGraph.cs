using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "E_NewEvent")]
public class EventGraph : NodeGraph
{ 
    DialogNode root;

    public DialogNode GetRoot()
    {
        foreach (DialogNode node in nodes) {
            if (DialogNode.Dialog.Start == node.DialogType)
                return node;
        }

        return null;
    }

    public DialogNode GetNextNode(DialogNode currentNode)
    {
        //TODO
        return null;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class MathGraph : NodeGraph
{
    public MathNode GetRoot()
    {
        if (nodes.Count <= 0) {
            return null;
        }

        return nodes[0] as MathNode;
    }

    public float GetRootValue()
    {
        var root = nodes[0] as MathNode;
        return (float)root.GetOutputPort("result").GetOutputValue();
    }
}


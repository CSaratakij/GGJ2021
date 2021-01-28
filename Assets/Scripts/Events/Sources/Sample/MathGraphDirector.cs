using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sample
public class MathGraphDirector : MonoBehaviour
{
    [SerializeField]
    MathGraph graph;

    MathNode root;
    MathNode currentNode;

    void Awake()
    {
        root = graph.GetRoot();
        currentNode = root;
    }

    void Start()
    {
        var a = (float)root.GetOutputPort("sum").GetOutputValue();
        Debug.Log("A" + a);
        foreach (MathNode node in graph.nodes) {
            var result = (float)node.GetOutputPort("result").GetOutputValue();
            Debug.Log($"Result : {result}");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            var otherPort = root.GetOutputPort("result").Connection;

            if (otherPort != null) {
                currentNode = otherPort.node as MathNode;
                float result = (float)currentNode.GetInputPort("a").GetInputValue();
                Debug.Log("Other Node input a is : "+ result);
            }
        }
    }
}

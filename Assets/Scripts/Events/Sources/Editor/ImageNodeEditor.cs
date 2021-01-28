using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(ImageNode))]
public class ImageNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        base.OnBodyGUI();

        ImageNode node = target as ImageNode;

        if (node.image != null) {
            EditorGUILayout.LabelField(new GUIContent(node.image.texture));
        }
    }
}


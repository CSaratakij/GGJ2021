using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeWidth(304)]
public class EventNode : DialogNode
{
    [Input] public Sprite npcImage;
    [TextArea]public string message;
    [Output(dynamicPortList=true)] public string[] choices; 

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.Event;
	}

	public override object GetValue(NodePort port)
    {
		return null;
	}
}

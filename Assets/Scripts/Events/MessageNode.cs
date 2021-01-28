using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeWidth(304)]
public class MessageNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;
    public Sprite image;
    [TextArea(4, 4)]public string message;
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeWidth(250)]
public class PromptNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(dynamicPortList=true)] public ItemScriptableObject[] choices; 

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.Prompt;
	}

	public override object GetValue(NodePort port)
    {
		return null;
	}
}


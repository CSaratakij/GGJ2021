using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeWidth(250)]
public class PromptNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    public PromptType promptType;
    [Output(dynamicPortList=true)] public ItemScriptableObject[] choices; 

    public enum PromptType
    {
        BuyItem,
        SellItem
    }

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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ResultNode : DialogNode 
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;
    public ResultAction[] actions;

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.Result;
	}

	public override object GetValue(NodePort port)
    {
        return null;
	}
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ResultNode : DialogNode 
{
    public ResultAction[] actions;

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.Result;
	}

	public override object GetValue(NodePort port)
    {
		return null; // Replace this
	}
}


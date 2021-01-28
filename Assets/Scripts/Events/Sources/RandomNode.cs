using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class RandomNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;
    public int chances = 50;

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.Random;
	}

	public override object GetValue(NodePort port)
    {
		return null;
	}
}


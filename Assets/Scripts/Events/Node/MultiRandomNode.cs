using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class MultiRandomNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(dynamicPortList=true)] public int[] chances; 

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.MultiRandom;
	}

	public override object GetValue(NodePort port)
    {
		return null; // Replace this
	}
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeWidth(250)]
public class LateResultNode : DialogNode
{
    public enum LateDuration
    {
        Day,
        Week,
        Month
    }

    [System.Serializable]
    public class LateInfo
    {
        public LateDuration duration;
        public int amount;
    }

    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;
    [Output(dynamicPortList=true)] public LateInfo[] infos;

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.LateResult;
	}

	public override object GetValue(NodePort port)
    {
		return null; // Replace this
	}
}

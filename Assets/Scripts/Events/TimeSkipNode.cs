using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class TimeSkipNode : DialogNode
{
    public enum SkipDuration
    {
        Day,
        Week,
        Month
    }

    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;

    public SkipDuration duration;
    public int amount = 1;

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.TimeSkip;
	}

	public override object GetValue(NodePort port)
    {
		return null; // Replace this
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class StartNode : DialogNode
{
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode start;

	protected override void Init() {
		base.Init();
        DialogType = Dialog.Start;
	}

	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}


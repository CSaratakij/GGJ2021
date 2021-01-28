using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class EndNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode end;

	protected override void Init() {
		base.Init();
        DialogType = Dialog.End;
	}

	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}


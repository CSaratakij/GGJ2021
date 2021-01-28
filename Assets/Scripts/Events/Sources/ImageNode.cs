using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XNode;

[NodeWidth(304)]
public class ImageNode : Node
{
    [Output] public Sprite image = null;

	protected override void Init() {
		base.Init();
	}

	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}


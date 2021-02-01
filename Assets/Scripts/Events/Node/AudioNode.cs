using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class AudioNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;
    public AudioType audioType;
    public AudioClip sound;

    public enum AudioType
    {
        SFX,
        BGM
    }

	// Use this for initialization
	protected override void Init()
    {
		base.Init();
        DialogType = DialogNode.Dialog.Audio;
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
    {
		return null; // Replace this
	}
}

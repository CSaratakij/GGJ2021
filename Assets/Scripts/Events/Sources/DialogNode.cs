using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class DialogNode : Node
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;

    public Dialog DialogType { get; protected set; }

    public enum Dialog
    {
        Event,
        Result
    }
}


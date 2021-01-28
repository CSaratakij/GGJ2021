using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class DialogNode : Node
{
    public Dialog DialogType { get; protected set; }

    public enum Dialog
    {
        Start,
        Message,
        Result,
        LateResult,
        Random,
        MultiRandom,
        Math,
        End,
        Prompt
    }
}


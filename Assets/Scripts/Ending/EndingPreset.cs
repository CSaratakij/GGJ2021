using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "EndingPreset", menuName = "Profile/Ending")]
public class EndingPreset : ScriptableObject
{
    public int priority;
    public string endingName;
    public ConditionAction[] condition;
    public Sprite sprite;
    [TextArea(5, 5)] public string description;
}



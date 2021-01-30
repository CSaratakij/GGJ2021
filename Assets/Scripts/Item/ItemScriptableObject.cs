using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemProfile", menuName = "Profile/Item", order = 1)]
public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    [TextArea(5, 5)] public string remark;
    public Sprite sprite;
    public bool isRequired;
    public bool effectActive;
    public bool hiddenEffectFromPlayer;
    public int degenerate;
    public int cost;
}


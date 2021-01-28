using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemProfile", menuName = "Profile/Item", order = 1)]
public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public int cost;
    public Sprite sprite;
}


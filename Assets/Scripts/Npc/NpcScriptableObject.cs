using UnityEngine;

[CreateAssetMenu(fileName = "NpcProfile", menuName = "Profile/Npc", order = 1)]
public class NpcScriptableObject : ScriptableObject
{
    public string npcName;
    public Sprite sprite;
}


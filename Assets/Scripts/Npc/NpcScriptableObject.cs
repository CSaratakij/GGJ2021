using UnityEngine;

[CreateAssetMenu(fileName = "NpcProfile", menuName = "Profile/Npc", order = 1)]
public class NpcScriptableObject : ScriptableObject
{
    public string npcName;
    [TextArea(5, 5)] public string remark;
    public Sprite sprite;
}


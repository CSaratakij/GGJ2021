using UnityEngine;

[CreateAssetMenu(fileName="PlayerProfilePreset", menuName = "Profile/Player")]
public class PlayerProfilePreset : ScriptableObject
{
    public int happiness;
    public int money;
    public int salary;
    public bool haveCat;
    public bool haveGirlFriend;
    [TextArea(5, 5)] public string remark;
}


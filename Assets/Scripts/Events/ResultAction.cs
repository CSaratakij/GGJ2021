using System;

[System.Serializable]
public class ResultAction
{
    public enum StatusType
    {
        Money,
        Happiness
    }

    public enum ActionType
    {
        Add,
        Remove,
        Set,
        LateAdd,
        LateRemove
    }

    public StatusType status;
    public ActionType action;
    public int amount;
}


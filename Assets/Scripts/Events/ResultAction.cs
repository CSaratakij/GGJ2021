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
        Set
    }

    public StatusType status;
    public ActionType action;
    public int amount;
}


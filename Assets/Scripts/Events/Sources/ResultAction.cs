using System;

[System.Serializable]
public class ResultAction
{
    public enum StatusType
    {
        Happiness,
        Money
    }

    public enum ActionType
    {
        Add,
        Remove,
        Random
    }

    public StatusType status;
    public ActionType action;
    public int amount;
}


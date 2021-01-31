using System;

[System.Serializable]
public class ResultAction
{
    public enum StatusType
    {
        Money,
        Happiness,
        Salary
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

[System.Serializable]
public class RelationshipAction
{
    public enum CharacterType
    {
        Cat,
        Girlfriend,
        Beggar,
        Salesman
    }
    public enum ActionType
    {
        //Add,
        //Remove,
        Set,
        Unlocked,
        Locked
    }
    public CharacterType character;
    public ActionType action;
    public int amount;
}

[System.Serializable]
public class ConditionAction
{
    public enum ConditionType
    {
        Health,
        Money,
        //Salary,
        //ItemActive,
        CatRelationship,
        GirlRelationship,
        BeggarRelationship,
        SalesmanRelationship
    }
    public enum CompareMethod
    {
        GreaterThan,
        GreaterThanOrEquals,
        Equals,
        LessThan,
        LessThanOrEquals
    }

    public ConditionType condition;
    public CompareMethod compare;
    public int amount;
}
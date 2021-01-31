using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class NpcProfile
{
    public bool isUnlocked;
    public int relationship;
    public NpcProfile()
    {

    }

    public NpcProfile(bool isUnlocked)
    {
        this.isUnlocked = isUnlocked;
        if (isUnlocked)
        {
            this.relationship = 2;
        }
    }

    public NpcProfile(bool isUnlocked, int relationship)
    {
        this.isUnlocked = isUnlocked;
        this.relationship = relationship;
    }
}

[Serializable]
public class PlayerProfile
{
    public int happiness;
    public int money;
    public int salary;
    public NpcProfile cat = new NpcProfile();
    public NpcProfile girl = new NpcProfile();
    public NpcProfile beggar = new NpcProfile(true);
    public NpcProfile salesman = new NpcProfile(true);
    public string remark;

    public List<ItemScriptableObject> items;

    public PlayerProfile()
    {
        items = new List<ItemScriptableObject>();
    }

    public void BuyItem(ItemScriptableObject item)
    {
        GameController.Instance.Player.RemoveMoney(item.cost);
        items.Add(item);
    }

    public void SellItem(ItemScriptableObject item)
    {
        GameController.Instance.Player.AddMoney(item.cost);
        items.Remove(item);
    }

    public void TakeItem(ItemScriptableObject item)
    {
        items.Add(item);
    }

    public bool HasItem(ItemScriptableObject item)
    {
        foreach (var collection in items)
        {
            bool result = item.itemName.Equals(collection.itemName);

            if (result) {
                return result;
            }
        }

        return false;
    }

    public void AddItem(ItemScriptableObject item)
    {
        items.Add(item);
    }

    public void EditResource(ResultAction[] actions)
    {
        foreach (ResultAction action in actions)
        {
            var statusType = action.status;
            var actionType = action.action;
            var amount = action.amount;

            switch (action.action)
            {
                case ResultAction.ActionType.Add:
                    switch (action.status)
                    {
                        case ResultAction.StatusType.Money:
                            AddMoney(amount);
                            break;

                        case ResultAction.StatusType.Happiness:
                            AddHappiness(amount);
                            break;

                        case ResultAction.StatusType.Salary:
                            AddSalary(amount);
                            break;

                        default:
                            break;
                    }
                    break;

                case ResultAction.ActionType.Remove:
                    switch (action.status)
                    {
                        case ResultAction.StatusType.Money:
                            RemoveMoney(amount);
                            break;

                        case ResultAction.StatusType.Happiness:
                            RemoveHappiness(amount);
                            break;

                        case ResultAction.StatusType.Salary:
                            RemoveSalary(amount);
                            break;

                        default:
                            break;
                    }
                    break;

                case ResultAction.ActionType.Set:
                    switch (action.status)
                    {
                        case ResultAction.StatusType.Money:
                            SetMoney(amount);
                            break;

                        case ResultAction.StatusType.Happiness:
                            SetHappiness(amount);
                            break;

                        case ResultAction.StatusType.Salary:
                            SetSalary(amount);
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

        }
    }

    private void SetSalary(int amount)
    {
        salary = amount;
    }

    private void RemoveSalary(int amount)
    {
        if (salary - amount >= 0)
        {
            salary -= amount;
        }
        else
        {
            salary = 0;
        }
    }

    private void AddSalary(int amount)
    {
        salary += amount;
    }

    public void EditResource(RelationshipAction[] actions)
    {
        foreach (RelationshipAction action in actions)
        {
            switch (action.action)
            {
                //case RelationshipAction.ActionType.Add:
                //    AddRelationship(action.character, action.amount);
                //    break;
                //case RelationshipAction.ActionType.Remove:
                //    RemoveRelationship(action.character, action.amount);
                //    break;
                case RelationshipAction.ActionType.Set:
                    SetRelationship(action.character, action.amount);
                    break;
                case RelationshipAction.ActionType.Unlocked:
                    UnlockedRelationship(action.character);
                    break;
                case RelationshipAction.ActionType.Locked:
                    LockedRelationship(action.character);
                    break;
            }
        }
    }

    private void LockedRelationship(RelationshipAction.CharacterType character)
    {
        switch (character)
        {
            case RelationshipAction.CharacterType.Girlfriend:
                girl = new NpcProfile();
                break;
            default:
                break;
        }
    }

    private void UnlockedRelationship(RelationshipAction.CharacterType character)
    {
        switch (character)
        {
            case RelationshipAction.CharacterType.Cat:
                cat = new NpcProfile(true);
                break;
            case RelationshipAction.CharacterType.Girlfriend:
                girl = new NpcProfile(true);
                break;
            default:
                break;
        }
    }

    private void SetRelationship(RelationshipAction.CharacterType character, int amount)
    {
        if (amount < 0) return;
        if (amount > 5) return;
        switch (character)
        {
            case RelationshipAction.CharacterType.Beggar:
                beggar.relationship = amount;
                break;
            case RelationshipAction.CharacterType.Cat:
                cat.relationship = amount;
                break;
            case RelationshipAction.CharacterType.Girlfriend:
                girl.relationship = amount;
                break;
            case RelationshipAction.CharacterType.Salesman:
                salesman.relationship = amount;
                break;
            default:
                break;
        }
    }

    private void RemoveRelationship(RelationshipAction.CharacterType character, int amount)
    {
        switch (character)
        {
            case RelationshipAction.CharacterType.Beggar:
                break;
            case RelationshipAction.CharacterType.Cat:
                break;
            case RelationshipAction.CharacterType.Girlfriend:
                break;
            case RelationshipAction.CharacterType.Salesman:
                break;
        }
    }

    private void AddRelationship(RelationshipAction.CharacterType character, int amount)
    {
        switch (character)
        {
            case RelationshipAction.CharacterType.Beggar:
                break;
            case RelationshipAction.CharacterType.Cat:
                break;
            case RelationshipAction.CharacterType.Girlfriend:
                break;
            case RelationshipAction.CharacterType.Salesman:
                break;
        }
    }

    public void SetMoney(int amount)
    {
        money = amount;
    }

    public void SetHappiness(int amount)
    {
        happiness = amount;
    }

    public void AddHappiness(int amount)
    {
        if (happiness + amount <= 100)
        {
            happiness += amount;
        }
        else
        {
            happiness = 100;
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void RemoveHappiness(int amount)
    {
        /* var result = (happiness - amount) < 0 ? 0 : (happiness - amount); */
        /* happiness = result; */
        if (happiness - amount > 0)
        {
            happiness -= amount;
        }
        else
        {
            happiness = 0;
        }
    }

    public void RemoveMoney(int amount)
    {
        /* var result = (money - amount) < 0 ? 0 : (money - amount); */
        /* money = result; */
        money -= amount;
    }

    bool CompareUtility(ConditionAction action, int source)
    {
        switch (action.compare)
        {
            case ConditionAction.CompareMethod.Equals:
                return source == action.amount;
            case ConditionAction.CompareMethod.GreaterThan:
                return source > action.amount;
            case ConditionAction.CompareMethod.GreaterThanOrEquals:
                return source >= action.amount;
            case ConditionAction.CompareMethod.LessThan:
                return source < action.amount;
            case ConditionAction.CompareMethod.LessThanOrEquals:
                return source <= action.amount;
            default:
                return false;
        }
    }

    internal bool ProcessHealthCondition(ConditionAction action)
    {
        return CompareUtility(action, happiness);
    }

    internal bool ProcessMoneyCondition(ConditionAction action)
    {
        return CompareUtility(action, money);
    }

    internal bool ProcessRelationshipCondition(ConditionAction action)
    {
        switch (action.condition)
        {
            case ConditionAction.ConditionType.CatRelationship:
                return CompareUtility(action, cat.relationship);
            case ConditionAction.ConditionType.GirlRelationship:
                return CompareUtility(action, girl.relationship);
            case ConditionAction.ConditionType.SalesmanRelationship:
                return CompareUtility(action, salesman.relationship);
            case ConditionAction.ConditionType.BeggarRelationship:
                return CompareUtility(action, beggar.relationship);
            default:
                return false;
        }
    }
}


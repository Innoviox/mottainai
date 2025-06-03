using System.Collections;
using System.Collections.Generic;

public enum Material
{
    Paper,
    Cloth,
    Stone,
    Clay,
    Metal
}

public enum ZoneType
{
    Task,
    Helpers,
    Sales,
    CraftBench,
    Gallery,
    GiftShop,
    Hand,
    Floor,
    Deck,
    LTask,
    RTask,
    Clerk,
    Tailor,
    Monk,
    Potter,
    Smith,   
}

public class Zone
{
    public ZoneType Type { get; private set; }
    public int Value { get; private set; }
    public Zone(ZoneType type, int value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type} (Value: {Value})";
    }
}

public enum ActionType
{
    Dummy,
    Return,
    PopTask,
    InTheMorning,
    ChooseTask,
    LTask,
    RTask,
    CTask,
    AtNight,
    DrawWaiting,
    Clerk,
    Tailor,
    Monk,
    Potter,
    Smith,
    Prayer,
    HandSupport,
    BenchSupport,
    ChooseSide,
}

public class Action
{
    public ActionType Type { get; private set; }
    public string Description { get; private set; }
    public ActionType SecondaryType { get; private set; }

    public int Value { get; set; } // Optional value for some actions

    public Action(ActionType type, string description, ActionType secondaryType = ActionType.Dummy)
    {
        Type = type;
        Description = description;
        SecondaryType = secondaryType;
    }

    public override string ToString()
    {
        return $"{Type} - {Description} (Secondary: {SecondaryType})";
    }

    public bool IsTask()
    {
        return Type == ActionType.Clerk ||
               Type == ActionType.Tailor ||
               Type == ActionType.Monk ||
               Type == ActionType.Potter ||
               Type == ActionType.Smith;
    }
}

public class Utils
{
    public static Material StringToMaterial(string materialString)
    {
        switch (materialString.ToLower())
        {
            case "paper":
                return Material.Paper;
            case "cloth":
                return Material.Cloth;
            case "stone":
                return Material.Stone;
            case "clay":
                return Material.Clay;
            case "metal":
                return Material.Metal;
            default:
                throw new System.ArgumentException("Invalid material type");
        }
    }

    public static string MaterialToString(Material material)
    {
        switch (material)
        {
            case Material.Paper:
                return "PAPER";
            case Material.Cloth:
                return "CLOTH";
            case Material.Stone:
                return "STONE";
            case Material.Clay:
                return "CLAY";
            case Material.Metal:
                return "METAL";
            default:
                throw new System.ArgumentException("Invalid material type");
        }
    }

    public static string GetJob(Material material)
    {
        switch (material)
        {
            case Material.Paper:
                return "CLERK";
            case Material.Cloth:
                return "TAILOR";
            case Material.Stone:
                return "MONK";
            case Material.Clay:
                return "POTTER";
            case Material.Metal:
                return "SMITH";
            default:
                throw new System.ArgumentException("Invalid material type");
        }
    }

    public static ActionType GetAction(Material material)
    {
        switch (material)
        {
            case Material.Paper:
                return ActionType.Clerk;
            case Material.Cloth:
                return ActionType.Tailor;
            case Material.Stone:
                return ActionType.Monk;
            case Material.Clay:
                return ActionType.Potter;
            case Material.Metal:
                return ActionType.Smith;
            default:
                throw new System.ArgumentException("Invalid material type");
        }
    }

    public static Material GetMaterialFromAction(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Clerk:
                return Material.Paper;
            case ActionType.Tailor:
                return Material.Cloth;
            case ActionType.Monk:
                return Material.Stone;
            case ActionType.Potter:
                return Material.Clay;
            case ActionType.Smith:
                return Material.Metal;
            default:
                throw new System.ArgumentException("Invalid action type");
        }
    }
}
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
    RTask
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
}

public class Action
{
    public ActionType Type { get; private set; }
    public string Description { get; private set; }
    public ActionType SecondaryType { get; private set; }

    public Action(ActionType type, string description, ActionType secondaryType = ActionType.Dummy)
    {
        Type = type;
        Description = description;
        SecondaryType = secondaryType;
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
}
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

public class Utils {
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
}
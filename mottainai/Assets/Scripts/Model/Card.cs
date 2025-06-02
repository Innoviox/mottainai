using UnityEngine;

public class Card
{
    private Material material;
    private int value;
    private string name;
    private string description;
    private string imagePath;
    private Sprite backSprite;
    private Sprite imageSprite;


    public Card(Material material, string name, string description, Sprite backSprite, Sprite imageSprite)
    {
        this.material = material;

        switch (material)
        {
            case Material.Paper:
                this.value = 1;
                break;
            case Material.Cloth:
            case Material.Stone:
                this.value = 2;
                break;
            case Material.Clay:
            case Material.Metal:
                this.value = 3;
                break;
            default:
                throw new System.ArgumentException("Invalid material type");
        }

        this.name = name;
        this.description = description;

        imagePath = $"Assets/images/{material.ToString().ToLower()}/{name.ToLower()}.png";
        if (!System.IO.File.Exists(imagePath))
        {
            throw new System.IO.FileNotFoundException($"Image file not found at path: {imagePath}");
        }

        this.backSprite = backSprite;
        this.imageSprite = imageSprite;
    }

    public Material Material
    {
        get { return material; }
    }

    public int Value
    {
        get { return value; }
    }

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public string ImagePath
    {
        get { return imagePath; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    
    public Sprite ImageSprite
    {
        get { return imageSprite; }
    }


    public override string ToString()
    {
        return $"{name} ({material}) - Value: {value}\n{description}";
    }

    public override bool Equals(object obj)
    {
        if (obj is Card otherCard)
        {
            return this.material == otherCard.material &&
                   this.value == otherCard.value &&
                   this.name == otherCard.name &&
                   this.description == otherCard.description &&
                   this.imagePath == otherCard.imagePath;
        }
        return false;
    }
}
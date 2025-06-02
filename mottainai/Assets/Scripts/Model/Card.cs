public class Card
{
    private Material material;
    private int value;
    private string name;
    private string description;
    private string imagePath;

    public Card(Material material, int value, string name, string description, string imagePath)
    {
        this.material = material;
        this.value = value;
        this.name = name;
        this.description = description;
        this.imagePath = imagePath;
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

    public override int GetHashCode()
    {
        return HashCode.Combine(material, value, name, description, imagePath);
    }
}
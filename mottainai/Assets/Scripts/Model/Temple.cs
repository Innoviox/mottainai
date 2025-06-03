using System.Collections;
using System.Collections.Generic;

public class Temple
{
    public Card? Task { get; set; }
    public List<Card> Helpers { get; set; }
    public List<Card> Sales { get; set; }
    public List<Card> CraftBench { get; set; }
    public List<Card> Gallery { get; set; }
    public List<Card> GiftShop { get; set; }

    public Temple()
    {
        Task = null;
        Helpers = new List<Card>();
        Sales = new List<Card>();
        CraftBench = new List<Card>();
        Gallery = new List<Card>();
        GiftShop = new List<Card>();
    }

    public int CountBenchMaterial(Material m)
    {
        int count = 0;
        foreach (Card card in CraftBench)
        {
            if (card.Material == m)
            {
                count += card.Value;
            }
        }
        return count;
    }
}
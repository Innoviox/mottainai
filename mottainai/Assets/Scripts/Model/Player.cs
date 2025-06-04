using System.Collections;
using System.Collections.Generic;

public class Player
{
    private Temple temple;
    private List<Card> hand;
    private bool hasPlayed;

    public Temple Temple
    {
        get { return temple; }
        set { temple = value; }
    }

    public List<Card> Hand
    {
        get { return hand; }
        set { hand = value; }
    }

    public bool HasPlayed
    {
        get { return hasPlayed; }
        set { hasPlayed = value; }
    }

    public List<Card> WaitingArea { get; set; } = new List<Card>();

    public Player()
    {
        temple = new Temple();
        hand = new List<Card>();
        hasPlayed = false;
        WaitingArea = new List<Card>();
    }

    public int TaskCount(Material m, Player p)
    {
        // todo overcoverage
        int actions = 1;

        var coverageMap = Utils.GetCoverageMap();
        if (!p.HasWork("Bangle"))
        {
            foreach (Card c in Temple.Gallery)
            {
                coverageMap[c.Material] += c.Value;
            }
        }

        foreach (Card c in Temple.Helpers)
        {
            if (coverageMap[c.Material] > 0 || (c.Material == Material.Stone && HasWork("Bangle")))
            {
                actions += 2;
                coverageMap[c.Material] -= 1;
            }
            else
            {
                actions += 1;
            }
        }

        return actions;
    }

    public int CalculateScore()
    {
        // todo overcoverage
        int score = 0;
        bool bench = HasWork("Bench");

        foreach (Card c in Temple.Gallery)
        {
            score += c.Value;
            if (c.Material == Material.Stone && bench)
            {
                score += 2;
            }
        }

        var coverageMap = Utils.GetCoverageMap();
        foreach (Card c in Temple.GiftShop)
        {
            coverageMap[c.Material] += c.Value;
            score += c.Value;
            if (c.Material == Material.Stone && bench)
            {
                score += 2;
            }
        }

        foreach (Card c in Temple.Sales)
        {
            if (coverageMap[c.Material] > 0 ||
                (HasWork("Pillar") && c.Material == MostSoldType()) ||
                (HasWork("Quilt") && (c.Material == Material.Paper || c.Material == Material.Cloth || c.Material == Material.Stone)))
            {
                coverageMap[c.Material] -= 1;
                score += c.Value;
            }
        }

        if (HasWork("Haniwa"))
        {
            coverageMap = Utils.GetCoverageMap();
            foreach (Card c in Temple.Helpers)
            {
                coverageMap[c.Material] += 1;
            }

            score += 3 * Utils.MaxValue(coverageMap);
        }

        if (HasWork("Teapot"))
        {
            coverageMap = Utils.GetCoverageMap();
            foreach (Card c in Temple.CraftBench)
            {
                coverageMap[c.Material] += 1;
            }

            score += 3 * Utils.MaxValue(coverageMap);
        }

        if (HasWork("Scroll"))
        {
            score += 3;
        }

        if (HasWork("Tapestry"))
        {
            if (GetZone("Tapestry").Type == ZoneType.Gallery)
            {
                score += Temple.Gallery.Count;
            }
            else
            {
                score += Temple.GiftShop.Count;
            }
        }

        return score;
    }

    public bool CanCraftFromBench(int index, Material m)
    {
        Card card = hand[index];
        int count = temple.CountBenchMaterial(card.Material);
        return card.Material == m && count + 1 >= card.Value;
    }

    public bool CanCraftFromHand(int index, Player[] players)
    {
        Card card = hand[index];
        int count = CountHandMaterial(card.Material);

        if (HasWork("Brick"))
        {
            foreach (Player player in players)
            {
                if (player.Temple.Task != null && player.Temple.Task.Material == card.Material)
                {
                    count += 1;
                }
            }
        }

        if (HasWork("Straw") && (card.Material == Material.Cloth || card.Material == Material.Clay))
        {
            count += 1;
        }

        return count >= card.Value;
    }

    public int CountHandMaterial(Material m)
    {
        int count = 0;
        foreach (Card card in hand)
        {
            if (card.Material == m)
            {
                count++;
            }
        }
        return count;
    }

    public void DrawWaiting()
    {
        Hand.AddRange(WaitingArea);
        WaitingArea.Clear();
    }

    public bool HasWork(string work)
    {
        foreach (Card card in Temple.GiftShop)
        {
            if (card.Name.ToLower() == work.ToLower())
            {
                return true;
            }
        }

        foreach (Card card in Temple.Gallery)
        {
            if (card.Name.ToLower() == work.ToLower())
            {
                return true;
            }
        }
        return false;
    }

    public Material MostSoldType()
    {
        var coverageMap = Utils.GetCoverageMap();
        foreach (Card c in Temple.Sales)
        {
            coverageMap[c.Material] += 1;
        }

        int maxCount = 0;
        Material mostSoldMaterial = Material.None;

        foreach (var kvp in coverageMap)
        {
            if (kvp.Value > maxCount)
            {
                maxCount = kvp.Value;
                mostSoldMaterial = kvp.Key;
            }
        }

        return mostSoldMaterial;
    }

    public Zone GetZone(string work, List<Button> buttons = null)
    {
        for (int i = 0; i < Temple.Gallery.Count; i++)
        {
            if (Temple.Gallery[i].Name == work)
            {
                return new Zone(ZoneType.Gallery, i, buttons);
            }
        }
        for (int i = 0; i < Temple.GiftShop.Count; i++)
        {
            if (Temple.GiftShop[i].Name == work)
            {
                return new Zone(ZoneType.GiftShop, i, buttons);
            }
        }

        return null;
    }
}
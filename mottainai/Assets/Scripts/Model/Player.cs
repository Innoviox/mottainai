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

    public int TaskCount(Material m)
    {
        // todo overcoverage
        int actions = 1;

        var coverageMap = Utils.GetCoverageMap();
        foreach (Card c in Temple.Gallery)
        {
            coverageMap[c.Material] += c.Value;
        }

        foreach (Card c in Temple.Helpers)
        {
            if (coverageMap[c.Material] > 0)
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

        foreach (Card c in Temple.Gallery)
        {
            score += c.Value;
        }

        var coverageMap = Utils.GetCoverageMap();
        foreach (Card c in Temple.GiftShop)
        {
            coverageMap[c.Material] += c.Value;
            score += c.Value;
        }

        foreach (Card c in Temple.Sales)
        {
            if (coverageMap[c.Material] > 0)
            {
                coverageMap[c.Material] -= 1;
                score += c.Value;
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

    public bool CanCraftFromHand(int index)
    {
        Card card = hand[index];
        int count = CountHandMaterial(card.Material);
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
}
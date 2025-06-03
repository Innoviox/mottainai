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

    public Player()
    {
        temple = new Temple();
        hand = new List<Card>();
        hasPlayed = false;
    }

    public int TaskCount(Material m)
    {
        // todo
        return 1;
    }

    public bool CanCraftFromBench(int index)
    {
        Card card = hand[index];
        int count = temple.CountBenchMaterial(card.Material);
        return count + 1 >= card.Value;
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
}
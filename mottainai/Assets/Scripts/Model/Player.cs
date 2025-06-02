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
}
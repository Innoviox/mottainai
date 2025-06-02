using System.Collections;
using System.Collections.Generic;

public class Game
{
    private Player[] players;
    private int currentPlayerIndex;
    private List<Card> deck;
    private List<Card> floor;

    public Game(List<Card> deck)
    {
        this.players = new Player[3];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player();
        }

        this.deck = deck;
        this.currentPlayerIndex = 0;
        this.floor = new List<Card>();
    }

    public void Deal()
    {
        ShuffleDeck();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].Hand = new List<Card>();
            for (int j = 0; j < 5; j++)
            {
                players[i].Hand.Add(DealCard());
            }

            // set dummy task
            players[i].Temple.SetTask(DealCard());
        }

        // select first player based on alphabetically first card
        int bestIndex = -1;
        string bestName = "";
        for (int i = 0; i < players.Length; i++)
        {
            Card c = DealCard();
            if (bestIndex == -1 || c.Name.CompareTo(bestName) < 0)
            {
                bestIndex = i;
                bestName = c.Name;
            }
            floor.Add(c);
        }

        currentPlayerIndex = bestIndex;
    }

    private void ShuffleDeck()
    {
        for (int i = deck.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    private Card DealCard()
    {
        if (deck.Length == 0)
        {
            return null;
        }

        Card dealtCard = deck[0];
        deck = deck[1..];
        return dealtCard;
    }

    public Player CurrentPlayer
    {
        get { return players[currentPlayerIndex]; }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Game
{
    private Player[] players;
    public Player[] Players
    {
        get { return players; }
    }

    private int currentPlayerIndex;
    public int CurrentPlayerIndex
    {
        get { return currentPlayerIndex; }
        set { currentPlayerIndex = value; }
    }

    private List<Card> deck;
    private List<Card> floor;
    private Sprite[] backs;
    private Sprite[] cardSprites;

    public Game(string cardsPath, Sprite[] backs, Sprite[] cardSprites)
    {
        this.backs = backs;
        this.cardSprites = cardSprites;

        LoadCards(cardsPath);

        this.players = new Player[3];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player();
        }


        this.currentPlayerIndex = 0;
        this.floor = new List<Card>();
    }

    private void LoadCards(string cardsPath)
    {
        deck = new List<Card>();

        StreamReader reader = new StreamReader(cardsPath); 

        string[] lines = reader.ReadToEnd().Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split("::");
            if (parts.Length < 3) continue;

            string name = parts[0].Trim();
            string materialStr = parts[1].Trim();
            string description = parts[2].Trim();

            Card card = new Card(Utils.StringToMaterial(materialStr), name, description);
            deck.Add(card);
        }
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
            players[i].Temple.Task = DealCard();
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
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    private Card DealCard()
    {
        if (deck.Count == 0)
        {
            return null;
        }

        Card dealtCard = deck[0];
        deck.RemoveAt(0);
        return dealtCard;
    }

}
using System.Collections;
using System.Collections.Generic;

public class Game
{
    private Player[] players;
    private int currentPlayerIndex;
    private List<Card> deck;
    private List<Card> floor;

    public Game(string cardsPath)
    {
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
        
        TextAsset cardsFile = Resources.Load<TextAsset>(cardsPath);
        if (cardsFile == null)
        {
            throw new System.Exception("Cards file not found at path: " + cardsPath);
        }

        string[] lines = cardsFile.text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split('::');
            if (parts.Length < 3) continue;

            string name = parts[0].Trim();
            string materialStr = parts[1].Trim();
            string description = parts[2].Trim();

            Card card = new Card(name, StringToMaterial(materialStr), description);
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
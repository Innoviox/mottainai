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
    public List<Card> Deck
    {
        get { return deck; }
    }

    private List<Card> floor;
    public List<Card> Floor
    {
        get { return floor; }
    }

    private Sprite[] backs;
    private Sprite[] cardSprites;
    private List<Zone> zones;
    public List<Zone> Zones
    {
        get { return zones; }
    }
    private List<Action> actions;
    public List<Action> Actions
    {
        get { return actions; }
    }
    private int actionIndex = -1;

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
        this.zones = new List<Zone>();
        this.actions = new List<Action>();
        this.actionIndex = -1;
    }

    private void LoadCards(string cardsPath)
    {
        deck = new List<Card>();

        StreamReader reader = new StreamReader(cardsPath);

        string[] lines = reader.ReadToEnd().Split('\n');

        // foreach (string line in lines)
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split("::");
            if (parts.Length < 3) continue;

            string name = parts[0].Trim();
            string materialStr = parts[1].Trim();
            string description = parts[2].Trim();

            Material m = Utils.StringToMaterial(materialStr);
            Card card = new Card(m, name, description, backs[(int)m], cardSprites[i]);
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

    public void Tick()
    {
        if (!players[currentPlayerIndex].HasPlayed)
        {
            players[currentPlayerIndex].HasPlayed = true;
        }

        if (actions.Count == 0)
        {
            actionIndex = -1;
            SetActions();
        }

        if (actionIndex >= actions.Count)
        {
            // EndTurn(); // todo
            return;
        }

        actionIndex++;
        Action action = actions[actionIndex];

        SetZones(action);

        if (action.Type == ActionType.Dummy)
        {
            foreach (Action newAction in Calculate(action))
            {
                actions.Insert(actionIndex, newAction);
            }
            Tick();
        }
        else if (action.Type == ActionType.PopTask)
        {
            floor.Add(players[currentPlayerIndex].Temple.Task);
            players[currentPlayerIndex].Temple.Task = null;

            Tick();
        }
    }

    private void SetActions()
    {
        // clear actions
        actions.Clear();

        // dummy morning task
        actions.Add(new Action(ActionType.Dummy, "Morning begins"));
        // return to limit of 5
        if (players[currentPlayerIndex].Hand.Count > 5)
        {
            actions.Add(new Action(ActionType.Return, "Return to hand limit of 5"));
        }
        else
        {
            actions.Add(new Action(ActionType.Dummy, "Return to hand limit of 5"));
        }
        // pop task
        actions.Add(new Action(ActionType.PopTask, "Pop task from temple to floor"));
        // perform "in the morning" effects (dummy for now)
        actions.Add(new Action(ActionType.Dummy, "Perform morning effects"));
        // choose new task
        actions.Add(new Action(ActionType.ChooseTask, "Choose a new task"));
        // dummy noon task
        actions.Add(new Action(ActionType.Dummy, "Noon begins"));
        // "calculate left task"
        actions.Add(new Action(ActionType.Dummy, "Calculate left task", ActionType.LTask));
        // "calculate right task"
        actions.Add(new Action(ActionType.Dummy, "Calculate right task", ActionType.RTask));
        // "calculate center task"
        actions.Add(new Action(ActionType.Dummy, "Calculate center task", ActionType.CTask));
        // dummy night task
        actions.Add(new Action(ActionType.Dummy, "Night begins"));
        // perform "in the night" effects (dummy for now)
        actions.Add(new Action(ActionType.Dummy, "Perform night effects"));
        // waiting area to hand
        actions.Add(new Action(ActionType.DrawWaiting, "Draw cards from waiting area to hand"));
    }

    private void SetZones(Action action)
    {
        zones.Clear();

        switch (action.Type)
        {
            case ActionType.Dummy:
            case ActionType.PopTask:
            case ActionType.DrawWaiting:
                break;
            case ActionType.ChooseTask:
                for (int i = 0; i < players[currentPlayerIndex].Hand.Count; i++)
                {
                    zones.Add(new Zone(ZoneType.Hand, i));
                }
                break;
            default:
                Debug.Log("Unhandled action type: " + action.Type);
                break;
        }
    }

    private List<Action> Calculate(Action action)
    {
        List<Action> newActions = new List<Action>();

        // todo handle LTask, RTask, CTask
        // todo handle works

        return newActions;
    }

    public string Log
    {
        get
        {
            string log = "";
            for (int i = 0; i < actions.Count; i++)
            {
                if (i == actionIndex)
                {
                    log += $"> ";
                }
                log += $"{actions[i].Description}\n";
            }
            return log;
        }
    }

    public Action currentAction
    {
        get
        {
            if (actionIndex < 0 || actionIndex >= actions.Count)
            {
                return null;
            }
            return actions[actionIndex];
        }
    }
}
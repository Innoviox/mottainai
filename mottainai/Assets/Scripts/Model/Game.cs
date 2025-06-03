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
    public bool NeedTick { get; set; } = false;

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

    public void Tick(string reason = "")
    {
        NeedTick = false;
        Debug.Log("Ticking because of " + reason);
        if (!players[currentPlayerIndex].HasPlayed)
        {
            players[currentPlayerIndex].HasPlayed = true;
        }

        if (actions.Count == 0)
        {
            actionIndex = -1;
            SetActions();
        }

        if (actionIndex >= actions.Count - 1)
        {
            EndTurn();
            return;
        }

        actionIndex++;
        Action action = actions[actionIndex];
        Debug.Log("Current action: " + action.Description);

        SetZones(action);

        if (action.Type == ActionType.Dummy)
        {
            List<Action> newActions = Calculate(action);
            for (int i = 0; i < newActions.Count; i++)
            {
                actions.Insert(actionIndex + 1 + i, newActions[i]);
            }

            // Tick("dummy was calculated");
            NeedTick = true;
        }
        else if (action.Type == ActionType.PopTask)
        {
            floor.Add(players[currentPlayerIndex].Temple.Task);
            players[currentPlayerIndex].Temple.Task = null;

            // Tick("task was popped");
            NeedTick = true;
        }
        else if (action.Type == ActionType.DrawWaiting)
        {
            players[currentPlayerIndex].DrawWaiting();

            // Tick("draw waiting");
            NeedTick = true;
        }
    }

    public void EndTurn()
    {
        // move to next player
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;

        // reset actions
        actionIndex = -1;
        actions.Clear();
        // Tick("turn was ended");
        NeedTick = true;
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

        if (action.Type == ActionType.Clerk || action.Type == ActionType.Tailor || action.Type == ActionType.Monk || action.Type == ActionType.Potter || action.Type == ActionType.Smith)
        {
            zones.Add(new Zone(ZoneType.Deck, 0));
            for (int i = 0; i < players[currentPlayerIndex].Hand.Count; i++)
            {
                if (players[currentPlayerIndex].CanCraftFromBench(i, Utils.GetMaterialFromAction(action.Type)))
                {
                    zones.Add(new Zone(ZoneType.Hand, i));
                }
            }
        }

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
            case ActionType.Clerk:
                for (int i = 0; i < players[currentPlayerIndex].Temple.CraftBench.Count; i++)
                {
                    zones.Add(new Zone(ZoneType.CraftBench, i));
                }
                break;
            case ActionType.Tailor:
                for (int i = 0; i < players[currentPlayerIndex].Hand.Count; i++)
                {
                    zones.Add(new Zone(ZoneType.Hand, i));
                }
                break;
            case ActionType.Monk:
            case ActionType.Potter:
                for (int i = 0; i < floor.Count; i++)
                {
                    zones.Add(new Zone(ZoneType.Floor, i));
                }
                break;
            case ActionType.Smith:
                for (int i = 0; i < players[currentPlayerIndex].Hand.Count; i++)
                {
                    if (players[currentPlayerIndex].CanCraftFromHand(i))
                    {
                        zones.Add(new Zone(ZoneType.Hand, i));
                    }
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

        if (action.SecondaryType == ActionType.LTask || action.SecondaryType == ActionType.RTask || action.SecondaryType == ActionType.CTask)
        {
            int playerN = action.SecondaryType == ActionType.LTask ? (currentPlayerIndex + 1) % players.Length :
                            action.SecondaryType == ActionType.RTask ? (currentPlayerIndex + 2) % players.Length :
                            currentPlayerIndex;

            if (players[playerN].Temple.Task == null || !players[playerN].HasPlayed)
            {
                if (action.SecondaryType == ActionType.CTask)
                {
                    newActions.Add(new Action(ActionType.Prayer, "Void center task replaced by prayer", action.SecondaryType));
                }
                return newActions;
            }

            Material m = players[playerN].Temple.Task.Material;
            int total = players[currentPlayerIndex].TaskCount(m);
            ActionType a = Utils.GetAction(m);
            for (int i = 0; i < total; i++)
            {
                newActions.Add(new Action(a, $"Perform {a} task #{i + 1}", action.SecondaryType));
            }
        }

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

    public void ChooseTask(int index)
    {
        if (index < 0 || index >= players[currentPlayerIndex].Hand.Count || currentAction == null || currentAction.Type != ActionType.ChooseTask)
        {
            Debug.LogError("Invalid task index: " + index);
            return;
        }

        Card chosenCard = players[currentPlayerIndex].Hand[index];
        players[currentPlayerIndex].Hand.RemoveAt(index);
        players[currentPlayerIndex].Temple.Task = chosenCard;
    }

    public void Pray()
    {
        players[currentPlayerIndex].WaitingArea.Add(DealCard());
    }

    public void Potter(int index)
    {
        Card potterCard = floor[index];
        players[currentPlayerIndex].Temple.CraftBench.Add(potterCard);
        floor.RemoveAt(index);
    }

    public void Monk(int index)
    {
        Card monkCard = floor[index];
        players[currentPlayerIndex].Temple.Helpers.Add(monkCard);
        floor.RemoveAt(index);
    }
}
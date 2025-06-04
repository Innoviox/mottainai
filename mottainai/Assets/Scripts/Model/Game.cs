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
        currentPlayerIndex = 0;
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
        MoveCardToFront("Amulet");
        MoveCardToFront("Chopsticks");
        MoveCardToFront("Bell");
        MoveCardToFront("Bench");
        MoveCardToFront("Doll");
        MoveCardToFront("Curtain");
        MoveCardToFront("Tower");
        MoveCardToFront("Mask");
        MoveCardToFront("Cloak");
        MoveCardToFront("Bangle");
    }

    private void MoveCardToFront(string cardName)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i].Name == cardName)
            {
                Card card = deck[i];
                deck.RemoveAt(i);
                deck.Insert(0, card);
                return;
            }
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
            if (players[currentPlayerIndex].HasWork("Chopsticks"))
            {
                actions.Insert(actionIndex + 1, new Action(ActionType.Chopsticks, "Optionally activate chopsticks", ActionType.Work));
            }
            else
            {
                floor.Add(players[currentPlayerIndex].Temple.Task);
                players[currentPlayerIndex].Temple.Task = null;

                // Tick("task was popped");
                NeedTick = true;
                if (!players[currentPlayerIndex].HasPlayed)
                {
                    players[currentPlayerIndex].HasPlayed = true;
                }
            }
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
        if (players[currentPlayerIndex].HasWork("Doll"))
        {
            actions.Add(new Action(ActionType.Doll, "Choose a new task (with Doll)", ActionType.Work));
        }
        else
        {
            actions.Add(new Action(ActionType.ChooseTask, "Choose a new task"));
        }
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

        if (action.IsTask())
        {
            int playerN = GetPlayerN(action);
            if (playerN < 0 || (playerN == currentPlayerIndex || !players[playerN].HasWork("Mask")))
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
        }

        if (action.Type == ActionType.Amulet)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Amulet", new List<Button> { Button.No }));
        }

        if (action.Type == ActionType.Clerk && players[currentPlayerIndex].HasWork("Bell"))
        {
            zones.Add(players[currentPlayerIndex].GetZone("Bell", new List<Button> { Button.Yes }));
        }

        if (action.Type == ActionType.Bowl)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Bowl", new List<Button> { Button.Yes, Button.No }));
        }

        if (action.Type == ActionType.Chopsticks)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Chopsticks", new List<Button> { Button.Yes, Button.No }));
        }

        if (action.Type == ActionType.StartCloakGallery)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Cloak", new List<Button> { Button.No }));
            zones.Add(new Zone(ZoneType.Gallery, players[currentPlayerIndex].Temple.Gallery.Count - 1, new List<Button> { Button.Return }));
        }

        if (action.Type == ActionType.StartCloakGiftShop)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Cloak", new List<Button> { Button.No }));
            zones.Add(new Zone(ZoneType.GiftShop, players[currentPlayerIndex].Temple.GiftShop.Count - 1, new List<Button> { Button.Return }));
        }

        if (action.Type == ActionType.Daidoro)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Daidoro", new List<Button> { Button.Yes, Button.No }));
        }

        if (action.Type == ActionType.Doll)
        {
            zones.Add(new Zone(ZoneType.LTask, 1));
            zones.Add(new Zone(ZoneType.RTask, 1));
        }

        if (action.Type == ActionType.EndCloak)
        {
            for (int i = 0; i < players[currentPlayerIndex].Hand.Count; i++)
            {
                if (players[currentPlayerIndex].Hand[i].Material == Material.Metal)
                {
                    zones.Add(new Zone(ZoneType.Hand, i));
                }
            }
        }

        if (action.Type == ActionType.DeckOfCards)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Deck of Cards", new List<Button> { Button.Yes, Button.No }));
        }

        if (action.Type == ActionType.Gong)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Gong", new List<Button> { Button.Yes, Button.No }));
        }

        if (action.Type == ActionType.Pinwheel)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Pinwheel", new List<Button> { Button.No }));
        }

        if (action.Type == ActionType.Pin)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Pin", new List<Button> { Button.Yes, Button.No }));
        }

        if (action.Type == ActionType.Stool)
        {
            zones.Add(players[currentPlayerIndex].GetZone("Stool", new List<Button> { Button.Yes, Button.No }));
        }

        switch (action.Type)
        {
            case ActionType.Dummy:
            case ActionType.PopTask:
            case ActionType.DrawWaiting:
                break;
            case ActionType.ChooseTask:
            case ActionType.Doll:
                for (int i = 0; i < players[currentPlayerIndex].Hand.Count; i++)
                {
                    zones.Add(new Zone(ZoneType.Hand, i));
                }
                break;
            case ActionType.Clerk:
            case ActionType.Amulet:
                for (int i = 0; i < players[currentPlayerIndex].Temple.CraftBench.Count; i++)
                {
                    zones.Add(new Zone(ZoneType.CraftBench, i));
                }
                break;
            case ActionType.Tailor:
            case ActionType.Pinwheel:
                for (int i = 0; i < players[currentPlayerIndex].Hand.Count; i++)
                {
                    zones.Add(new Zone(ZoneType.TailorReturn, i));
                }
                zones.Add(new Zone(ZoneType.CTask, 0));
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
                    if (players[currentPlayerIndex].CanCraftFromHand(i, players))
                    {
                        zones.Add(new Zone(ZoneType.Hand, i));
                    }
                }
                break;
            case ActionType.Return:
                for (int i = 0; i < players[currentPlayerIndex].Hand.Count; i++)
                {
                    zones.Add(new Zone(ZoneType.Hand, i));
                }
                break;
            case ActionType.ChooseSide:
                zones.Add(new Zone(ZoneType.Gallery, 0));
                zones.Add(new Zone(ZoneType.GiftShop, 0));
                break;
            default:
                Debug.Log("Unhandled action type: " + action.Type);
                break;
        }
    }

    private int GetPlayerN(Action action)
    {
        if (action.SecondaryType == ActionType.LTask)
        {
            return (currentPlayerIndex + 1) % players.Length;
        }
        else if (action.SecondaryType == ActionType.RTask)
        {
            return (currentPlayerIndex + 2) % players.Length;
        }
        else if (action.SecondaryType == ActionType.CTask)
        {
            return currentPlayerIndex;
        }
        return -1; // Invalid action type
    }

    private List<Action> Calculate(Action action)
    {
        List<Action> newActions = new List<Action>();

        // todo handle works

        if (action.SecondaryType == ActionType.LTask || action.SecondaryType == ActionType.RTask || action.SecondaryType == ActionType.CTask)
        {
            int playerN = GetPlayerN(action);

            if (players[playerN].Temple.Task == null || !players[playerN].HasPlayed)
            {
                if (action.SecondaryType == ActionType.CTask)
                {
                    newActions.Add(new Action(ActionType.Prayer, "Void center task replaced by prayer", action.SecondaryType));
                }
                return newActions;
            }

            Material m = players[playerN].Temple.Task.Material;
            int total = players[currentPlayerIndex].TaskCount(m, players[playerN]);
            ActionType a = Utils.GetAction(m);

            if (action.SecondaryType != ActionType.CTask)
            {
                if (players[playerN].HasWork("Curtain") && (m == Material.Cloth || m == Material.Metal))
                {
                    if (players[currentPlayerIndex].CountHandMaterial(m) == 0)
                    {
                        return newActions;
                    }
                }

                if (players[playerN].HasWork("Tower") && (m == Material.Paper || m == Material.Stone || m == Material.Clay))
                {
                    if (players[currentPlayerIndex].CountHandMaterial(m) == 0)
                    {
                        return newActions;
                    }
                }
            }
            else if (action.SecondaryType == ActionType.CTask && action.Value == 1)
            {
                total++;
            }

            for (int i = 0; i < total; i++)
            {
                newActions.Add(new Action(a, $"Perform {a} task #{i + 1}", action.SecondaryType));
            }
        }
        else if (action.Type == ActionType.InTheMorning)
        {
            if (players[currentPlayerIndex].HasWork("Bowl"))
            {
                newActions.Add(new Action(ActionType.Bowl, "Optionally perform bowl action", ActionType.Work));
            }

            if (players[currentPlayerIndex].HasWork("Daidoro"))
            {
                newActions.Add(new Action(ActionType.Daidoro, "Optionally perform daidoro action", ActionType.Work));
            }

            if (players[currentPlayerIndex].HasWork("Pin"))
            {
                newActions.Add(new Action(ActionType.Pin, "Optionally perform pin action", ActionType.Work));
            }
        }
        else if (action.Type == ActionType.AtNight)
        {
            if (players[currentPlayerIndex].HasWork("Pinwheel"))
            {
                newActions.Add(new Action(ActionType.Pinwheel, "Optionally perform pinwheel action", ActionType.Work));
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

        if (players[currentPlayerIndex].HasWork("Gong"))
        {
            actions.Insert(actionIndex + 1, new Action(ActionType.Gong, "Optionally activate gong", ActionType.Work));
        }
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

    public void Return(int index)
    {
        Card returnedCard = players[currentPlayerIndex].Hand[index];
        players[currentPlayerIndex].Hand.RemoveAt(index);
        deck.Add(returnedCard);

        if (players[currentPlayerIndex].Hand.Count <= 5)
        {
            NeedTick = true;
        }
    }

    public void StartCraft(int index)
    {
        ActionType secondaryType = currentAction.Type == ActionType.Smith ? ActionType.Smith : ActionType.Dummy;
        Action act = new Action(ActionType.ChooseSide, "Choose side for crafting", secondaryType);
        act.Value = index;
        actions.Insert(actionIndex + 1, act);
        NeedTick = true;
    }

    public void EndCraft(int index, bool left)
    {
        Card card = players[currentPlayerIndex].Hand[index];
        if (left)
        {
            players[currentPlayerIndex].Temple.Gallery.Add(card);
            if (players[currentPlayerIndex].HasWork("Cloak"))
            {
                actions.Insert(actionIndex + 1, new Action(ActionType.StartCloakGallery, "Optionally activate cloak", ActionType.Work));
            }
        }
        else
        {
            players[currentPlayerIndex].Temple.GiftShop.Add(card);
            if (players[currentPlayerIndex].HasWork("Cloak"))
            {
                actions.Insert(actionIndex + 1, new Action(ActionType.StartCloakGiftShop, "Optionally activate cloak", ActionType.Work));
            }
        }
        players[currentPlayerIndex].Hand.RemoveAt(index);

        if (players[currentPlayerIndex].HasWork("Amulet") && players[currentPlayerIndex].Temple.CraftBench.Count > 0)
        {
            actions.Insert(actionIndex + 1, new Action(ActionType.Amulet, "Optionally activate amulet", ActionType.Work));
        }

        if (card.Material == Material.Paper && currentAction.SecondaryType == ActionType.Smith && players[currentPlayerIndex].HasWork("Deck of Cards"))
        {
            actions.Insert(actionIndex + 1, new Action(ActionType.DeckOfCards, "Optionally activate deck of cards", ActionType.Work));
        }

        // if (card.Material == Material.Paper && players[currentPlayerIndex].HasWork("Poem"))
        // {
        //     actions.Insert(actionIndex + 1, new Action(ActionType.Poem, "Optionally activate poem", ActionType.Work));
        // }

        if (players[currentPlayerIndex].HasWork("Stool") && (card.Material == Material.Stone || card.Material == Material.Clay || card.Material == Material.Metal))
        {
            actions.Insert(actionIndex + 1, new Action(ActionType.Stool, "Optionally activate stool", ActionType.Work));
        }
    }

    public void Clerk(int index)
    {
        Card clerkCard = players[currentPlayerIndex].Temple.CraftBench[index];
        players[currentPlayerIndex].Temple.Sales.Add(clerkCard);
        players[currentPlayerIndex].Temple.CraftBench.RemoveAt(index);
    }

    public void Bell()
    {
        players[currentPlayerIndex].Temple.Sales.Add(DealCard());
    }

    public void Bowl()
    {
        players[currentPlayerIndex].Temple.CraftBench.Add(DealCard());
    }

    public void Chopsticks(bool use)
    {
        if (use)
        {
            players[currentPlayerIndex].Temple.Sales.Add(players[currentPlayerIndex].Temple.Task);
            players[currentPlayerIndex].Temple.Task = null;
        }
        else
        {
            floor.Add(players[currentPlayerIndex].Temple.Task);
            players[currentPlayerIndex].Temple.Task = null;
        }
    }

    public void TailorReturn(int index)
    {
        Card tailorCard = players[currentPlayerIndex].Hand[index];
        players[currentPlayerIndex].Hand.RemoveAt(index);
        deck.Add(tailorCard);

        // Remove Deck from Zones
        // Remove TailorReturn from Zones if it's higher than hand count
        var i = 0;
        while (i < zones.Count)
        {
            if (zones[i].Type == ZoneType.Deck || (zones[i].Type == ZoneType.TailorReturn && zones[i].Value >= players[currentPlayerIndex].Hand.Count))
            {
                zones.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    public void Tailor()
    {
        while ((players[currentPlayerIndex].Hand.Count + players[currentPlayerIndex].WaitingArea.Count) < 5 && deck.Count > 0)
        {
            players[currentPlayerIndex].Hand.Add(DealCard());
        }
    }

    public void StartCloak()
    {
        if (currentAction.Type == ActionType.StartCloakGallery)
        {
            Card c = players[currentPlayerIndex].Temple.Gallery[players[currentPlayerIndex].Temple.Gallery.Count - 1];
            players[currentPlayerIndex].Temple.Gallery.RemoveAt(players[currentPlayerIndex].Temple.Gallery.Count - 1);
            deck.Add(c);
        }
        else
        {
            Card c = players[currentPlayerIndex].Temple.GiftShop[players[currentPlayerIndex].Temple.GiftShop.Count - 1];
            players[currentPlayerIndex].Temple.GiftShop.RemoveAt(players[currentPlayerIndex].Temple.GiftShop.Count - 1);
            deck.Add(c);
        }

        if (players[currentPlayerIndex].CountHandMaterial(Material.Metal) > 0)
        {
            Action act = new Action(ActionType.EndCloak, "Choose metal work to craft for free", ActionType.Dummy);
            actions.Insert(actionIndex + 1, act);
        }

        NeedTick = true;
    }

    public void Daidoro()
    {
        while (floor.Count < 3 && deck.Count > 0)
        {
            floor.Add(DealCard());
        }
    }

    public void DeckOfCards()
    {
        players[currentPlayerIndex].WaitingArea.Add(DealCard());
    }

    public void Doll(int playerIndex)
    {
        players[currentPlayerIndex].Temple.Task = players[playerIndex].Temple.Task;
        players[playerIndex].Temple.Task = null;

        for (int i = 0; i < Actions.Count; i++)
        {
            if (actions[i].SecondaryType == ActionType.CTask)
            {
                actions[i].Value = 1; // Set the value to 1 to indicate that the task was taken
                break;
            }
        }
    }

    public void Gong()
    {
        for (int i = 0; i < 3; i++)
        {
            players[currentPlayerIndex].WaitingArea.Add(DealCard());
        }

        Zone gongZone = players[currentPlayerIndex].GetZone("Gong");
        List<Card> list = gongZone.Type == ZoneType.Gallery ? players[currentPlayerIndex].Temple.Gallery : players[currentPlayerIndex].Temple.GiftShop;
        Card gong = list[gongZone.Value];
        list.RemoveAt(gongZone.Value);
        players[currentPlayerIndex].WaitingArea.Add(gong);
    }

    public void Pinwheel(int index)
    {
        Card pinwheelCard = players[currentPlayerIndex].Hand[index];
        players[currentPlayerIndex].Hand.RemoveAt(index);
        deck.Add(pinwheelCard);
        players[currentPlayerIndex].WaitingArea.Add(DealCard());
    }

    public void Pin()
    {
        actions.Insert(actionIndex + 1, new Action(ActionType.Tailor, "Take tailor action [pinwheel]", ActionType.Work));
    }

    public void Stool()
    {
        players[currentPlayerIndex].WaitingArea.Add(DealCard());
    }
}
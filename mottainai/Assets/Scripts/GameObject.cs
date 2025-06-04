using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameObject : MonoBehaviour
{
    public Transform playerPrefab;
    public Transform cardPrefab;
    public Transform cardHighlightPrefab;
    public Transform tailorButtonPrefab;
    public string cardsPath;
    public Sprite[] backs;
    public Sprite[] cardSprites;

    private Game game;

    private PlayerObject[] playerObjects;
    private InputAction clickAction;
    private InputAction pointAction;
    private List<Transform> highlights = new List<Transform>();

    private void Awake()
    {
        playerObjects = new PlayerObject[3];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            Transform playerTransform = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            playerObjects[i] = playerTransform.GetComponent<PlayerObject>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");

        game = new Game(cardsPath, backs, cardSprites);
        game.Deal();

        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].Player = game.Players[i];
            playerObjects[i].Refresh(i == game.CurrentPlayerIndex);
        }

        Refresh();
        BeginTurn();
    }

    void DrawFloor()
    {
        transform.Find("Canvas/DeckCount").GetComponent<TMPro.TextMeshProUGUI>().text = game.Deck.Count.ToString();

        Transform floorTransform = transform.Find("Floor");
        foreach (Transform child in floorTransform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < game.Floor.Count; i++)
        {
            Transform cardTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, floorTransform);
            CardObject cardObject = cardTransform.GetComponent<CardObject>();
            cardObject.Card = game.Floor[i];
            cardObject.transform.localPosition = new Vector3(i * 40, 0, 0);
            cardObject.Refresh();
        }
    }

    void BeginTurn()
    {
        Tick("begin turn");
    }

    private void Tick(string reason = "", bool tickGame = true)
    {
        Debug.Log("[gameobject] Tick: " + reason);

        ClearHighlights();

        if (tickGame)
        {
            game.Tick("gameobject ticked");
        }

        Refresh();
        UpdateLog();
    }

    public void Update()
    {
        if (clickAction.WasReleasedThisFrame())
        {
            Debug.Log("[gameobject] Click action release");
            PerformRaycast();
        }

        if (game.NeedTick)
        {
            Tick("game needs tick");
        }
    }

    private void PerformRaycast()
    {
        Vector2 pointerPosition = pointAction.ReadValue<Vector2>();

        var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(pointerPosition));
        if (!rayHit.collider) return;

        Transform hitTransform = rayHit.collider.transform;
        if (game.currentAction != null)
        {
            Debug.Log("[gameobject] Raycast hit: " + hitTransform.name + " for action: " + game.currentAction.Type);
            if ((game.currentAction.Type == ActionType.ChooseTask || game.currentAction.Type == ActionType.Doll) && hitTransform.name.StartsWith("CardHighlight_Hand"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[2]);
                game.ChooseTask(index);
                Tick("clicked hand");
            }
            else if (game.currentAction.IsTask() && hitTransform.name.StartsWith("CardHighlight_Deck"))
            {
                game.Pray();
                Tick("clicked deck");
            }
            else if (game.currentAction.Type == ActionType.Potter && hitTransform.name.StartsWith("CardHighlight_Floor"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[2]);
                game.Potter(index);
                Tick("clicked floor");
            }
            else if (game.currentAction.Type == ActionType.Monk && hitTransform.name.StartsWith("CardHighlight_Floor"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[2]);
                game.Monk(index);
                Tick("clicked floor");
            }
            else if (game.currentAction.Type == ActionType.Return && hitTransform.name.StartsWith("CardHighlight_Hand"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[2]);
                game.Return(index);
            }
            else if ((game.currentAction.IsTask() || game.currentAction.Type == ActionType.EndCloak) && hitTransform.name.StartsWith("CardHighlight_Hand"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[2]);
                game.StartCraft(index);
            }
            else if (game.currentAction.Type == ActionType.ChooseSide && hitTransform.name.StartsWith("CardHighlight_Side_"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[2]);
                game.EndCraft(game.currentAction.Value, index == 0);
                Tick("clicked side highlight");
            }
            else if ((game.currentAction.Type == ActionType.Clerk || game.currentAction.Type == ActionType.Amulet) && hitTransform.name.StartsWith("CardHighlight_CraftBench_"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[2]);
                game.Clerk(index);
                Tick("clicked sales");
            }
            else if (game.currentAction.Type == ActionType.Tailor && hitTransform.name.StartsWith("TailorHighlight_"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[1]);
                game.TailorReturn(index);
                Tick(reason: "removing deck highlight", tickGame: false);
            }
            else if (game.currentAction.Type == ActionType.Pinwheel && hitTransform.name.StartsWith("TailorHighlight_"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[1]);
                game.Pinwheel(index);
                Tick();
            }
            else if (game.currentAction.Type == ActionType.Tailor && hitTransform.name.StartsWith("CardHighlight_Task_"))
            {
                game.Tailor();
                Tick(reason: "clicked tailor task");
            }
            else if (game.currentAction.Type == ActionType.Doll && hitTransform.name.StartsWith("CardHighlight_Task"))
            {
                int index = int.Parse(hitTransform.name.Split('_')[2]);
                Debug.Log("[gameobject] Clicked task highlight: " + index);
                game.Doll(index);
                Tick("clicked task highlight for doll");
            }
            else if (hitTransform.name.StartsWith("Button_No"))
            {
                Debug.Log("[gameobject] Clicked No button");
                if (game.currentAction.Type == ActionType.Amulet || game.currentAction.Type == ActionType.Bowl || game.currentAction.Type == ActionType.StartCloakGallery || game.currentAction.Type == ActionType.StartCloakGiftShop || game.currentAction.Type == ActionType.Daidoro || game.currentAction.Type == ActionType.DeckOfCards || game.currentAction.Type == ActionType.Gong || game.currentAction.Type == ActionType.Pinwheel)
                {
                    Tick();
                }
                else if (game.currentAction.Type == ActionType.Chopsticks)
                {
                    game.Chopsticks(false);
                    Tick("clicked chopsticks no");
                }
            }
            else if (hitTransform.name.StartsWith("Button_Yes"))
            {
                string item = hitTransform.name.Split("_")[2];
                Debug.Log("[gameobject] Clicked Yes button for item: " + item);
                if (item.ToUpper() == "BELL")
                {
                    game.Bell();
                    Tick("clicked bell");
                }
                else if (item.ToUpper() == "BOWL")
                {
                    game.Bowl();
                    Tick("Clicked bowl");
                }
                else if (item.ToUpper() == "CHOPSTICKS")
                {
                    game.Chopsticks(true);
                    Tick("clicked chopsticks yes");
                }
                else if (item.ToUpper() == "DAIDORO")
                {
                    game.Daidoro();
                    Tick("clicked daidoro");
                }
                else if (item.ToUpper() == "DECK OF CARDS")
                {
                    game.DeckOfCards();
                    Tick("clicked deck of cards");
                }
                else if (item.ToUpper() == "GONG")
                {
                    game.Gong();
                    Tick("clicked gong");
                }
                else if (hitTransform.name.StartsWith("Button_Return"))
                {
                    string item = hitTransform.name.Split("_")[2];
                    if (game.currentAction.Type == ActionType.StartCloakGallery || game.currentAction.Type == ActionType.StartCloakGiftShop)
                    {
                        game.StartCloak();
                    }
                }
            }
    }


    private void UpdateLog()
    {
        Transform log = transform.Find("Canvas/Scroll View/Viewport/Log");
        log.GetComponent<TMPro.TextMeshProUGUI>().text = game.Log;
    }

    private void Refresh()
    {
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].Reposition(i, game.CurrentPlayerIndex);
        }
        DrawFloor();
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].Refresh(i == game.CurrentPlayerIndex);
        }
        HighlightZones(game.Zones);
    }

    private void HighlightZones(List<Zone> zones)
    {
        foreach (Zone zone in zones)
        {
            Debug.Log("Highlighting " + zone.ToString());
            switch (zone.Type)
            {
                case ZoneType.Hand:
                    playerObjects[game.CurrentPlayerIndex].HighlightHand(zone.Value);
                    break;
                case ZoneType.Floor:
                    HighlightFloor(zone.Value);
                    break;
                case ZoneType.Deck:
                    HighlightDeck();
                    break;
                case ZoneType.Gallery:
                    playerObjects[game.CurrentPlayerIndex].HighlightSide(false, zone.Value, zone.Buttons);
                    break;
                case ZoneType.GiftShop:
                    playerObjects[game.CurrentPlayerIndex].HighlightSide(true, zone.Value, zone.Buttons);
                    break;
                case ZoneType.CraftBench:
                    playerObjects[game.CurrentPlayerIndex].HighlightCraftBench(zone.Value);
                    break;
                case ZoneType.TailorReturn:
                    playerObjects[game.CurrentPlayerIndex].HighlightTailorReturn(zone.Value);
                    break;
                case ZoneType.LTask:
                    playerObjects[(game.CurrentPlayerIndex + 1) % 3].HighlightTask((game.CurrentPlayerIndex + 1) % 3);
                    break;
                case ZoneType.RTask:
                    playerObjects[(game.CurrentPlayerIndex + 2) % 3].HighlightTask((game.CurrentPlayerIndex + 2) % 3);
                    break;
                case ZoneType.CTask:
                    playerObjects[game.CurrentPlayerIndex].HighlightTask(game.CurrentPlayerIndex);
                    break;
                default:
                    Debug.Log("Unhandled zone type: " + zone.Type);
                    break;
            }
        }
    }

    private void HighlightFloor(int index)
    {
        Transform floor = transform.Find("Floor");
        Transform highlightTransform = Instantiate(cardHighlightPrefab, new Vector3(0, 0, 0), Quaternion.identity, floor);
        highlightTransform.localPosition = new Vector3(index * 40, 0, 0);
        highlightTransform.name = "CardHighlight_Floor_" + index;
        highlights.Add(highlightTransform);
    }

    private void HighlightDeck()
    {
        Transform deckCount = transform.Find("Canvas/DeckCount");
        Transform highlightTransform = Instantiate(cardHighlightPrefab, new Vector3(0, 0, 0), Quaternion.identity, deckCount);
        highlightTransform.localPosition = new Vector3(0, 0, 0);
        highlightTransform.name = "CardHighlight_Deck";
        highlights.Add(highlightTransform);
    }

    private void ClearHighlights()
    {
        foreach (Transform highlight in highlights)
        {
            Destroy(highlight.gameObject);
        }
        highlights.Clear();
    }
}

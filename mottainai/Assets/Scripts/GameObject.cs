using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameObject : MonoBehaviour
{
    public Transform playerPrefab;
    public Transform cardPrefab;
    public Transform cardHighlightPrefab;
    public string cardsPath;
    public Sprite[] backs;
    public Sprite[] cardSprites;

    private Game game;

    private PlayerObject[] playerObjects;
    private InputAction clickAction;
    private InputAction pointAction;

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
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].Reposition(i, game.CurrentPlayerIndex);
        }

        Tick();
    }

    private void Tick()
    {
        game.Tick();
        Refresh();
        UpdateLog();
    }

    public void Update()
    {
        if (clickAction.WasPerformedThisFrame())
        {
            PerformRaycast();
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
            if (game.currentAction.Type == ActionType.ChooseTask)
            {
                if (hitTransform.name.StartsWith("CardHighlight"))
                {
                    int index = int.Parse(hitTransform.name.Split('_')[2]);
                    game.ChooseTask(index);
                    Tick();
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
                default:
                    Debug.LogWarning("Unhandled zone type: " + zone.Type);
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
    }

    private void HighlightDeck()
    {
        Transform deckCount = transform.Find("Canvas/DeckCount");
        Transform highlightTransform = Instantiate(cardHighlightPrefab, new Vector3(0, 0, 0), Quaternion.identity, deckCount);
        highlightTransform.localPosition = new Vector3(0, 0, 0);
        highlightTransform.name = "CardHighlight_Deck";
    }
}

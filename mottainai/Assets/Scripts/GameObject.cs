using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObject : MonoBehaviour
{
    public Transform playerPrefab;
    public Transform cardPrefab;
    public string cardsPath;
    public Sprite[] backs;
    public Sprite[] cardSprites;

    private Game game;

    private PlayerObject[] playerObjects;
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

    void Tick()
    {
        game.Tick();
        Refresh();
        UpdateLog();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateLog()
    {
        Transform log = transform.Find("Canvas/Scroll View/Viewport/Log");
        log.GetComponent<TMPro.TextMeshProUGUI>().text = game.Log;
    }

    void Refresh()
    {
        DrawFloor();
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].Refresh(i == game.CurrentPlayerIndex);
        }
        HighlightZones(game.Zones);
    }

    void HighlightZones(List<Zone> zones)
    {
        foreach (Zone zone in zones)
        {
            Debug.Log("Highlighting " + zone.ToString());
            switch (zone.Type)
            {
                case ZoneType.Hand:
                    playerObjects[game.CurrentPlayerIndex].HighlightHand(zone.Value);
                    break;
                default:
                    Debug.LogWarning("Unhandled zone type: " + zone.Type);
                    break;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObject : MonoBehaviour
{
    public Transform playerPrefab;
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
            playerObjects[i].Refresh();
            playerObjects[i].Show(i == game.CurrentPlayerIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

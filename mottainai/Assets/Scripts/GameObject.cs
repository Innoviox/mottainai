using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObject : MonoBehaviour
{
    public Transform playerPrefab;
    public string cardsPath;

    private Game game;

    private PlayerObject[] playerObjects;
    private void Awake()
    {
        playerObjects = new PlayerObject[3];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            Transform playerTransform = Instantiate(playerPrefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
            playerObjects[i] = playerTransform.GetComponent<PlayerObject>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        game = new Game(cardsPath);
        game.Deal();

        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].Player = game.Players[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

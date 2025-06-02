using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObject : MonoBehaviour
{
    public Transform playerPrefab;
    public string cardsPath;

    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        game = new Game(cardsPath);
        game.Deal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

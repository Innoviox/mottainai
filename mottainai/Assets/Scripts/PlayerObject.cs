using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public Transform cardPrefab;

    private Player player;
    public Player Player
    {
        get { return player; }
        set { player = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void Refresh()
    {
        // foreach (Transform child in transform)
        // {
        //     Destroy(child.gameObject);
        // }

        // for (int i = 0; i < player.Hand.Count; i++)
        // {
        //     Transform cardTransform = Instantiate(cardPrefab, new Vector3(i * 10, 0, 0), Quaternion.identity, transform);
        //     CardObject cardObject = cardTransform.GetComponent<CardObject>();
        //     cardObject.Card = player.Hand[i];
        //     cardObject.Refresh();
        // }
    }

    public void Reposition(int index, int currentIndex)
    {
        if (index == currentIndex)
        {
            transform.position = new Vector3(0, -50, 0);
            transform.rotation = Quaternion.identity;
        }
        else if (index == (currentIndex + 1) % 3)
        {
            transform.position = new Vector3(-100, 10, 0);
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (index == (currentIndex + 2) % 3)
        {
            transform.position = new Vector3(100, 10, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
}

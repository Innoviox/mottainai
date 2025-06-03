using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public Transform cardPrefab;
    public Transform cardHighlightPrefab;

    private Player player;
    public Player Player
    {
        get { return player; }
        set { player = value; }
    }

    private List<Transform> highlights = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        highlights = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void Refresh(bool active)
    {
        DrawHand(active);

        Transform dummyTask = transform.Find("DummyTask");
        if (player.HasPlayed)
        {
            dummyTask.gameObject.SetActive(false);
            if (player.Temple.Task != null)
            {
                Transform taskTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
                CardObject cardObject = taskTransform.GetComponent<CardObject>();
                cardObject.Card = player.Temple.Task;
                cardObject.transform.localPosition = new Vector3(0, 15, 0);
                cardObject.transform.localRotation = Quaternion.Euler(0, 0, 270);
                cardObject.Refresh();
            }
        }
        else
        {
            dummyTask.gameObject.SetActive(true);
        }

        if (active)
        {
            transform.Find("Canvas/WaitingCount").GetComponent<TMPro.TextMeshProUGUI>().text = player.WaitingArea.Count.ToString();
        }
        else
        {
            transform.Find("Canvas/WaitingCount").GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }

        ClearHighlights();
    }

    public void DrawHand(bool active)
    {
        Transform hand = transform.Find("Hand");
        foreach (Transform child in hand)
        {
            Destroy(child.gameObject);
        }

        if (active)
        {
            for (int i = 0; i < player.Hand.Count; i++)
            {
                Transform cardTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, hand);
                CardObject cardObject = cardTransform.GetComponent<CardObject>();
                cardObject.Card = player.Hand[i];
                cardObject.transform.localPosition = new Vector3(i * 40, 0, 0);
                cardObject.Refresh();
            }
        }
    }

    public void Reposition(int index, int currentIndex)
    {
        if (index == currentIndex)
        {
            transform.position = new Vector3(0, 0, 0);
            transform.rotation = Quaternion.identity;
        }
        else if (index == (currentIndex + 1) % 3)
        {
            transform.position = new Vector3(-145, 10, 0);
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (index == (currentIndex + 2) % 3)
        {
            transform.position = new Vector3(145, 10, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    public void HighlightHand(int index)
    {
        Transform hand = transform.Find("Hand");
        Transform highlightTransform = Instantiate(cardHighlightPrefab, new Vector3(0, 0, 0), Quaternion.identity, hand);
        highlightTransform.localPosition = new Vector3(index * 40, 0, 0);
        highlightTransform.name = "CardHighlight_Hand_" + index;
        highlights.Add(highlightTransform);
    }

    public void ClearHighlights()
    {
        if (highlights == null) return;
        foreach (Transform highlight in highlights)
        {
            if (highlight == null) continue;
            Destroy(highlight.gameObject);
        }
        highlights.Clear();
    }
}

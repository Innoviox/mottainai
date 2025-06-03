using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public Transform cardPrefab;
    public Transform cardHighlightPrefab;
    public Transform tailorButtonPrefab;
    public Transform yesPrefab;
    public Transform noPrefab;
    private Dictionary<Button, Transform> buttonPrefabs = new Dictionary<Button, Transform>();

    private Player player;
    public Player Player
    {
        get { return player; }
        set { player = value; }
    }

    private List<Transform> highlights = new List<Transform>();
    private Transform taskTransform;

    // Start is called before the first frame update
    void Start()
    {
        highlights = new List<Transform>();
        buttonPrefabs[Button.Yes] = yesPrefab;
        buttonPrefabs[Button.No] = noPrefab;
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
        if (taskTransform != null)
        {
            Destroy(taskTransform.gameObject);
            taskTransform = null;
        }
        DrawHand(active);

        Transform dummyTask = transform.Find("DummyTask");
        if (player.HasPlayed)
        {
            dummyTask.gameObject.SetActive(false);
            if (player.Temple.Task != null)
            {
                Debug.Log("Drawing task for player: " + player.Temple.Task.Name);
                taskTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
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


        transform.Find("Canvas/Score").GetComponent<TMPro.TextMeshProUGUI>().text = player.CalculateScore().ToString();
        transform.Find("Canvas/WaitingCount").GetComponent<TMPro.TextMeshProUGUI>().text = player.WaitingArea.Count.ToString();

        DrawTemple(active);

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

    public void DrawTemple(bool active)
    {
        Transform temple = transform.Find("Temple");
        foreach (Transform child in temple)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < player.Temple.Helpers.Count; i++)
        {
            Transform helperTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, temple);
            CardObject cardObject = helperTransform.GetComponent<CardObject>();
            cardObject.Card = player.Temple.Helpers[i];
            cardObject.transform.localPosition = new Vector3(i * -9 - 20, 3.5f, 0);
            cardObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            cardObject.Refresh();
        }

        for (int i = 0; i < player.Temple.Sales.Count; i++)
        {
            Transform saleTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, temple);
            CardObject cardObject = saleTransform.GetComponent<CardObject>();
            cardObject.Card = player.Temple.Sales[i];
            cardObject.transform.localPosition = new Vector3(i * 9 + 13, 3, 0);
            cardObject.Refresh();
        }

        for (int i = 0; i < player.Temple.CraftBench.Count; i++)
        {
            Transform benchTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, temple);
            CardObject cardObject = benchTransform.GetComponent<CardObject>();
            cardObject.Card = player.Temple.CraftBench[i];
            cardObject.transform.localPosition = new Vector3(0, i * -9 - 11, 0);
            cardObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
            cardObject.Refresh();
        }

        for (int i = 0; i < player.Temple.Gallery.Count; i++)
        {
            Transform galleryTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, temple);
            CardObject cardObject = galleryTransform.GetComponent<CardObject>();
            cardObject.Card = player.Temple.Gallery[i];
            cardObject.transform.localPosition = new Vector3(i * 40 - 60, -3, 0);
            cardObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            cardObject.Refresh();
        }

        for (int i = 0; i < player.Temple.GiftShop.Count; i++)
        {
            Transform giftTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, temple);
            CardObject cardObject = giftTransform.GetComponent<CardObject>();
            cardObject.Card = player.Temple.GiftShop[i];
            cardObject.transform.localPosition = new Vector3(i * 40 + 60, -3, 0);
            cardObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            cardObject.Refresh();
        }
    }

    public void HighlightSide(bool side, int value, List<Button> buttons)
    {
        if (buttons == null || buttons.Count == 0)
        {
            Transform highlightTransform = Instantiate(cardHighlightPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
            highlightTransform.localPosition = new Vector3((side ? -1 : 1) * (50 + 40 * value), 0, 0);
            highlightTransform.name = "CardHighlight_Side_" + (side ? 0 : 1);
            highlights.Add(highlightTransform);
        }
        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Button button = buttons[i];
                if (buttonPrefabs.ContainsKey(button))
                {
                    Transform buttonTransform = Instantiate(buttonPrefabs[button], new Vector3(0, 0, 0), Quaternion.identity, transform);
                    buttonTransform.localPosition = new Vector3((side ? 1 : -1) * (50 + 40 * value - 10 * i), 20, 0);
                    buttonTransform.name = "Button_" + button.ToString() + "_" + value;
                    highlights.Add(buttonTransform);
                }
                else
                {
                    Debug.LogWarning("Button prefab not found for: " + button);
                }
            }
        }
    }

    public void HighlightCraftBench(int value)
    {
        Transform craftBench = transform.Find("Temple/CraftBench");
        Transform highlightTransform = Instantiate(cardHighlightPrefab, new Vector3(0, 0, 0), Quaternion.identity, craftBench);
        highlightTransform.localPosition = new Vector3(0, value * -9 - 11, 0);
        highlightTransform.transform.localRotation = Quaternion.Euler(0, 0, -90);
        highlightTransform.name = "CardHighlight_CraftBench_" + value;
        highlights.Add(highlightTransform);
    }

    public void HighlightTailorReturn(int index)
    {
        Transform hand = transform.Find("Hand");
        Transform highlightTransform = Instantiate(tailorButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, hand);
        highlightTransform.localPosition = new Vector3(index * 40, 40, 0);
        highlightTransform.name = "TailorHighlight_" + index;
        highlights.Add(highlightTransform);
    }

    public void HighlightTask()
    {
        Transform highlightTransform = Instantiate(cardHighlightPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        highlightTransform.localPosition = new Vector3(0, 15, 0);
        highlightTransform.name = "CardHighlight_Task_";
        highlights.Add(highlightTransform);
    }
}

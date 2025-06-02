using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    private Card card;
    public Card Card
    {
        get { return card; }
        set { card = value; }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Refresh()
    {
        transform.Find("Canvas/Name").GetComponent<TMPro.TextMeshProUGUI>().text = card.Name.ToUpper();
        transform.Find("Canvas/Description").GetComponent<TMPro.TextMeshProUGUI>().text = card.Description;
        transform.Find("Canvas/Type1").GetComponent<TMPro.TextMeshProUGUI>().text = Utils.MaterialToString(card.Material);
        transform.Find("Canvas/Type2").GetComponent<TMPro.TextMeshProUGUI>().text = Utils.MaterialToString(card.Material);
        transform.Find("Canvas/Type3").GetComponent<TMPro.TextMeshProUGUI>().text = Utils.GetJob(card.Material);
        transform.Find("Canvas/Value").GetComponent<TMPro.TextMeshProUGUI>().text = card.Value.ToString();

        transform.Find("Image").GetComponent<SpriteRenderer>().sprite = card.ImageSprite;
        transform.GetComponent<SpriteRenderer>().sprite = card.BackSprite;
    }
}

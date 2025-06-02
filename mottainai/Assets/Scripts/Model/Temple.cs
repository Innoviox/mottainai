using System.Collections;
using System.Collections.Generic;

public class Temple
{
    private Card? task;
    private List<Card> helpers;
    private List<Card> sales;
    private List<Card> craftBench;
    private List<Card> gallery;
    private List<Card> giftShop;

    public Card? Task
    {
        get { return task; }
        set { task = value; }
    }
}
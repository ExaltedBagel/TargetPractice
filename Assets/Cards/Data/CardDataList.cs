using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CardDataList : ScriptableObject
{
    [SerializeField]
    private List<CardData> cardPList;

    public List<CardData> cardList { get { return cardPList; } set { cardPList = value; } }

    public CardData findByID(string uniqueID)
    {
        foreach (CardData x in cardPList.ToList<CardData>())
        {
            if (x.uniqueId == uniqueID)
                return x;
        }
        return null;
    }
}


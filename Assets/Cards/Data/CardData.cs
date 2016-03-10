using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class CardData : IEquatable<CardData>
{
    public CardData()
    {
        cardName = "New Card";
        //Create a unique ID for the unit
        uniqueID = System.Guid.NewGuid().ToString();
    }

    [SerializeField]
    private string uniqueID;

    public string uniqueId { get { return uniqueID; } }
    public string linkedTo;
    public CardType cardType;

    //Attributes
    public string cardName;
    public int[] manaCost = new int[5];
    public Elements school;
    public int rechargeTime;

    //Infos it might hold depending on the card type
    public int charges;
    public UnitTypes type;
    public EquipType eType;
    public int durability;
    public int[] manaYield = new int[5];
    //Attack it is linked to
    //Creature it is linked to


    public bool Equals(CardData other)
    {
        if (Equals(this, null) && Equals(other, null))
            return true;
        else if (Equals(this, null) | Equals(other, null))
            return false;
        else if (Equals(this.uniqueId == other.uniqueId))
            return true;
        else
            return false;
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as CardData);
    }
    public override int GetHashCode()
    {
        return this.GetHashCode();
    }
}


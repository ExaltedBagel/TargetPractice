using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Card : MonoBehaviour, IEquatable<Card>
{
    //Public functions
    public Card()
    {
        
    }

    public abstract void playCard();

    public void discardCard() { }
    public void shuffleCard() { }

    public virtual void init(CardData card)
    {
        unitDataList = GameObject.Find("References").GetComponent<DBReferences>().unitDataList;
        atkDataList = GameObject.Find("References").GetComponent<DBReferences>().atkDataList;
        manaCost = new int[5];
        this.name = card.cardName;
        for(int i =0; i < manaCost.Length; i++)
        {
            this.manaCost[i] = card.manaCost[i];
            Debug.Log(this.manaCost[i]);
        }
            
        this.school = card.school;
        this.rechargeTime = card.rechargeTime;
        //MaybeChange?
        
    }

    //Attributes
    public string cardName;
    public int[] manaCost;
    public Elements school;
    public int rechargeTime;

    protected bool isPlayable;

    //Databases
    protected static UnitDataList unitDataList;
    protected static AttackDataList atkDataList;

    /*Redefine equalities*/
    public bool Equals(Card other)
    {
        if (Equals(this, null) && Equals(other, null))
            return true;
        else if (Equals(this, null) | Equals(other, null))
            return false;
        else if (this.GetInstanceID() == other.GetInstanceID())
            return true;
        else
            return false;
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as Unit);
    }
    public override int GetHashCode()
    {
        return GetInstanceID();
    }
}
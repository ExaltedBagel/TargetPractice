using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Hero : UnitBase
{
    Hero() : base() { }

    public int memory;
    public int impulse;

    private int[] mPool;
    public int[] manaPool { get { return mPool; } }

    public List<CrystalObject> crystals;
    public List<Card> cardsPlayed;

    public new void init(UnitData uData)
    {
        base.init(uData);
        memory = uData.memory;
        impulse = uData.impulse;
        mPool = new int[5];
        crystals = new List<CrystalObject>();
        cardsPlayed = new List<Card>();
        resetHeroTurn();
    }

    
   
    public void endCasting()
    {
        HarvestedCrystal.resetUsedAlpha();
        UnitAttackHandler.atkReady = false;
        UnitMoveHandler.moveReady = false;
        resetAtkCircle();
        toggleAtkCircle(false);
        if(GameObject.Find("Effector") != null)
            GameObject.Find("Effector").GetComponentInChildren<Effector>().deleteEffector();
        TurnHandler.unitDoneTurn(this);
    }

    public void resetHeroTurn()
    {
        for (int i = 0; i < mPool.Length; i++)
            mPool[i] = 0;
        cardsPlayed = new List<Card>();
        rechargeHeroCrystals();
        clearUnusedCrystals();
    }

    public bool canPlayCard(Card card)
    {
        Debug.Log(card.manaCost[1]);
        if (!payMana(card.manaCost))
            return false;

        if (card.GetType() == typeof(UnitCard))
            return (cardsOfType(card) < Math.Max(memory / 2, 1));
        else if (card.GetType() == typeof(SpellCard))
            return (cardsOfType(card) < Math.Max(impulse / 3, 1));
        else if (card.GetType() == typeof(CrystalCard))
        {
            if (crystals.Count < impulse)
                return (cardsOfType(card) < Math.Max(memory, 1));
        }
        //Equipments
        else
            return (cardsOfType(card) < Math.Max(memory / 2, 1));
        return false;
    }

    /// --------------------------------------------------------------------
    /// MANA THINGS
    /// Return values warn about visual updates to be done
    /// --------------------------------------------------------------------
    public void harvestMana(CrystalObject crystal)
    {

        Debug.Log("Mana harvest!");
        for (int i = 0; i < mPool.Length; i++)
        {
            mPool[i] += crystal.manaYield[i];
            for (int j = 0; j < crystal.manaYield[i]; j++)
                createCrystal((Elements)i);
        }
            

        //If the crystal is depleted, delete it after sending card to Underworld.
        crystal.Used = true;
        crystal.Durability--;
        crystal.setUsedAlpha(true);
        if (crystal.Durability < 1)
        {
            crystals.Remove(crystal);
            crystal.LinkedCard.discardCard();
            Destroy(crystal.gameObject);
        }

        Debug.Log("ManaPool is now: " + mPool[0] + " " + mPool[1] + " " + mPool[2] + " " + mPool[3] + " " + mPool[4]);
    }

    private void createCrystal(Elements sch)
    {
        GameObject crystal = (GameObject)Instantiate(Resources.Load("Mana/HarvestedCrystal") as GameObject, Vector3.zero, Quaternion.identity);
        crystal.GetComponent<HarvestedCrystal>().init(sch);
    }

    public bool payMana(int[] cost)
    {
        Debug.Log("Cost is");
        bool hasMana = true;
        for (int i = 0; i < mPool.Length; i++)
        {
            mPool[i] -= cost[i];
            if (mPool[i] < 0)
                hasMana = false;
        }

        if (!hasMana)
            refundMana(cost);

        return hasMana;
    }

    public void refundMana(int[] cost)
    {
        Debug.Log("Could not play card");
        for (int i = 0; i < mPool.Length; i++)
        {
            mPool[i] += cost[i];
        }
    }

    public void convertMana(Elements sch)
    {
        mPool[(int)sch]--;
        mPool[0]++;
    }

    public void clearHarvestedCrystals(int[] manaCost)
    {
        int[] remainingCost = manaCost;
        //Find all the harvestedCrystals
        HarvestedCrystal[] harvestCrystals = GameObject.Find("HarvestedArea").GetComponentsInChildren<HarvestedCrystal>();
        for (int i = harvestCrystals.Length - 1; i >= 0; i--)
        {
            if (remainingCost[(int)harvestCrystals[i].School] > 0)
            {
                remainingCost[(int)harvestCrystals[i].School]--;
                Destroy(harvestCrystals[i].gameObject);
            }
        }
    }

    public void clearUnusedCrystals()
    {
        //Find all the harvestedCrystals
        HarvestedCrystal[] harvestCrystals = GameObject.Find("HarvestedArea").GetComponentsInChildren<HarvestedCrystal>();
        for (int i = harvestCrystals.Length - 1; i >= 0; i--)
        {     
            Destroy(harvestCrystals[i].gameObject);
        }
    }

    public void rechargeHeroCrystals()
    {
        foreach(CrystalObject x in crystals)
        {
            x.Used = false;
            x.setUsedAlpha(false);
        }
    }

    private int cardsOfType(Card card)
    {
        int n = 0;
        Debug.Log("Card played is " + card.GetType().ToString());
        foreach (Card x in cardsPlayed)
        {
            Debug.Log("Card already played is " + x.GetType().ToString());
            if (x.GetType() == card.GetType())
                n++;
        }
            
        return n;
    }
}


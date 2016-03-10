using System;
using System.Collections.Generic;
using UnityEngine;

public class CrystalCard : Card
{
    public CrystalCard() : base() { }
    public override void init(CardData card)
    {
        base.init(card);
        this.durability = card.durability;
        manaYield = new int[5];

        for (int i = 0; i < manaYield.Length; i++)
            manaYield[i] = card.manaYield[i];
    }
    public override void playCard()
    {
        GameObject crystal = (GameObject)Instantiate(Resources.Load("Mana/EmptyCrystal") as GameObject, Vector3.zero, Quaternion.identity);
        crystal.GetComponent<CrystalObject>().init(this);

        CardPlayerHandler.PlayerCard = this;
        CardPlayerHandler.PlayerCrystal = crystal;

    }

    public int Durability { get { return durability; } set { durability = value; } }

    public int durability;

    public int[] manaYield;
}

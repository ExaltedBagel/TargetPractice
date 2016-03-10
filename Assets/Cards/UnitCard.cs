using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitCard : Card
{
    public UnitCard() : base() {  }
    public override void playCard()
    {
        Debug.Log("PlayingCard!");
        GameObject unit;

        //if (unitData.isHero)
        //    createHero(out unit);
        //else
            createUnit(out unit);

        CardPlayerHandler.PlayerCard = this;
        CardPlayerHandler.PlayerUnit = unit;

        UnitHandler.currentHero.toggleAtkCircle(CardPlayerHandler.SUMMON_RANGE * UnitHandler.currentHero.impulse);
    }

    public void summonHero()
    {
        Debug.Log("PlayingCard!");
        GameObject unit;

        createHero(out unit);

        CardPlayerHandler.PlayerCard = this;
        CardPlayerHandler.PlayerHero = unit; 
    }

    public void createUnit(out GameObject obj)
    {
        obj = Instantiate(Resources.Load("Unit/EmptyUnit") as GameObject);
        obj.SetActive(true);
        obj.GetComponent<Unit>().init(unitData);
    }

    public void createHero(out GameObject obj)
    {
        obj = Instantiate(Resources.Load("Unit/EmptyHero") as GameObject);
        obj.SetActive(true);
        obj.GetComponent<Hero>().init(unitData);
    }

    public override void init(CardData card)
    {
        base.init(card);
        this.unitData = unitDataList.findByID(card.linkedTo);
        
    }

    public UnitData unitData;
    public bool isHero;
}

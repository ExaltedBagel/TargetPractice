using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    public AttackData atkData;
    public AttackObject atkObject;

    public SpellCard() : base()
    {
        
    }

    public override void init(CardData card)
    {
        base.init(card);
        atkData = atkDataList.findByID(card.linkedTo);

    }

    public override void playCard()
    {
        UnitHandler.unit1 = UnitHandler.currentHero;
        Debug.Log("IsPlayable!");
        GameObject attackObject;

        attackObject = Instantiate(Resources.Load("Attack/EmptyAttack") as GameObject);
        attackObject.SetActive(true);
        attackObject.name = "Effector";

        createAttack(attackObject);
        attackObject.GetComponent<AttackObject>().init();

        CardPlayerHandler.PlayerCard = this;
        CardPlayerHandler.PlayerSpell = attackObject;

    }

    public void createAttack(GameObject attackObject)
    {
        switch(atkData.pattern)
        {
            case AttackPattern.SINGLE:
                attackObject.GetComponent<AttackObject>().LinkedAttack = new AttackSingleTarget(atkData);
                break;
            case AttackPattern.BLAST:
                attackObject.GetComponent<AttackObject>().LinkedAttack = new AttackBlastCircle(atkData);
                break;
        }
        Debug.Log(attackObject.GetComponent<AttackObject>().LinkedAttack.aName);
        attackObject.GetComponent<AttackObject>().drawSpellRange();
    }

}

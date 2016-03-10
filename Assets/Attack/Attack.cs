using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Effector could be static? Maybe
[Serializable]
public abstract class Attack
{
    public string aName = "Name";
    private List<DamageType> dmgTypes = new List<DamageType>();

    protected AttackPattern attackPattern;
    public AttackPattern attackPatt { get { return attackPattern; } }

    /*******************************/
    //DAMAGE STATS
    //
    // Roll: 0-100, chance of success per die
    // dmg: Value on a success
    // nDice: Ammount of dice rolled
    //
    /*******************************/
    public float range = 5f;

    public int roll = 50;
    public int dmg = 2;
    public int nDice = 5;

    protected string type;
   
    public Attack(AttackData atk)
    {
        this.aName = atk.aName;
        this.range = atk.range;
        this.dmg = atk.dmg;
        this.roll = atk.roll;
        this.nDice = atk.nDice;

        foreach (DamageType x in atk.dmgTypes)
            dmgTypes.Add(x);
    }

    public abstract void setTargets(List<UnitBase> targets);
    public abstract void applyAttack();
    public abstract void launchAttack();

    public void cancelAttack()
    {
        UnitAttackHandler.atkReady = false;
        UnitMoveHandler.moveReady = false;
        UnitHandler.canSelect = true; // You can make another action
        UnitHandler.unit1.toggleAtkCircle(false);
        if(GameObject.Find("Effector") != null)
            GameObject.Find("Effector").GetComponentInChildren<Effector>().deleteEffector();
    }

    protected void endAttack()
    {
        cancelAttack();
        TurnHandler.unitDoneTurn(UnitHandler.unit1);
    }

    public List<DamageType> getTypes()
    {
        return dmgTypes;
    }

}

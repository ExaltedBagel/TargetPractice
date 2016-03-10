using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**************************************************/
/*BLAST AREA TARGET*/
/**************************************************/
public class AttackBlastCircle : Attack
{
    public float radius;

    List<UnitBase> targets = new List<UnitBase>();

    public AttackBlastCircle(AttackData atkData) : base(atkData) { this.radius = atkData.radius; this.attackPattern = AttackPattern.BLAST; }

    /**************************************************/
    /*Applying attacks*/
    /**************************************************/
    public override void setTargets(List<UnitBase> targets)
    {
        this.targets = targets;
    }

    public override void applyAttack()
    {
        foreach (UnitBase x in targets)
        {
            UnitHandler.unit1.attackUnit(x, this);
        }
    }

    public override void launchAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitAttackHandler.indicator.value))
        {
            bool isFound = false;
            //Must have clicked in the target circle
            //Check all the y that are sane for the attack circle
            for(int i= -20; i < 0 && !isFound; i++)
            {
                Debug.Log(hit.point);
                if (UnitHandler.unit1.atkCircle.GetComponent<Collider>().bounds.Contains(hit.point + new Vector3(0, 0.1f * i, 0)))
                    isFound = true;
            }
            //Not in attack circle
            if (!isFound)
                return;
            //if (UnitHandler.unit1.atkCircle.GetComponent<Collider>().bounds.Contains(hit.point - new Vector3(0,0.01f,0))) // == "AtkCircle")
            //{
                targets = Effector.getTargets();
                if (targets.Count != 0)
                {
                    applyAttack();
                    endAttack();
                }
            //}
        }
    }
}
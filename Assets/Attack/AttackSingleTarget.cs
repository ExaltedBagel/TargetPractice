using System;
using UnityEngine;
using System.Collections.Generic;


/**************************************************/
/*SINGLE TARGET*/
/**************************************************/
[Serializable]
public class AttackSingleTarget : Attack
{
    UnitBase target;

    public AttackSingleTarget(AttackData atkData) : base(atkData) { this.attackPattern = AttackPattern.SINGLE; }

    /**************************************************/
    /*Applying attacks*/
    /**************************************************/
    public override void setTargets(List<UnitBase> targets) { if(targets != null && targets.Count > 0)this.target = targets[0]; Debug.Log("Target Set"); }

    public override void applyAttack(){ UnitHandler.unit1.attackUnit(target, this); }

    public override void launchAttack()
    {
        target = null;
        Debug.Log("DEBUG: LaunchAttack Single Target");
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitHandler.currentEnnemyLayer))
        {
            Debug.Log("AttackConnected!");
            Debug.Log(hit.collider.transform.root.gameObject.GetComponent<UnitBase>().uName);
            target = UnitHandler.units.Find(x => x.getId() == hit.collider.transform.root.gameObject.GetInstanceID());
            target.printName();
            //If we have an active unit and an active target
            if (!(Equals(UnitHandler.unit1, null)))
            {
                //Unit 1 attacks his target
                setTargets(Effector.getTargets());
                if(target!= null)
                    UnitHandler.unit1.attackUnit(target, this);
                endAttack();
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAttackHandler : MonoBehaviour {

    public static bool atkReady;
    public static List<Unit> targetList = new List<Unit>();

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        /*********************************************************/
        /*ATTACK COMMAND ON RIGHT CLICKED TARGET */
        /*********************************************************/
        if (Input.GetMouseButtonDown(1))
        {
            if (atkReady)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitHandler.team2))
                {
                    UnitHandler.unit2 = UnitHandler.units.Find(x => x.getId() == hit.collider.gameObject.GetInstanceID());
                    UnitHandler.unit2.printName();

                    if (!(Equals(UnitHandler.unit1, null)) && UnitAttackHandler.targetList.Contains(UnitHandler.unit2))
                    {
                        UnitAttackHandler.atkReady = false;
                        UnitMoveHandler.moveReady = false;

                        UnitHandler.unit1.attackUnit(UnitHandler.unit2);
                        UnitHandler.unit1.toggleAtkCircle(false);

                        if (UnitHandler.unit1.hasActed && UnitHandler.unit1.hasMoved)
                        {
                            Debug.Log("Unit no longer in bucket");
                            TurnHandler.removeFromBucket(UnitHandler.unit1, UnitHandler.unit1.team);
                        }
                            
                        DebugHUDHandler.showTeam1();
                    }
                }
            }
        }
    }

    /**************************************************/
    /*ATTACK A UNIT*/
    /*************************************************/
    public void toggleAtk()
    {
        if (!UnitHandler.unit1.hasActed)
        {
            UnitHandler.unit1.init(); //THIS SHOULD BE REMOVED WHEN INIT IS BETTER HANDLED
            UnitHandler.unit1.toggleAtkCircle(true);
            listTargets();
            if (targetList.Count != 0)
                atkReady = true;
            else
                UnitHandler.unit1.toggleAtkCircle(false);
        }
        else
            Debug.Log("Unit has already acted");
    }

    /**************************************************/
    /*MAKES LIST OF ENNEMY TARGETS*/
    /*************************************************/
    public void listTargets()
    {
        targetList.Clear();
        Collider[] targets = UnitHandler.unit1.overlapArea(UnitHandler.team2);

        foreach (Collider target in targets)
        {
            //Add targets to the target list
            Unit newTarget = UnitHandler.units.Find(x => x.getId() == target.gameObject.GetInstanceID());

            //Only add new targets to the list
            if (!targetList.Contains(newTarget))
                targetList.Add(newTarget);
        }

        targetList.RemoveAll(x => !x.tryRays(UnitHandler.unit1.unit));
        UnitListUI.updateText();
        Debug.Log(targetList.Count);

    }

    /*********************************************************/
    /*BUTTON FUNCTION TO READY ATTACK */
    /*********************************************************/
    public void attackUnit()
    {
        if (!(Equals(UnitHandler.unit1, null)))
        {
            if (!UnitHandler.unit1.hasActed)
            {
                Debug.Log("AttackReady");
                atkReady = true;
            }
            else
                Debug.Log("Unit has no more attacks left");
        }

    }


}

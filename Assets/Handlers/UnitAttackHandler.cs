using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnitAttackHandler : MonoBehaviour
{
    public static bool atkReady;

    public static LayerMask indicator = (1 << 13);

    private static Attack currentAtk;
    private GameObject newAtk;
    private AttackObject newAtkScript;
    public List<UnitBase> targetList;

    public Canvas canvas;

    LayerMask targettedTeam;

    int atkIndex;

    // Use this for initialization
    void Start()
    {
        targetList = new List<UnitBase>();
        //showAttackList();
    }

    // Update is called once per frame
    void Update()
    {

        /*********************************************************/
        /*ATTACK COMMAND ON RIGHT CLICKED TARGET */
        /*********************************************************/
        if (atkReady)  
        {
            if (Input.GetMouseButtonDown(1))
            {
                
                if(Effector.getTargets().Count > 0)
                {
                    currentAtk.launchAttack();
                }
                    
            }
            else if(Input.GetMouseButtonDown(0))
            {
                currentAtk.cancelAttack();
            }
        }
        
    }

    /**************************************************/
    /*ATTACK A UNIT*/
    /*************************************************/
    public void toggleAtk(int atkNumb)
    {
        if (!Equals(UnitHandler.unit1, null) && !UnitHandler.unit1.hasActed && UnitHandler.canSelect && !atkReady)
        {
            //Get the current attack
            currentAtk = UnitHandler.unit1.attacks[atkNumb];

            //Create empty attack and populate it
            Debug.Log("Creating AttackObject!");
            newAtk = Instantiate(Resources.Load("Attack/EmptyAttack") as GameObject);
            newAtk.SetActive(true);
            newAtk.name = "Effector";
            //Link the attack to create the attack sphere
            newAtkScript = newAtk.GetComponent<AttackObject>();
            newAtkScript.LinkedAttack = currentAtk;
            newAtkScript.init();

            //Resize and show the attack circle range
            UnitHandler.unit1.atkCircle.transform.localScale = new Vector3(currentAtk.range, 0.2f, currentAtk.range);
            //Put it at last waypoint position            
            UnitHandler.unit1.toggleAtkCircle(true);

            //Check if the attack is a blast attack
            if(UnitHandler.unit1.attacks[atkNumb] is AttackBlastCircle)
            {
                //Toggle the area circle for the blast attack
                atkReady = true;
                //targettingCircle.GetComponent<TargetCircle>().toggleTargetBlastCircle((AttackBlastCircle)currentAtk);
            }
            else
            {
                //List the targets
                if(listTargets())
                    atkReady = true;
                else
                {
                    UnitHandler.unit1.toggleAtkCircle(false);
                    newAtk.GetComponentInChildren<Effector>().deleteEffector();
                }
                    
            }
            
        }
        else
            Debug.Log("Unit has already acted");
    }  

    /**************************************************/
    /*MAKES LIST OF ENNEMY TARGETS*/
    /*************************************************/
    public bool listTargets()
    {
        targetList.Clear();
        Collider[] targets;

        if (UnitHandler.unit1.team == 1)
        {
            targets = UnitHandler.unit1.overlapArea(UnitHandler.team2);
        }
        else
        {
            targets = UnitHandler.unit1.overlapArea(UnitHandler.team1);
        }

        if (targets.Length == 0)
            return false;

        foreach (Collider target in targets)
        {
            Debug.Log("Colliders found x " + target.gameObject.name + " " + target.gameObject.GetInstanceID());
            //Add targets to the target list
            UnitBase newTarget = UnitHandler.units.Find(x => x.getId() == target.gameObject.GetInstanceID());
            Debug.Log("Target added " + newTarget.uName);
            //Only add new targets to the list
            if (!targetList.Contains(newTarget))
            {
                targetList.Add(newTarget);
                Debug.Log("Target added " + targetList.Count);
            }


        }

        targetList.RemoveAll(x => !x.tryRays(UnitHandler.unit1.unit, UnitHandler.currentEnnemyLayer));
        if (targetList.Count == 0)
            return false;
        else
            return true;

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

    /**********************************************************/
    /*SHOW ATTACK LIST */
    /**********************************************************/
    public void showAttackList()
    {

    }

    /**********************************************************/
    /*LAUNCH ATTACKS */
    /**********************************************************/

    public void launchAttack(int atkNumber)
    {
        atkIndex = atkNumber;
        toggleAtk(atkNumber);
    }

    public static void showCircle(int atkNumb)
    {
        Debug.Log("Showing Circle");
        if (!Equals(UnitHandler.unit1, null) && !UnitHandler.unit1.hasActed)
        {
            //Resize and show the attack circle
            UnitHandler.unit1.atkCircle.transform.localScale = new Vector3(UnitHandler.unit1.attacks[atkNumb].range, 0.2f, UnitHandler.unit1.attacks[atkNumb].range);
            UnitHandler.unit1.atkCircle.transform.position = UnitMoveHandler.lastWayPoint();
            UnitHandler.unit1.toggleAtkCircle(true);

        }
    }

    public static void clearAttack()
    {
       if(currentAtk != null)
        {
            currentAtk.cancelAttack();
            currentAtk = null;
        }
            
    }
}

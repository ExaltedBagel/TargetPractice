using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class UnitHandler : MonoBehaviour
{

    static public List<Unit> units = new List<Unit>();
    LayerMask team1;
    LayerMask team2;

    Unit unit1;
    Unit unit2;
    static public List<Unit> targetList = new List<Unit>();
    static public List<Unit> listTeam1 = new List<Unit>();
    static public List<Unit> listTeam2 = new List<Unit>();
    EventSystem ev;

    bool moveReady;
    bool atkReady;

    static LineRenderer unitPathRender;

    void Awake()
    {
        ev = this.GetComponentInParent<EventSystem>();
        ev.GetComponentInParent<PhysicsRaycaster>();
        unitPathRender = GetComponentInChildren<LineRenderer>();
        moveReady = false;
        team1 = (1 << 10);
        team2 = (1 << 11);
    }
	
	// Update is called once per frame
	void Update () {

        //Selection
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;

            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, team1))
            {
                unit1 = units.Find(x => x.getId() == hit.collider.gameObject.GetInstanceID());
                unit1.printName();

            }
            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                unit1 = null;
                Debug.Log("Unselected");
            }
            else
            {
                Debug.Log("OverMenu");
            }
            
        }
        //Debugs
        else if (Input.GetMouseButtonDown(1))
        {
            //DEBUG
            foreach (Unit target in targetList)
                Debug.Log( target.unit.name + " " + target.getId());


            if (!(Equals(unit1, null)) && moveReady)
            {
                atkReady = false;
                moveReady = false;
                unit1.moveUnit();
            }

            if(atkReady)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, team2))
                {
                    unit2 = units.Find(x => x.getId() == hit.collider.gameObject.GetInstanceID());
                    unit2.printName();

                    if (!(Equals(unit1, null)) && targetList.Contains(unit2))
                    {
                        atkReady = false;
                        moveReady = false;
                        unit1.attackUnit(unit2);
                        unit1.toggleAtkCircle(false);
                    }
                }
            }
            
        }
        
        /*********************************************/
        /*MOVING INDICATOR --- Ca fuck ton curseur si le move est illégal. Reste a trouver un model de pointeurs legal XD*/
        /*********************************************/
        if(moveReady)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (0 < unit1.CalculatePathLength(hit.point) - unit1.speed)
                    CursorHandler.cursorNo();
                else
                    CursorHandler.cursorStd();


            }
        }

        if (!(Equals(unit1, null)))
            if (unit1.moving)
            {
                unit1.updatePos();
            }
                
    }

    /********************************************/
    /*PROTOTYPES*/
    /********************************************/

    public static void addToUnitList(Unit newUnit)
    {
        units.Add(newUnit);
        DebugHUDHandler.updateText();
    }

    public void moveUnit()
    {
        if(!(Equals(unit1, null)))
        {
            Debug.Log("MoveReady");
            moveReady = true;
        }
           
    }

    public void attackUnit()
    {
        if (!(Equals(unit1, null)))
        {
            Debug.Log("AttackReady");
            atkReady = true;
        }

    }

    /**************************************************/
    /*ATTACK A UNIT*/
    /*************************************************/


    public void toggleAtk()
    {
        unit1.init();
        unit1.toggleAtkCircle(true);
        listTargets();
        if (targetList.Count != 0)
            atkReady = true;
        else
            unit1.toggleAtkCircle(false);
    }



    public void listTargets()
    {
        targetList.Clear();
        Collider[] targets = unit1.overlapArea(team2);
        
        foreach (Collider target in targets)
        {
            //Add targets to the target list
            Unit newTarget = units.Find(x => x.getId() == target.gameObject.GetInstanceID());

            //Only add new targets to the list
            if(!targetList.Contains(newTarget))
                targetList.Add(newTarget);
        }

        targetList.RemoveAll( x => !x.tryRays(unit1.unit));
        UnitListUI.updateText();
        Debug.Log(targetList.Count);       

    }

    /**********************************************************/
    /*SORT TEAMS*/
    /**********************************************************/
    public void makeTeams()
    {
        listTeam1 = new List<Unit>();
        listTeam2 = new List<Unit>();

        foreach(Unit x in units)
        {
            if(x.team == 1)
                listTeam1.Add(x);
        }
        
        DebugHUDHandler.showTeam1();
}


    /*********************************************************/
    /*FLUFF */ 
    /*********************************************************/
    public static void renderPath(Vector3[] points)
    {
        unitPathRender.SetVertexCount(points.Length);

        for(int i = 0; i < points.Length; i++)
        {         
            unitPathRender.SetPosition(i, points[i] + new Vector3(0, 0.5f, 0));
        }
    }

    public static void erasePath()
    {
        unitPathRender.SetVertexCount(0);
    }

}

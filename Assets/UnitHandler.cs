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
                renderPathableArea(unit1);

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
                DebugHUDHandler.showTeam1();
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
                        DebugHUDHandler.showTeam1();
                    }
                }
            }
            
        }
        
        /*********************************************/
        /*MOVING INDICATOR --- Ca fuck ton curseur si le move est illégal. Reste a trouver un model de pointeurs legal XD*/
        /*********************************************/
        if(moveReady && !(unit1.hasActed && unit1.hasMoved))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                unit1.CalculatePathLength(hit.point);
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
            if (!(unit1.hasActed && unit1.hasMoved))
            {
                Debug.Log("MoveReady");
                moveReady = true;
            }
            else
                Debug.Log("Unit has no more actions left");
        }
           
    }

    public void attackUnit()
    {
        if (!(Equals(unit1, null)))
        {
            if (!unit1.hasActed )
            {
                Debug.Log("AttackReady");
                atkReady = true;
            }
            else
                Debug.Log("Unit has no more attacks left");
        }

    }

    /**************************************************/
    /*ATTACK A UNIT*/
    /*************************************************/


    public void toggleAtk()
    {
        if (!unit1.hasActed)
        {
            unit1.init();
            unit1.toggleAtkCircle(true);
            listTargets();
            if (targetList.Count != 0)
                atkReady = true;
            else
                unit1.toggleAtkCircle(false);
        }
        else
            Debug.Log("Unit has already acted");
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
            if (x.team == 1)
                listTeam1.Add(x);
            else
                listTeam2.Add(x);
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


    public static void renderPathableArea(Unit unit1)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        
        
        for (int angle = 0; angle < 360; angle += 5)
        {
            bool pointFound = false;
            float effectiveSpeed = unit1.speed;
            while (!pointFound)
            {
                Vector3 origin = (unit1.unit.transform.position + ((Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * effectiveSpeed) + Vector3.up * 10));
                Ray checker = new Ray(origin, Vector3.down);
                RaycastHit hit;
                int worldLayer = 1 << 8;
                if (Physics.Raycast(checker, out hit, Mathf.Infinity, worldLayer))
                {
                    Vector3 dest = unit1.CalculatePathEndPoint(hit.point);
                    if (dest != new Vector3(0, -100f, 0))
                    {
                        pathPoints.Add(unit1.CalculatePathEndPoint(hit.point));
                        Debug.DrawLine(unit1.unit.transform.position, dest, Color.red, 10f);
                        pointFound = true;
                    }
                    //If point is not found or innaccessible, try closer
                    else if (effectiveSpeed > 0.1f)
                    {
                        effectiveSpeed -= unit1.speed * 0.1f;
                    }
                    else
                    {
                        Debug.Log("point not accessible");
                        pointFound = true;
                    }
                }
                //Point may be off the map also
                else if (effectiveSpeed > 0.1f)
                {
                    effectiveSpeed -= unit1.speed * 0.1f;
                }
                else
                {
                    Debug.Log("point not accessible");
                    pointFound = true;
                }
            }
        }

    }



    public static void erasePath()
    {
        unitPathRender.SetVertexCount(0);
    }

}

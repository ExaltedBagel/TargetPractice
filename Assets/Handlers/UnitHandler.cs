using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class UnitHandler : MonoBehaviour
{

    //HandlerLinks
    public UnitAttackHandler unitAttackHandler;
    public UnitMoveHandler unitMoveHandler;

    static public List<UnitBase> units = new List<UnitBase>();
    static public LayerMask team1 = (1 << 10);
    static public LayerMask team2 = (1 << 11);
    static public LayerMask unitsLayer = ((1 << 10)| (1 << 11));
    static public LayerMask currentAllyLayer;
    static public LayerMask currentEnnemyLayer;
    static public LayerMask worldLayer = (1 << 8);
    static public LayerMask currentPathLayer;

    static public UnitBase unit1;
    static public UnitBase unit2;
    static public Hero hero1;
    static public Hero hero2;
    static public Hero currentHero;
    static public List<UnitBase> listTeam1 = new List<UnitBase>();
    static public List<UnitBase> listTeam2 = new List<UnitBase>();

    EventSystem ev;

    //Selection Things
    public Material mouseOver;
    public Material selected;

    public GameObject selector;
    private Selector selectScript;

    static public bool canSelect = true;

    static public LineRenderer unitPathRender;

    void OnEnable()
    {
        Debug.Log("Unit Handler turned on");
        ev = this.GetComponentInParent<EventSystem>();
        ev.GetComponentInParent<PhysicsRaycaster>();
        unitPathRender = GetComponentInChildren<LineRenderer>();
        Instantiate(selector);
        selectScript = selector.GetComponent<Selector>();
    }

    // Update is called once per frame
    void Update()
    {

        //Selection
        if (Input.GetMouseButtonDown(0))
        {
            //If no units are selected
            if(Equals(Selector.selected, null))
            {
                //If we are hovering a possible unit
                if(Selector.hovered != null)
                {
                    //Hover Over a friendly unit shows the selection circle
                    int layerValue = (1 << Selector.hovered.layer);
                    if ((layerValue & currentAllyLayer.value) > 0)
                    {
                        unit1 = Selector.hovered.GetComponent<UnitBase>();
                        selectScript.setSelected(unit1.unit);
                        AtkHUD.setAttackHUD(unit1);
                    }
                }
                //TODO
                //Sets the button names and activates them
            }

            //UnitPreviously selected may be unselected only if no actions have been taken already
            else if (!EventSystem.current.IsPointerOverGameObject() && !Equals(unit1, null) && !unit1.hasActed && !unit1.hasMoved)
            {
                //Immediatly Select another unit
                if (Selector.hovered != null)
                {
                    //Hover Over a friendly unit shows the selection circle
                    int layerValue = (1 << Selector.hovered.layer);
                    if ((layerValue & currentAllyLayer.value) > 0)
                    {
                        Selector.clearTargetted(UnitHandler.unit1);
                        unit1 = Selector.hovered.GetComponent<UnitBase>();
                        selectScript.setSelected(unit1.unit);
                    }
                }

                //We want to unselect a selected unit
                clearUnit(UnitHandler.unit1);
                Debug.Log("Unselected");
            }
            //Mouse is over menu
            else
            {
                Debug.Log("OverMenu!");

            }
        }

    }

    /********************************************/
    /*ADDS A NEW UNIT TO THE UNIT LIST*/
    /********************************************/

    public static void addToUnitList(UnitBase newUnit)
    {
        if(TurnHandler.isTeam1Turn)
            addToUnitList(newUnit, 1);
        else
            addToUnitList(newUnit, 2);
    }

    public static void addToUnitList(UnitBase newUnit, int team)
    {
        if (team == 1)
        {
            newUnit.team = 1;
            newUnit.unit.layer = 10;
            if (newUnit.GetType() == typeof(Hero))
                hero1 = (Hero)newUnit;
        }
        else
        {
            newUnit.team = 2;
            newUnit.unit.layer = 11;
            if (newUnit.GetType() == typeof(Hero))
                hero2 = (Hero)newUnit;
        }
        //newUnit.unit.transform.FindChild("SelectCircle").gameObject.layer = newUnit.unit.layer;
        units.Add(newUnit);
    }
    /********************************************/
    /*REMOVES A NEW UNIT TO THE UNIT LIST*/
    /********************************************/
    public static void removeFromUnitList(UnitBase removedUnit)
    {
        TurnHandler.removeFromBucket(removedUnit, removedUnit.team);
        units.Remove(removedUnit);
//        DebugHUDHandler.updateText();
    }


    /**********************************************************/
    /*SORT TEAMS*/
    /**********************************************************/
    public static void makeTeams()
    {
        listTeam1 = new List<UnitBase>();
        listTeam2 = new List<UnitBase>();

        foreach (UnitBase x in units)
        {
            if (x.team == 1 && !listTeam1.Contains(x))
            {
                listTeam1.Add(x);
            }
            else if (x.team == 2 && !listTeam2.Contains(x))
            {
                listTeam2.Add(x);
            }

        }
        if (listTeam1.Count == 0 || listTeam2.Count == 0)
            Debug.Log("Only one team remaining on the field");

        //DebugHUDHandler.showTeam1();
    }

    /**********************************************/
    /*WAIT*/
    /**********************************************/
    public void unitWait()
    {
        if (!Equals(unit1, null) && canSelect)
        {
            endUnitTurn(unit1);
        }
    }

    //Clears Selection of unit1
    public static void clearUnit(UnitBase unit)
    {
        if (Equals(unit, unit1))
        {
            UnitAttackHandler.clearAttack();
            Selector.clearTargetted(unit);
            unit1 = null;
            AtkHUD.hideAttackHUD();
        }
    }

    public static void endUnitTurn(UnitBase unit)
    {
        Debug.Log("Unit wait");
        Debug.Log("Unit no longer in bucket");
        unit.hasActed = true;
        unit.hasMoved = true;
        TurnHandler.removeFromBucket(unit, unit.team);
        clearUnit(unit);
    }

    public static void erasePath()
    {
        unitPathRender.SetVertexCount(0);
    }

}
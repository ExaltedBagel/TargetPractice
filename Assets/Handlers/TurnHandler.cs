using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnHandler : MonoBehaviour {

    //Buckets hold units which have still not been activated for action
    static public List<UnitBase> Team1Bucket = new List<UnitBase>();
    static public List<UnitBase> Team2Bucket = new List<UnitBase>();

    static bool newTurn = true;
    static public bool isTeam1Turn = true;

    // Use this for initialization
    void OnEnable () {
        newTurn = true;
        Debug.Log("Turn Handler On!");
    }
	
	// Update is called once per frame
	void Update () {
	    if(newTurn)
        {
            Debug.Log("New Turn!");

            //If the teams were not made, make the teams
            UnitHandler.makeTeams();
            fillBuckets();
            resetUnitActions();
            //DebugHUDHandler.showBucket1();

            //Set the layers for raycasts selections
            if (isTeam1Turn)
            {
                UnitHandler.currentAllyLayer = UnitHandler.team1;
                UnitHandler.currentEnnemyLayer = UnitHandler.team2;
                UnitHandler.currentPathLayer = UnitHandler.team2 | (1 << 12);
                UnitHandler.currentHero = UnitHandler.hero1;
            }
                
            else
            {
                UnitHandler.currentAllyLayer = UnitHandler.team2;
                UnitHandler.currentEnnemyLayer = UnitHandler.team1;
                UnitHandler.currentPathLayer = UnitHandler.team1 | (1 << 12);
                UnitHandler.currentHero = UnitHandler.hero2;
            }

            UnitHandler.currentHero.resetHeroTurn();
            newTurn = false;
        }
	}

    public static void fillBuckets()
    {
        for (int i = 0; i < UnitHandler.listTeam1.Count; i++)
            if(!Team1Bucket.Contains(UnitHandler.listTeam1[i]))
                Team1Bucket.Add(UnitHandler.listTeam1[i]);
        for (int i = 0; i < UnitHandler.listTeam2.Count; i++)
            if (!Team2Bucket.Contains(UnitHandler.listTeam2[i]))
                Team2Bucket.Add(UnitHandler.listTeam2[i]);
    }

    public static void addToBucket(UnitBase unitAdded, int team)
    {
        if (team == 1)
            Team1Bucket.Add(unitAdded);
        else
            Team2Bucket.Add(unitAdded);
    }

    

    /**********************************************/
    /*REMOVE UNITS FROM TEAM BUCKETS*/
    /*New turn starts for other team when all moves are done -- Might become a new function*/
    /********************************************************/
    public static void removeFromBucket(UnitBase unitRemoved, int team)
    {
        if (team == 1)
        {
            Team1Bucket.Remove(unitRemoved);
            if (Team1Bucket.Count == 0)
            {
                Debug.Log("No more units in 1");
                if (isTeam1Turn)
                    teamSwitch();
            }
        }
        else
        {
            Team2Bucket.Remove(unitRemoved);
            if (Team2Bucket.Count == 0)
            {
                Debug.Log("No more units in 2");
                if(!isTeam1Turn)
                    teamSwitch();
            }     
        }      
    }

    /************************************/
    /*TEAM SWITCH*/
    /************************************/
    private static void teamSwitch()
    {
        //Start a new turn!
        Debug.Log("Switching turns");
        isTeam1Turn = !isTeam1Turn;
        //UnitHandler.clearUnit();
        newTurn = true;
    }

    public static void resetUnitActions()
    {
        for(int i=0; i< UnitHandler.units.Count; i++)
        {
            UnitHandler.units[i].hasActed = false;
            UnitHandler.units[i].hasMoved = false;
        }
    }

    public static bool unitDoneTurn(UnitBase unit)
    {
        if (unit.hasActed && unit.hasMoved)
        {
            Debug.Log("Unit no longer in bucket");
            UnitHandler.endUnitTurn(unit);
            return true;
        }
        return false;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnHandler : MonoBehaviour {

    //Buckets hold units which have still not been activated for action
    static public List<Unit> Team1Bucket = new List<Unit>();
    static public List<Unit> Team2Bucket = new List<Unit>();

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void fillBuckets()
    {
        for (int i = 0; i < UnitHandler.listTeam1.Count; i++)
            Team1Bucket.Add(UnitHandler.listTeam1[i]);
        for (int i = 0; i < UnitHandler.listTeam2.Count; i++)
            Team2Bucket.Add(UnitHandler.listTeam2[i]);
    }

    public static void addToBucket(Unit unitAdded, int team)
    {
        if (team == 1)
            Team1Bucket.Add(unitAdded);
        else
            Team2Bucket.Add(unitAdded);
    }

    public static void removeFromBucket(Unit unitRemoved, int team)
    {
        if (team == 1)
        {
            Team1Bucket.Remove(unitRemoved);
            if (Team1Bucket.Count == 0)
                Debug.Log("No more units in 1");
        }
        else
        {
            Team2Bucket.Remove(unitRemoved);
            if (Team2Bucket.Count == 0)
                Debug.Log("No more units in 2");
        }
           
    }
}

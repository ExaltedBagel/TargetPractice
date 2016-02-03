using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugHUDHandler : MonoBehaviour {

    static Text windowText;

    // Use this for initialization
    void Awake()
    {
        windowText = GetComponent<Text>();
    }

    public void bucketUp()
    {
        showBucket1();
    }

    // Update is called once per frame
    static public void updateText()
    {
        string unitsInScene = "Unit List: \n";
        foreach (Unit unit in UnitHandler.units)
            unitsInScene += (unit.uName + "\n");
        windowText.text = unitsInScene;
    }


    static public void showTeam1()
    {
        string team1 = "Team1 List: \n";
        foreach (Unit unit in UnitHandler.listTeam1)
        { 
            team1 += (unit.uName);
            if (!unit.hasMoved)
                team1 += " M";
            if (!unit.hasActed)
                team1 += " A";
            team1 += "\n";

        }
        windowText.text = team1;
    }

    static public void showTeam2()
    {
        string team2 = "Team2 List: \n";
        foreach (Unit unit in UnitHandler.listTeam2)
        {
            team2 += (unit.uName);
            if (!unit.hasMoved)
                team2 += " M";
            if (!unit.hasActed)
                team2 += " A";
            team2 += "\n";

        }
        windowText.text = team2;
    }

    static public void showBucket1()
    {
        string buck1 = "Buck1 List: \n";
        foreach (Unit unit in TurnHandler.Team1Bucket)
        {
            buck1 += (unit.uName);
            buck1 += "\n";
        }

    windowText.text = buck1;
    }
}


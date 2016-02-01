using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugHUDHandler : MonoBehaviour {

    static Text windowText;

    // Use this for initialization
    void Start()
    {
        windowText = GetComponent<Text>();
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
            team1 += (unit.uName + "\n");
        windowText.text = team1;
    }
}


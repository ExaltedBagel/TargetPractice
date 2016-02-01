using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitListUI : MonoBehaviour {

    static Text windowText;

	// Use this for initialization
	void Start () {
        windowText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	static public void updateText () {
        string badGuys = "Target List: \n";
        foreach (Unit baddie in UnitAttackHandler.targetList)
            badGuys += (baddie.uName + "\n");
        windowText.text = badGuys;
    }
}

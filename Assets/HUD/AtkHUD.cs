using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AtkHUD : MonoBehaviour {

    static public Button[] buttons = new Button[3];
    static int activeButtons = 0;

	// Use this for initialization
	void Start () {
	    for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = this.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
	}
	
    public static void setAttackHUD(UnitBase unit)
    {
        activeButtons = 0;
        foreach(Attack x in unit.attacks)
        {
            setButtonText(activeButtons, unit.attacks[activeButtons].aName);
            setButtonActive(activeButtons, true);
            activeButtons++;
        }
    }

    public static void hideAttackHUD()
    {
        while(activeButtons>0 )
        {
            activeButtons--;
            setButtonActive(activeButtons, false);
        }
    }

    private static void setButtonActive(int index, bool state)
    {
        buttons[index].gameObject.SetActive(state);
    }

    public static void setButtonText(int index, string name)
    {
        
        Text text = buttons[index].transform.GetChild(0).gameObject.GetComponent<Text>();
        text.text = name;

            
    }

    // Update is called once per frame
    public static int getNActive()
    {
        return activeButtons;
    }
}

using UnityEngine;
using System.Collections;

public class MatchStarter : MonoBehaviour {

	// Update is called once per frame
	void Start () {
        //If no card is being played, place a hero
        placeHeroes();
	}

    public void placeHeroes()
    {
        if (UnitHandler.hero1 == null)
        {
            Debug.Log("Placing Hero 1");
            DeckHandler.HeroCard1.summonHero();
            GameObject.Find("Spawn1").transform.FindChild("AttackCircle").gameObject.SetActive(true);

        }
        else if (UnitHandler.hero2 == null)
        {
            GameObject.Find("Spawn1").transform.FindChild("AttackCircle").gameObject.SetActive(false);
            Debug.Log("Placing Hero 2");
            TurnHandler.isTeam1Turn = false;
            DeckHandler.HeroCard2.summonHero();
            GameObject.Find("Spawn2").transform.FindChild("AttackCircle").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("Spawn2").transform.FindChild("AttackCircle").gameObject.SetActive(false);
            Debug.Log("Both Heroes placed. Game begins");
            startTurn1();
        }
            
    }

    void startTurn1()
    {
        //Enable all the normal handlers
        Debug.Log("Start turn 1");
        TurnHandler.isTeam1Turn = true;
        GameObject handler = GameObject.Find("MainGameHandle");
        handler.GetComponent<UnitHandler>().enabled = true;
        handler.GetComponent<TurnHandler>().enabled = true;
        handler.GetComponent<UnitMoveHandler>().enabled = true;
        handler.GetComponent<UnitAttackHandler>().enabled = true;
        this.enabled = false;
    }
}

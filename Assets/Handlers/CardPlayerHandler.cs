using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class CardPlayerHandler : MonoBehaviour
{
    private static float S_RANGE = 5.0f;
    public static float SUMMON_RANGE { get { return S_RANGE; } }

    static Card playedCard;
    static public Card PlayerCard { get { return playedCard; } set { playedCard = value; } }

    static GameObject playedUnit;
    static public GameObject PlayerUnit { get { return playedUnit; } set { playedUnit = value; } }

    static GameObject playedSpell;
    static public GameObject PlayerSpell { get { return playedSpell; } set { playedSpell = value; } }

    static GameObject playedHero;
    static public GameObject PlayerHero { get { return playedHero; } set { playedHero = value; } }

    static GameObject playedCrystal;
    static public GameObject PlayerCrystal { get { return playedCrystal; } set { playedCrystal = value; } }

    void Update()
    {
        if(playedUnit != null)
        {
            summoning();
        }
        else if(playedSpell != null)
        {
            castingSpell();
        }
        else if(playedHero != null)
        {
            summoningHero();
        }
        else if(playedCrystal != null)
        {
            castingCrystal();
        }
    }

    private void summoning()
    {
        //Position on plane
        RaycastHit hit;

        if (Input.GetMouseButtonDown(1))
        {
            //Change this to be indicator after!
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitAttackHandler.indicator))
            {
                //Must have clicked in the target circle
                if (hit.collider.gameObject.tag == "AtkCircle")
                {
                    playedUnit.transform.parent = null;
                    playedCard.transform.SetParent(GameObject.Find("PlayedCard").transform);
                    playedCard.gameObject.SetActive(false);

                    playedUnit.GetComponent<UnitBase>().LinkedCard = (UnitCard)playedCard;

                    //Add Unit to Unit List. His team is determined by current team turn.
                    UnitHandler.addToUnitList(playedUnit.GetComponent<UnitBase>());

                    //Clear the stuff
                    UnitHandler.currentHero.clearHarvestedCrystals(playedCard.manaCost);

                    //Add the card to the cards played
                    UnitHandler.currentHero.cardsPlayed.Add(playedCard);

                    playedCard = null;
                    playedUnit = null;

                    
                    UnitHandler.currentHero.endCasting();
                }

            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            UnitHandler.currentHero.refundMana(playedCard.manaCost);
            Destroy(playedUnit);
            playedCard.transform.SetParent(GameObject.Find("Hand").transform);
            UnitHandler.currentHero.endCasting();
        }
        else
        {
            //Adjust position of summon to desired location
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitHandler.worldLayer))
            {
                playedUnit.transform.position = hit.point + new Vector3(0f, 0.5f, 0f);
            }
        }

    }

    private void castingSpell()
    {
        //Position on plane
        RaycastHit hit;

        if (Input.GetMouseButtonDown(1))
        {
            //Change this to be indicator after!
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitAttackHandler.indicator))
            {
                Debug.Log("TOUCHED LAYER: " + hit.collider.gameObject.layer);
                //Must have clicked in the target circle
                if (hit.collider.gameObject.tag == "AtkCircle")
                {
                    playedSpell.transform.parent = null;
                    UnitHandler.currentHero.hasActed = true;

                    playedSpell.GetComponent<AttackObject>().LinkedAttack.setTargets(Effector.getTargets());
                    playedSpell.GetComponent<AttackObject>().LinkedAttack.applyAttack();

                    Destroy(playedSpell);
                    UnitHandler.currentHero.toggleAtkCircle(false);

                    //Attach the card to the played card
                    playedCard.transform.SetParent(GameObject.Find("PlayedCard").transform);
                    playedCard.gameObject.SetActive(false);
                    playedSpell.GetComponent<AttackObject>().LinkedCard = (SpellCard)playedCard;

                    UnitHandler.currentHero.clearHarvestedCrystals(playedCard.manaCost);
                    
                    //Add the card to the cards played
                    UnitHandler.currentHero.cardsPlayed.Add(playedCard);

                    playedCard = null;
                    playedSpell = null;

                    UnitHandler.currentHero.endCasting();
                }

            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            UnitHandler.currentHero.refundMana(playedCard.manaCost);
            Destroy(playedSpell);
            UnitHandler.currentHero.toggleAtkCircle(false);
            UnitHandler.currentHero.endCasting();
            playedCard.transform.SetParent(GameObject.Find("Hand").transform);
        }
        else
        {
            //Adjust position of summon to desired location
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitHandler.worldLayer))
            {
                playedSpell.transform.position = hit.point + new Vector3(0f, 0.5f, 0f);
            }
        }
    }

    private void summoningHero()
    {
        //Position on plane
        RaycastHit hit;

        if (Input.GetMouseButtonDown(1))
        {
            //Change this to be indicator after!
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitAttackHandler.indicator))
            {
                
                //Must have clicked in the target circle
                if (hit.collider.gameObject.tag == "AtkCircle")
                {
                    playedHero.transform.parent = null;
                    playedCard.transform.SetParent(GameObject.Find("PlayedCard").transform);
                    playedCard.gameObject.SetActive(false);

                    playedHero.GetComponent<UnitBase>().LinkedCard = (UnitCard)playedCard;

                    //Add Unit to Unit List. His team is determined by current team turn.
                    UnitHandler.addToUnitList(playedHero.GetComponent<UnitBase>());

                    playedCard = null;
                    playedHero = null;

                    GameObject.Find("MainGameHandle").GetComponent<MatchStarter>().placeHeroes();
                }
            }
            if(hit.collider != null)
                Debug.Log(hit.collider.gameObject.layer);
        }
        else
        {
            //Adjust position of summon to desired location
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitHandler.worldLayer))
            {
                playedHero.transform.position = hit.point + new Vector3(0f, 0.5f, 0f);
            }
        }
    }

    private void castingCrystal()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //playedSpell.transform.parent = null;

            //Destroy(playedCrystal);
            playedCrystal.GetComponent<CrystalObject>().setUsedAlpha(false);

            //Attach the card to the played card
            playedCard.transform.SetParent(GameObject.Find("PlayedCard").transform);
            playedCard.gameObject.SetActive(false);
            playedCrystal.GetComponent<CrystalObject>().LinkedCard = (CrystalCard)playedCard;

            UnitHandler.currentHero.clearHarvestedCrystals(playedCard.manaCost);

            //Add the card to the cards played
            UnitHandler.currentHero.cardsPlayed.Add(playedCard);

            //Assign to the owner, and add crystal to his list
            UnitHandler.currentHero.crystals.Add(playedCrystal.GetComponent<CrystalObject>());

            playedCard = null;
            playedCrystal = null;

            UnitHandler.currentHero.endCasting();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            UnitHandler.currentHero.refundMana(playedCard.manaCost);
            Destroy(playedCrystal);
            UnitHandler.currentHero.toggleAtkCircle(false);
            UnitHandler.currentHero.endCasting();
            playedCard.transform.SetParent(GameObject.Find("Hand").transform);
        }
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckHandler : MonoBehaviour {

    public List<GameObject> deck;
    GameObject heroCard;

    public static UnitCard HeroCard1;
    public static UnitCard HeroCard2;

    // Use this for initialization
    void Start () {
        deck = new List<GameObject>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
            deck.Add(gameObject.transform.GetChild(i).gameObject);
        shuffleDeck();   
	}
	
    public void loadHeroCard()
    {
        Debug.Log("Loading Heroes for " + this.name);
            
        if (this.gameObject.name == "Deck1")
        {
            HeroCard1 = findHero();
        }                
        else
        {
            HeroCard2 = findHero();
        }                   
    }

    UnitCard findHero()
    {
        //Find the Hero card, save it, and remove it from the deck afterwards. Decks only have 1 hero
        UnitCard chosenHero = null;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject card = deck[i];
            if (card.GetComponent<Card>().GetType() == typeof(UnitCard) && card.GetComponent<UnitCard>().unitData.isHero)
            {
                chosenHero = card.GetComponent<UnitCard>();
                //deck.Remove(card);
                //card.transform.SetParent(null);
            }
                
        }
        return chosenHero;
    }

    void drawCard()
    {
        //Sets the parent to be the hand
        deck[gameObject.transform.childCount - 1].transform.SetParent((GameObject.Find("Hand").transform));
        deck.Remove(deck[gameObject.transform.childCount]);
    }

    void shuffleDeck()
    {
        List<GameObject> shuffledDeck = new List<GameObject>();
        Debug.Log("Shuffle Deck");
        while(deck.Count != 0)
        {
            int index = Random.Range(0, deck.Count-1);
            shuffledDeck.Add(deck[index]);
            deck.RemoveAt(index);
        }
        deck = shuffledDeck;
    }
}

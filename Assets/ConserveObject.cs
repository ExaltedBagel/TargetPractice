using UnityEngine;
using System.Collections;

public class ConserveObject : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        Object.DontDestroyOnLoad(this.gameObject);
    }

    public void startGame()
    {
        DeckHandler deck1 = GameObject.Find("Deck1").GetComponent<DeckHandler>();
        DeckHandler deck2 = GameObject.Find("Deck2").GetComponent<DeckHandler>();
        deck1.loadHeroCard();
        deck2.loadHeroCard();
        Application.LoadLevel(1);

    }
}

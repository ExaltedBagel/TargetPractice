using UnityEngine;
using System.Collections;

public class Warrior : MonoBehaviour {

    Unit warrior;

    // Use this for initialization
    void Start()
    {

        warrior = new Unit(this.gameObject, "Warrior");

        warrior.hp = 5;
        warrior.speed = 30;
        warrior.atk = 5;
        warrior.team = 2;
        UnitHandler.addToUnitList(warrior);
    }
}


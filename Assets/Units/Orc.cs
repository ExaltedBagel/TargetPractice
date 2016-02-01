using UnityEngine;
using System.Collections;

public class Orc : MonoBehaviour {

    Unit orc;

    // Use this for initialization
    void Start()
    {

        orc = new Unit(this.gameObject, "Orc");

        orc.hp = 5;
        orc.speed = 30;
        orc.atk = 5;
        orc.team = 1;
        UnitHandler.addToUnitList(orc);
    }
}

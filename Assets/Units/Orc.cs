using UnityEngine;
using System.Collections;

public class Orc : MonoBehaviour {

    Unit orc;

    // Use this for initialization
    void Start()
    {

        orc = new Unit(this.gameObject, "Orc");

        orc.hp = 20;
        orc.speed = 10;
        orc.atk = 1;
        orc.team = 1;
        UnitHandler.addToUnitList(orc);
    }
}

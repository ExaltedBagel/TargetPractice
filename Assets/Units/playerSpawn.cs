using UnityEngine;
using System.Collections;

public class playerSpawn : MonoBehaviour {

    Unit player;

    // Use this for initialization
    void Start () {
        player = new Unit(this.gameObject, "Jamahl");
        player.hp = 20;
        player.speed = 10;
        UnitHandler.addToUnitList(player);
    }

}

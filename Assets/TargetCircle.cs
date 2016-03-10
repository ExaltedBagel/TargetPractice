using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetCircle : MonoBehaviour {

    public Camera mainCam;
    public Material safe;
    public Material target;
    
    private static List<UnitBase> targets = new List<UnitBase>();

    public static List<UnitBase> getTargets()
    {
        return targets;
    }

    public void toggleTargetBlastCircle(AttackBlastCircle attack)
    {
        gameObject.transform.localScale = new Vector3(attack.radius, 0.2f, attack.radius);
        gameObject.SetActive(true);
    }

    public void sleepBlastCircle()
    {
        targets = new List<UnitBase>();
        gameObject.SetActive(false);
    }

    private UnitBase findTarget(GameObject target)
    {
        foreach (UnitBase unit in UnitHandler.units)
        {
            if (unit.unit.GetHashCode() == target.GetHashCode())
                return unit;
        }
        return null;
    }      

// Update is called once per frame
void Update () {
        //UnitHandler.currentEnnemyLayer = UnitHandler.team2;
        
        /*
        //Position on plane
        RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitHandler.worldLayer))
        {
            gameObject.transform.position = hit.point + new Vector3(0f, 0.2f, 0f);
        }
        */
    }

}

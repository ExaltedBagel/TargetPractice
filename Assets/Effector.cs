using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Effector : MonoBehaviour {

    public Camera mainCam;
    public Material safe;
    public Material target;
    private static LayerMask targetLayer;

    private static List<UnitBase> targets = new List<UnitBase>();

    public static List<UnitBase> getTargets()
    {
        return targets;
    }

    public static void setLayer(bool isOffensive)
    {
        if (isOffensive)
            setLayer(UnitHandler.currentEnnemyLayer);
        else
            setLayer(UnitHandler.currentAllyLayer);
    }

    public static void setLayer(LayerMask layer)
    {
        targetLayer = layer;
    }

    public void deleteEffector()
    {
        targets = new List<UnitBase>();
        Destroy(gameObject.transform.root.gameObject);
    }

    //Modded
    private UnitBase findTarget(GameObject target)
    {
        foreach (UnitBase unit in UnitHandler.units)
        {
            if (Equals(unit, target.GetComponent<UnitBase>()))
                return unit;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        //UnitHandler.currentEnnemyLayer = UnitHandler.team2;       
        //Position on plane
        RaycastHit hit;       
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitHandler.worldLayer))
        {
            gameObject.transform.position = hit.point + new Vector3(0f, 0.5f, 0f);
        }
        
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Entered ");
        int layerTouched = 1 << col.gameObject.transform.root.gameObject.layer;
        // col.GetComponent<Renderer>().material = target;
        if ((layerTouched & targetLayer) > 0)
        {
            if(!targets.Contains(findTarget(col.transform.root.gameObject)))
                targets.Add(findTarget(col.transform.root.gameObject));
        }

        Debug.Log(targets.Count + " in the list.");

    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log("Left");
        int layerTouched = 1 << col.gameObject.transform.root.gameObject.layer;
        // col.GetComponent<Renderer>().material = safe;
        if ((layerTouched & targetLayer) > 0)
        {
            targets.Remove(findTarget(col.transform.root.gameObject));
        }
        Debug.Log(targets.Count + " in the list.");
    }
}

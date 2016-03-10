using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selector : MonoBehaviour
{
    public Camera mainCam;
    public Material ally;
    public Material enemy;
    public Material selection;

    public static GameObject selected;
    public static GameObject hovered;

    private static int hoveredCount = 0;

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
        //Position on plane
        RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, UnitHandler.worldLayer))
        {
            gameObject.transform.position = hit.point + new Vector3(0f, 0.8f, 0f);
        }
        
    }

    void OnTriggerEnter(Collider col)
    {

        if (selected != null && col.gameObject.transform.root.gameObject == selected)
            return;

        int layerTouched = 1 << col.gameObject.layer;
        if ((layerTouched & UnitHandler.currentEnnemyLayer.value) > 0)
        {
            //Find game object, make him the hovered one
            if (hovered != col.gameObject)
            {
                hovered = col.gameObject.transform.root.gameObject;
            }
                
            //Activate and color selection circle
            hovered.transform.FindChild("SelectCircle").gameObject.SetActive(true);
            hovered.transform.FindChild("SelectCircle").gameObject.GetComponent<Renderer>().material = enemy;
        }
        else if ((layerTouched & UnitHandler.currentAllyLayer.value) > 0)
        {
            //Find game object, make him the hovered one
            if(hovered == null )
                hovered = col.gameObject.transform.root.gameObject;
            //Activate and color selection circle if its not already selected
            if(!Equals(hovered, selected))
            {
                hovered.transform.FindChild("SelectCircle").gameObject.SetActive(true);
                hovered.transform.FindChild("SelectCircle").gameObject.GetComponent<Renderer>().material = ally;
            } 
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (hovered == null)
            return;
        int layerTouched = 1 << col.gameObject.layer;
        if ((layerTouched & UnitHandler.unitsLayer.value) > 0)
        {
            if(!Equals(hovered, null) && !Equals(hovered, selected) && Equals(hovered, col.gameObject.transform.root.gameObject))
            {
                hovered.transform.FindChild("SelectCircle").gameObject.SetActive(false);
                hovered = null;
            }              
        }
    }


    public void setSelected(GameObject unit)
    {
        Debug.Log("Selecting");
        selected = unit;
        selected.transform.FindChild("SelectCircle").gameObject.GetComponent<Renderer>().material = selection;
        hovered = null;
    }

    public static void clearTargetted(UnitBase unit)
    {
        if(Equals(unit.unit, selected))
        {
            selected.transform.FindChild("SelectCircle").gameObject.SetActive(false);
            UnitMoveHandler.moveReady = false;
            UnitAttackHandler.atkReady = false;
            UnitHandler.canSelect = true;
            selected = null;
        }      
    }

}


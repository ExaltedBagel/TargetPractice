using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetCircle : MonoBehaviour {

    public Camera mainCam;
    public Material safe;
    public Material target;

    private List<GameObject> targets = new List<GameObject>();

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //Position on plane
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if(hit.collider.tag == "Plane")
                this.transform.position = hit.point;
        }

    }


    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Entered");
        col.GetComponent<Renderer>().material = target;
        targets.Add(col.gameObject);
        Debug.Log(targets.Count + " in the list.");

    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log("Left");
        col.GetComponent<Renderer>().material = safe;
        targets.Remove(col.gameObject);
        Debug.Log(targets.Count + " in the list.");
    }
}

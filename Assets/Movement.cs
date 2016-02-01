using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public LayerMask player = 1;
    public LayerMask world = 1;

    //Movements
    bool moving = false;
    bool selected = false;
    public float speed = 12.0f;

    private GameObject unit;
    private Vector3 destination;
    private Vector3 startPoint;

    //Moving stuff
    private float movFrac = 0.0f;


	// Use this for initialization
	void Start () {
	
	}
	
    bool checkPath()
    {


        return true;
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            
            //Position on plane
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!selected)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, player))
                {
                    if (hit.collider.tag == "Player")
                    {
                        Debug.Log("FoundHim!");
                        unit = hit.collider.gameObject;
                        startPoint = unit.transform.position;
                        selected = true;
                    }
                }
            }
            else
            {
                if (!moving && Physics.Raycast(ray, out hit, Mathf.Infinity, world))
                {
                    if (hit.collider.tag == "Plane")
                    {
                        Debug.Log("Distance: " + Vector3.Distance(unit.transform.position, hit.point));
                        moving = true;
                        destination = hit.point;

                    }

                }
            }                     
        }

        //Moving unit
        if (moving)
        {
            if (unit.transform.position != destination)
            {
                movFrac += Time.deltaTime;
                unit.transform.position = Vector3.MoveTowards(startPoint, destination, movFrac * speed);
                Debug.Log(movFrac);
            }
            else
            {
                Debug.Log("Arrived!");
                moving = false;
                startPoint = unit.transform.position;
                movFrac = 0.0f;
            }
        }
    }
}

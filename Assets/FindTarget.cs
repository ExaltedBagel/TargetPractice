using UnityEngine;
using System.Collections;


public class FindTarget : MonoBehaviour {

    public GameObject attacker;
    public GameObject defender;

	// Use this for initialization
	void Start () {
	
	}
	
    public bool tryRays()
    {
        bool result = false;
        Vector3 baseVector = defender.transform.position - attacker.transform.position;
        float angle = 0f;
        //Sweep the target
        for (; angle < 30 && !result; angle += 0.5f)
        {
            RaycastHit hit;
            Ray ray = new Ray(attacker.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * baseVector);
            Debug.DrawRay(attacker.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * baseVector, Color.green, 3.0f);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == defender.name)
                    result = true;
            }

            ray = new Ray(attacker.transform.position, Quaternion.AngleAxis(-angle, Vector3.up) * baseVector);
            Debug.DrawRay(attacker.transform.position, Quaternion.AngleAxis(-angle, Vector3.up) * baseVector, Color.red, 3.0f);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == defender.name)
                    result = true;
            }

        }


        return result;
    }

	// Update is called once per frame
	void Update () {
        
    }

}

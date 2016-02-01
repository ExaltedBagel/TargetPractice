using UnityEngine;
using System.Collections;

public class Targetted : MonoBehaviour {

    static public bool targeted;
    static public Material standard;
    static public Material affected;
    static Renderer rend;


	// Use this for initialization
	void Start () {
        targeted = false;
        rend = gameObject.GetComponent<Renderer>();
	}
	
    static public void makeTarget(bool state)
    {
        if (state)
        {
            rend.material = affected;
            targeted = true;
        }
        else
        {
            rend.material = standard;
            targeted = false;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}



}

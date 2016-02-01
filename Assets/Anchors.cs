using UnityEngine;
using System.Collections;

public class Anchors : MonoBehaviour {

    private Vector3[] anchors;
    private Collider col;

	// Use this for initialization
	void Start () {
        col = gameObject.GetComponent<Collider>();
        anchors = new Vector3[8];

        anchors[0] = col.bounds.center - col.bounds.extents;
        anchors[1] = col.bounds.center + col.bounds.extents;
        //The other corners can be found by adding some extents, subtracting others.Something like:

        anchors[2] = new Vector3(col.bounds.center.x - col.bounds.extents.x,
                         col.bounds.center.y + col.bounds.extents.y,
                         col.bounds.center.z + col.bounds.extents.z);
    }

}

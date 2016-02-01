using UnityEngine;
using System.Collections;

public class CharMove : MonoBehaviour {

    NavMeshAgent nav;
    private float dist = 10.0f;
    private float remDist = 0.0f;
    private bool moving = false;
    private GameObject tra;

    void Start()
    {
        nav = GetComponentInChildren<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !moving)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {

                remDist = CalculatePathLength(hit.point) - dist;
                nav.destination = hit.point;

            }
        }

        if (!moving && nav.hasPath && !nav.pathPending)
        {
            nav.Resume();
            Debug.Log("Remaining dist: " + remDist);
            Debug.Log("Total Lenght: " + nav.remainingDistance);
            moving = true;
        }

        if (((nav.remainingDistance < remDist) || (nav.remainingDistance <= 0.1)) && nav.hasPath && moving)
        {
            nav.Stop();
            nav.ResetPath();
            moving = false;
        }

    }

    float CalculatePathLength(Vector3 targetPosition)
    {
        // Create a path and set it based on a target position.
        NavMeshPath path = new NavMeshPath();
        if (nav.enabled)
            nav.CalculatePath(targetPosition, path);

        // Create an array of points which is the length of the number of corners in the path + 2.
        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

        // The first point is the enemy's position.
        allWayPoints[0] = transform.position;

        // The last point is the target position.
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        // The points inbetween are the corners of the path.
        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }

        // Create a float to store the path length that is by default 0.
        float pathLength = 0;

        // Increment the path length by an amount equal to the distance between each waypoint and the next.
        for (int i = 0; i < allWayPoints.Length - 1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }

        return pathLength;
    }
}

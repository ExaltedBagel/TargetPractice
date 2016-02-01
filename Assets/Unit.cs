using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Unit : Component, IEquatable<Unit>
{
    /*************************************/
    //Basis
    /************************************/
    public Unit(GameObject unit, string name) { this.unit = unit; gameID = unit.GetInstanceID(); uName = name; }
    private int gameID;
    public GameObject unit;
    private LayerMask world = (1 << 8);
    public int team { get; set; }
    public bool hasMoved = false;
    public bool hasActed = false;

    /*************************************/
    //Stats
    /************************************/
    public string uName { get; set; }
    public int hp { get; set; }
    public float speed { get; set; }
    public int atk { get; set; }
    public int actionPoints { get; set; }

    /*************************************/
    //Movement
    /************************************/
    public bool moving { get; set; }
    public float remDist;
    NavMeshAgent nav;
    private Vector3 destination;
    private Vector3 startPoint;
    private float movSpeed = 12.0f;
    private float movFrac = 0.0f;

    /*************************************/
    //Combat
    /************************************/
    private GameObject atkCircle;
    private float atkRadius = 5.0f;

    void Start()
    {
        
        moving = false;
        atk = 1;        
    }

    //A RETRAVAILLER********************************************************************************************************
    public void init()
    {
        atkCircle = unit.transform.GetChild(0).gameObject;
        nav = unit.GetComponentInChildren<NavMeshAgent>();
        
    }



    /*************************************/
    //Functions
    /************************************/
    public void printName()
    {
        Debug.Log(uName);
    }

    public int getId ()
    {
        return gameID;
    }

    /*************************************/
    //Atk Function
    /************************************/
    public void toggleAtkCircle(bool state)
    {
        Debug.Log("Atk");
        if (state)
            atkCircle.SetActive(true);
        else
            atkCircle.SetActive(false);
    }

    public Collider[] overlapArea(LayerMask targetTeam)
    {
        Collider[] ennemies = Physics.OverlapSphere(atkCircle.transform.position, atkRadius, targetTeam);
        foreach (Collider ennemy in ennemies)
            Debug.Log(ennemy.gameObject.name);

        return ennemies;
    }

    public bool tryRays(GameObject target)
    {
        bool result = false;
        Vector3 baseVector = target.transform.position - unit.transform.position;
        float angle = 0f;
        //Sweep the target
        for (; angle < 30 && !result; angle += 0.5f)
        {
            RaycastHit hit;
            Ray ray = new Ray(unit.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * baseVector);
            Debug.DrawRay(unit.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * baseVector, Color.green, 3.0f);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == target.name)
                    result = true;
            }

            ray = new Ray(unit.transform.position, Quaternion.AngleAxis(-angle, Vector3.up) * baseVector);
            Debug.DrawRay(unit.transform.position, Quaternion.AngleAxis(-angle, Vector3.up) * baseVector, Color.red, 3.0f);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == target.name)
                    result = true;
            }

        }
        return result;
    }

    public void attackUnit(Unit target)
    {
        
        target.hp -= this.atk;
        Debug.Log(target.uName + " : " + target.hp + " hp left!");
        hasActed = true;
        
    }


    /*************************************/
    //Move Functions
    /************************************/
    public void moveUnit()
    {
        if((!hasActed || !hasMoved))
        { 
            //Find position to move to
            RaycastHit hit;
            moving = true;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                remDist = this.CalculatePathLength(hit.point) - speed;
                nav.destination = hit.point;

            }
        }
        else
        {
            Debug.Log("Unit is out of actions");
        }

    }

    public float CalculatePathLength(Vector3 targetPosition)
    {
        if (Equals(nav, null))
            init();
        // Create a path and set it based on a target position.
        NavMeshPath path = new NavMeshPath();

        if (nav.enabled)
            nav.CalculatePath(targetPosition, path);
        
        // Create an array of points which is the length of the number of corners in the path + 2.
        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];
        // The first point is the enemy's position.
        allWayPoints[0] = unit.transform.position;
        // The last point is the target position.
        allWayPoints[allWayPoints.Length - 1] = targetPosition;
        // The points inbetween are the corners of the path.
        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }
        
        // Create a float to store the path length that is by default 0.
        float pathLength = 0;
        bool pathTooLong = false;
        // Increment the path length by an amount equal to the distance between each waypoint and the next.
        for (int i = 0; i < allWayPoints.Length - 1 && !pathTooLong; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);

            //If the path is too long, the too long part is cropped.

            if (pathLength > speed)
            {
                Debug.Log(pathLength);
                pathTooLong = true;
                Vector3[] newPath = new Vector3[i+2];
                for (int j = 0; j < i+1; j++)
                    newPath[j] = allWayPoints[j];


                Vector3 dir = Vector3.Normalize(allWayPoints[i + 1] - allWayPoints[i]);
                float dist = (pathLength - speed);
                
                newPath[i + 1] = allWayPoints[i + 1] - (dist* dir);
                allWayPoints = newPath;
            }           
        }
        //
        UnitMoveHandler.renderPath(allWayPoints);

        return pathLength;
    }

    /**************************************************/
    /*Might need to check if its pathable */
    /**************************************************/
    public Vector3 CalculatePathEndPoint(Vector3 targetPosition)
    {
        if (Equals(nav, null))
            init();
        // Create a path and set it based on a target position.
        NavMeshPath path = new NavMeshPath();

        if (nav.enabled)
            nav.CalculatePath(targetPosition, path);
        if (nav.pathStatus == NavMeshPathStatus.PathComplete)
        {
            // Create an array of points which is the length of the number of corners in the path + 2.
            Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];
            // The first point is the enemy's position.
            allWayPoints[0] = unit.transform.position;
            // The last point is the target position.
            allWayPoints[allWayPoints.Length - 1] = targetPosition;
            // The points inbetween are the corners of the path.
            for (int i = 0; i < path.corners.Length; i++)
            {
                allWayPoints[i + 1] = path.corners[i];
            }

            // Create a float to store the path length that is by default 0.
            float pathLength = 0;
            bool pathTooLong = false;
            // Increment the path length by an amount equal to the distance between each waypoint and the next.
            for (int i = 0; i < allWayPoints.Length - 1 && !pathTooLong; i++)
            {
                pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);

                //If the path is too long, the too long part is cropped.

                if (pathLength > speed)
                {
                    Debug.Log(pathLength);
                    pathTooLong = true;
                    Vector3[] newPath = new Vector3[i + 2];
                    for (int j = 0; j < i + 1; j++)
                        newPath[j] = allWayPoints[j];


                    Vector3 dir = Vector3.Normalize(allWayPoints[i + 1] - allWayPoints[i]);
                    float dist = (pathLength - speed);

                    newPath[i + 1] = allWayPoints[i + 1] - (dist * dir);
                    allWayPoints = newPath;
                }
            }
           

            return allWayPoints[allWayPoints.Length-1]; //Returns final point of the path
        }
        else
            return new Vector3(0f, -100f, 0f); //DummyVector to represent its not a good spot.
    }

    public Vector3[] CalculatePathSections(Vector3 targetPosition)
    {
        if (Equals(nav, null))
            init();
        // Create a path and set it based on a target position.
        NavMeshPath path = new NavMeshPath();

        if (nav.enabled)
            nav.CalculatePath(targetPosition, path);
        if (nav.pathStatus == NavMeshPathStatus.PathComplete)
        {
            // Create an array of points which is the length of the number of corners in the path + 2.
            Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];
            // The first point is the enemy's position.
            allWayPoints[0] = unit.transform.position;
            // The last point is the target position.
            allWayPoints[allWayPoints.Length - 1] = targetPosition;
            // The points inbetween are the corners of the path.
            for (int i = 0; i < path.corners.Length; i++)
            {
                allWayPoints[i + 1] = path.corners[i];
            }

            // Create a float to store the path length that is by default 0.
            float pathLength = 0;
            bool pathTooLong = false;
            // Increment the path length by an amount equal to the distance between each waypoint and the next.
            for (int i = 0; i < allWayPoints.Length - 1 && !pathTooLong; i++)
            {
                pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);

                //If the path is too long, the too long part is cropped.

                if (pathLength > speed)
                {
                    Debug.Log(pathLength);
                    pathTooLong = true;
                    Vector3[] newPath = new Vector3[i + 2];
                    for (int j = 0; j < i + 1; j++)
                        newPath[j] = allWayPoints[j];


                    Vector3 dir = Vector3.Normalize(allWayPoints[i + 1] - allWayPoints[i]);
                    float dist = (pathLength - speed);

                    newPath[i + 1] = allWayPoints[i + 1] - (dist * dir);
                    allWayPoints = newPath;
                }
            }
            Vector3[] finalPath = new Vector3[allWayPoints.Length - 1];

            for (int i = 0; i < finalPath.Length; i++)
                finalPath[i] = allWayPoints[i + 1];

            return finalPath; //Returns all points of the path minus the first point
        }
        else
            return null; //DummyVector to represent its not a good spot.
    }

    public void updatePos()
    {
        if (!moving && nav.hasPath && !nav.pathPending)
        {
            UnitMoveHandler.renderPath(nav.path.corners);
            nav.Resume();
            Debug.Log("Remaining dist: " + remDist);
            Debug.Log("Total Lenght: " + nav.remainingDistance);
            moving = true;
        }

        if (((nav.remainingDistance < remDist) || (nav.remainingDistance <= 0.1)) && nav.hasPath && moving)
        {
            UnitHandler.erasePath();
            nav.Stop();
            nav.ResetPath();
            moving = false;
            if (!hasMoved)
                hasMoved = true;
            else
                hasActed = true;
            //Check if unit has any actions left
            if (UnitHandler.unit1.hasActed && UnitHandler.unit1.hasMoved)
            {
                Debug.Log("Unit no longer in bucket");
                TurnHandler.removeFromBucket(UnitHandler.unit1, UnitHandler.unit1.team);
            }
        }
    }


    /*************************************/
    //Redefined Equalities
    /************************************/

    public bool Equals(Unit other)
    {
        if (Equals(this, null) && Equals(other, null))
            return true;
        else if (Equals(this, null) | Equals(this, null))
            return false;
        else if (gameID == other.gameID)
            return true;
        else
            return false;
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as Unit);
    }
    public override int GetHashCode()
    {
        return gameID; ;
    }



}

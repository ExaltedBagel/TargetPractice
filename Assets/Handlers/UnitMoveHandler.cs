using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitMoveHandler : MonoBehaviour {

    public static bool moveReady;

    private Vector3 nextDest;
    public static List<Vector3> wayPoints = new List<Vector3>();
    float remainingDistance = 0.0f;

    /**************/
    /*Multi waypoints */
    /*****************/
    public static Transform selectedUnit;
    float speed = 4.0f;
    int currentPoint = 0;
    static GameObject circleMove;
    LayerMask wayPointRay;
    public Material circleColor;

    // Use this for initialization
    void Start () {
        moveReady = false;
        wayPointRay = -1;
        wayPointRay -= (1 << 13);

    }

    // Update is called once per frame
    void Update()
    {
        /*********************************************/
        /*MOVING PATH INDICATOR */
        /*********************************************/
        if (moveReady && !(UnitHandler.unit1.hasActed && UnitHandler.unit1.hasMoved))
        {
            if (wayPoints.Count == 0)
            {
                remainingDistance = UnitHandler.unit1.speed;
                selectedUnit = Selector.selected.transform;
                wayPoints.Add(selectedUnit.position);
                createMoveCircle();
            }
            else
            {
                renderWaypoints();
            }   

            //Debug.Log(checkPathValid());
        }
        else if (!(Equals(UnitHandler.unit1, null)))
            if (UnitHandler.unit1.moving)
            {
                //Update the position of the unit
                updatePosition();
            }

        /*********************************************/
        /*MOVE COMMAND */
        /*********************************************/
        if (Input.GetMouseButtonDown(1))
        {
            if (!(Equals(UnitHandler.unit1, null)) && moveReady)
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    removeLastWayPoint();
                }

                //If the point may be reached in a straight line, add it to the waypoints
                else if(checkPathValid())
                {
                    //If the point is not too far
                    if(remainingDistance >= Vector3.Distance(nextDest, wayPoints[wayPoints.Count - 1]))
                    {
                        Destroy(circleMove);
                        remainingDistance -= Vector3.Distance(nextDest, wayPoints[wayPoints.Count - 1]);
                        wayPoints.Add(nextDest);
                        createMoveCircle();
                        Debug.Log("Added point : " + wayPoints.Count + " Points are now present. Distance left: " + remainingDistance);
                    }
                    
                }

            }

        }
        /****************************************************/
        /*CANCEL MOVE COMMAND */
        /****************************************************/
        if (Input.GetMouseButtonDown(0) && moveReady)
        {
            abortMove();
        }

        //Applies the path to the current unit
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Space pressed");
            UnitAttackHandler.atkReady = false;
            moveReady = false;
            UnitHandler.canSelect = false;
            UnitHandler.unit1.moving = true;
        }
    }

    public void moveUnit()
    {
        if (!(Equals(UnitHandler.unit1, null)) && !moveReady && UnitHandler.canSelect && !UnitAttackHandler.atkReady)
        {
            if (!(UnitHandler.unit1.hasActed && UnitHandler.unit1.hasMoved))
            {
                Debug.Log("MoveReady");
                moveReady = true;
                UnitHandler.canSelect = false;
                //remainingDistance = UnitHandler.unit1.speed;
                //showThreat(true);
            }
            else
                Debug.Log("Unit has no more actions left");
        }
    }

    /********************************************************/
    /*MULTIPLE POINT PATH -without the navmesh*/
    /********************************************************/

    bool checkPathValid()
    {

        //Shoot a ray to hit the plane
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If ray hits the plane, get the destination position
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, wayPointRay))
            if (hit.collider.gameObject.tag == "Plane")
            {
                nextDest = hit.point + new Vector3(0f, 0.5f);
            }
            else
                return false;

        //Try a sphere cast from the unit to the current position
        ray = new Ray(wayPoints[wayPoints.Count - 1], nextDest - wayPoints[wayPoints.Count - 1]);
        Debug.Log("NextDest = " + nextDest.x + " , " + nextDest.y + " , " + nextDest.z);
        Debug.DrawRay(wayPoints[wayPoints.Count - 1], nextDest, Color.green);
        //If the sphere hits nothing, they way point is valid, else returns false
        return !Physics.SphereCast(ray, 0.5f, out hit, Vector3.Distance(nextDest, wayPoints[wayPoints.Count - 1]), UnitHandler.currentPathLayer.value);

    }

    //Check if point is reached in a close enough matter
    bool pointIsReached()
    {
        if (selectedUnit.position.x < wayPoints[currentPoint + 1].x + 0.1 && selectedUnit.position.x > wayPoints[currentPoint + 1].x - 0.1)
            if (selectedUnit.position.y < wayPoints[currentPoint + 1].y + 0.1 && selectedUnit.position.y > wayPoints[currentPoint + 1].y - 0.1)
                if (selectedUnit.position.z < wayPoints[currentPoint + 1].z + 0.1 && selectedUnit.position.z > wayPoints[currentPoint + 1].z - 0.1)
                    return true;

        return false;
    }
    
    void updatePosition()
    {
        //Debug.Log("Updating");
        float step = speed * Time.deltaTime;
        if(pointIsReached())
        {
            Debug.Log("Point Reached");
            currentPoint++;

            //If movement is done
            if (currentPoint+1 == wayPoints.Count)
            {
                UnitHandler.unit1.moving = false;
                Debug.Log("Move done");

                //Reset movement values and remove visuals
                wayPoints = new List<Vector3>();
                currentPoint = 0;
                UnitHandler.erasePath();
                showThreat(false);
                Destroy(circleMove);

                //Reactivate Control
                UnitHandler.canSelect = true;

                //Change unit action state
                if (!UnitHandler.unit1.hasMoved)
                    UnitHandler.unit1.hasMoved = true;
                else
                    UnitHandler.unit1.hasActed = true;

                Debug.Log("Unit might be done");

                TurnHandler.unitDoneTurn(UnitHandler.unit1);
            }           
        }
        else
        {
            selectedUnit.position = Vector3.MoveTowards(selectedUnit.position, wayPoints[currentPoint + 1], step);
        }
    }


    void createMoveCircle()
    {
        circleMove = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        circleMove.transform.position = new Vector3(wayPoints[wayPoints.Count - 1].x, 0.2f, wayPoints[wayPoints.Count - 1].z);
        circleMove.GetComponent<Renderer>().material = circleColor;
        circleMove.layer = 13; //set circle to indicator layer
        circleMove.transform.localScale = new Vector3(remainingDistance * 2, 0.1f, remainingDistance * 2);
    }

    void removeLastWayPoint()
    {
        if (wayPoints.Count > 1)
        {
            Debug.Log("Cancelling last point");
            Destroy(circleMove);
            remainingDistance += Vector3.Distance(wayPoints[wayPoints.Count - 1], wayPoints[wayPoints.Count - 2]);
            wayPoints.RemoveAt(wayPoints.Count - 1);
            UnitHandler.unitPathRender.SetVertexCount(wayPoints.Count);
            createMoveCircle();
        }
        else
            abortMove();
        
    }

    void abortMove()
    {
        Debug.Log("Move aborted");
        Destroy(circleMove);
        wayPoints = new List<Vector3>();
        moveReady = false;
        UnitHandler.unitPathRender.SetVertexCount(0);
        UnitHandler.canSelect = true;
        showThreat(false);
    }

    public static Vector3 lastWayPoint()
    {
        Debug.Log("Current count: " + wayPoints.Count);
        if (wayPoints.Count == 0)
            return UnitHandler.unit1.unit.transform.position;
        else
            return wayPoints[wayPoints.Count - 1];
    }

    /*********************************************************/
    /*FLUFF */
    /*********************************************************/
    public static void renderPath(Vector3[] points)
    {
        UnitHandler.unitPathRender.SetVertexCount(points.Length);

        for (int i = 0; i < points.Length; i++)
        {
            UnitHandler.unitPathRender.SetPosition(i, points[i] + new Vector3(0, 0.5f, 0));
        }
    }

    public static void renderWaypoints()
    {
        UnitHandler.unitPathRender.SetVertexCount(wayPoints.Count);

        for (int i = 0; i < wayPoints.Count; i++)
        {
            UnitHandler.unitPathRender.SetPosition(i, wayPoints[i]);
        }
    }

    /********************************************************/
    /*THREAT AREAS */
    /********************************************************/
    void showThreat(bool state)
    {
        if (TurnHandler.isTeam1Turn)
            foreach (UnitBase unit in UnitHandler.listTeam2)
                unit.toggleAtkCircle(state);
        else
            foreach (UnitBase unit in UnitHandler.listTeam1)
                unit.toggleAtkCircle(state);
    }


}

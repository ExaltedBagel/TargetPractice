using UnityEngine;
using System.Collections;

public class UnitMoveHandler : MonoBehaviour {

    public static bool moveReady;

    // Use this for initialization
    void Start () {
        moveReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*********************************************/
        /*MOVING PATH INDICATOR */
        /*********************************************/
        if (moveReady && !(UnitHandler.unit1.hasActed && UnitHandler.unit1.hasMoved))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                UnitHandler.unit1.CalculatePathLength(hit.point);
                
            }

        }

        if (!(Equals(UnitHandler.unit1, null)))
            if (UnitHandler.unit1.moving)
            {
                UnitHandler.unit1.updatePos();
            }

        /*********************************************/
        /*MOVE COMMAND */
        /*********************************************/
        if (Input.GetMouseButtonDown(1))
        {
            if (!(Equals(UnitHandler.unit1, null)) && moveReady)
            {
                UnitAttackHandler.atkReady = false;
                moveReady = false;
                UnitHandler.canSelect = false;
                UnitHandler.unit1.moveUnit();
                DebugHUDHandler.showTeam1();
            }
        }
    }

    public void moveUnit()
    {
        if (!(Equals(UnitHandler.unit1, null)) && !moveReady && UnitHandler.canSelect)
        {
            if (!(UnitHandler.unit1.hasActed && UnitHandler.unit1.hasMoved))
            {
                Debug.Log("MoveReady");
                moveReady = true;
            }
            else
                Debug.Log("Unit has no more actions left");
        }

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
}

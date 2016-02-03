using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class UnitHandler : MonoBehaviour
{
    static public List<Unit> units = new List<Unit>();
    static public LayerMask team1;
    static public LayerMask team2;
    static public LayerMask unitsLayer;

    static public Unit unit1;
    static public Unit unit2;
    static public List<Unit> listTeam1 = new List<Unit>();
    static public List<Unit> listTeam2 = new List<Unit>();

    EventSystem ev;

    //Selection Things
    public Material mouseOver;
    public Material selected;
    
    public static GameObject hovered;
    public static GameObject lastSelected;

    static public bool canSelect = true;

    static public LineRenderer unitPathRender;

    /*********************************************************/
    /*DEBUG PART*/
    /*********************************************************/
    bool initiated = false;



    void Awake()
    {
        ev = this.GetComponentInParent<EventSystem>();
        ev.GetComponentInParent<PhysicsRaycaster>();
        unitPathRender = GetComponentInChildren<LineRenderer>();       
        team1 = (1 << 10);
        team2 = (1 << 11);
        unitsLayer = (team1 | team2);       
    }
	
	// Update is called once per frame
	void Update () {

        updateSelectionCircle();
        //Selection
        if (Input.GetMouseButtonDown(0) && canSelect)
        {
            RaycastHit hit;

            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, unitsLayer))
            {
                GameObject objDetected = hit.collider.gameObject;
                //If you click the circle, you get the actual unit tied to the circle.
                if (!Equals(objDetected.transform.root, objDetected.transform))
                    objDetected = objDetected.transform.root.gameObject;       

                unit1 = units.Find(x => x.getId() == objDetected.GetInstanceID());
                unit1.printName();

                if (!Equals(lastSelected, null))
                    lastSelected.SetActive(false);

                lastSelected = hovered;

                updateSelectionCircle();
                //updatePolygon();
                //updateLine();
                //updateContourPoints();

            }
            else if (!EventSystem.current.IsPointerOverGameObject() && !Equals(unit1, null) && !unit1.moving)
            {
                unit1.toggleAtkCircle(false);
                unit1 = null;
                lastSelected.SetActive(false);
                lastSelected = null;
                Debug.Log("Unselected");
            }
            else
            {
                Debug.Log("OverMenu");
            }
            
        }
       
    }

    

    /********************************************/
    /*ADDS A NEW UNIT TO THE UNIT LIST*/
    /********************************************/

    public static void addToUnitList(Unit newUnit)
    {
        units.Add(newUnit);
        newUnit.init();
        DebugHUDHandler.updateText();
    }

    /********************************************/
    /*REMOVES A NEW UNIT TO THE UNIT LIST*/
    /********************************************/
    public static void removeFromUnitList(Unit removedUnit)
    {
        TurnHandler.removeFromBucket(removedUnit, removedUnit.team);
        units.Remove(removedUnit);
        DebugHUDHandler.updateText();
    }


    /**********************************************************/
    /*SORT TEAMS*/
    /**********************************************************/
    public void makeTeams()
    {
        listTeam1 = new List<Unit>();
        listTeam2 = new List<Unit>();

        foreach(Unit x in units)
        {
            if (x.team == 1)
                listTeam1.Add(x);
            else
                listTeam2.Add(x);
        }
        
        DebugHUDHandler.showTeam1();
}


    /********************************************/
    /*SELECTION CIRCLE UPDATE*/
    /*******************************************/
    private void updateSelectionCircle()
    {
        RaycastHit hit;
        if(!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, unitsLayer))
        {
            if (hit.collider.tag == "Unit" && hit.collider.gameObject != unit1)
            {
                hovered = units.Find(x => x.getId() == hit.collider.gameObject.GetInstanceID()).unit;
                if (!Equals(hovered, null))
                {
                    GameObject circle = hovered.transform.GetChild(2).gameObject;
                    Renderer render = circle.GetComponent<Renderer>();
                    render.material = mouseOver;
                    circle.SetActive(true);
                    if(!Equals(hovered, lastSelected))
                        hovered = circle;
                }
            }
            else if (!Equals(unit1, null) && !Equals(lastSelected, null) && !Equals(lastSelected.GetComponent<Renderer>().material, selected))
            {
                Renderer render = lastSelected.GetComponent<Renderer>();
                render.material = selected;
            }

        }
        else if(!Equals(hovered, null) && !Equals(hovered, lastSelected))
        {
            hovered.SetActive(false);
            hovered = null;
        }
    }


    /********************************************/
    /*SELECTION CIRCLE UPDATE*/
    /*******************************************/

    /******************************************************************/
    /*PATH RENDER STUFF THAT DOESNT WORK YET*/
    /*****************************************************************/

    public static List<Vector3> renderPathableArea(Unit unit1)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        pathPoints.Add(unit1.unit.transform.position); //Add the center as first point
        
        for (int angle = 0; angle < 360; angle += 5)
        {
            bool pointFound = false;
            float effectiveSpeed = unit1.speed;
            while (!pointFound)
            {
                Vector3 origin = (unit1.unit.transform.position + ((Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * effectiveSpeed) + Vector3.up * 10));
                Ray checker = new Ray(origin, Vector3.down);
                RaycastHit hit;
                int worldLayer = 1 << 8;
                if (Physics.Raycast(checker, out hit, Mathf.Infinity, worldLayer))
                {
                    Vector3 dest = unit1.CalculatePathEndPoint(hit.point);
                    if (dest != new Vector3(0, -100f, 0))
                    {
                        pathPoints.Add(dest);
                        pointFound = true;
                    }
                    //If point is not found or innaccessible, try closer
                    else if (effectiveSpeed > 0.1f)
                    {
                        effectiveSpeed -= unit1.speed * 0.1f;
                    }
                    else
                    {
                        Debug.Log("point not accessible");
                        pathPoints.Add(origin); //Maybe bad
                        pointFound = true;
                    }
                }
                //Point may be off the map also
                else if (effectiveSpeed > 0.1f)
                {
                    effectiveSpeed -= unit1.speed * 0.1f;
                }
                else
                {
                    Debug.Log("point not accessible");
                    pathPoints.Add(origin); //Maybe bad
                    pointFound = true;
                }
            }
        }

        return pathPoints;
        
    }

    /***********************************************************
    /* http://forum.unity3d.com/threads/draw-polygon.54092/ */
    /* Big credits to them */
    /*********************************************************/
    private Mesh drawMesh(List<Vector3> pathPoints)
    {
        Mesh mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh;

        //Vertices
        Vector3[] vertices = new Vector3[pathPoints.Count];

        for (int i = 0; i < pathPoints.Count; i++)
        {
            vertices[i] = pathPoints[i];
        }

        //UV (kind of irrelevant if we are not using texture)
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            if ((i % 2) == 0)
            {
                uvs[i] = new Vector2(0, 0);
            }
            else
            {
                uvs[i] = new Vector2(1, 1);
            }
        }

        //Triangles
        int[] tris = new int[3 * (vertices.Length - 2)];    //3 verts per triangle * num triangles
        //Each C is a corner, c1 is always the center point.
        int C1;
        int C2;
        int C3;

        C1 = 0;
        C2 = 1;
        C3 = 2;

        for (int i = 0; i < tris.Length; i += 3)
        {
            tris[i] = C1;
            tris[i + 1] = C2;
            tris[i + 2] = C3;

            C2++;
            C3++;
        }

        //Assign data to mesh
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;

       

        //Name the mesh
        mesh.name = "MyMesh";

        //Return the mesh
        return mesh;

    }


    public void updatePolygon()
    {
        Debug.Log("DrawingMesh");
        Mesh mesh = new Mesh();
        
        GameObject myChildObject = unit1.unit.transform.GetChild(1).gameObject;
        myChildObject.transform.position = new Vector3(0, 0, 0);
        

        //Components
        MeshFilter MF = myChildObject.GetComponent<MeshFilter>();
        MeshRenderer MR = myChildObject.GetComponent<MeshRenderer>();

        MF.mesh.Clear();

        //Create mesh
        mesh = drawMesh(renderPathableArea(unit1));

        //Recalculations
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();


        //Assign materials
        MR.material = MF.GetComponent<Material>();

        //Assign mesh to game object
        MF.mesh = mesh;
        
    }
 
    public void updateContourPoints()
    {
        
        Debug.Log("DrawingParticles");
        List<Vector3> pathPoints = renderPathContourPoints();
        /*
        ParticleSystem.Particle[] parts = new ParticleSystem.Particle[pathPoints.Count];
        
        GameObject myChildObject = unit1.unit.transform.GetChild(1).gameObject;
        myChildObject.transform.position = new Vector3(0, 0, 0);

        for (int i = 0; i < pathPoints.Count; i++)
        {
            parts[i].position = pathPoints[i];
            parts[i].color = Color.blue;
            parts[i].size = 15f;
        }

        ParticleSystem PS = myChildObject.GetComponent<ParticleSystem>();
        PS.SetParticles(parts, parts.Length);
        PS.Emit(parts.Length);
        */
    }

    public List<Vector3> renderPathContourPoints()
    {
        List<Vector3> pathPoints = new List<Vector3>();
        //pathPoints.Add(unit1.unit.transform.position); //Add the center as first point

        for (int angle = 0; angle < 360; angle += 5)
        {
            bool pointFound = false;
            float effectiveSpeed = unit1.speed;
            while (!pointFound)
            {
                Vector3 origin = (unit1.unit.transform.position + ((Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * effectiveSpeed) + Vector3.up * 10));
                Ray checker = new Ray(origin, Vector3.down);
                RaycastHit hit;
                int worldLayer = 1 << 8;
                if (Physics.Raycast(checker, out hit, Mathf.Infinity, worldLayer))
                {
                    Vector3[] dest = unit1.CalculatePathSections(hit.point);
                    if (dest != null)
                    {
                        foreach(Vector3 point in dest)
                            if(!pathPoints.Contains(point))
                                pathPoints.Add(point);
                        pointFound = true;
                    }
                    //If point is not found or innaccessible, try closer
                    
                    else if (effectiveSpeed > 0.1f)
                    {
                        effectiveSpeed -= unit1.speed * 0.1f;
                    }
                    
                    else
                    {
                        Debug.Log("point not accessible");
                        //pathPoints.Add(origin); //Maybe bad
                        pointFound = true;
                    }
                }
                //Point may be off the map also
                else if (effectiveSpeed > 0.1f)
                {
                    effectiveSpeed -= unit1.speed * 0.1f;
                }
                else
                {
                    Debug.Log("point not accessible");
                    //pathPoints.Add(origin); //Maybe bad
                    pointFound = true;
                }
            }
        }

        return pathPoints;
    }

    public void updateLine()
    {
        Debug.Log("DrawingLine");

        GameObject myChildObject = unit1.unit.transform.GetChild(1).gameObject;
        myChildObject.transform.position = new Vector3(0, 0, 0);

        LineRenderer line = myChildObject.GetComponent<LineRenderer>();
        List<Vector3> pathPoints = renderPathContourPoints();

        line.SetVertexCount(pathPoints.Count);
        for(int i= 0; i < pathPoints.Count; i++)
        {
            line.SetPosition(i, pathPoints[i]);
        }
    }

    public static void erasePath()
    {
        unitPathRender.SetVertexCount(0);
        
    }

}

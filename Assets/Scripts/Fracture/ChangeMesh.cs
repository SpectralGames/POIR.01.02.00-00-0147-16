using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeMesh : MonoBehaviour
{
    public Plane plane;
    public GameObject heightReferenceObject;
    public GameObject instance;
    public float heightReferenceFloat = 1.5f;
    float heightCutOff;
    public bool cutUp = true;


    Mesh mesh = null;// inst.GetComponent<MeshFilter>().mesh;
    int[] triangles;// mesh.triangles;
    Vector3[] vertices;// = mesh.vertices;
    Vector2[] uv;// = mesh.uv;
    Vector3[] normals;// = mesh.normals;
    List<Vector3> vertList = new List<Vector3>();
    List<Vector2> uvList = new List<Vector2>();
    List<Vector3> normalsList = new List<Vector3>();
    List<int> trianglesList = new List<int>();

    void Start ()
    {
        if (instance == null)
            instance = this.gameObject;
        Init(instance);
    }

    private void Init (GameObject inst)
    {
        mesh = inst.GetComponent<MeshFilter>().mesh;
        triangles = mesh.triangles;
        vertices = mesh.vertices;
        uv = mesh.uv;
        normals = mesh.normals;


        if (heightReferenceObject != null)
        {
            heightCutOff = heightReferenceObject.transform.position.y;
        }
        else
        {
            heightCutOff = heightReferenceFloat;
        }

        int i = 0;
        while (i < vertices.Length)
        {
            vertList.Add(vertices[i]);
            uvList.Add(uv[i]);
            normalsList.Add(normals[i]);
            i++;
        }


        if (cutUp)
            CutUp();
        else
            CutDown();


        //triangles = trianglesList.ToArray();
        //vertices = vertList.ToArray();

        uv = uvList.ToArray();
        normals = normalsList.ToArray();
        //mesh.Clear();
        mesh.triangles = triangles;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.normals = normals;
    }

    //private void OnDrawGizmos ()
    //{
    //    SnapVertexes();        
    //}

    private List<Vector3> connectors = new List<Vector3>();
    private List<int> connectorsIndexes = new List<int>();

    private List<Vector3> toConnect = new List<Vector3>();
    private List<int> toConnectIndexes = new List<int>();


    private void CutUp ()
    {
        Debug.Log("triangles.Length " + triangles.Length);
        //vertList.Clear();
        //uvList.Clear();
        //normalsList.Clear();
        for (int triCount = 0; triCount < triangles.Length; triCount += 3)
        {
            if ((instance.transform.TransformPoint(vertices[triangles[triCount]]).y <= heightCutOff) &&
                (instance.transform.TransformPoint(vertices[triangles[triCount + 1]]).y <= heightCutOff) &&
                (instance.transform.TransformPoint(vertices[triangles[triCount + 2]]).y <= heightCutOff))
            {
                //AddNewTriangles(triCount);
                AddConnectors(triCount);
                //AddNewVertices(triCount);
                //AddNewUVs(triCount);
                //AddNewNormals(triCount);
            }
            else
            {
                AddVertexesToConnects(triCount);
            }
        }
        Debug.Log("CutUp connectors.Count: " + connectors.Count);
        Debug.Log("CutUp toConnect.Count: " + toConnect.Count);
        //DrawConnectors();
        SnapVertexes();
    }

    private void CutDown ()
    {
        Debug.Log("triangles.Length " + triangles.Length);
        //vertList.Clear();
        //uvList.Clear();
        //normalsList.Clear();
        for (int triCount = 0; triCount < triangles.Length; triCount += 3)
        {
            if ((instance.transform.TransformPoint(vertices[triangles[triCount]]).y >= heightCutOff) ||
                (instance.transform.TransformPoint(vertices[triangles[triCount + 1]]).y >= heightCutOff) ||
                (instance.transform.TransformPoint(vertices[triangles[triCount + 2]]).y >= heightCutOff ))
            {
                //AddNewTriangles(triCount); // nie dodawaj nowych trojkatow
                AddConnectors(triCount);
                //AddNewVertices(triCount);
                //AddNewUVs(triCount);
                //AddNewNormals(triCount);
            }
            else
            {
                AddVertexesToConnects(triCount);
            }
        }
        Debug.Log("CutDown connectors.Count: " + connectors.Count);
        Debug.Log("CutDown toConnect.Count: " + toConnect.Count);
        //DrawConnectors();
        SnapVertexes();
    }

    private void AddConnectors(int triCount)
    {
        connectors.Add(vertices[triangles[triCount]]);
        connectors.Add(vertices[triangles[triCount + 1]]);
        connectors.Add(vertices[triangles[triCount + 2]]);

        connectorsIndexes.Add(triangles[triCount]);
        connectorsIndexes.Add(triangles[triCount + 1]);
        connectorsIndexes.Add(triangles[triCount + 2]);
    }

    private void AddVertexesToConnects(int triCount)
    {
        toConnect.Add(vertices[triangles[triCount]]);
        toConnect.Add(vertices[triangles[triCount + 1]]);
        toConnect.Add(vertices[triangles[triCount + 2]]);

        toConnectIndexes.Add(triangles[triCount]);
        toConnectIndexes.Add(triangles[triCount + 1]);
        toConnectIndexes.Add(triangles[triCount + 2]);
    }

    private void DrawConnectors()
    {
        foreach (Vector3 v in connectors)
        {
            GameObject con = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            con.transform.SetParent(instance.transform);
            con.transform.localPosition = v;
            float scaleSize = 0.1f;
            con.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        }
    }

    private void SnapVertexes()
    {
        
        Gizmos.color = Color.red;
        foreach (int i in toConnectIndexes)
        {
            float minDist = Mathf.Infinity;
            Vector3 v = vertices[i];
            Vector3 vSnap = Vector3.zero;

            foreach (Vector3 vec in connectors)
            {
                float currDist = Vector3.Distance(v, vec);
                if (currDist < minDist)
                {
                    minDist = currDist;
                    vSnap = vec;
                }
            }

            vertices[i] = vSnap;
            //Debug.Log(vSnap);
            //Gizmos.DrawLine(vertices[i] + instance.transform.position, vSnap + instance.transform.position);
            //v = vSnap;
        }
    }

    private void AddNewVertices (int triCount)
    {
        vertList.Add(vertices[triangles[triCount]]);
        vertList.Add(vertices[triangles[triCount + 1]]);
        vertList.Add(vertices[triangles[triCount + 2]]);
    }

    private void AddNewTriangles (int triCount)
    {
        trianglesList.Add(triangles[triCount]);
        trianglesList.Add(triangles[triCount + 1]);
        trianglesList.Add(triangles[triCount + 2]);
    }

    private void AddNewUVs (int triCount)
    {
        Debug.Log(uv.Length + ", tri: " + triCount);
        uvList.Add(uv[triCount]);
        uvList.Add(uv[triCount + 1]);
        uvList.Add(uv[triCount + 2]);
    }

    private void AddNewNormals (int triCount)
    {
        normalsList.Add(normals[triCount]);
        normalsList.Add(normals[triCount + 1]);
        normalsList.Add(normals[triCount + 2]);
    }

}
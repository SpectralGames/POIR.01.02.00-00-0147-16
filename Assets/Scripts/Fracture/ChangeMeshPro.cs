using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeMeshPro : MonoBehaviour
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


        Debug.Log("---------------------");
        foreach (int j in trianglesList)
        {
            
            Debug.Log(j);
        }
        Debug.Log("---------------------");

        triangles = trianglesList.ToArray();
        vertices = vertList.ToArray();

        Debug.Log("triangles: " + triangles.Length + ", vert: " + vertices.Length);

        mesh.Clear();
        uv = uvList.ToArray();
        normals = normalsList.ToArray();
        //mesh.Clear();
        mesh.triangles = triangles;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.normals = normals;

       // WeldVertices(mesh);
    }

    private void CutUp ()
    {
        Debug.Log("triangles.Length " + triangles.Length);
        //vertList.Clear();

        for (int triCount = 0; triCount < triangles.Length; triCount += 3)
        {
            if ((instance.transform.TransformPoint(vertices[triangles[triCount]]).y <= heightCutOff) &&
                (instance.transform.TransformPoint(vertices[triangles[triCount + 1]]).y <= heightCutOff) &&
                (instance.transform.TransformPoint(vertices[triangles[triCount + 2]]).y <= heightCutOff))
            {
                AddNewTriangles(triCount);
                AddVertexes(triCount);
                AddConnectors(triCount);
            }
        }
        vertList = DeleteDuplicates(vertList);
       // DrawConnectors();
    }

    private void CutDown ()
    {
        Debug.Log("triangles.Length " + triangles.Length);
        //vertList.Clear();

        for (int triCount = 0; triCount < triangles.Length; triCount += 3)
        {
            if ((instance.transform.TransformPoint(vertices[triangles[triCount]]).y >= heightCutOff) ||
                (instance.transform.TransformPoint(vertices[triangles[triCount + 1]]).y >= heightCutOff) ||
                (instance.transform.TransformPoint(vertices[triangles[triCount + 2]]).y >= heightCutOff))
            {
                AddNewTriangles(triCount);
                AddVertexes(triCount);
                AddConnectors(triCount);
            }
        }
        vertList = DeleteDuplicates(vertList);
        //DrawConnectors();
    }

    private void AddConnectors(int triCount)
    {
        connectors.Add(vertices[triangles[triCount]]);
        connectors.Add(vertices[triangles[triCount + 1]]);
        connectors.Add(vertices[triangles[triCount + 2]]);
    }

    private void DrawConnectors ()
    {
        connectors = DeleteDuplicates(connectors);
        foreach (Vector3 v in connectors)
        {
            GameObject con = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            con.transform.SetParent(instance.transform);
            con.transform.localPosition = v;
            float scaleSize = 0.1f;
            con.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        }
    }

    private List<Vector3> DeleteDuplicates (List<Vector3> list)
    {
        List<Vector3> newList = new List<Vector3>();

        foreach (Vector3 item in list)
        {
            if (!newList.Contains(item))
                newList.Add(item);
        }

        return newList;
    }

    
    private void AddNewTriangles (int triCount)
    {
        trianglesList.Add(triangles[triCount]);
        trianglesList.Add(triangles[triCount + 1]);
        trianglesList.Add(triangles[triCount + 2]);
    }

    private List<Vector3> connectors = new List<Vector3>();
    private void AddVertexes(int triCount)
    {
        vertList.Add(vertices[triangles[triCount]]);
        vertList.Add(vertices[triangles[triCount + 1]]);
        vertList.Add(vertices[triangles[triCount + 2]]);
    }

    private void ShowTriangles()
    {

    }


    public static void WeldVertices (Mesh aMesh, float aMaxDelta = 0.001f)
    {
        var verts = aMesh.vertices;
        var normals = aMesh.normals;
        var uvs = aMesh.uv;
        List<int> newVerts = new List<int>();
        int[] map = new int[verts.Length];
        // create mapping and filter duplicates.
        for (int i = 0; i < verts.Length; i++)
        {
            var p = verts[i];
            var n = normals[i];
            var uv = uvs[i];
            bool duplicate = false;
            for (int i2 = 0; i2 < newVerts.Count; i2++)
            {
                int a = newVerts[i2];
                if (
                    (verts[a] - p).sqrMagnitude <= aMaxDelta && // compare position
                    Vector3.Angle(normals[a], n) <= aMaxDelta && // compare normal
                    (uvs[a] - uv).sqrMagnitude <= aMaxDelta // compare first uv coordinate
                    )
                {
                    map[i] = i2;
                    duplicate = true;
                    break;
                }
            }
            if (!duplicate)
            {
                map[i] = newVerts.Count;
                newVerts.Add(i);
            }
        }
        // create new vertices
        var verts2 = new Vector3[newVerts.Count];
        var normals2 = new Vector3[newVerts.Count];
        var uvs2 = new Vector2[newVerts.Count];
        for (int i = 0; i < newVerts.Count; i++)
        {
            int a = newVerts[i];
            verts2[i] = verts[a];
            normals2[i] = normals[a];
            uvs2[i] = uvs[a];
        }
        // map the triangle to the new vertices
        var tris = aMesh.triangles;
        for (int i = 0; i < tris.Length; i++)
        {
            tris[i] = map[tris[i]];
        }
        Debug.Log("verts: " + verts2.Length + ", norm: " + normals2.Length);
        aMesh.vertices = verts2;
        aMesh.normals = normals2;
        aMesh.uv = uvs2;
        aMesh.triangles = tris;
    }
}
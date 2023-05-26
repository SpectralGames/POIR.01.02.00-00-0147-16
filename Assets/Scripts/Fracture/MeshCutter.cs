using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MeshCutter
{
    public Mesh mesh;// = GetComponent<MeshFilter>().mesh;
    public int[] triangles;// = mesh.triangles;
    public Vector3[] vertices;// = mesh.vertices;
    public Vector2[] uv;// = mesh.uv;
    public Vector3[] normals;// = mesh.normals;
    public List<Vector3> vertList = new List<Vector3>();
    public List<Vector2> uvList = new List<Vector2>();
    public List<Vector3> normalsList = new List<Vector3>();
    public List<int> trianglesList = new List<int>();
    private GameObject objectInstance;


    public MeshCutter(GameObject go)
    {
        objectInstance = go;
        mesh = go.GetComponent<MeshFilter>().mesh;
        if (mesh != null)
        {
            triangles = mesh.triangles;
            vertices = mesh.vertices;
            uv = mesh.uv;
            normals = mesh.normals;
        }
        //else error

        FillLists();
    }

    private void FillLists()
    {
        int i = 0;
        while (i < vertices.Length)
        {
            vertList.Add(vertices[i]);
            uvList.Add(uv[i]);
            normalsList.Add(normals[i]);
            i++;
        }
    }

    public void SplitAlongPlane(Plane plane)
    {
        Debug.Log("triangles.Length: " + triangles.Length);
        for (int triCount = 0; triCount < triangles.Length; triCount += 3)
        {
            if (plane.GetSide(objectInstance.transform.TransformPoint(vertices[triangles[triCount]])) &&
                plane.GetSide(objectInstance.transform.TransformPoint(vertices[triangles[triCount + 1]])) &&
                plane.GetSide(objectInstance.transform.TransformPoint(vertices[triangles[triCount + 2]])) )
            {
                AddNewTriangles(triCount);
            }
        }

        //CreateNew();
    }

    private void AddNewTriangles (int triCount)
    {
        trianglesList.Add(triangles[triCount]);
        trianglesList.Add(triangles[triCount + 1]);
        trianglesList.Add(triangles[triCount + 2]);
    }

    public void SplitAlongOpositePlane (Plane plane)
    {
        for (int triCount = 0; triCount < triangles.Length; triCount += 3)
        {
            if (!plane.GetSide(objectInstance.transform.TransformPoint(vertices[triangles[triCount]])) &&
                !plane.GetSide(objectInstance.transform.TransformPoint(vertices[triangles[triCount + 1]])) &&
                !plane.GetSide(objectInstance.transform.TransformPoint(vertices[triangles[triCount + 2]])))
            {
                AddNewTriangles(triCount);
            }
        }

        CreateNew();
    }

    public void CreateNew()
    {
        triangles = trianglesList.ToArray();
        vertices = vertList.ToArray();
        uv = uvList.ToArray();
        normals = normalsList.ToArray();
        //mesh.Clear();
        mesh.triangles = triangles;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.normals = normals;
        objectInstance.GetComponent<MeshFilter>().mesh.RecalculateBounds();

        if (!objectInstance.GetComponent<Rigidbody>())
        {
            objectInstance.AddComponent<Rigidbody>();
            objectInstance.AddComponent<Rigidbody>();
            objectInstance.GetComponent<Rigidbody>().mass *= 0.5f;
        }
    }
}
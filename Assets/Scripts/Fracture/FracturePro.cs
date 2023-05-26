using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 
    znalezc vertxy ktore sa najblizej plane'a (connectors)
    wziac vertexy ktore sa po drugiej stronie plane'a i dac im pozycje
    najblizszego vertexa z connectors
 */


public class FracturePro : MonoBehaviour
{
    bool fractureToPoint = false;
    int totalMaxFractures = 1;
    float forcePerDivision = 3.0f;
    public float minBreakingForce = 0.0f;
    int maxFracturesPerCall = 3;
    float randomOffset = 0.0f;
    Vector3 minFractureSize = Vector3.zero;
    Vector3 grain = Vector3.one;
    float useCollisionDirection = 0.0f;
    bool fractureAtCenter = false;
    float destroyAllAfterTime = 0.0f;
    float destroySmallAfterTime = 0.0f;
    GameObject instantiateOnBreak;
    float totalMassIfStatic = 1.0f;


    void Awake ()
    {
        Rigidbody rigidBody = this.gameObject.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.sleepThreshold = 20f;
            rigidBody.Sleep();
        }
    }

    //-------------------------------------------------------------------
    void Start ()
    {

    }

    //-------------------------------------------------------------------
    void OnCollisionEnter (Collision collision)
    {
        Vector3 point = collision.contacts[0].point;
        Vector3 vec = collision.relativeVelocity * UsedMass(collision);
        FractureAtPoint(point, vec);
    }

    //-------------------------------------------------------------------
    void FractureAtPoint (Vector3 hit, Vector3 force)
    {
        if (force.magnitude < Mathf.Max(minBreakingForce, forcePerDivision))
        { return; }

        int iterations = Mathf.Min(Mathf.RoundToInt(force.magnitude / forcePerDivision), Mathf.Min(maxFracturesPerCall, totalMaxFractures));
        Vector3 point = transform.worldToLocalMatrix.MultiplyPoint(hit);
        //Fracture(point, force, iterations);
        StartCoroutine(Fracture(point, force));
    }

    //-------------------------------------------------------------------
    public IEnumerator Fracture (Vector3 point, Vector3 force)
    {
        // define the splitting plane by the user settings.
        if (fractureAtCenter)
        {
            point = GetComponent<MeshFilter>().mesh.bounds.center;
        }
        Vector3 vec = Vector3.Scale(grain, Random.insideUnitSphere).normalized;
        Vector3 sub = transform.worldToLocalMatrix.MultiplyVector(force.normalized) * useCollisionDirection * Vector3.Dot(transform.worldToLocalMatrix.MultiplyVector(force.normalized), vec);
        Plane plane = new Plane(vec - sub, Vector3.Scale(Random.insideUnitSphere, this.GetComponent<MeshFilter>().mesh.bounds.size) * randomOffset + point);

        // create the clone
        GameObject newObject = Instantiate(gameObject, transform.position, transform.rotation);

        MeshCutter objA = new MeshCutter(gameObject);
        MeshCutter objB = new MeshCutter(newObject);

        if (this.GetComponent<Rigidbody>())
        {
            newObject.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity;
        }
        // arrays of the verts
        Vector3[] vertsA = gameObject.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] vertsB = newObject.GetComponent<MeshFilter>().mesh.vertices;
        Vector3 average = Vector3.zero;
        foreach (Vector3 i in vertsA)
        {
            average += i;
        }

        Debug.Log("all vert: " + average);
        average /= gameObject.GetComponent<MeshFilter>().mesh.vertexCount;
        Debug.Log("avg: " + average);
        average -= plane.GetDistanceToPoint(average) * plane.normal;
        Debug.Log("plane n: " + plane.normal);
        Debug.Log("avg = distToPoint - plane: " + plane.GetDistanceToPoint(average) + " - " + average);

        //-------------------------------------------------------------------
        int broken = 0;
        // split geometry along plane
        if (fractureToPoint)
        {
            for (int i = 0; i < gameObject.GetComponent<MeshFilter>().mesh.vertexCount; i++)
            {
                if (plane.GetSide(vertsA[i]))
                {
                    vertsA[i] = average;
                    //broken += 1; // ?
                }
                else
                {
                    vertsB[i] = average;
                }
            }
        }
        else
        {
            for (int i = 0; i < gameObject.GetComponent<MeshFilter>().mesh.vertexCount; i++)
            {
                if (plane.GetSide(vertsA[i]))
                {
                    vertsA[i] -= plane.GetDistanceToPoint(vertsA[i]) * plane.normal;
                    broken += 1;
                }
                else
                {
                    vertsB[i] -= plane.GetDistanceToPoint(vertsB[i]) * plane.normal;
                }
            }

            objA.SplitAlongPlane(plane);
            objB.SplitAlongOpositePlane(plane);
        }

        gameObject.GetComponent<MeshFilter>().mesh.triangles = objA.triangles;
        gameObject.GetComponent<MeshFilter>().mesh.vertices = objA.vertices;
        gameObject.GetComponent<MeshFilter>().mesh.uv = objA.uvList.ToArray();
        gameObject.GetComponent<MeshFilter>().mesh.normals = objA.normals;
        gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();


        newObject.GetComponent<MeshFilter>().mesh.triangles = objB.triangles;
        newObject.GetComponent<MeshFilter>().mesh.vertices = objB.vertices;
        newObject.GetComponent<MeshFilter>().mesh.uv = objB.uvList.ToArray();
        newObject.GetComponent<MeshFilter>().mesh.normals = objB.normals;
        newObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        newObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();

        yield return null;
    }

    private void Foo (GameObject go, Plane plane) // 
    {
        if (go == null)
            return;
        Vector3[] vert = go.GetComponent<MeshFilter>().mesh.vertices;

        Vector3 avg = Vector3.zero;

        foreach (Vector3 v in vert)
        {
            //DrawSphere(go, v);
            if (Mathf.Abs(plane.GetDistanceToPoint(v)) < 0.01f)
            {
                avg += v;
                DrawSphere(go, v, 0.05f);
            }
        }

        avg /= vert.Length;
        DrawSphere(go, avg, 0.1f);
    }

    private void DrawSphere (GameObject go, Vector3 v, float size)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(size, size, size);
        sphere.transform.SetParent(go.transform);
        sphere.transform.localPosition = v;
    }


    //--------------------------------------------------------------
    float UsedMass (Collision collision)
    {
        if (collision.rigidbody)
        {
            if (this.GetComponent<Rigidbody>())
            {
                if (collision.rigidbody.mass < this.GetComponent<Rigidbody>().mass)
                {
                    return (collision.rigidbody.mass);
                }
                else
                {
                    return (this.GetComponent<Rigidbody>().mass);
                }
            }
            else
            {
                return (collision.rigidbody.mass);
            }
        }
        else if (this.GetComponent<Rigidbody>())
        {
            return (this.GetComponent<Rigidbody>().mass);
        }
        else
        {
            return (1f);
        }
    }

}

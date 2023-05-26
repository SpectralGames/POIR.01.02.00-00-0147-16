using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureMesh : MonoBehaviour
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
    bool smartJoints = false;
    float destroyAllAfterTime = 0.0f;
    float destroySmallAfterTime = 0.0f;
    GameObject instantiateOnBreak;
    float totalMassIfStatic = 1.0f;
    [HideInInspector] public List<Joint> joints;

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
        if (this.GetComponent<Rigidbody>())
        {
            List<Joint> temp = new List<Joint>();
            foreach (Joint j in GameObject.FindObjectsOfType<Joint>())
            {
                if (j.connectedBody == this.GetComponent<Rigidbody>())
                {
                    temp.Add(j);
                }
            }

            if (temp.Count > 0)
            {
                joints = new List<Joint>();
                foreach (Joint j in temp)
                {
                    joints.Add(j);
                }
            }
        }
    }

    //-------------------------------------------------------------------
    void OnCollisionEnter (Collision collision)
    {
        var point = collision.contacts[0].point;
        var vec = collision.relativeVelocity * UsedMass(collision);
        FractureAtPoint(point, vec);
    }

    //-------------------------------------------------------------------
    void FractureAtPoint (Vector3 hit, Vector3 force)
    {
        if (force.magnitude < Mathf.Max(minBreakingForce, forcePerDivision))
        { return; }

        var iterations = Mathf.Min(Mathf.RoundToInt(force.magnitude / forcePerDivision), Mathf.Min(maxFracturesPerCall, totalMaxFractures));
        var point = transform.worldToLocalMatrix.MultiplyPoint(hit);
        //Fracture(point, force, iterations);
        StartCoroutine(Fracture(point, force, iterations));
    }

    //-------------------------------------------------------------------
    public IEnumerator Fracture (Vector3 point, Vector3 force, int iterations)
    {
        if (instantiateOnBreak && force.magnitude >= Mathf.Max(minBreakingForce, forcePerDivision))
        {
            Instantiate(instantiateOnBreak, transform.position, transform.rotation);
            instantiateOnBreak = null;
        }
        while (iterations > 0)
        {
            // if we are smaller than our minimum fracture size in any dimension, no more divisions.
            if (totalMaxFractures == 0 || Vector3.Min(gameObject.GetComponent<MeshFilter>().mesh.bounds.size, minFractureSize) != minFractureSize)
            {
                if (destroySmallAfterTime >= 1)
                {
                    Destroy(this.GetComponent<MeshCollider>(), destroySmallAfterTime - 1);
                    Destroy(gameObject, destroySmallAfterTime);
                }
                totalMaxFractures = 0;
                yield break;
                //return;
            }
            totalMaxFractures -= 1;
            iterations -= 1;
            // define the splitting plane by the user settings.
            if (fractureAtCenter)
            {
                point = GetComponent<MeshFilter>().mesh.bounds.center;
            }
            var vec = Vector3.Scale(grain, Random.insideUnitSphere).normalized;
            var sub = transform.worldToLocalMatrix.MultiplyVector(force.normalized) * useCollisionDirection * Vector3.Dot(transform.worldToLocalMatrix.MultiplyVector(force.normalized), vec);
            Plane plane = new Plane(vec - sub, Vector3.Scale(Random.insideUnitSphere, this.GetComponent<MeshFilter>().mesh.bounds.size) * randomOffset + point);

            // create the clone
            var newObject = Instantiate(gameObject, transform.position, transform.rotation);
            if (this.GetComponent<Rigidbody>())
            {
                newObject.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity;
            }
            // arrays of the verts
            var vertsA = gameObject.GetComponent<MeshFilter>().mesh.vertices;
            var vertsB = newObject.GetComponent<MeshFilter>().mesh.vertices;
            var average = Vector3.zero;
            foreach (Vector3 i in vertsA)
            {
                average += i;
            }

                                                                                    //Debug.Log("all vert: " + average);
            average /= gameObject.GetComponent<MeshFilter>().mesh.vertexCount;     // Debug.Log("avg: " + average);
            average -= plane.GetDistanceToPoint(average) * plane.normal;           // Debug.Log("plane n: " + plane.normal);
                                                                                   // Debug.Log("avg = distToPoint - plane: " + plane.GetDistanceToPoint(average) + " - " + average);

            //-------------------------------------------------------------------
            var broken = 0;
            // split geometry along plane
            if (fractureToPoint)
            {
                for (var i = 0; i < gameObject.GetComponent<MeshFilter>().mesh.vertexCount; i++)
                {
                    if (plane.GetSide(vertsA[i]))
                    {
                        vertsA[i] = average;
                        broken += 1;
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
            }
            // IMPORTANT: redo if we have a problem splitting; without this, we will get a lot of non-manifold meshes, convexhull errors and maybe even crash the game.
            if (broken == 0 || broken == gameObject.GetComponent<MeshFilter>().mesh.vertexCount)
            {
                totalMaxFractures += 1;
                iterations += 1;
                Destroy(newObject);
                // this yield is here JUST so that when a large amount of objects are being broken, the screen doesn't freeze for a long time. It allows the screen to refresh before we're finnished, but if you don't, it might slow the script down trying to break a loop of bad planes.
                yield return null; // trzeba zrobic korutyne z tego
            }
            // if all's fine, apply the changes to each mesh
            else
            {
                gameObject.GetComponent<MeshFilter>().mesh.vertices = vertsA;
                newObject.GetComponent<MeshFilter>().mesh.vertices = vertsB;
                gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                newObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                newObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                if (this.gameObject.GetComponent<MeshCollider>())
                {
                    gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
                    newObject.GetComponent<MeshCollider>().sharedMesh = newObject.GetComponent<MeshFilter>().mesh;
                }
                // if we weren't using a convexhull, the pieces colliders won't work right. It's best for everyone if we just remove them.
                else
                {
                    Destroy(this.GetComponent<Collider>());
                    Destroy(gameObject, 1);
                }

                // smartjoints will allow joints to function properly.
                #region pomin
                if (smartJoints)
                {
                    var jointsb = this.GetComponents<Joint>();
                    if (jointsb != null)
                    {
                        // Basically, it goes through each joint and sees if the object A or B are closer to the connected body. Whichever is closer keeps the joint.
                        for (int i = 0; i < jointsb.Length; i++)
                        {
                            if (jointsb[i].connectedBody != null && plane.GetSide(transform.worldToLocalMatrix.MultiplyPoint(jointsb[i].connectedBody.transform.position)))
                            {
                                if (jointsb[i].gameObject.GetComponent<FractureMesh>().joints != null)
                                {
                                    // If we're attached to a fracture object and the new object is closer, switch the connected object's joint variable at the correct index.
                                    for (int j = 0; j < jointsb[i].gameObject.GetComponent<FractureMesh>().joints.Count; j++) //   Joint c in jointsb[i].gameObject.GetComponent<FractureMesh>().joints) {
                                    {
                                        Joint c = jointsb[i].gameObject.GetComponent<FractureMesh>().joints[j];
                                        if (c == jointsb[i])
                                        {
                                            c = newObject.GetComponents<Joint>()[i];
                                        }
                                    }
                                }
                                Destroy(jointsb[i]);
                            }
                            else
                            {
                                Destroy(newObject.GetComponents<Joint>()[i]);
                            }
                        }
                    }
                    // joints contains all joints this object is attached to. It checks if the joint still exists, and if the new object is closer. If so, changes the connection. It then removes the joint from the joints variable at the correct index.
                    if (joints != null)
                    {
                        for (int i = joints.Count - 1; i >= 0; i--)
                        {
                            if (joints[i] && plane.GetSide(transform.worldToLocalMatrix.MultiplyPoint(joints[i].transform.position)))
                            {
                                joints[i].connectedBody = newObject.GetComponent<Rigidbody>();
                                joints.RemoveAt(i);
                            }
                            else
                            {
                                List<Joint> temp = new List<Joint>(joints);
                                temp.RemoveAt(i);
                                newObject.GetComponent<FractureMesh>().joints = temp;
                            }
                        }
                    }
                }
                // if we don't have smartJoints, the code is much shorter. destroy all joints.
                else
                {
                    if (this.GetComponent<Joint>() != null)
                    {
                        var myJoints = this.GetComponents<Joint>();
                        foreach (var singleJoint in myJoints)
                            Destroy(singleJoint);

                        //var newObjectJoints = newObject.GetComponents<Joint>();
                        foreach (var singleJoint in myJoints)
                            Destroy(singleJoint);
                        //for (int i=0; i<this.GetComponents<Joint>().Length;i++){
                        //	Destroy(this.GetComponents<Joint>()[i]);
                        //	Destroy(newObject.GetComponents<Joint>()[i]);
                        //}
                    }
                    if (joints != null)
                    {
                        for (int i = 0; i < joints.Count; i++)
                        {
                            Destroy(joints[i]);
                        }
                        joints = null;
                    }
                }
                #endregion
                // if the script is attached to a static object, make it dynamic. If not, divide the mass up.
                if (!this.GetComponent<Rigidbody>())
                {
                    gameObject.AddComponent<Rigidbody>();
                    newObject.AddComponent<Rigidbody>();
                    this.GetComponent<Rigidbody>().mass = totalMassIfStatic;
                    newObject.GetComponent<Rigidbody>().mass = totalMassIfStatic;
                }
                gameObject.GetComponent<Rigidbody>().mass *= 0.5f;
                newObject.GetComponent<Rigidbody>().mass *= 0.5f;
                gameObject.GetComponent<Rigidbody>().centerOfMass = transform.worldToLocalMatrix.MultiplyPoint3x4(gameObject.GetComponent<Collider>().bounds.center);
                newObject.GetComponent<Rigidbody>().centerOfMass = transform.worldToLocalMatrix.MultiplyPoint3x4(newObject.GetComponent<Collider>().bounds.center);

                newObject.GetComponent<FractureMesh>().Fracture(point, force, iterations);

                if (destroyAllAfterTime >= 1)
                {
                    Destroy(newObject.GetComponent<MeshCollider>(), destroyAllAfterTime - 1);
                    Destroy(this.GetComponent<MeshCollider>(), destroyAllAfterTime - 1);
                    Destroy(newObject, destroyAllAfterTime);
                    Destroy(gameObject, destroyAllAfterTime);
                }
                // this yield is here JUST so that when a large amount of objects are being broken, the screen doesn't freeze for a while.
                yield return null;
            }// if not broken end
            //Foo(gameObject, plane);
        }// while itterations end
        if (totalMaxFractures == 0 || Vector3.Min(gameObject.GetComponent<MeshFilter>().mesh.bounds.size, minFractureSize) != minFractureSize)
        {
            if (destroySmallAfterTime >= 1)
            {
                Destroy(this.GetComponent<MeshCollider>(), destroySmallAfterTime - 1);
                Destroy(gameObject, destroySmallAfterTime);
            }
            totalMaxFractures = 0;
        }
    }

    private void Foo(GameObject go, Plane plane) // 
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
        //DrawSphere(go, avg, 0.1f);
    }

    private void DrawSphere(GameObject go, Vector3 v, float size)
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

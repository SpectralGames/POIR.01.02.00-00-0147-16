using UnityEngine;
using System.Collections;

public class BezierPathController : MonoBehaviour
{


    public BezierPath bezierPath;
    private float distance = 0.0f;
    private float speed = 10.0f;

    private BezierPath.BezierPathPoint pathPoint = new BezierPath.BezierPathPoint();

    void Update()
    {
        if (bezierPath)
        {
            pathPoint.position = transform.position;
            pathPoint.rotation = transform.rotation;

            distance += speed * Time.deltaTime / bezierPath.Length(pathPoint.currentNextPoint);

            bezierPath.Evaluate(distance, ref pathPoint);

            if (pathPoint.currentPath != bezierPath)
            {
                //Debug.Log("Reached: "+pathPoint.currentPath);
                bezierPath = pathPoint.currentPath;
            }
            distance = pathPoint.distanceFromStart;

            transform.position = pathPoint.position;
            transform.rotation = pathPoint.rotation;
        }
    }
}

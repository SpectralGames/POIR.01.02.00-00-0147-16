using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TDPath : MonoBehaviour
{
    Transform mTrans;
    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    [System.Serializable]
    public class Links
    {
        public Transform target;
        public string pointName;
    }
    public Links[] links;

    public enum EPathType
    {
        Square,
        CatmullRom,
        //CustomChainCatmullRom,
        Bezier,
    }

    [System.Serializable]
    public class TDPathPoint
    {
        public Vector3 position;

        public int nextPoint = -1;

        public Vector3 tangentPos1;
        public Vector3 tangentPos2;

        public Quaternion forward = Quaternion.identity;
        public Vector3 forwardDirection;

        public string pointName;
    }

    //public EAxis roadAxis = EAxis.XYZ;

    public EPathType pathType = EPathType.Square;
    public float squareSize = 1;

    public float precission = 1f;


    public TDPathPoint[] points;

    public Vector3[] bakedPath;
    public float bakedPathLength;

    [System.Serializable]
    public class TDBakedPath
    {
        public Vector3[] bakedPath;
        public float[] bakedDistance;
        public Quaternion[] bakedRotation;
        public float splineLen;
        public string startPointName;
        public string endPointName;

        public bool Evaluate (float distanceFromStart, ref float overDistance, ref Vector3 position, ref Quaternion rotation, ref Quaternion splineRotation, ref string nextPointName)
        {
            if (bakedDistance != null && bakedDistance.Length > 0)
            {
                // testing, we are over spline
                if (distanceFromStart >= bakedDistance[bakedDistance.Length - 1])
                {
                    overDistance = distanceFromStart - bakedDistance[bakedDistance.Length - 1];
                    return false;
                }
                else
                {
                    // we are in spline between start and stop
                    for (int a = 0; a < bakedDistance.Length; a++)
                    {
                        // looking for first border point
                        if (bakedDistance[a] >= distanceFromStart)
                        {
                            // if we are at first point
                            if (a == 0)
                            {
                                // return first baked point
                                position = bakedPath[0];
                                rotation = bakedRotation[0];
                                overDistance = 0.0f;
                                return true;
                            }
                            else
                            {
                                // actual fragment len
                                float fragmentLen = bakedDistance[a] - bakedDistance[a - 1];
                                float deltaFromFragmentStart = distanceFromStart - bakedDistance[a - 1];
                                float t = deltaFromFragmentStart / fragmentLen;

                                overDistance = bakedDistance[bakedDistance.Length - 1] - distanceFromStart;

                                position = Vector3.Lerp(bakedPath[a - 1], bakedPath[a], t);
                                rotation = Quaternion.Slerp(bakedRotation[a - 1], bakedRotation[a], t);
                                splineRotation = Quaternion.LookRotation(bakedPath[a] - bakedPath[a - 1]);

                                nextPointName = endPointName;

                                return true;
                            }
                        }
                    }

                    Debug.LogError("Fatal error");


                    overDistance = 0.0f;

                    return false;
                }
            }
            else
            {
                overDistance = -1.0f;
                return false;
            }
        }
    }

    public List<TDBakedPath> listOfBakedPaths = new List<TDBakedPath>();

    void Awake ()
    {
        mTrans = transform;
    }

    public void BakePath ()
    {
        switch (pathType)
        {
            case EPathType.Square:
            BakePath_Square();
            break;
            case EPathType.CatmullRom:
            BakePath_CatmulaRoma();
            break;
            case EPathType.Bezier:
            BakePath_Bezier();
            break;
            //case EPathType.CustomChainCatmullRom:
            //    BakePath_CustomChainCatmullRom();
            //    break;
        }
    }



    private void BakePath_Square ()
    {
        Vector3[] arrayOfPoints = new Vector3[points.Length];

        for (int a = 0; a < points.Length; a++)
        {
            TDPathPoint p1 = points[a];
            arrayOfPoints[a] = p1.position;
        }

        BakeLinearPath(arrayOfPoints);
    }

    private void BakePath_CatmulaRoma ()
    {
        Vector3[] arrayOfPoints = ConvertToCatmullRomArray();

        BakeLinearPath(arrayOfPoints);
    }

    private void BakePath_Bezier ()
    {
        listOfBakedPaths = new List<TDBakedPath>();

        for (int a = 0; a < points.Length; a++)
        {
            TDBakedPath bakedPath = new TDBakedPath();

            TDPath.TDPathPoint p1 = points[a];

            if (p1.nextPoint > -1 && p1.nextPoint < points.Length)
            {
                TDPath.TDPathPoint p2 = points[p1.nextPoint];

                TDBezierPath.BakePath(new TDPathPoint[2] { p1, p2 }, Matrix4x4.identity, out bakedPath.bakedPath, out bakedPath.bakedDistance, out bakedPath.bakedRotation);
                bakedPath.startPointName = p1.pointName;
                bakedPath.endPointName = p2.pointName;
                listOfBakedPaths.Add(bakedPath);
                //Debug.Log("Bake from "+a+" to "+p1.nextPoint);
            }
            else
            {
                bakedPath.bakedPath = new Vector3[0];
                listOfBakedPaths.Add(bakedPath);
                //Debug.Log("Can't Bake from "+a+" to "+p1.nextPoint);
            }
        }
    }

    private void BakePath_CustomChainCatmullRom ()
    {
        /*
        listOfBakedCustomPaths = new List<Vector3[]>();

        Vector3[] controlPoints= new Vector3[4];
        int[] controlPointsIndex = new int[4];

        for(int a=0; a<points.Length; a++)
        {
            if(a-1<0)
                controlPointsIndex[0]=a;
            else
            {
                controlPointsIndex[0] = GetIndexOfPointWhereParentIs(a);
            }

            controlPointsIndex[1]=a;
            controlPointsIndex[2]=points[a].nextPoint;
            controlPointsIndex[3]=points[points[a].nextPoint].nextPoint;

            for(int d=0; d<controlPoints.Length; d++)
            {
                controlPoints[d]=points[controlPointsIndex[d]].position;
                Debug.Log(string.Format("Point {0} : {1}",d,controlPoints[d]));
            }

            Vector3[] array = TDCatmullRomSpline.ConvertToCatmullRomArray(controlPoints);
            listOfBakedCustomPaths.Add( array );
        }
        */
    }

    private int GetIndexOfPointWhereParentIs (int index)
    {
        for (int a = 0; a < points.Length; a++)
        {
            TDPath.TDPathPoint p = points[a];
            if (p.nextPoint == index)
                return a;
        }
        return -1;
    }

    private void BakeLinearPath (Vector3[] arrayOfPoints)
    {
        bakedPathLength = 0.0f;

        precission = Mathf.Max(0.15f, precission);

        //float splineLength = GetLength(arrayOfPoints);
        int arrayIndex = 1;

        List<Vector3> listOfBakedPoints = new List<Vector3>();
        listOfBakedPoints.Add(arrayOfPoints[0]);

        //Debug.Log("===== START ");

        float startDistance = 0;
        while (arrayIndex < arrayOfPoints.Length)
        {

            float nearDistance = 0;
            Vector3 lastPoint = listOfBakedPoints[listOfBakedPoints.Count - 1];
            int lastGoodIndex = arrayIndex;
            //Vector3 skippedPoint = lastPoint;

            //Debug.Log(string.Format("- strt iterating, nearDistance: {0} lastGoodIndex: {1}, lastPoint: {2}", nearDistance, lastGoodIndex, lastPoint));
            for (lastGoodIndex = arrayIndex; lastGoodIndex < arrayOfPoints.Length; lastGoodIndex++)
            {
                Vector3 nextPossiblePoint = arrayOfPoints[lastGoodIndex];
                float d = Vector3.Magnitude(nextPossiblePoint - lastPoint);

                //Debug.Log(string.Format("-- for: lastGoodIndex: {0} distance: {1}, nextPossiblePoint: {2}",lastGoodIndex,d,nextPossiblePoint)); 

                if (nearDistance + d < precission)
                {
                    startDistance += d;
                    nearDistance += d;
                    //skippedPoint = nextPossiblePoint;
                    //lastPoint = nextPossiblePoint;

                    //Debug.Log(string.Format("--- skip point"));
                }
                else
                {
                    Vector3 overPoint = arrayOfPoints[lastGoodIndex];

                    float len = Vector3.Magnitude(overPoint - lastPoint);

                    float w = precission / len;
                    Vector3 newGoodPoint = Vector3.Lerp(lastPoint, overPoint, w);
                    listOfBakedPoints.Add(newGoodPoint);
                    //Debug.Log(string.Format("Adding new point {0}",newGoodPoint)); 
                    break;
                }
            }

            arrayIndex = lastGoodIndex;
        }


        //Debug.Log("===== STOP ");

        Vector3 firstPoint = listOfBakedPoints[0];
        for (int a = 1; a < listOfBakedPoints.Count - 1; a++)
        {
            Vector3 next = listOfBakedPoints[a];

            bakedPathLength += Vector3.Magnitude(next - firstPoint);
            firstPoint = next;
        }

        bakedPath = listOfBakedPoints.ToArray();
        //Debug.Log("Baked len: " + bakedPathLength);

    }

    float GetLength (Vector3[] points)
    {
        float len = 0;
        Vector3 p1 = points[0];
        Vector3 p2;
        for (int a = 1; a < points.Length; a++)
        {
            p2 = points[a];

            len += Vector3.Magnitude(p2 - p1);
            p1 = p2;
        }

        return len;
    }



    public Vector3[] ConvertToCatmullRomArray ()
    {
        List<Vector3> list = new List<Vector3>();

        Vector3 p1 = Vector3.zero;

        for (int a = 0; a < points.Length - 1; a++)
        {
            Vector3[] tab = new Vector3[4];

            for (int p = 0; p < 4; p++)
            {
                int idx = a + p - 1;
                if (idx < 0)
                    idx = 0;
                if (idx >= points.Length)
                    idx = points.Length - 1;

                tab[p] = points[idx].position;
            }

            float time = 0.0f;
            p1 = TDCatmullRomSpline.GetCatmulaRomaSpline(tab, time);

            while (time < 1.0f)
            {
                //list.Add( cachedTransform.TransformPoint(p1));
                list.Add(p1);


                time += 0.1f;
                Vector3 p2 = TDCatmullRomSpline.GetCatmulaRomaSpline(tab, time);
                p1 = p2;
            }
        }
        //list.Add( cachedTransform.TransformPoint(p1));
        list.Add(p1);

        return list.ToArray();
    }

    public void GetPositionFromWeight (float weight, out Vector3 position, out Quaternion rotation)
    {
        /*
        switch (bakedPath.Length)
        {
            case 0:
                {
                    position = Vector3.zero;
                    rotation = Quaternion.identity;
                }
                break;
            case 1:
                {
                    position = bakedPath[0];
                    rotation = cachedTransform.rotation;
                }
                break;
            default:
                {
                    weight = Mathf.Clamp01(weight);

                    float destDistance = bakedPathLength * weight;

                    int startIndex = (int)(destDistance / precission);
                    if (startIndex >= bakedPath.Length)
                        startIndex = bakedPath.Length - 1;

                    int nextIndex = startIndex + 1;
                    if (nextIndex >= bakedPath.Length)
                        nextIndex = startIndex;

                    float miniWeight = destDistance % precission;

                    position = Vector3.Lerp(bakedPath[startIndex], bakedPath[nextIndex], miniWeight / precission);

                    // last point
                    if (startIndex == bakedPath.Length - 1)
                    {
                        Vector3 dir = bakedPath[startIndex] - bakedPath[startIndex - 1];
                        rotation = Quaternion.LookRotation(dir);
                    }
                    else
                    {
                        Vector3 dir = bakedPath[nextIndex] - bakedPath[startIndex];
                        rotation = Quaternion.LookRotation(dir);
                    }

                    position = cachedTransform.TransformPoint(position);
                    rotation = cachedTransform.rotation * rotation;
                }
                break;
            }
            */
        position = Vector3.zero;
        rotation = Quaternion.identity;
    }

    public void GetPositionFromDistanceFromStart (float distance, out Vector3 position, out Quaternion rotation)
    {
        switch (pathType)
        {
            default:
            {
                float weight = Mathf.Clamp01(distance / bakedPathLength);

                GetPositionFromWeight(weight, out position, out rotation);
            }
            break;
            case EPathType.Bezier:
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
            }
            break;
        }
    }

    public TDBakedPath Evaluate (TDPath.TDBakedPath bakedPath, float distanceFromStart, ref float overDistance, ref Vector3 position, ref Quaternion rotation, ref Quaternion splineRotation, ref string nextPointName)
    {
        if (!bakedPath.Evaluate(distanceFromStart, ref overDistance, ref position, ref rotation, ref splineRotation, ref nextPointName))
        {
            int index = listOfBakedPaths.IndexOf(bakedPath);
            int nextIndex = points[index].nextPoint;

            if (overDistance > 0.0f && nextIndex > -1)
            {
                TDBakedPath newPath = listOfBakedPaths[nextIndex];
                distanceFromStart -= overDistance;
                float temp = 0.0f;
                newPath.Evaluate(overDistance, ref temp, ref position, ref rotation, ref splineRotation, ref nextPointName);
                return newPath;
            }
            else
            {
                overDistance = -1.0f;
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return null;
            }
        }
        return bakedPath;
    }



    private static List<TDPath> pathsToBake = new List<TDPath>();
    void OnEnable ()
    {
        pathsToBake.Add(this);
    }

    void OnDisable ()
    {
        pathsToBake.Remove(this);
    }

    [System.Obsolete]
    public static void BakeLinks ()
    {
        /*
        foreach(TDPath path in pathsToBake)
        {
            if(path.links!=null && path.links.Length>0)
            {
                bool needBake = false;
                foreach(TDPath.Links link in path.links)
                {
                    foreach(TDPath.TDPathPoint point in path.points)
                    {
                        if(point.pointName==link.pointName)
                        {
                            needBake = true;
                            point.position = path.transform.InverseTransformPoint( link.target.position);
                        }
                    }
                }

                if(needBake)
                    path.BakePath();
            }
        }
        */
    }
}

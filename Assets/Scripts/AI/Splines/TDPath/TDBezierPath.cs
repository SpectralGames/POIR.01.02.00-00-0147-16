using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public static class TDBezierPath
{
    static float[] tArray = new float[10] { 0f, 0.111f, 0.222f, 0.333f, 0.444f, 0.555f, 0.666f, 0.777f, 0.888f, 1f };
	//static float[] tArray = new float[10] { 0f, 0.25f, 0.5f, 0.75f, 1f, 1f, 1f, 1f, 1f, 1f }; // wystarczy 5 punktów
    static float[] t3Array = new float[10] { 
        0f, 
        tArray[1] * tArray[1] * tArray[1], 
        tArray[2] * tArray[2] * tArray[2], 
        tArray[3] * tArray[3] * tArray[3], 
        tArray[4] * tArray[4] * tArray[4], 
        tArray[5] * tArray[5] * tArray[5], 
        tArray[6] * tArray[6] * tArray[6], 
        tArray[7] * tArray[7] * tArray[7], 
        tArray[8] * tArray[8] * tArray[8], 
        1f};    
    static float[] oneMinusTArrayTemp = new float[10] { 
        1f, 
        1f - tArray[1], 
        1f - tArray[2], 
        1f - tArray[3], 
        1f - tArray[4], 
        1f - tArray[5], 
        1f - tArray[6], 
        1f - tArray[7], 
        1f - tArray[8], 
        0f };
    static float[] oneMinusTArray = new float[10] { 
        0f, 
        3f * (1f - tArray[1]) * tArray[1] * tArray[1], 
        3f * (1f - tArray[2]) * tArray[2] * tArray[2], 
        3f * (1f - tArray[3]) * tArray[3] * tArray[3], 
        3f * (1f - tArray[4]) * tArray[4] * tArray[4], 
        3f * (1f - tArray[5]) * tArray[5] * tArray[5], 
        3f * (1f - tArray[6]) * tArray[6] * tArray[6], 
        3f * (1f - tArray[7]) * tArray[7] * tArray[7], 
        3f * (1f - tArray[8]) * tArray[8] * tArray[8], 
        0f };
    static float[] oneMinusT2Array = new float[10] { 
        0f,
        3f * oneMinusTArrayTemp[1] * oneMinusTArrayTemp[1] * tArray[1], 
        3f * oneMinusTArrayTemp[2] * oneMinusTArrayTemp[2] * tArray[2], 
        3f * oneMinusTArrayTemp[3] * oneMinusTArrayTemp[3] * tArray[3], 
        3f * oneMinusTArrayTemp[4] * oneMinusTArrayTemp[4] * tArray[4], 
        3f * oneMinusTArrayTemp[5] * oneMinusTArrayTemp[5] * tArray[5], 
        3f * oneMinusTArrayTemp[6] * oneMinusTArrayTemp[6] * tArray[6], 
        3f * oneMinusTArrayTemp[7] * oneMinusTArrayTemp[7] * tArray[7], 
        3f * oneMinusTArrayTemp[8] * oneMinusTArrayTemp[8] * tArray[8], 
        0f };
    static float[] oneMinusT3Array = new float[10] { 
        oneMinusTArrayTemp[0] * oneMinusTArrayTemp[0] * oneMinusTArrayTemp[0], 
        oneMinusTArrayTemp[1] * oneMinusTArrayTemp[1] * oneMinusTArrayTemp[1], 
        oneMinusTArrayTemp[2] * oneMinusTArrayTemp[2] * oneMinusTArrayTemp[2], 
        oneMinusTArrayTemp[3] * oneMinusTArrayTemp[3] * oneMinusTArrayTemp[3], 
        oneMinusTArrayTemp[4] * oneMinusTArrayTemp[4] * oneMinusTArrayTemp[4], 
        oneMinusTArrayTemp[5] * oneMinusTArrayTemp[5] * oneMinusTArrayTemp[5], 
        oneMinusTArrayTemp[6] * oneMinusTArrayTemp[6] * oneMinusTArrayTemp[6], 
        oneMinusTArrayTemp[7] * oneMinusTArrayTemp[7] * oneMinusTArrayTemp[7], 
        oneMinusTArrayTemp[8] * oneMinusTArrayTemp[8] * oneMinusTArrayTemp[8], 
        0f };

    
    public static Vector3 Bezier(Vector3 p1, Vector3 p1TangentWorldPos, Vector3 p2, Vector3 p2TangentWorldPos, float t)
    {
        float oneMinusT = 1.0f - t;
        float oneMinusT2 = oneMinusT * oneMinusT;
        float oneMinusT3 = oneMinusT2 * oneMinusT;

        return oneMinusT3 * p1 + 3.0f * oneMinusT2 * t * p1TangentWorldPos + 3.0f * oneMinusT * t * t * p2TangentWorldPos + t * t * t * p2;
    }

    public static void BakePath(TDPath.TDPathPoint[] points, Matrix4x4 transform, out Vector3[] bakedPositions, out float[] bakedDistances, out Quaternion[] bakedRotations)
    {
        bakedPositions = new Vector3[10];
        bakedDistances = new float[10];
        bakedRotations = new Quaternion[10];

        float distance = 0;
        Vector3 firstPoint = Vector3.zero;
        for( int a = 0; a < points.Length - 1; a++ )
        {
            for( int i = 0; i < 10; i++ )
            {
                float t = 0.111f * i;

                Vector3 localTangent1 = points[a].tangentPos2;
                Vector3 localTangent2 = points[a + 1].tangentPos1;

                Vector4 worldTangent1 = new Vector4(localTangent1.x, localTangent1.y, localTangent1.z, 1.0f);
                Vector4 worldTangent2 = new Vector4(localTangent2.x, localTangent2.y, localTangent2.z, 1.0f);

                Matrix4x4 transform1 = Matrix4x4.TRS(points[a].position, Quaternion.identity, Vector3.one);
                Matrix4x4 transform2 = Matrix4x4.TRS(points[a + 1].position, Quaternion.identity, Vector3.one);

                worldTangent1 = transform * transform1 * worldTangent1;
                worldTangent2 = transform * transform2 * worldTangent2;

                //Debug.Log(String.Format("{0} {1} {2} {3}",points[a].position,worldTangent1,worldTangent2,points[a+1].position));
                Vector3 bakedPoint = Bezier(points[a].position, worldTangent1, points[a + 1].position, worldTangent2, t);

                if( a == 0 && i == 0 )
                {
                    firstPoint = bakedPoint;
                }
                else
                {
                    distance += Vector3.Magnitude(bakedPoint - firstPoint);
                    firstPoint = bakedPoint;
                }

                bakedPositions[i] = bakedPoint;
                bakedDistances[i] = distance;
                bakedRotations[i] = Quaternion.Slerp(points[a].forward, points[a + 1].forward, t);
            }
        }
    }

    public static void BakePath(BezierPath bezierPath)
    {
        if( bezierPath.controlPoint.nextPoints.Length == 0 )
        {
            bezierPath.bakedPaths = null;
            return;
        }

		if( bezierPath.bakedPaths == null || bezierPath.bakedPaths.Length != bezierPath.controlPoint.nextPoints.Length ) //utworz baked path dla kazdego punktu nextPoint
			bezierPath.bakedPaths = new BezierPath.BakedPathStruct[bezierPath.controlPoint.nextPoints.Length];

		for(int i = 0; i < bezierPath.controlPoint.nextPoints.Length; i++)
		{
	        if( bezierPath.bakedPaths[i].bakedPath == null || bezierPath.bakedPaths[i].bakedPath.Length != 10 )
	            bezierPath.bakedPaths[i].bakedPath = new BezierPath.BezierBakedPoint[10];

	        float distance = 0;
	        Vector3 firstPoint = Vector3.zero;

	        Vector3 localTangent1 = bezierPath.controlPoint.tangentPos2;
	        Vector3 localTangent2 = bezierPath.controlPoint.nextPoints[i].controlPoint.tangentPos1;

	        Vector3 worldTangent1 = bezierPath.transform.TransformPoint(localTangent1);
	        Vector3 worldTangent2 = bezierPath.controlPoint.nextPoints[i].transform.TransformPoint(localTangent2);

	        BezierPath startBezier = bezierPath;
	        BezierPath endBezier = bezierPath.controlPoint.nextPoints[i];

	        Vector3 start = bezierPath.transform.position;
	        Vector3 stop = bezierPath.controlPoint.nextPoints[i].transform.position;

	        if( bezierPath.bakedPaths[i].bakedPath[0] == null )
	            bezierPath.bakedPaths[i].bakedPath[0] = new BezierPath.BezierBakedPoint();

	        Vector3 bakedPoint = oneMinusT3Array[0] * start + oneMinusT2Array[0] * worldTangent1 + oneMinusTArray[0] * worldTangent2 + t3Array[0] * stop;
	        firstPoint = bakedPoint;
	        bezierPath.bakedPaths[i].bakedPath[0].position = bakedPoint;
	        bezierPath.bakedPaths[i].bakedPath[0].rotation = Quaternion.Slerp(startBezier.controlPoint.forward, endBezier.controlPoint.forward, tArray[0]);
	        bezierPath.bakedPaths[i].bakedPath[0].distance = distance;

	        for( int j = 1; j < 10; j++ )
	        {
	            if( bezierPath.bakedPaths[i].bakedPath[j] == null )
	                bezierPath.bakedPaths[i].bakedPath[j] = new BezierPath.BezierBakedPoint();

	            bakedPoint = oneMinusT3Array[j] * start + oneMinusT2Array[j] * worldTangent1 + oneMinusTArray[j] * worldTangent2 + t3Array[j] * stop;

	            distance += Vector3.Magnitude(bakedPoint - firstPoint);
	            firstPoint = bakedPoint;

	            bezierPath.bakedPaths[i].bakedPath[j].position = bakedPoint;
	            bezierPath.bakedPaths[i].bakedPath[j].rotation = Quaternion.Slerp(startBezier.controlPoint.forward, endBezier.controlPoint.forward, tArray[j]); // j
	            bezierPath.bakedPaths[i].bakedPath[j].distance = distance;
	        }
		}
    }//bake path
}
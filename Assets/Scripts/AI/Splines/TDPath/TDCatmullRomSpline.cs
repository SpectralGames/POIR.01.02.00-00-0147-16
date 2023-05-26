using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TDCatmullRomSpline
{
    /// <summary>
    /// Gets the catmula roma spline from 4 points
    /// </summary>
    /// <returns>The catmula roma spline.</returns>
    /// <param name="p">4 points</param>
    /// <param name="t">T.</param>
    public static Vector3 GetCatmulaRomaSpline( Vector3[] p, float t )
    {
        Vector3 ret;
        
        float t2= t*t;
        float t3 = t2*t;
        
        ret = 0.5f * ( ( 2.0f * p[1] ) +
                      (-1.0f*p[0]  + p[2] ) * t +
                      ( 2.0f * p[0] - 5.0f * p[1] + 4.0f * p[2] - p[3]) * t2 +
                      (-p[0] + 3.0f*p[1]- 3.0f*p[2] + p[3]) * t3);
        return ret;
    }

    /// <summary>
    /// Converts to catmull rom array.
    /// </summary>
    /// <returns>The to catmull rom array.</returns>
    /// <param name="controlPoints">Control points.</param>
    public static Vector3[] ConvertToCatmullRomArray(Vector3[] controlPoints)
    {
        List<Vector3> list = new List<Vector3>();
        
        Vector3 p1 = Vector3.zero;

        float time=0.0f;
        p1=TDCatmullRomSpline.GetCatmulaRomaSpline( controlPoints, time);
        
        while(time<1.0f)
        {
            list.Add( p1);

            time+=0.1f;
            Vector3 p2 = TDCatmullRomSpline.GetCatmulaRomaSpline( controlPoints, time );
            p1=p2;                  
        }           

        list.Add( p1);
        
        return list.ToArray();
    }

}

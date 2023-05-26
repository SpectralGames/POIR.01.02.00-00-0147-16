using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PathpointDrawer : MonoBehaviour
{
    private List<Pathpoint> pathpoints = new List<Pathpoint>();
    Color GizmosColor = Color.white;

    private void Start ()
    {
        pathpoints = GameObject.FindObjectsOfType<Pathpoint>().ToList();
    }

    //protected void OnDrawGizmos ()
    //{
    //    Gizmos.color = GizmosColor;
            
    //    foreach (Pathpoint pp in pathpoints)
    //    {
    //        foreach (Pathpoint p in pp.nextWaypoints)
    //            Gizmos.DrawLine(pp.position, p.position);

    //        if (pp.isAvailable)
    //            Gizmos.color = Color.green;
    //        else
    //            Gizmos.color = Color.red;

    //        Gizmos.DrawSphere(pp.position, 0.2f);

    //    }        
    //}
}
 
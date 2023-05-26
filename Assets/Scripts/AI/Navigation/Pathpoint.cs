using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Pathpoint : MonoBehaviour
{
    public VirginTower virginTower = null;
    public bool isAvailable = true;
    [HideInInspector] public Vector3 position;
    [HideInInspector] public Quaternion rotation;

    [SerializeField] private List<Pathpoint> nextWaypoints = new List<Pathpoint>();
    [SerializeField] private List<Pathpoint> previousWaypoints = new List<Pathpoint>();


    private void Start ()
    {
        position = this.gameObject.transform.position;
        rotation = this.gameObject.transform.rotation;
    }

    public virtual Pathpoint GetNextPathPoint ()
    {
        return GetPathPoint(nextWaypoints);
    }

    public virtual Pathpoint GetPreviousPathPoint ()
    {
        return GetPathPoint(previousWaypoints);
    }

    protected Pathpoint GetPathPoint (List<Pathpoint> pathpoints)
    {
        if (pathpoints.Count <= 0)
            return null;

        List<Pathpoint> temp = new List<Pathpoint>();// pathpoints.ToList();
        temp = pathpoints;
        temp = temp.FindAll(x => x.isAvailable);
        if (temp.Count <= 0)
            return null;
        Pathpoint p = temp[UnityEngine.Random.Range(0, temp.Count)];
        if (p != null)
        {
            //Debug.Log("Path change: " + p.name);
            return p;
        }

        return null;
    }
}
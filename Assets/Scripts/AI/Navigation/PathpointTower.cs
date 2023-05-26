using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathpointTower : Pathpoint
{
    //public VirginTower virginTower;

    private void Start ()
    {
        position = this.gameObject.transform.position;
        rotation = this.gameObject.transform.rotation;
    }
}
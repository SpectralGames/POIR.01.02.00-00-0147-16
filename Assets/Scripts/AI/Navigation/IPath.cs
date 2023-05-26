using UnityEngine;
//using UnityEditor;

public interface IPath
{
    Quaternion GetPathPointRotation { get; set; }
    Vector3 GetPathPointPosition { get; set; }
    VirginTower GetVirginTower { get; set; }
}
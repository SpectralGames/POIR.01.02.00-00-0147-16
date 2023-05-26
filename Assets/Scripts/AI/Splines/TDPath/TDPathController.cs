using UnityEngine;
using System.Collections;

public class TDPathController : MonoBehaviour
{
    private TDPath currentPath;
    private TDPath.TDBakedPath currentBakedPath;
    private float currentBakedPathPosition = 0.0f;

    public TDPath debugPath;

    void Awake()
    {
        if(debugPath!=null)
            SetPath(debugPath);
    }

    public void SetPath(TDPath path)
    {
        currentPath = path;
        currentBakedPath = path.listOfBakedPaths[0];
        currentBakedPathPosition = 0.0f;
    }

    private float speed = 10.5f;

	void Update ()
    {




        if(currentPath!=null && currentBakedPath!=null)
        {
            Vector3 pos = transform.position;
            Quaternion rot=transform.rotation;
            Quaternion splineRot = transform.rotation;
            float over=0.0f;
            currentBakedPathPosition += Time.deltaTime * speed;
            string name = null;

            TDPath.TDBakedPath path = currentPath.Evaluate(currentBakedPath, currentBakedPathPosition, ref over, ref pos, ref rot, ref splineRot, ref name);
            if(path!=currentBakedPath)
            {
                currentBakedPath = path;
                currentBakedPathPosition = over;

            }
            else
            {

            }

            if(over>=0.0f)
            {
                Debug.Log(over + " "+pos);
                transform.position = pos;
                transform.rotation = Quaternion.Slerp(transform.rotation,splineRot,0.1f);
            }
        }


	}
}

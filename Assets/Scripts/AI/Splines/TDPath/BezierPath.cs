using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BezierPath : MonoBehaviour
{
    [System.Serializable]
    public class BezierControlPoint
    {
        public VirginTower virginTower;
        public bool isTriggerEndPoint;
        public Vector3 tangentPos1;
        public Vector3 tangentPos2;
        public Quaternion forward;
        public float offsetPosition;
        public BezierPath[] nextPoints = new BezierPath[0];
        public BezierPath[] prevPoints = new BezierPath[0];
    }

    [System.Serializable]
    public class BezierBakedPoint
    {
        public Vector3 position;
        public float distance;
        public Quaternion rotation;
    }

    public class BezierPathPoint
    {
        public Vector3 position;
        public Quaternion rotation;
        
        //public Quaternion splineRotation;
        public float distanceToNextPoint;
        public float distanceFromStart;
        //public BezierPath nextPath;
        public int currentNextPoint;
        public BezierPath currentPath;
        public float currentOffsetPosition;
    }

    public BezierControlPoint controlPoint;

    [System.Serializable]
    public struct BakedPathStruct
    {
        public BezierBakedPoint[] bakedPath;
    }
    public BakedPathStruct[] bakedPaths;


    public bool isActive = true;
	public string forwardAnimatorTrigger = null;
	public string backwardAnimatorTrigger = null;
	//public float forwardSpeedFactor = 1f;
	//public float backwardSpeedFactor = 1f;
	public BezierSpeedCurve forwardSpeedCurve, backwardSpeedCurve;

	[System.Serializable]
	public struct BezierSpeedCurve
	{
		public float curveDistance;
		public AnimationCurve curve;
	}

    Color GizmosColor = Color.white;
    public bool DrawSpline = true;
    [HideInInspector]
    public float pointDistanceFactor;
    public void BakePath(bool withPrev)
    {
        if (isActive == false)	return;
		for(int i=0; i<controlPoint.nextPoints.Length; i++) if(controlPoint.nextPoints[i] == null) return;
		for(int i=0; i<controlPoint.prevPoints.Length; i++) if(controlPoint.prevPoints[i] == null) return;

        TDBezierPath.BakePath(this);
        if (withPrev)
        {
            for (int i = 0; i < controlPoint.prevPoints.Length; i++)
            {
                TDBezierPath.BakePath(controlPoint.prevPoints[i]);
            }
        }
    }


    /*protected override void Update()
    {
        if(NeedUpdate)
        {
            BakePath(true);
        }
    }*/


    protected void OnDrawGizmos()
    {
        if (DrawSpline && controlPoint != null)
        {
            Gizmos.color = GizmosColor;
            //if(controlPoint.nextPoint!=null)
            //    Gizmos.DrawLine(transform.position, controlPoint.nextPoint.cachedTransform.position);

            if (bakedPaths != null && bakedPaths.Length > 0)
            {
                for (int i = 0; i < bakedPaths.Length; i++)
                {
                    for (int a = 0; a < bakedPaths[i].bakedPath.Length - 1; a++)
                    {
                        Gizmos.DrawLine(bakedPaths[i].bakedPath[a].position, bakedPaths[i].bakedPath[a + 1].position);
                    }
                }
            }

            if (isActive)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawSphere(transform.position, 0.40f);

            GizmosColor = Color.white;
        }
    }

    protected void OnDrawGizmosSelected()
    {
        GizmosColor = Color.green;
    }

    public void Evaluate(float distanceFromStart, ref BezierPathPoint pathPoint, bool isGoingForward = true)
    {
        int _currentNextPoint = pathPoint.currentNextPoint;
        if (isGoingForward)
        {
            if (bakedPaths[_currentNextPoint].bakedPath != null && bakedPaths[_currentNextPoint].bakedPath.Length > 0)
            {
                int currentPathLenth = bakedPaths[_currentNextPoint].bakedPath.Length;

                if (distanceFromStart >= bakedPaths[_currentNextPoint].bakedPath[currentPathLenth - 1].distance) //przekroczylem dystans do ostatniego punktu baked path, zmien punkt
                {
                    if (controlPoint.nextPoints[_currentNextPoint].bakedPaths == null || controlPoint.nextPoints[_currentNextPoint].bakedPaths.Length <= 0)
                    {
                        // dotarlem do konca trasy
                        distanceFromStart = bakedPaths[_currentNextPoint].bakedPath[currentPathLenth - 1].distance;
                        pathPoint.currentPath = null;
                    }
                    else
                    {
                        // dotarlem do kolejnego punktu
                        //Debug.Log(this + " Evaluate(), next point go");
                        float delta = distanceFromStart - bakedPaths[_currentNextPoint].bakedPath[currentPathLenth - 1].distance;
                        //ustaw sobie kolejny punkt, sprawdz eventy, dostepnosc trasy, itd
                        controlPoint.nextPoints[_currentNextPoint].OnNextPointReached(ref pathPoint);
                        controlPoint.nextPoints[_currentNextPoint].Evaluate(delta, ref pathPoint, isGoingForward);
                        return;
                    }
                }
                else
                {
                    pathPoint.currentPath = this;
                }

                //Debug.Log(this + " Evaluate(), pathPoint.position: " + pathPoint.position);

                // if we are at first point
                if (bakedPaths[_currentNextPoint].bakedPath[0].distance >= distanceFromStart)
                {
                    // return first baked point
                    pathPoint.position = bakedPaths[_currentNextPoint].bakedPath[0].position;
                    pathPoint.rotation = bakedPaths[_currentNextPoint].bakedPath[0].rotation;
                    //pathPoint.splineRotation = Quaternion.LookRotation(bakedPaths[_currentNextPoint].bakedPath[1].position - bakedPaths[_currentNextPoint].bakedPath[0].position);
                    pathPoint.distanceToNextPoint = bakedPaths[_currentNextPoint].bakedPath[currentPathLenth - 1].distance;
                    pathPoint.distanceFromStart = 0.0f;
                    return;
                }
                // we are in spline between start and stop
                for (int a = 1; a < bakedPaths[_currentNextPoint].bakedPath.Length; a++)
                {
                    // looking for first border point
                    if (bakedPaths[_currentNextPoint].bakedPath[a].distance >= distanceFromStart)
                    {
                        // actual fragment len
                        float fragmentLen = bakedPaths[_currentNextPoint].bakedPath[a].distance - bakedPaths[_currentNextPoint].bakedPath[a - 1].distance;
                        float deltaFromFragmentStart = distanceFromStart - bakedPaths[_currentNextPoint].bakedPath[a - 1].distance;
                        float t = deltaFromFragmentStart / fragmentLen;
                    
                        pathPoint.distanceToNextPoint = bakedPaths[_currentNextPoint].bakedPath[currentPathLenth - 1].distance - distanceFromStart;
                        pathPoint.distanceFromStart = distanceFromStart;

                        pathPoint.position = Vector3.Lerp(bakedPaths[_currentNextPoint].bakedPath[a - 1].position, bakedPaths[_currentNextPoint].bakedPath[a].position, t);
                        pathPoint.rotation = Quaternion.Slerp(bakedPaths[_currentNextPoint].bakedPath[a - 1].rotation, bakedPaths[_currentNextPoint].bakedPath[a].rotation, t);

						pointDistanceFactor = distanceFromStart / bakedPaths[_currentNextPoint].bakedPath[bakedPaths[_currentNextPoint].bakedPath.Length-1].distance;
						pathPoint.rotation = Quaternion.Slerp(this.transform.rotation, controlPoint.nextPoints[_currentNextPoint].transform.rotation, pointDistanceFactor);

                        pathPoint.currentOffsetPosition = Mathf.Lerp(controlPoint.offsetPosition, controlPoint.nextPoints[_currentNextPoint].controlPoint.offsetPosition, pointDistanceFactor);
                        return;
                    }
                }

                Debug.LogError(this + " Evaluate() - Fatal error");
                return;
            }
            else
            {
                pathPoint.currentPath = null;
                return;
            }

        }
        else
        { //wracam sie po splajnie // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //

            if ((bakedPaths != null && bakedPaths.Length > 0) || controlPoint.prevPoints.Length > 0)
            {
                if (distanceFromStart <= 0.02f) //dotarlem do poczatku splajna
                {
					if (controlPoint.prevPoints == null || controlPoint.prevPoints.Length <= 0) // || controlPoint.prevPoints[_currentNextPoint].bakedPaths == null || controlPoint.prevPoints[_currentNextPoint].bakedPaths.Length <= 0)
                    {
                        // dotarlem do poczatku trasy
                        distanceFromStart = 0f;
                        pathPoint.currentPath = null;
                        return;
                    }
                    else
                    {
                        // dotarlem do kolejnego punktu
                        //ustaw sobie kolejny punkt, sprawdz eventy, dostepnosc trasy, itd
                        this.OnPrevPointReached(ref pathPoint);
						int nextPoint = pathPoint.currentNextPoint;
						//nowy punkt jest teraz w pathPoint.currentNextPoint, znajdz moj aktualny punkt wsrod nextPoints punktu, ktory wybralem w OnPrevPointReached
						int myPointInPrevPointsIndex = 0;
						for(int index=0; index < controlPoint.prevPoints[nextPoint].controlPoint.nextPoints.Length; index++)
						{
							if(this.gameObject == controlPoint.prevPoints[nextPoint].controlPoint.nextPoints[index].gameObject)
							{
								myPointInPrevPointsIndex = index;
							}
						}
						int previousPathLength = controlPoint.prevPoints[nextPoint].bakedPaths[myPointInPrevPointsIndex].bakedPath.Length;
						float delta = controlPoint.prevPoints[nextPoint].bakedPaths[myPointInPrevPointsIndex].bakedPath[previousPathLength - 1].distance - Mathf.Abs(distanceFromStart);

						pathPoint.currentPath = controlPoint.prevPoints[nextPoint];
						pathPoint.currentNextPoint = myPointInPrevPointsIndex;
						controlPoint.prevPoints[nextPoint].Evaluate(delta, ref pathPoint, isGoingForward);

                        return;
                    }
                }

                int currentPathLenth = pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath.Length;
                // if we are at first point
                /*if( bakedPaths[_currentNextPoint].bakedPath[0].distance >= distanceFromStart )
				{
					// return first baked point
					pathPoint.position = bakedPaths[_currentNextPoint].bakedPath[0].position;
					pathPoint.rotation = bakedPaths[_currentNextPoint].bakedPath[0].rotation;
					//pathPoint.splineRotation = Quaternion.LookRotation(bakedPaths[_currentNextPoint].bakedPath[1].position - bakedPaths[_currentNextPoint].bakedPath[0].position);
					pathPoint.distanceToNextPoint = bakedPaths[_currentNextPoint].bakedPath[currentPathLenth - 1].distance;
					pathPoint.distanceFromStart = 0.0f;
					return;
				}*/
                // we are in spline between start and stop
                for (int a = pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath.Length - 2; a >= 0; a--)
                {
                    // looking for first border point
                    if (pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[a].distance < distanceFromStart)
                    {
                        // actual fragment len
                        float fragmentLen = pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[a + 1].distance - pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[a].distance;
                        float deltaFromFragmentStart = pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[a + 1].distance - distanceFromStart;
                        float t = deltaFromFragmentStart / fragmentLen;

                        pathPoint.distanceToNextPoint = distanceFromStart - pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[0].distance;
                        pathPoint.distanceFromStart = distanceFromStart;

                        pathPoint.position = Vector3.Lerp(pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[a + 1].position, pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[a].position, t);
                        //pathPoint.rotation = Quaternion.Slerp(pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[a + 1].rotation, pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[a].rotation, t) * Quaternion.Euler(Vector3.up * 180f);
						//pathPoint.rotation = Quaternion.LookRotation(bakedPaths[_currentNextPoint].bakedPath[a].position - bakedPaths[_currentNextPoint].bakedPath[a - 1].position) * Quaternion.Euler(Vector3.up * 180f);
						pointDistanceFactor = distanceFromStart / pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath[pathPoint.currentPath.bakedPaths[_currentNextPoint].bakedPath.Length-1].distance;
						pathPoint.rotation = Quaternion.Slerp(this.transform.rotation, pathPoint.currentPath.controlPoint.nextPoints[_currentNextPoint].transform.rotation, pointDistanceFactor) * Quaternion.Euler(Vector3.up * 180f);
                        if(controlPoint.nextPoints.Length > 0)
                             pathPoint.currentOffsetPosition = Mathf.Lerp(controlPoint.offsetPosition, controlPoint.nextPoints[_currentNextPoint].controlPoint.offsetPosition, pointDistanceFactor);
                        return;
                    }
                }

                Debug.LogError(this + " Evaluate() - Fatal error");
                return;
            }
            else
            {
                pathPoint.currentPath = null;
                return;
            }
        }
    }

    public void OnNextPointReached(ref BezierPathPoint pathPoint)
    {
        // jestem na poczatku nowego punktu ----> lastPoint.nextPoint
        int nextPoint = UnityEngine.Random.Range(0, controlPoint.nextPoints.Length); //ustaw losowo kolejny punkt
        if (controlPoint.nextPoints[nextPoint].isActive == false) //jesli sciezka nie jest aktywna, wybierz nastepny
            nextPoint = (nextPoint + 1) % controlPoint.nextPoints.Length;
        pathPoint.currentNextPoint = nextPoint;
    }

    public void OnPrevPointReached(ref BezierPathPoint pathPoint)
    {
		//Debug.Log(pathPoint.currentPath.name + "  " + controlPoint.prevPoints.Length + "  " + this.name);
		//Debug.Break();
        // jestem na poczatku nowego punktu ----> lastPoint.nextPoint
        int prevPoint = UnityEngine.Random.Range(0, controlPoint.prevPoints.Length); //ustaw losowo kolejny punkt
        if (controlPoint.prevPoints[prevPoint].isActive == false) //jesli sciezka nie jest aktywna, wybierz nastepny
            prevPoint = (prevPoint + 1) % controlPoint.prevPoints.Length;
        pathPoint.currentNextPoint = prevPoint;
    }


    public float Length(int _currentNextPoint)
    {
        if (bakedPaths[_currentNextPoint].bakedPath != null && bakedPaths[_currentNextPoint].bakedPath.Length == 5)
            return bakedPaths[_currentNextPoint].bakedPath[9].distance;
        else
            return 0.0f;
    }
}


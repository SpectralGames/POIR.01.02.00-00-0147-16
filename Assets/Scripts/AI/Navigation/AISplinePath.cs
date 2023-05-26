using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class AISplinePath : INavigation
{
    #region Members
    private AI aiParent;

    protected BezierPath bezierPath, bezierPathTriggered;
    protected BezierPath.BezierPathPoint pathPoint = new BezierPath.BezierPathPoint();

    protected bool allowMovingWithSpline = true;
    protected bool isGoingForward = true;
    protected float splineOffset;
    protected float splineSpeed = 1f;
    public float rotationSpeed = 20.0f;
    protected bool isEndMoving = false;
    private Rigidbody rigidbody;
    protected BezierPath.BezierSpeedCurve currentForwardCurve, currentBackwardCurve;
    protected float initCurrentForwardCurveDistance, initCurrentBackwardCurveDistance;
    private float randomOffsetPosition =0;
    private float lastRandomOffsetPosition = 0;

    #endregion Members

    #region Properties
    public Transform CachedTransform
    {
        get
        {
            return aiParent.transform;
        }
    }
    #endregion Properties

    #region Inits
    public AISplinePath(AI ai, BezierPath start)
    {
        Init(ai, start);
    }

    public virtual void Init(AI ai, BezierPath start)
    {
        randomOffsetPosition = UnityEngine.Random.Range(-1f, 1f);
        //bezierPath = start;
        aiParent = ai;
        this.SetPath(start);

        splineSpeed = aiParent.walkSpeed * aiParent.speedFactor;

        DisableNavMesh();
        AddRigidBody();
    }
    #endregion Inits

    #region Logic
    private void DisableNavMesh()
    {
        NavMeshAgent navMeshAgent = aiParent.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
            navMeshAgent.enabled = false;
    }

    private void AddRigidBody()
    {
        rigidbody = aiParent.GetComponent<Rigidbody>();
        if (rigidbody == null)
            rigidbody = aiParent.gameObject.AddComponent<Rigidbody>();

        rigidbody.isKinematic = true;
    }

    protected void SetPath(BezierPath path)
    {
        if (path == null)
        { Debug.LogWarning(this + " SetPath() with null path"); return; }

        pathPoint.currentNextPoint = 0;
        bezierPath = path;
        bezierPath.Evaluate(0.0f, ref pathPoint, isGoingForward);
        bezierPathTriggered = null;
      
     
        //CachedTransform.position = pathPoint.position;
        //CachedTransform.rotation = pathPoint.rotation * Quaternion.Euler(Vector3.up);

        OnSetPath();
    }

    protected virtual void OnSetPath()
    { }

    protected virtual void ReachedPoint(BezierPath point)
    {

        if (point != null)
        { //mam jeszcze punkty, wyzeruj inny, o który zahaczyłem
            bezierPathTriggered = null;
        }
        lastRandomOffsetPosition = randomOffsetPosition;
        randomOffsetPosition = UnityEngine.Random.Range(-1f, 1f);
        //if (OnReachedPoint != null)
        //    OnReachedPoint(this, point);
        //Debug.Log(point.name + " osiagniety");
    }

    protected virtual void OnSplineEnd(BezierPath path, int headedPoint = 0)
    {
        Debug.Log("end spline:: " + path);
        Debug.Log("end spline name:: " + path.name);
        if (isGoingForward)
        {
            isGoingForward = false;
            this.SetPath(path.controlPoint.nextPoints[headedPoint]); //wypusc go z powrotem
                                                                     // ReachedDestination();
        }
        else
        { //dotarl do konca splajna wracajac
          // this.OnComeback();
          // jesli doszedl do celu z dziewice to die invoke
        }
    }
    #endregion Logic

    #region INavigation
    public virtual void Tick()
    {
        //base.Update();
        if (!aiParent.isAlive)
            return;

        if (!aiParent.isMoving)
            return;

        if (allowMovingWithSpline && bezierPath != null)
        {
            pathPoint.distanceFromStart += (isGoingForward ? 1f : -1f) * splineSpeed * Time.deltaTime;
            //Debug.Log("przed evaluate " + pathPoint.currentPath);
            bezierPath.Evaluate(pathPoint.distanceFromStart, ref pathPoint, isGoingForward);
            //Debug.Log("po evaluate " + pathPoint.currentPath);
            if (!isEndMoving && pathPoint.currentPath != bezierPath)
            {
                //Debug.Log(this + " new reached point");
                ReachedPoint(pathPoint.currentPath);
                if (pathPoint.currentPath == null && bezierPathTriggered == null) //jesli dotarlem do konca i o nic po drodze nie zahaczylem
                {
                    //Debug.Log(this + " spline end");
                    OnSplineEnd(bezierPath, 0);//pathPoint.currentNextPoint);
                }
                else  //Debug.Log(this + " setting new point");
                {
                    if (bezierPathTriggered != null)  //jesli zlapalem wyjscie na innego splajna
                    {
                        this.SetPath(bezierPathTriggered);
                    }
                    else
                    { //nic nie zlapalem, ale to nie koniec splajna - zmien punkt na nowy
                        if (isGoingForward && pathPoint.currentPath.forwardAnimatorTrigger != null &&
                            !string.IsNullOrEmpty(pathPoint.currentPath.forwardAnimatorTrigger))
                        {
                            aiParent.Animator.SetAnimatorTrigger(pathPoint.currentPath.forwardAnimatorTrigger);
                        }
                        else if (isGoingForward == false && bezierPath.backwardAnimatorTrigger != null &&
                            !string.IsNullOrEmpty(bezierPath.backwardAnimatorTrigger))
                        {
                            aiParent.Animator.SetAnimatorTrigger(bezierPath.backwardAnimatorTrigger); // cofam sie, wiec odpal triggera z poprzedniego node'a
                        }

                        if (isGoingForward && pathPoint.currentPath.forwardSpeedCurve.curveDistance > 0.1f)
                        {
                            currentForwardCurve = pathPoint.currentPath.forwardSpeedCurve;
                            initCurrentForwardCurveDistance = currentForwardCurve.curveDistance;
                            //currentForwardCurve.curveDistance -= pathPoint.distanceFromStart;
                        }
                        else if (isGoingForward == false && bezierPath.backwardSpeedCurve.curveDistance > 0.1f)
                        {
                            currentBackwardCurve = bezierPath.backwardSpeedCurve;
                            initCurrentBackwardCurveDistance = currentBackwardCurve.curveDistance;
                            //currentBackwardCurve.curveDistance -= pathPoint.distanceFromStart;
                        }

                        //aiParent.SetSpeedFactor(isGoingForward ? pathPoint.currentPath.forwardSpeedFactor : bezierPath.backwardSpeedFactor);

                        bezierPath = pathPoint.currentPath;
                    }
                }
            }

            if (!aiParent.isMoving)
                return;
            else
            {
                if (isGoingForward && currentForwardCurve.curveDistance > 0f)
                {
                    currentForwardCurve.curveDistance -= splineSpeed * Time.deltaTime;
                    //Debug.Log(this.aiParent.transform.GetChild(0).name + " forward: " + (1f - currentForwardCurve.curveDistance/initCurrentForwardCurveDistance).ToString());
                    aiParent.SetSpeedFactor(currentForwardCurve.curve.Evaluate(1f - currentForwardCurve.curveDistance / initCurrentForwardCurveDistance));
                }
                else if (isGoingForward == false && currentBackwardCurve.curveDistance > 0f)
                {
                    currentBackwardCurve.curveDistance -= splineSpeed * Time.deltaTime;
                    //Debug.Log(this.aiParent.transform.GetChild(0).name + " backward: " + (1f - currentBackwardCurve.curveDistance/initCurrentBackwardCurveDistance).ToString());
                    aiParent.SetSpeedFactor(currentBackwardCurve.curve.Evaluate(1f - currentBackwardCurve.curveDistance / initCurrentBackwardCurveDistance));
                }
            }
            // Quaternion.Slerp(this.transform.rotation, controlPoint.nextPoints[_currentNextPoint].transform.rotation, pointDistanceFactor);
            CachedTransform.position = pathPoint.position + (CachedTransform.right * ( Mathf.Lerp(lastRandomOffsetPosition, randomOffsetPosition, bezierPath.pointDistanceFactor) * pathPoint.currentOffsetPosition));
            CachedTransform.rotation = pathPoint.rotation;// +  (randomOffsetPosition * pathPoint.currentOffsetPosition); // Quaternion.Slerp(CachedTransform.rotation, pathPoint.rotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void GetBackPosition()
    {
        // sprawdzić czy jest next point, jeśli jest to idź dalej, jeśli nie wróć do początkowego punktu
        if (pathPoint.currentPath.controlPoint.nextPoints.Length == 0)
            isGoingForward = false;
    }

    public void Enable()
    {
        aiParent.isMoving = true;
        //rigidbody.isKinematic = false;
    }

    public void Disable()
    {
        aiParent.isMoving = false;
        //rigidbody.isKinematic = true;
    }

    public float WalkSpeed
    {
        get
        {
            return aiParent.walkSpeed;
        }

        set
        {
            aiParent.walkSpeed = value;
        }
    }

    public void UpdateSpeedFactor()
    {
        splineSpeed = aiParent.walkSpeed * aiParent.speedFactor * aiParent.speedFactorFromAnimation;
    }

    #region IPath
    public Quaternion GetPathPointRotation
    {
        get
        {
            return pathPoint.rotation;
        }
        set
        {
            pathPoint.rotation = value;
        }
    }

    public Vector3 GetPathPointPosition
    {
        get
        {
            return pathPoint.currentPath.transform.position;
        }
        set
        {
            pathPoint.currentPath.transform.position = value;
        }
    }

    public VirginTower GetVirginTower
    {
        get
        {
            //if (pathPoint == null)
            //    Debug.Log("GetVirginTower pathpoint null");
            //if (pathPoint.currentPath == null)
            //    Debug.Log("GetVirginTower pathpoint.currentPath null"); // pust current path
            //if (pathPoint.currentPath.controlPoint == null)
            //    Debug.Log("GetVirginTower pathPoint.currentPath.controlPoint null");
            //if (pathPoint.currentPath.controlPoint.nextPoints[pathPoint.currentNextPoint] == null)
            //    Debug.Log("GetVirginTower pathPoint.currentPath.controlPoint.nextPoints[pathPoint.currentNextPoint] null");
            //if (pathPoint.currentPath.controlPoint.nextPoints[pathPoint.currentNextPoint].controlPoint == null)
            //    Debug.Log("GetVirginTower pathPoint.currentPath.controlPoint.nextPoints[pathPoint.currentNextPoint].controlPoint null");

            if (pathPoint.currentPath == null)
                return null;
            return pathPoint.currentPath.controlPoint.nextPoints[pathPoint.currentNextPoint].controlPoint.virginTower;
            //return bezierPath.controlPoint.virginTower;
        }
        set
        {
            pathPoint.currentPath.controlPoint.nextPoints[pathPoint.currentNextPoint].controlPoint.virginTower = value;
            //bezierPath.controlPoint.virginTower = value;
        }
    }
    public bool IsMovingEnabled()
    {
        return aiParent.isMoving;
    }
    #endregion IPath
    #endregion INavigation
}

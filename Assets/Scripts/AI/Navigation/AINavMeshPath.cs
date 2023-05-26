using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class AINavMeshPath : INavigation
{
    #region Members
    [HideInInspector] public NavMeshAgent navMeshAgent;
    protected Pathpoint pathPoint;
    protected Pathpoint backPathPoint; // punkt do ktorego sie wraca kiedy zdobedzie sie dziewice, ale w przypadku dziewcy do wiezy

    protected AI aiParent;
    [SerializeField] protected float stoppingDistance = 1.0f;

    protected NavMeshPath path;
    protected Vector3 hitPosition;
	protected Rigidbody myRigidbody;
    #endregion Members

    #region Inits
    public AINavMeshPath(AI ai, Pathpoint start)
    {
        Init(ai, start);
    }


    public virtual void Init (AI ai)
    {
        this.aiParent = ai;
        InitNavMesh();
		AddRigidBody();
    }

    public virtual void Init(AI ai, Pathpoint start)
    {
        this.aiParent = ai;
        this.pathPoint = start;
        this.backPathPoint = start;
        InitNavMesh();
		AddRigidBody();
    }

	private void AddRigidBody ()
	{
		myRigidbody = aiParent.GetComponent<Rigidbody>();
		if (myRigidbody == null)
			myRigidbody = aiParent.gameObject.AddComponent<Rigidbody>();

		myRigidbody.isKinematic = true;
	}

    protected virtual void InitNavMesh ()
    {
        navMeshAgent = aiParent.gameObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            navMeshAgent = aiParent.gameObject.AddComponent<NavMeshAgent>();
        }

        navMeshAgent.autoBraking = false;
        navMeshAgent.acceleration = 20.0f;
        navMeshAgent.radius = aiParent.navMeshAgentRadius;
        navMeshAgent.autoRepath = true;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

        navMeshAgent.stoppingDistance = 1.0f; // nie potrzbne
        stoppingDistance = navMeshAgent.stoppingDistance;

        navMeshAgent.speed = aiParent.walkSpeed;

        if (pathPoint != null)
            navMeshAgent.SetDestination(pathPoint.position /* + offsetPosition*/);
        //CheckDestination(pathPoint.position);
    }
    #endregion

    #region Logic
    protected void CheckDestination (Vector3 destination)
    {
        path = new NavMeshPath();

        if (NavMesh.CalculatePath(aiParent.transform.position, destination, NavMesh.AllAreas, path))
            navMeshAgent.SetDestination(destination);
        else
            FindNearestPointOnNavMesh(destination);
    }

    protected virtual void FindNearestPointOnNavMesh (Vector3 destination)
    {
        float range = 10.0f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, range, NavMesh.AllAreas))
        {
            hitPosition = hit.position;
            Debug.Log("on nav mesh");
            pathPoint.position = hitPosition;
			navMeshAgent.SetDestination(pathPoint.position); //navMeshAgent.SetDestination(backPathPoint.position);
        }
        else
            Debug.LogError("not on navmesh");
    }

    //protected virtual void FindNearestPointOnNavMesh (Vector3 center)
    //{
    //    float range = 10.0f;
    //    Vector3 result = Vector3.zero;

    //    NavMeshHit hit;

    //    if (NavMesh.SamplePosition(center, out hit, range, NavMesh.AllAreas))
    //    {
    //        result = hit.position;
    //        Debug.Log(aiParent.gameObject.name + ", virgin hit mask: " + NavMesh.AllAreas);
    //        navMeshAgent.SetDestination(result);
    //    }
    //}
    #endregion

    #region INavigation
    public virtual void Tick () { }

    public virtual void Enable ()
    {
        //navMeshAgent.enabled = true;
        if (navMeshAgent.isOnNavMesh)
            navMeshAgent.isStopped = false;
        //GetBackPosition();
    }

    public virtual void Disable ()
    {
        //navMeshAgent.enabled = false;
        if (navMeshAgent.isOnNavMesh)
            navMeshAgent.isStopped = true;
    }

    public float WalkSpeed
    {
        get {
            return aiParent.walkSpeed;
        }

        set {
            aiParent.walkSpeed = value;
        }
    }

    public virtual void GetBackPosition () // virtual, virgin wlacza i wylacza kontroler
    {
        pathPoint = backPathPoint;
        //if (aiParent is Virgin)
        //    CheckDestination(pathPoint.position);
        Debug.Log("backposition: " + pathPoint.position + ", dest: " + navMeshAgent.destination);
        navMeshAgent.SetDestination(backPathPoint.position); // sprawdzic czy jest na navmeshu, jak nie to wziac krawedz
    }

    public void UpdateSpeedFactor ()
    {
		navMeshAgent.speed = aiParent.walkSpeed * aiParent.speedFactor * aiParent.speedFactorFromAnimation;
    }

    #region IPath
    public Quaternion GetPathPointRotation
    {
        get {
            return pathPoint.rotation;
        }

        set {
            pathPoint.rotation = value;
        }
    }
    public Vector3 GetPathPointPosition
    {
        get {
            return pathPoint.position;
        }
        set {
            pathPoint.position = value;
        }
    }
    public VirginTower GetVirginTower
    {
        get {
            if (pathPoint != null)
                return pathPoint.virginTower;
            else
                return null;
        }

        set {
            pathPoint.virginTower = value;
        }
    }

	public bool IsMovingEnabled()
	{
		return navMeshAgent.enabled && navMeshAgent.isOnNavMesh && navMeshAgent.isStopped == false;
	}
    #endregion IPath
    #endregion INavigation
}
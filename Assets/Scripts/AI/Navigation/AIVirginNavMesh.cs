using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;

// TODO: sprawdzic czy jak upada czy jest na navMeshu
public class AIVirginNavMesh : AINavMeshPath
{
    private Virgin virgin;

    //public AIVirginNavMeshController () { }

    public AIVirginNavMesh (Virgin ai, Pathpoint start) : base(ai, start)
    {
        this.virgin = ai;
        Init(ai, start);
    }

    public void Init (Virgin virgin, Pathpoint start)
    {
        this.virgin = virgin;
        aiParent = virgin;
        pathPoint = start;
        backPathPoint = start;
        InitNavMesh();
    }

	public override void Enable ()
	{
		navMeshAgent.enabled = true;
		//GetBackPosition();
	}

	public override void Disable ()
	{
		navMeshAgent.enabled = false;
	}

    protected override void InitNavMesh ()
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

        navMeshAgent.stoppingDistance = 0.2f; // nie potrzbne
        stoppingDistance = navMeshAgent.stoppingDistance;

        //CheckDestination(backPathPoint.position + new Vector3(0, 0.1f, 0));
        //navMeshAgent.SetDestination(backPathPoint.position);
        navMeshAgent.speed = aiParent.walkSpeed;
        //Debug.Log("init dest: " + navMeshAgent.destination);
        //navMeshAgent.enabled = false;
		this.Disable();
        //CheckDestination(navMeshAgent.destination);
    }

    public override void Tick ()
    {
        //if (!ai.isRunning)
        //    return;

        if (navMeshAgent == null || !navMeshAgent.enabled)
            return;

        //Debug.Log("tick navmesh virgin: " + backPathPoint.position);
        //navMeshAgent.SetDestination(backPathPoint.position);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= stoppingDistance /*&& pathPoint.position == navMeshAgent.destination*/)
        {
            Debug.Log("virgin path destination: " + navMeshAgent.destination + ", v pos: " + virgin.transform.position + 
                "pathpoint: " + pathPoint.position + ", bpos: " + backPathPoint.position);
            virgin.ArrivedToTower();

            //navMeshAgent.enabled = false;
            virgin.PlayBalconyAnimation();
            // odpal animacje balcony
            // koniec sciezki, jest w wiezy
            // albo dodac kilka sciezek, do wiezy, teleport na gore i pozniej na balkon
        }
    }


    public override void GetBackPosition () // virtual, virgin wlacza i wylacza kontroler
    {
        pathPoint = backPathPoint;
        CheckDestination(pathPoint.position);
        Debug.Log("backposition: " + pathPoint.position + ", dest: " + navMeshAgent.destination);
        navMeshAgent.SetDestination(backPathPoint.position); // sprawdzic czy jest na navmeshu, jak nie to wziac krawedz
    }
}
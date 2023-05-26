using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class AIEnemyNavMesh : AINavMeshPath // moze klasa abstrakcyjna, bardziej navMesh controller
{
    private Enemy enemy;
    public Vector3 temporaryPoint; // tymczasowy punkt do ktorego bedzie dazyl, np wywolany przez jakis event jak cofniecie, strach itd
    private Vector3 offsetPosition = Vector3.zero; // offset do celu, raczej nie potrzebne

    private Vector3 Destination
    {
        get {
            return pathPoint.position + offsetPosition;
        }
    }
    
   // public AIEnemyNavMesh () { }

    public AIEnemyNavMesh (Enemy ai, Pathpoint start) : base(ai, start)
    {
        this.enemy = ai;
        Init(ai, start);
        enemy.OnDie += OnDie;
    }

    public override void Init (AI ai, Pathpoint start)
    {
        base.Init(ai, start);

        Vector3 rand = UnityEngine.Random.insideUnitCircle * 3;
        offsetPosition = new Vector3(rand.x, 0.0f, rand.y); // offset
        Debug.Log("AIEnemyController init");
    }


    private void DrawLeaderLine ()
    {
        if (pathPoint != null)
            Debug.DrawLine(enemy.transform.position, pathPoint.position + offsetPosition, Color.red);
    }

    public void OnPathEnd ()
    {
        Debug.Log("end path navmesh");
        foreach(var virgin in enemy.carryVirginList )
        {
            navMeshAgent.isStopped = true;
            enemy.isAlive = false;
            virgin.OnTeleport_Invoke(); //?
            enemy.OnTeleport_Invoke();
        }
        enemy.carryVirginList.Clear();

    }

    private Vector3 CalculateSlotPosition ()
    {
        if (pathPoint == null)
        {
            Debug.LogError("PATH NOT FOUND");
            //return null;
        }
        
        return pathPoint.position + offsetPosition;
    }
    

    private void OnDie ()
    {
		if(navMeshAgent.isActiveAndEnabled)
      	  navMeshAgent.isStopped = true;
    }

    public override void Tick ()
    {
        if (!enemy.isAlive || enemy.BlockMovement)
            return;

        DrawLeaderLine(); // raczej nie potrzebne bedzie w stanach takich jak chodzenie czy bieganie

        if (navMeshAgent == null || !navMeshAgent.enabled)
            return;

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= stoppingDistance && pathPoint != null)
        {
            if (pathPoint == backPathPoint && enemy.carryVirginList.Count > 0)
            {
                //
                OnPathEnd();
				return;
            }

            pathPoint = pathPoint.GetNextPathPoint();
            if (pathPoint != null)
            {
                navMeshAgent.SetDestination(pathPoint.position);
                //Destiantion_PathOnNavMesh(navMeshAgent.destination);
                // dodaj offset do punktu, ale sprawdz czy jest dostepny taki punkt, jak nie to bierz krawedz
            }
            else
            {
                OnPathEnd();
            }

            //if (navMeshAgent.remainingDistance <= GetBackPosition())
        }
    }
}
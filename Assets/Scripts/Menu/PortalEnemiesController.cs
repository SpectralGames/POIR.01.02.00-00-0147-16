using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEnemiesController : MonoBehaviour 
{
	public BezierPath spawnPoint;
	public GameObject[] randomEnemiesToSpawn;
	private static Enemy currentEnemy;
	// Use this for initialization
	void Start () 
	{
		this.SpawnRandomEnemy();
	}

	private void SpawnRandomEnemy()
	{
		Vector3 pos = spawnPoint.transform.position;
		Quaternion rot = spawnPoint.transform.rotation;

		GameObject enemyClone = Instantiate(randomEnemiesToSpawn[Random.Range(0, randomEnemiesToSpawn.Length)], pos, rot, this.transform);
		currentEnemy = enemyClone.GetComponent<Enemy>();
		currentEnemy.Init(spawnPoint.GetComponent<Pathpoint>(), spawnPoint, NavigationType.Splines);
		currentEnemy.StopAllowAttackPlayer ();
		(currentEnemy as AI).OnDie += this.OnCurrentEnemyDied;
		(currentEnemy as AI).OnTakeDamage += this.OnCurrentEnemyTakeDamage;
		(currentEnemy.NavigationController as AIEnemySpline).OnSplineEndAction += this.OnCurrentEnemySplineEnd;

		ObjectPool.Instance.enemyList.Add(currentEnemy);
	}

	private void OnCurrentEnemySplineEnd()
	{
		currentEnemy.isMoving = false;
		currentEnemy.Animator.SetAnimatorBool("Idle", true);
		currentEnemy.Animator.SetAnimatorBool("Walk", false);
	}

	private void OnCurrentEnemyDied()
	{
		ObjectPool.Instance.RemoveEnemy(currentEnemy, false);
		currentEnemy = null;
		Invoke("SpawnRandomEnemy", 8f);
	}

	private void OnCurrentEnemyTakeDamage()
	{
		currentEnemy.CancelInvoke();
	}

	public static Enemy GetCurrentEnemy()
	{
		return currentEnemy;
	}

	public void EnableAttack(bool value)
	{   
		currentEnemy?.EnableAttack(value);
	}
		
	public Vector3 GetCurrentEnemyPosition()
	{
		if (currentEnemy != null) {
			return currentEnemy.transform.position + (Vector3.up * (currentEnemy.GetAIHeight()/2));
		} else {
			return this.transform.position;
		}
	}
}

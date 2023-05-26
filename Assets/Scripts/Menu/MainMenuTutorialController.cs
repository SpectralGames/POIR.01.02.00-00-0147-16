using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenuTutorialController : MonoBehaviour 
{
	public Action OnCurrentEnemyDiedCompleted;
	public Action<string> OnSummoningWarriorCompleted;

    TowerBase currentTower;
    // Use this for initialization
    void Start () 
	{
		this.TryToBindTheCurrentEnemy();
		this.TryToBindTheMaidenSpawnPoint();
	}


	private void OnCurrentEnemyDied()
	{
		if(OnCurrentEnemyDiedCompleted != null)
			OnCurrentEnemyDiedCompleted();
		
		this.TryToBindTheCurrentEnemy();
	}

	private void TryToBindTheCurrentEnemy()
	{
		Enemy currentEnemy = PortalEnemiesController.GetCurrentEnemy();
		if(currentEnemy != null)
		{
			currentEnemy.OnDie += this.OnCurrentEnemyDied;
		}else{
			Invoke("TryToBindTheCurrentEnemy", 0.5f);
		}
	}

	private void TryToBindTheMaidenSpawnPoint()
	{
		GameObject spawnPoint = GameObject.Find("MaidenSpawnPoint");
		spawnPoint.GetComponent<MaidenSpawnTower>().onTowerSpawned += this.OnTowerSpawned;
	}

	private void OnTowerSpawned(TowerBase tower)
	{
        currentTower = tower;
        if (OnSummoningWarriorCompleted != null)
			OnSummoningWarriorCompleted.Invoke(tower.towerType.ToString());
	}

    public void DestroyCurrentTower()
    {
        if (currentTower != null)
        {
            currentTower.Die();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}

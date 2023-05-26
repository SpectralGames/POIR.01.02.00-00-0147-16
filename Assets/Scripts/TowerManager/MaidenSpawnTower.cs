using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaidenSpawnTower : MonoBehaviour 
{
	public Transform spawnPoint;
	public GameObject particleCloud;
	public Action<TowerBase> onTowerSpawned;
	private TowerBase currentTowerPlaced = null;

	// Use this for initialization
	void Start () 
	{
		
	}
		
		
	public void OnTowerSpawned(TowerBase towerSpawned)
	{
		particleCloud.gameObject.SetActive (false);
		towerSpawned.OnDie += OnCurrentTowerDied;
		currentTowerPlaced = towerSpawned;

		if(onTowerSpawned != null)
			onTowerSpawned.Invoke(currentTowerPlaced);
	}

	public bool IsSpawnPointOccupied()
	{
		return currentTowerPlaced != null;
	}

	private void OnCurrentTowerDied()
	{
		particleCloud.gameObject.SetActive (true);
		currentTowerPlaced = null;
	}
			
	public Vector3 GetSpawnPoint()
	{
		return spawnPoint.transform.position;
	}

	public void RemoveTower()
	{
		if (currentTowerPlaced != null) {
			currentTowerPlaced.GetComponent<TowerBase> ().Die();
		}
	}

	public bool IsTowerPlaced()
	{
		Debug.Log ("tower:" + currentTowerPlaced);
		return currentTowerPlaced == null ? false : true;
	}
		

}

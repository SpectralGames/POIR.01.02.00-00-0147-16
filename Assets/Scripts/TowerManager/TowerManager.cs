using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TowerManager : MonoBehaviour {

	public List<GameObject> towersList = new List<GameObject>();


	void Start ()
	{
		
		//towersParent.transform.SetParent(this.transform);
	}


		
	private GameObject FindTowerByType(EAttackType type)
	{
		foreach (GameObject tower in towersList) {
			if (tower.GetComponent<TowerBase>().towerType == type)
				return tower;
		}
		return null;
	}
}

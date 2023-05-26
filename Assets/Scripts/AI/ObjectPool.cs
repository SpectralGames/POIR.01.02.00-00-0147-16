using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// klasa trzymajaca referencje do list obiektow
// lub innych pojedynczych

public class ObjectPool
{
    public List<Virgin> virgins = new List<Virgin>();
	public List<TowerBase> towerList = new List<TowerBase>();
	public List<Enemy> enemyList = new List<Enemy>();
	public List<TowerTeleport> towerTeleportList = new List<TowerTeleport> ();
	public List<ThrowableObject> throwableList = new List<ThrowableObject>();
	public List<GameObject> boltList;
	public List<GameObject> fireBoltList;
	public List<GameObject> iceBoltList;
	public PlayerController player;
	public GameplayController gameplayController;
    public GameObject WorldCanvas;

    private static ObjectPool _instance;
    public static ObjectPool Instance
    {
        get {
            // jesli nie zostal zainicjowany
            if (_instance == null)
            {
                _instance = new ObjectPool();
                _instance.Init();
            }
            return _instance;
        }
    }
    
    public void Init()
    {
        virgins = GameObject.FindObjectsOfType<Virgin>().ToList();
        player = GameObject.FindObjectOfType<PlayerController>();
        gameplayController = GameObject.FindObjectOfType<GameplayController>();
        boltList = new List<GameObject>();
        fireBoltList = new List<GameObject>();
        iceBoltList = new List<GameObject>();
        WorldCanvas = GameObject.Find("WorldCanvas");
    }
    public void RemoveVirgin(Virgin virgin)
	{
		if (this.virgins.Contains (virgin)) 
		{
			this.virgins.Remove (virgin);
			gameplayController.OnVirginLost ();
		}
	}

	public int GetAliveVirginCount()
	{
		return this.virgins.Count;
	}

	public int GetAliveEnemyCount()
	{
		return this.enemyList.Count;
	}

	public void RemoveEnemy(Enemy enemy, bool sendCallbackEnemyDie)
	{
		if (this.enemyList.Contains (enemy)) 
		{
			this.enemyList.Remove (enemy);
			if(gameplayController != null && sendCallbackEnemyDie)
				gameplayController.OnEnemyDie (enemy);
		}
	}

	public void ClearData()
	{
		towerList.Clear ();
		enemyList.Clear ();
		virgins.Clear ();
		towerTeleportList.Clear ();
		throwableList.Clear ();
	}

	public GameObject GetBolt(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (!list[i].gameObject.activeInHierarchy)
			{
				return list[i];
			}
				
		}
		return null;
	}

	public void ReturnBolt(GameObject bolt)
	{
		bolt.SetActive(false);
		bolt.transform.position = new Vector3(0, 0, 0);
		
	}
}
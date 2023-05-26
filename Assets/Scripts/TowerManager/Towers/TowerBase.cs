using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
public enum ETowerType
{
	None,


}
*/
public class TowerBase : AI {

	public EAttackType towerType;

	public Transform spawnWeaponPivot;
	public GameObject particlesDeathPrefab;

	protected float attackRate;
	protected int totalHealth;
	protected float attackRange;
	protected float totalAttackTime;
	protected float lifeTime;
	protected int towerLevel;

	protected StatusBar statusBar;
	protected List<Enemy> enemiesInRange;
	protected Enemy nearestEnemy; 
	private float attackRateCounter = 0;
	private float lifeTimeCounter = 0;

	protected float lookAheadOffset = 0f;



	// Use this for initialization

	protected virtual void Awake()
	{
		enemiesInRange = new List<Enemy>();
        animator = new TowerAnimator(this);
	}

	void Start () 
	{
		Rigidbody rigidBody = gameObject.AddComponent <Rigidbody> ();
		rigidBody.isKinematic = true;
        attackRateCounter = (60f / (float)attackRate);// - .2f;
    }

    public virtual void OnSetValues(int level)
	{
		if(level == -1) // debugowo, jesli jest zablokowany to ustaw mu domyślnie 1 
			level = 1;

		if(level > -1) //gdy level=-1 -> nieodblokowany, nie sciagaj wartosci
		{
			string currentItemCodeName = towerType.ToString();
			attackRange = XMLItemsReader.GetRadiusValue(currentItemCodeName, level);
			attackRate = XMLItemsReader.GetAttackRateValue(currentItemCodeName, level);	
			totalHealth = XMLItemsReader.GetHealthValue(currentItemCodeName, level);
			lifeTime = XMLItemsReader.GetLifeTimeValue (currentItemCodeName, level);
		}

		towerLevel = level;
        attackRateCounter = (60f / (float)attackRate);// - .2f;

	
		InitHealth(GetAIHeight() + .9f);
	}

	protected void InitHealth (float height)
	{
		try
		{
			float towerHeight = 3.3f; // wysokość czarodziejki przypisana na stale
			statusBar = Instantiate(Resources.Load<StatusBar>("StatusBar"), Vector3.zero, Quaternion.identity);
			statusBar.Init(this, totalHealth , towerHeight, true);
			statusBar.EnableTimeBar(lifeTime);
		}
		catch (Exception e)
		{
			Debug.LogError("Health nie znaleziony " + e.Message);
		}
	}

	// Update is called once per frame
	protected virtual void Update () 
	{
		if (isAlive) {
			FindEnemyAndFight ();
			CheckLifeTime ();

		}
		statusBar?.Tick();
	}
		
	protected void FindEnemyAndFight()
	{
		CheckEnemyInRange ();

		//czysc liste ze zniszczonych wrogow
		for (int i = enemiesInRange.Count - 1; i >= 0; i--)
		{
			if(enemiesInRange[i] == null || (enemiesInRange[i] != null && enemiesInRange[i].isAlive == false)) 
				enemiesInRange.RemoveAt(i);
		}

		Transform nearestEnemyTransform = CalculationHelper.FindNearestObject (enemiesInRange, this.transform.position);
		if (nearestEnemyTransform != null) { //znalazlem wroga, patrz na niego
			nearestEnemy = nearestEnemyTransform.gameObject.GetComponent<Enemy> ();
			Vector3 forwardVector = (nearestEnemy.transform.position + nearestEnemy.transform.forward * lookAheadOffset) - this.transform.position;
			forwardVector.y = this.transform.forward.y;
			this.transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (forwardVector), Time.deltaTime * 5);

		} else {
			nearestEnemy = null;
            attackRateCounter = (60f / (float)attackRate);
        }

		if(nearestEnemy != null)
		{
			//odpal animacje ataku jesli jest wrog w zasiegu ataku
			//Debug.Log("END MOVE ATTACK: " + this.gameObject.name);
			if (attackRateCounter >= 60f / (float)attackRate && nearestEnemy != null)
			{
				attackRateCounter = 0f;
				this.PlayAnimAttack();
			}
			attackRateCounter += Time.deltaTime;
		}
	}

	private void CheckLifeTime()
	{
		if (statusBar != null) 
		{
			lifeTimeCounter += Time.deltaTime;
			statusBar.timePoints = lifeTimeCounter;
			if (lifeTimeCounter >= lifeTime) {
				Die ();
			}
		}
	}
		
	protected virtual void CheckEnemyInRange()
	{
		foreach (Enemy enemy in ObjectPool.Instance.enemyList)
		{
			if (enemy.isAlive) 
			{
				if (enemiesInRange.Contains (enemy)) {
					if (Vector3.Distance (new Vector3 (this.transform.position.x, 0f, this.transform.position.z), new Vector3 (enemy.transform.position.x, 0f, enemy.transform.position.z)) > attackRange) {
						enemiesInRange.Remove (enemy);
					}
				}
				else
				{
					if (Vector3.Distance (new Vector3 (this.transform.position.x, 0f, this.transform.position.z), new Vector3 (enemy.transform.position.x, 0f, enemy.transform.position.z)) < attackRange) {
						enemiesInRange.Add (enemy);
					}
				}
			}
		}
	}

	public virtual void TakeDamage(float dmg) 
	{
		if (!isAlive) return;

		statusBar.healthPoints -= dmg;
		if (statusBar.healthPoints <= 0.0f) {
			Die ();
		} else {
			animator.SetAnimatorTrigger("Hit");
			OnTakeDamage_Invoke ();
		}
		CancelInvoke("MakeAttack");
	}
		
	public virtual void Die()
	{
		isAlive = false;
		animator.SetAnimatorTrigger("Dead");
		OnDie_Invoke();
	
		CancelInvoke ();
		Instantiate (particlesDeathPrefab, transform.position + Vector3.up, Quaternion.identity);
		StartCoroutine(this.OnStartTrashing()); 

	}

	protected override IEnumerator OnStartTrashing()
	{
		float initScaleFactor = transform.localScale.x;
		Vector3 initPosition = transform.localPosition;
		float trashingTime = 0;
		while(trashingTime < .5)
		{
			trashingTime += Time.deltaTime;
			transform.localPosition = Vector3.Lerp (initPosition, initPosition + Vector3.up * 1, trashingTime);
			yield return null;
		}
		Destroy(this.gameObject);
	}

	public void HideHealthBar()
	{
		statusBar.gameObject.SetActive (false);
	}

	protected virtual void PlayAnimAttack() { }

	public virtual void MakeAttack()  {
		if(attackAudioClips.Length > 0 && SoundManager.instance != null)
		{
			int audioClipIndex = UnityEngine.Random.Range(0, attackAudioClips.Length);
			SoundManager.instance.PlaySceneEffect(attackAudioClips[audioClipIndex], gameObject.transform.position, -1f, 0f, 0.5f);
		}
	
	}

    /// <summary>
    /// Ma na ceu wyłączenie wyjątków zgłaszanych przez animator podczas eventów. 
    /// </summary>
    public void Invoke() { }


}

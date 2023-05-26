using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFire : TowerBase
{
	public GameObject attackEffectPrefab;

	protected override void Awake()
	{
		base.Awake();
		this.lookAheadOffset = 1.5f;
	}

	protected override void PlayAnimAttack()
	{
        animator.SetAnimatorTrigger("AttackFire");
		//animator.SetTrigger("AttackFire");
	//	Invoke("MakeAttack", timeForAttackEvent);
	}

	public override void MakeAttack()
	{
		base.MakeAttack ();
		if (nearestEnemy != null)
		{
			//KFBulletControllerBase bulletController = currentArrowInAir.GetComponent<KFBulletControllerBase> ();
			GameObject attackEffect = GameObject.Instantiate (attackEffectPrefab, this.spawnWeaponPivot.position, Quaternion.LookRotation((nearestEnemy.transform.position+nearestEnemy.GetAIHeight()*Vector3.up + nearestEnemy.transform.forward*lookAheadOffset)-this.spawnWeaponPivot.position), this.spawnWeaponPivot.transform) as GameObject;
			if(attackEffect.GetComponent<MaidenWeaponBaseController>() != null)
				attackEffect.GetComponent<MaidenWeaponBaseController>().OnInit(this.spawnWeaponPivot.position, this.spawnWeaponPivot.forward, towerLevel);
		}
	}


	protected override void Update () 
	{
		base.Update();

		//debug attack
		if(Input.GetKeyDown(KeyCode.X))
		{
			this.nearestEnemy = GameObject.FindObjectOfType<Enemy>();
			if(this.nearestEnemy != null)
				enemiesInRange.Add(nearestEnemy);
		}
	}

	/*
	protected override void OnSwitchGeometryToSpecificLevel(int level)
	{
		
	}
	*/
}

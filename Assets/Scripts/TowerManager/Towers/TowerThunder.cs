using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerThunder : TowerBase
{
	public GameObject attackEffectPrefab;

	protected override void PlayAnimAttack()
	{
        animator.SetAnimatorTrigger("AttackThunder");
		//animator.SetTrigger("AttackThunder");
		//Invoke("MakeAttack", timeForAttackEvent);


	}

	public override void MakeAttack()
	{
		base.MakeAttack ();
		if (nearestEnemy != null)
		{
			//KFBulletControllerBase bulletController = currentArrowInAir.GetComponent<KFBulletControllerBase> ();
			Vector3 targetPosition = nearestEnemy.transform.position;
			targetPosition += nearestEnemy.transform.forward;// offset strzału, przewiduje gdzie będzie enemy na podstawie jego kierunku i prędkości
			GameObject attackEffect = GameObject.Instantiate (attackEffectPrefab, this.transform.position + Vector3.up, nearestEnemy.transform.localRotation) as GameObject;
			if(attackEffect.GetComponent<MaidenWeaponBaseController>() != null)
				attackEffect.GetComponent<MaidenWeaponBaseController>().OnInit(targetPosition, (targetPosition-this.transform.position).normalized, towerLevel);
		}
	}

	/*
	protected override void OnSwitchGeometryToSpecificLevel(int level)
	{
		
	}
	*/
}

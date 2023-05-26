using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerIce : TowerBase
{
	public GameObject attackEffectPrefab;

	protected override void PlayAnimAttack()
	{
        animator.SetAnimatorTrigger("AttackIce");
		//animator.SetTrigger("AttackIce");
		//Invoke("MakeAttack", timeForAttackEvent);
	}

	protected override void CheckEnemyInRange()
	{
		List<Enemy> enemyList = ObjectPool.Instance.enemyList;
		for (int i = enemyList.Count - 1; i >= 0; i--)
		{
			if (enemyList[i].isAlive && enemiesInRange.Contains(enemyList[i]) == false && enemyList[i].GetCurrentStateID() != StateID.Frozen) 
			{
				if (Vector3.Distance (new Vector3(this.transform.position.x, 0f, this.transform.position.z), new Vector3(enemyList[i].transform.position.x, 0f, enemyList[i].transform.position.z)) < attackRange)
				{
					enemiesInRange.Add (enemyList[i]);
				}
			}else if(enemyList[i].isAlive && enemiesInRange.Contains(enemyList[i]) && enemyList[i].GetCurrentStateID() == StateID.Frozen)
			{
				enemiesInRange.Remove(enemyList[i]); //usun z listy zamrozonego wroga, zeby nie atakowala go wiezyczka kolejny raz
			}
		}
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

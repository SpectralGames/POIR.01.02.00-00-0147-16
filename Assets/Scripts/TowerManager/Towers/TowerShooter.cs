using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerShooter : TowerBase
{
	public GameObject arrow;
	private GameObject currentArrowInAir;

	protected override void PlayAnimAttack()
	{
        animator.SetAnimatorTrigger("AttackCrossbow");
	//	Invoke("MakeAttack", timeForAttackEvent);

	}
		
	public override void MakeAttack()
	{
		base.MakeAttack ();
		if (nearestEnemy != null)
		{
			Vector3 initAttackPosition;
			if (spawnWeaponPivot != null) {
				initAttackPosition = spawnWeaponPivot.position;
			} else {
				initAttackPosition = this.transform.position + Vector3.up;	
			}

			currentArrowInAir = GameObject.Instantiate (arrow, initAttackPosition, Quaternion.identity) as GameObject;

			MaidenDistanceWeapon arrowController = currentArrowInAir.GetComponent<MaidenDistanceWeapon>();
			arrowController.Init(towerLevel);

			Vector3 targetPosition = Vector3.zero;

			if (nearestEnemy.NavigationController.IsMovingEnabled()) {
				if (nearestEnemy.hitPivot != null) {
					targetPosition = nearestEnemy.hitPivot.position + nearestEnemy.transform.forward * (nearestEnemy.walkSpeed * .1f);
				} else {
					targetPosition = nearestEnemy.transform.position + nearestEnemy.transform.forward * (nearestEnemy.walkSpeed * .1f) + (Vector3.up * (nearestEnemy.GetAIHeight () * .65f));
				}
			} 
			else
			{
				if (nearestEnemy.hitPivot != null) {
					targetPosition = nearestEnemy.hitPivot.position;
				} else {
					targetPosition = nearestEnemy.transform.position + (Vector3.up * (nearestEnemy.GetAIHeight () * .65f));
				}
			}
			float distance = Vector3.Distance (currentArrowInAir.transform.position, targetPosition);
			arrowController.OnShoot(targetPosition, 0, .1f);

		}
	}

	/*
	protected override void OnSwitchGeometryToSpecificLevel(int level)
	{
		
	}
	*/
}

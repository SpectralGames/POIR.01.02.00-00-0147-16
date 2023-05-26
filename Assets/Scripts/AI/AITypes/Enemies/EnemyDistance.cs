using System;
using System.Collections;
using UnityEngine;



public class EnemyDistance : Enemy
{
    #region Actions
    #endregion Actions

    #region Members
	public GameObject weapon;
	public Transform spawnWeaponPivot;
	public GameObject weaponToHideAfterAnimation;
    #endregion


    #region Inits
    private void Start ()
    {
	    attackRange = UnityEngine.Random.Range(attackRange, attackRange + 4);
    }
    public override void Init (Pathpoint startPathPoint, BezierPath startBezierPath, NavigationType navigationType)
    {
        base.Init(startPathPoint, startBezierPath, navigationType);
        //CreateFSM();
    }


    #endregion Inits

    #region Logic
    
    private void Update ()
    {
        if (!isInit)
            return;

        NavigationController?.Tick(); // new
        //aiController?.Tick();
        statusBar?.Tick();
        if (!isSpawnedByHand)
        {
	        finiteStateMachine.CurrentState.Reason();
	        finiteStateMachine.CurrentState.Act();
        }
        
		ChargeAttack ();
    }

    public override void ForceOnLookAt (Transform target = null)
    {
        StopCoroutine("OnLookAt");
        //if (target == null)
        //    target = aiController.EnemyFixer.currentTargetInformation; // zmienic  nie bedzie enemyfixer
        StartCoroutine(OnLookAt(target));
    }

	public override void StartAnimationAttack()
	{
		if (enemyType == EEnemyType.Archer) {
			OnAttack_Invoke ("AttackArcher");
		} else if (enemyType == EEnemyType.Axe) {
			OnAttack_Invoke ("AttackAxe");
			Invoke ("HideWeaponAfterAnimation", 1);
		} else if (enemyType == EEnemyType.IceShaman) {
			OnAttack_Invoke ("AttackIceShaman");
		} else {
			OnAttack_Invoke ("Attack");
		}
	}

	private void HideWeaponAfterAnimation()
	{
		weaponToHideAfterAnimation.SetActive (false);
		StartCoroutine (ShowWeaponAnim ());
	}

	private IEnumerator ShowWeaponAnim()
	{
		yield return new WaitForSeconds (1);
		weaponToHideAfterAnimation.SetActive (true);

		Vector3 initScale = weaponToHideAfterAnimation.transform.localScale;
		weaponToHideAfterAnimation.transform.localScale = Vector3.zero;

		float destAnimTime = .5f;
		float currentAnimTime = 0;
		while (currentAnimTime <= destAnimTime)
		{
			currentAnimTime += Time.deltaTime;
			weaponToHideAfterAnimation.transform.localScale = initScale * (currentAnimTime / destAnimTime);
			yield return null;
		}
		weaponToHideAfterAnimation.transform.localScale = initScale;

	}

		
	public override void MakeAttack ()
	{
		ResetAttack ();

		if(attackAudioClips.Length > 0)
		{
			int audioClipIndex = UnityEngine.Random.Range(0, attackAudioClips.Length);
			SoundManager.instance.PlaySceneEffect(attackAudioClips[audioClipIndex], gameObject.transform.position, -1f, 0f, 0.5f);
		}


		Vector3 initAttackPosition;
		if (spawnWeaponPivot != null) {
			initAttackPosition = spawnWeaponPivot.position;
		} else {
			initAttackPosition = this.transform.position + Vector3.up;	
		}

		GameObject weaponInAir = GameObject.Instantiate (weapon, initAttackPosition, Quaternion.identity) as GameObject;
		EnemyDistanceWeapon weaponController = weaponInAir.GetComponent<EnemyDistanceWeapon>();
		weaponController.SetShooterReference (this.gameObject);

		Vector3 targetPosition = Vector3.zero;

		if (finiteStateMachine.CurrentStateID == StateID.AttackingTower) 
		{
			targetPosition = targetTowerBase.transform.position + Vector3.up * targetTowerBase.GetAIHeight();
		} 
		else if (finiteStateMachine.CurrentStateID == StateID.AttackingPlayer)
		{
			targetPosition = ObjectPool.Instance.player.GetPlayerHeadPosition ();
		}

		float distance = Vector3.Distance (weaponInAir.transform.position, targetPosition);
		weaponController.OnShoot(targetPosition, 0, .1f);
	}
		
    /*protected override void CreateFSM ()
    {
        // TODO: stworz mozliwe stany // podac stringa, moze dictionary
        EnemyMovementState movement = CreateMovementState();
        //EnemyTakeDamageState takeDamage = CreateTakeDamageState();
        EnemyAvoidingSpellState avoid = CreateAvoidState();

        //EnemyCarryVirginState virgin = new EnemyCarryVirginState(package);
        EnemyDeathState death = new EnemyDeathState(package);

		EnemyAttackState attackPlayer = CreateAttackState();
		EnemyAttackTowerState attackTower = CreateAttackTowerState ();

        finiteStateMachine = new FSMSystem();
        finiteStateMachine.AddState(movement);
        //finiteStateMachine.AddState(takeDamage);
        finiteStateMachine.AddState(avoid);
        finiteStateMachine.AddState(death);
        //finiteStateMachine.AddState(virgin);
		finiteStateMachine.AddState(attackPlayer);
		finiteStateMachine.AddState(attackTower);
    }

    private EnemyAvoidingSpellState CreateAvoidState ()
    {
        EnemyAvoidingSpellState avoid = new EnemyAvoidingSpellState(package);
        avoid.AddTransition(Transition.LostPlayer, StateID.Movement);
        avoid.AddTransition(Transition.Hit, StateID.TakingDamage);
        avoid.AddTransition(Transition.DeadlyHit, StateID.Death);
        return avoid;
    }

    private EnemyTakeDamageState CreateTakeDamageState ()
    {
        EnemyTakeDamageState takeDamage = new EnemyTakeDamageState(package, animator.GetAnimations("Hit"));
        takeDamage.AddTransition(Transition.LostPlayer, StateID.Movement); //?? // transisiton = nie jest juz w polu razenia
        takeDamage.AddTransition(Transition.Hit, StateID.TakingDamage);
        takeDamage.AddTransition(Transition.DeadlyHit, StateID.Death);
        takeDamage.AddTransition(Transition.SpellThrew, StateID.AvoidingSpell);
        return takeDamage;
    }

    private EnemyMovementState CreateMovementState ()
    {
        EnemyMovementState movement = new EnemyMovementState(package, animator.GetAnimations("Walk"), animator.GetAnimations("Run"));
        movement.AddTransition(Transition.SawPlayer, StateID.AttackingPlayer);
        movement.AddTransition(Transition.Hit, StateID.TakingDamage);
        movement.AddTransition(Transition.DeadlyHit, StateID.Death);
        movement.AddTransition(Transition.SpellThrew, StateID.AvoidingSpell);
        movement.AddTransition(Transition.VirginIsClose, StateID.CarryVirign);
		movement.AddTransition(Transition.SawTowerBase, StateID.AttackingTower);
        return movement;
    }

	private EnemyAttackState CreateAttackState ()
	{
		EnemyAttackState attack = new EnemyAttackState(package);
		attack.AddTransition(Transition.Hit, StateID.TakingDamage);
		attack.AddTransition(Transition.DeadlyHit, StateID.Death);
		attack.AddTransition(Transition.LostPlayer, StateID.Movement);
		return attack;
	}

	private EnemyAttackTowerState CreateAttackTowerState ()
	{
		EnemyAttackTowerState attackTower = new EnemyAttackTowerState(package);
		attackTower.AddTransition(Transition.Hit, StateID.TakingDamage);
		attackTower.AddTransition(Transition.DeadlyHit, StateID.Death);
		attackTower.AddTransition(Transition.TowerBaseDie, StateID.Movement);

		return attackTower;
	}*/
		
    #endregion Logic
}
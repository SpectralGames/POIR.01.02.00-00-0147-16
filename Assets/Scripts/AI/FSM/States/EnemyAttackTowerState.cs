using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyAttackTowerState : FSMState
{
    //private EnemyStatePackage package;
    //private float changeMovementDitance = 7.5f;

    private Enemy enemy;
    //private bool isRun = false;

    //private List<AnimationClip> runAnimations = new List<AnimationClip>();
    //private List<AnimationClip> walkAnimations = new List<AnimationClip>();


    #region Constrcutors
	public EnemyAttackTowerState (params StateID[] statesToTransition) : base(statesToTransition)
    {
		this.stateID = StateID.AttackingTower;
    }

	public EnemyAttackTowerState (EnemyStatePackage package, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;

		this.stateID = StateID.AttackingTower;
    }

	public EnemyAttackTowerState (EnemyStatePackage package, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;

		this.stateID = StateID.AttackingTower;
    }

	public EnemyAttackTowerState (EnemyStatePackage package, List<AnimationClip> walkClips, List<AnimationClip> runClips, params StateID[] statesToTransition) : base(statesToTransition)
    {
       // this.package = package;
        this.enemy = package.enemy;

        //runAnimations = new List<AnimationClip>(runClips);
        //walkAnimations = new List<AnimationClip>(walkClips);

		this.stateID = StateID.AttackingTower;
    }
    #endregion Constructors




	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
	{
		enemy.StopMovement ();
		enemy.targetTowerBase.OnDie += OnTowerBaseDie;
        enemy.NavigationController.Disable();
    }

	public override void DoBeforeLeaving (FSMStateArgs additionalLeavingArgs = null)
	{
		base.DoBeforeLeaving(additionalLeavingArgs);
		enemy.targetTowerBase.OnDie -= OnTowerBaseDie;
        enemy.NavigationController.Enable();
    }


    public override void Reason ()
    {
		
       // throw new NotImplementedException();
    }

    public override void Act ()
    {
        if (enemy.BlockMovement) return;
		Vector3 forwardVector = enemy.targetTowerBase.transform.position - enemy.transform.position;
		forwardVector.y = enemy.transform.forward.y;
		enemy.transform.rotation = Quaternion.Slerp (enemy.transform.rotation, Quaternion.LookRotation (forwardVector), Time.deltaTime * 5);
		enemy.Attack ();
    }

	private void OnTowerBaseDie()
	{
		enemy.targetTowerBase.OnDie -= OnTowerBaseDie;
		enemy.SetTransition(StateID.Movement);  //Transition.TowerBaseDie);
	}
}
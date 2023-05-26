using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyAttackState : FSMState
{
    //private EnemyStatePackage package;
    private float changeMovementDitance = 7.5f;

    private Enemy enemy;
    private bool isRun = false;

    private List<AnimationClip> runAnimations = new List<AnimationClip>();
    private List<AnimationClip> walkAnimations = new List<AnimationClip>();


    #region Constrcutors
	public EnemyAttackState (params StateID[] statesToTransition) : base(statesToTransition)
    {
		this.stateID = StateID.AttackingPlayer;
    }

	public EnemyAttackState (EnemyStatePackage package, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;

        this.stateID = StateID.AttackingPlayer;
    }

	public EnemyAttackState (EnemyStatePackage package, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;

        this.stateID = StateID.AttackingPlayer;
    }

	public EnemyAttackState (EnemyStatePackage package, List<AnimationClip> walkClips, List<AnimationClip> runClips, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;

        runAnimations = new List<AnimationClip>(runClips);
        walkAnimations = new List<AnimationClip>(walkClips);

        this.stateID = StateID.AttackingPlayer;
    }
    #endregion Constructors

	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
	{
		enemy.StopMovement ();
	//	enemy.targetTowerBase.OnDie += OnTowerBaseDie;
	}

	public override void DoBeforeLeaving (FSMStateArgs additionalLeavingArgs = null)
	{
		base.DoBeforeLeaving(additionalLeavingArgs);
		enemy.NavigationController.Enable ();
	}
		
    public override void Reason ()
    {
		PlayerIsClose ();
    }

    private void PlayerIsClose()
    {
        if (enemy.WaitForAttackAnimationToEnd)
        {
            if (Vector3.Distance(enemy.transform.position + Vector3.up * enemy.GetAIHeight(), ObjectPool.Instance.player.GetPlayerHeadPosition()) > (enemy.attackRange + .1f) && !enemy.isAttackCharged && !enemy.IsPlayingAttackAnimation)
            {
                enemy.SetTransition(StateID.Movement);  //(Transition.LostPlayer);
            }
        }
        else
        {
            if (Vector3.Distance(enemy.transform.position + Vector3.up * enemy.GetAIHeight(), ObjectPool.Instance.player.GetPlayerHeadPosition()) > (enemy.attackRange + .1f))
            {
                enemy.SetTransition(StateID.Movement);  //(Transition.LostPlayer);
            }
        }
    }

 //   private void PlayerIsClose()
	//{

	//	if (Vector3.Distance (enemy.transform.position + Vector3.up * enemy.GetAIHeight(), ObjectPool.Instance.player.GetPlayerHeadPosition()) > (enemy.attackRange + .1f)) {
	//		enemy.SetTransition(StateID.Movement);  //(Transition.LostPlayer);
	//	}
	//}
		
	public override void Act ()
	{
        if (enemy.BlockMovement) return;
		Vector3 forwardVector = ObjectPool.Instance.player.GetPlayerHeadPosition() - enemy.transform.position;
		forwardVector.y = enemy.transform.forward.y;
		enemy.transform.rotation = Quaternion.Slerp (enemy.transform.rotation, Quaternion.LookRotation (forwardVector), Time.deltaTime * 5);

		enemy.Attack ();
	}
}
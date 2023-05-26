using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMFrozenStateArgs : FSMStateArgs
{
	public FSMFrozenStateArgs(float targetFreezeTime)
	{
		this.freezeTime = targetFreezeTime;
	}
	public float freezeTime;
}

public class EnemyFrozenState : FSMState
{
	private FSMSystem parentStateSytem;
	private StateID lastStateIDBeforeEntering;
	private Enemy enemy;
	private float freezeTime;
	private bool isFreezeActive;
	//private List<AnimationClip> carryAnimations = new List<AnimationClip>();

	#region Constructors
	public EnemyFrozenState(Enemy parentEnemy, params StateID[] statesToTransition) : base(statesToTransition)
	{
		this.enemy = parentEnemy;
		this.stateID = StateID.Frozen;
	}

	public EnemyFrozenState(Enemy parentEnemy, FSMSystem system, params StateID[] statesToTransition) : base(statesToTransition)
	{
		//this.package = package;
		this.enemy = parentEnemy;
		this.parentStateSytem = system;
		this.stateID = StateID.Frozen;
	}

	public EnemyFrozenState(Enemy parentEnemy, FSMSystem system, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
	{
		this.enemy = parentEnemy;
		this.parentStateSytem = system;
		//carryAnimations = new List<AnimationClip>(anim);
		this.stateID = StateID.Frozen;
	}
	#endregion Constructors


	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
	{
		base.DoBeforeEntering(additionalEnteringArgs);

		freezeTime = (additionalEnteringArgs as FSMFrozenStateArgs).freezeTime;
		isFreezeActive = true;

		(enemy.Animator as EnemyAnimator).SetAnimatorPauseState(true);
		enemy.NavigationController.Disable();

		if(parentStateSytem != null)
		{
			this.lastStateIDBeforeEntering = parentStateSytem.LastStateID;
		}
	}

	public override void DoBeforeLeaving(FSMStateArgs additionalLeavingArgs = null)
	{
		base.DoBeforeLeaving(additionalLeavingArgs);

		(enemy.Animator as EnemyAnimator).SetAnimatorPauseState(false);
		enemy.NavigationController.Enable();
	}

	public override void Reason ()
	{
		freezeTime -= Time.deltaTime;

		if(freezeTime <= 0f && isFreezeActive)
		{
			isFreezeActive = false;
			if(enemy.statusBar.healthPoints > 0.0f) //skonczyl sie freeze i postac przezyla
			{
				enemy.SetTransition(this.lastStateIDBeforeEntering);
			}
		}else if(freezeTime <= 0f && isFreezeActive == false)
		{
			if(enemy.statusBar.healthPoints <= 0.0f) //skonczyl sie freeze i postac nie przezyla
			{
				this.AddTransition(StateID.Death);
				enemy.SetTransition(StateID.Death);
				this.DeleteTransition(StateID.Death);
			}
		}
	}
		

	public override void Act ()
	{

	}
}

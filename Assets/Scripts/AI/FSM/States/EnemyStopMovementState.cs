using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMStopMovementStateArgs : FSMStateArgs
{
	public FSMStopMovementStateArgs(float stopMovementTime)
	{
		this.stopMovementTime = stopMovementTime;
	}
	public float stopMovementTime;
}

public class EnemyStopMovementState : FSMState 
{
	#region Members
	//private EnemyStatePackage package;
	private FSMSystem parentStateSytem;
	private StateID lastStateIDBeforeEntering;
	private Enemy enemy;
	private float stopMovementTime;
	private bool isStopActive;
	//private List<AnimationClip> deathAnimations = new List<AnimationClip>();
	#endregion Members

	#region Constructors
	public EnemyStopMovementState(Enemy parentEnemy, params StateID[] statesToTransition) : base(statesToTransition)
	{
		this.enemy = parentEnemy;
		this.stateID = StateID.StopMovement;
	}

	public EnemyStopMovementState(Enemy parentEnemy, FSMSystem system, params StateID[] statesToTransition) : base(statesToTransition)
	{
		//this.package = package;
		this.enemy = parentEnemy;
		this.parentStateSytem = system;
		this.stateID = StateID.StopMovement;
	}

	public EnemyStopMovementState(Enemy parentEnemy, FSMSystem system, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
	{
		this.enemy = parentEnemy;
		this.parentStateSytem = system;
		this.stateID = StateID.StopMovement;
	}
	#endregion Constructors

	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
	{
		base.DoBeforeEntering(additionalEnteringArgs);

		stopMovementTime = (additionalEnteringArgs as FSMStopMovementStateArgs).stopMovementTime;
		isStopActive = true;

		if(parentStateSytem != null)
		{
			this.lastStateIDBeforeEntering = parentStateSytem.LastStateID;
		}

		enemy.StopMovement();
    }

	public override void DoBeforeLeaving (FSMStateArgs additionalLeavingArgs = null)
	{
		base.DoBeforeLeaving(additionalLeavingArgs);
	}

	public override void Reason ()
	{
		stopMovementTime -= Time.deltaTime;

		if(stopMovementTime <= 0f && isStopActive)
		{
			isStopActive = false;

			if(enemy.statusBar.healthPoints > 0.0f) //skonczyl sie freeze i postac przezyla
			{
				enemy.SetTransition(this.lastStateIDBeforeEntering);
			}else{
				enemy.SetTransition(StateID.Death);
			}
		}
	}

	public override void Act ()
	{

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyTakeDamageState : FSMState // jest sens to robic?
{
    #region Members
    //private EnemyStatePackage package;
    private Enemy enemy;

    //private List<AnimationClip> animations = new List<AnimationClip>();
    #endregion Mebmers

    #region Constructors
	public EnemyTakeDamageState(params StateID[] statesToTransition) : base(statesToTransition)
    {
        stateID = StateID.TakingDamage;
    }

	public EnemyTakeDamageState(EnemyStatePackage package, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;
        stateID = StateID.TakingDamage;
    }

	public EnemyTakeDamageState(EnemyStatePackage package, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;
        //animations = new List<AnimationClip>(anim);
        stateID = StateID.TakingDamage;
    }
    #endregion Constructors

	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
    {
		base.DoBeforeEntering(additionalEnteringArgs);
        //enemy.TakeDamage(10.0f);
        //enemy.OnTakeDamage_Invoke();
        //enemy.SetTransition(Transition.LostPlayer);
    }

    public override void Reason ()
    {
		enemy.SetTransition(StateID.Movement); // Transition.LostPlayer);
    }

    public override void Act ()
    {
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyDeathState : FSMState
{
    #region Members
    //private EnemyStatePackage package;
    private Enemy enemy;
    //private List<AnimationClip> deathAnimations = new List<AnimationClip>();
    #endregion Members

    #region Constructors
    public EnemyDeathState ()
    {
        stateID = StateID.Death;
    }

	public EnemyDeathState (EnemyStatePackage package, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;
        stateID = StateID.Death;
    }

	public EnemyDeathState (EnemyStatePackage package, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;
        //deathAnimations = new List<AnimationClip>(anim);
        stateID = StateID.Death;
    }
    #endregion Constructors

	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
    {
        enemy.OnDie_Invoke();
    }

    public override void Reason ()
    {
        
    }

    public override void Act ()
    {
        
    }
}
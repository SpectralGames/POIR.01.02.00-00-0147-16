using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyCarryVirginState : FSMState
{
    //private EnemyStatePackage package;

    private Enemy enemy;
    //private List<AnimationClip> carryAnimations = new List<AnimationClip>();

    #region Constructors
	public EnemyCarryVirginState(params StateID[] statesToTransition) : base(statesToTransition)
    {
        this.stateID = StateID.CarryVirign;
    }

	public EnemyCarryVirginState(EnemyStatePackage package, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;
        this.stateID = StateID.CarryVirign;
    }

	public EnemyCarryVirginState(EnemyStatePackage package, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
    {
       // this.package = package;
        this.enemy = package.enemy;
        //carryAnimations = new List<AnimationClip>(anim);
        this.stateID = StateID.CarryVirign;
    }
    #endregion Constructors

	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
    {
        // losuj animacje noszenia dziewicy
        //if (enemy == null)
        //    Debug.LogError("Carry virgin on enemy null");
        foreach (var virgin in enemy.carryVirginList)
        {
            virgin.SetParent(enemy);
            virgin.OnCapture_Invoke();
        }

        enemy.Animator.SetAnimatorBool("Carry", true);
        enemy.NavigationController.GetBackPosition();
        Debug.Log("DoBeforeEntering EnemyCarryVirginState !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        // trzeba pobrac info ktora wlasnie dziewice niesie
        // zmien animacje dla enemy
    }

	public override void DoBeforeLeaving (FSMStateArgs additionalLeavingArgs = null)
    {
        // z tego stanu mozna tylko wyjsc kiedy sie umrze
        // jak umrze, porzuc dziewice
		base.DoBeforeLeaving(additionalLeavingArgs);
    }

    public override void Reason ()
    {
       
    }


   

    public override void Act ()
    {
        
    }


}
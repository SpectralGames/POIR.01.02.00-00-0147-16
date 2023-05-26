using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyAvoidingSpellState : FSMState
{
    #region Members
    //private EnemyStatePackage package;
    private Enemy enemy;
    private float startSpeed;
    //private List<AnimationClip> avoidAnimations = new List<AnimationClip>();

    private float stopTimer = 2.0f;
    #endregion

    #region Constructors
	public EnemyAvoidingSpellState (params StateID[] statesToTransition) : base(statesToTransition)
    {
        stateID = StateID.AvoidingSpell;
    }

	public EnemyAvoidingSpellState (EnemyStatePackage package, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;

        this.startSpeed = enemy.walkSpeed;
        stateID = StateID.AvoidingSpell;
    }

	public EnemyAvoidingSpellState (EnemyStatePackage package, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
    {
        //this.package = package;
        this.enemy = package.enemy;
        this.startSpeed = enemy.walkSpeed;
        //avoidAnimations = new List<AnimationClip>(anim);
        stateID = StateID.AvoidingSpell;
    }
    #endregion Constructors

	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
    {
        startSpeed = enemy.walkSpeed;
    }

	public override void DoBeforeLeaving (FSMStateArgs additionalLeavingArgs = null)
    {
        enemy.NavigationController.WalkSpeed = startSpeed;
        //enemy.AIController.navMeshAgent.speed = startSpeed;
        stopTimer = 2.0f;
    }

    public override void Reason () // nawet nie potrzebne jesli jest paczka przesylana
    {
        stopTimer -= Time.deltaTime;
        if (stopTimer < 0.0f)
			enemy.SetTransition(StateID.Movement); //(Transition.LostPlayer);
    }

    public override void Act ()
    {
        // uciekaj od miejsca uderzenia jesli, jest mozliwosc to przyspiesz do przodu, jesli masz mozliwosc ucieszki do transition do Run
        // jesli nie zdarzysz to zrob odskok do tylu
        // akcja nie wykonuje sie moze dla wsyzstkich tylko dla jakiegos procentu
        // przeciwnikow, zeby kazdy nie ominal tego spella

        // moze to wszystko w do on start
        //if (Vector3.Distance(spell.transform.position, npc.transform.position) < 2.0f)
        //{
        //    // run
        //}
        //else
        //{
        //    // odskocz, 
        //    enemy.AIController.navMeshAgent.speed = 0.0f;
        //    // play animation
        //}
    }    
}
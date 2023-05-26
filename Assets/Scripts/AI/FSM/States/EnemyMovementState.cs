using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyMovementState : FSMState
{
    #region Members
    private EnemyStatePackage package;
    private float changeMovementDitance = 0; // dystans od gracza
    private List<PathpointTower> virginTowers = new List<PathpointTower>();

    private Enemy enemy;
    private bool isRun = false;

    private List<AnimationClip> runAnimations = new List<AnimationClip>();
    private List<AnimationClip> walkAnimations = new List<AnimationClip>();

    #endregion

    #region Constrcutors
	public EnemyMovementState (params StateID[] statesToTransition) : base(statesToTransition)
    {
        stateID = StateID.Movement;
        virginTowers = GameObject.FindObjectsOfType<PathpointTower>().ToList();
    }

	public EnemyMovementState(EnemyStatePackage package, params StateID[] statesToTransition) : base(statesToTransition)
    {
        this.package = package;
        this.enemy = package.enemy;
        stateID = StateID.Movement;
        virginTowers = GameObject.FindObjectsOfType<PathpointTower>().ToList();
    }

	public EnemyMovementState(EnemyStatePackage package, List<AnimationClip> anim, params StateID[] statesToTransition) : base(statesToTransition)
    {
        this.package = package;
        this.enemy = package.enemy;
        InitAnimations(anim);
        stateID = StateID.Movement;
        virginTowers = GameObject.FindObjectsOfType<PathpointTower>().ToList();
    }
		
	public EnemyMovementState (EnemyStatePackage package, List<AnimationClip> walkClips, List<AnimationClip> runClips, params StateID[] statesToTransition) : base(statesToTransition)
    {
        this.package = package;
        this.enemy = package.enemy;
        runAnimations = new List<AnimationClip>(runClips);
        walkAnimations = new List<AnimationClip>(walkClips);
        stateID = StateID.Movement;
        virginTowers = GameObject.FindObjectsOfType<PathpointTower>().ToList();
    }
    #endregion Constructors

    private void InitAnimations(List<AnimationClip> clips)
    {
        foreach (AnimationClip ac in clips)
        {
            if (ac.name.Contains("Run"))
                runAnimations.Add(ac);
        }

        foreach (AnimationClip ac in clips)
        {
            if (ac.name.Contains("Walk"))
                walkAnimations.Add(ac);
        }
    }

	public override void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null)
	{
		base.DoBeforeEntering(additionalEnteringArgs);
		enemy.NavigationController.Enable ();
		SetMovement(isRun, enemy.walkSpeed);
	}
		
    public override void Reason ()
    {
		if (!enemy.isAlive || enemy.IsAttackEnabled() == false)
            return;

        VirginTowerIsClose();
        VirginIsClose();
        TowerBaseIsClose();
		PlayerIsClose ();

    }

    private void VirginTowerIsClose()
    {
        if (enemy.CheckHasEmptyPivotsForVirgin() == false)
            return;

       // Debug.Log("Enemy name movement: " + enemy.name);

        VirginTower virginTower = enemy.NavigationController.GetVirginTower;
        if (virginTower != null)
        {
            if (virginTower.IsTowerEmpty())
                return;
           // Debug.Log("enemy: " + enemy.name + ", Is Virgin Tower: " + enemy.NavigationController.GetPathPointPosition);
            Vector3 pos = enemy.NavigationController.GetPathPointPosition;

            if (Vector3.Distance(enemy.transform.position, pos) < 2.0f)
            {
                foreach (var pivot in enemy.carryPivotList)
                {
                    if (pivot.childCount == 0)
                    {
                        Virgin v = virginTower.TakeAvailableVirgin();
                        enemy.carryVirginList.Add(v);
                        v.Act.Captured = true;
                        v.SetParent(enemy);
                        v.OnCapture_Invoke();
                    }
                }
                  

                enemy.Animator.SetAnimatorBool("Carry", true);
                enemy.NavigationController.GetBackPosition();

				this.DeleteTransition(StateID.AttackingPlayer);
				this.DeleteTransition(StateID.AttackingTower);
				//enemy.SetTransition(StateID.CarryVirign); //(Transition.VirginIsClose);
            }
        }  
    }

    private void VirginIsClose ()
    {
        if (enemy.CheckHasEmptyPivotsForVirgin() == false)
            return;

        foreach (Virgin v in ObjectPool.Instance.virgins)
        {
            // sprawdz czy ma do zlapania jakas
            if (Vector3.Distance(enemy.transform.position, v.transform.position) < 2.0f &&
                !v.Act.Captured && !v.Act.InTower)
            {
                // zabierz dziewice
                foreach (var pivot in enemy.carryPivotList)
                {
                    if (pivot.childCount == 0)
                    {
                        enemy.carryVirginList.Add(v);
                        v.Act.Captured = true;
                        v.SetParent(enemy);
                        v.OnCapture_Invoke();
                        break;
                    }
                }

                enemy.Animator.SetAnimatorBool("Carry", true);
                enemy.NavigationController.GetBackPosition();

				this.DeleteTransition(StateID.AttackingPlayer);
				this.DeleteTransition(StateID.AttackingTower);
				//enemy.SetTransition(StateID.CarryVirign); //(Transition.VirginIsClose);  w tym stanie trzeba wyowlac action dla virgin
                break;
            }
        }
    }

	private void TowerBaseIsClose()
	{
		List<TowerBase> towersInRange = new List<TowerBase> ();
		foreach (TowerBase towerBase in ObjectPool.Instance.towerList)
		{
			if (towerBase.isAlive) {
				if (Vector3.Distance (enemy.transform.position, towerBase.transform.position) < enemy.attackRange) {
					towersInRange.Add (towerBase);
				}
			}
		}
		//TowerBase nearestTowerBase = null;
		Transform nearestTransform = CalculationHelper.FindNearestObject (towersInRange, enemy.transform.position);
		if (nearestTransform != null) { //znalazlem wroga, patrz na niego
			enemy.targetTowerBase = nearestTransform.gameObject.GetComponent<TowerBase> ();
			enemy.SetTransition(StateID.AttackingTower); //(Transition.SawTowerBase);
		} else {
			//nearestTowerBase = null;
		}
	}

	private void PlayerIsClose()
	{
		if (enemy.AllowAttackPlayer ())
        {
			if (Vector3.Distance (enemy.transform.position + Vector3.up * enemy.GetAIHeight (), ObjectPool.Instance.player.GetPlayerHeadPosition ()) < enemy.attackRange) {
				enemy.SetTransition(StateID.AttackingPlayer); //(Transition.SawPlayer);
			}
		}
	}
		
    public override void Act () // jakas paczka z wszystkimi informacjami
    {
        if (enemy.BlockMovement)
            return;

        if (Vector3.Distance(package.player.transform.position, enemy.transform.position) < changeMovementDitance) // np kiedy wiadomo ze z tego stanu po jakims czasie dochodzi sie do innego
        {
            Run();
            //npc.transform.LookAt(player.transform); // TODO: lerp
        }
        else if (Vector3.Distance(package.player.transform.position, enemy.transform.position) > changeMovementDitance) // np kiedy wiadomo ze z tego stanu po jakims czasie dochodzi sie do innego
        {
            Walk();
        }
    }

    private void Walk()
    {
        if (isRun)
        {
            isRun = false;
            SetMovement(isRun, -enemy.additionalSpeedToRun);
        }
			
		enemy.SetSpeedFactorFromAnimation(enemy.Animator.GetCurrentClipSpeed());
		enemy.NavigationController.UpdateSpeedFactor ();
    }

    private void SetMovement(bool isRun, float speed)
    {
        enemy.Animator.SetAnimatorBool("Walk", !isRun);
        enemy.Animator.SetAnimatorBool("Run", isRun);
		enemy.Animator.SetAnimatorBool ("Idle", false);
    }
    
    private void Run()
    {
        if (!isRun)
        {
            isRun = true;
            SetMovement(isRun, enemy.additionalSpeedToRun);
        }
			
		enemy.SetSpeedFactorFromAnimation(enemy.Animator.GetCurrentClipSpeed());
		enemy.NavigationController.UpdateSpeedFactor ();
    }
}
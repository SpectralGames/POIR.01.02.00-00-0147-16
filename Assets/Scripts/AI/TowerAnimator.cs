using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TowerAnimator : AIAnimator
{
    private TowerBase tower;

    private List<AnimationClip> deathClips = new List<AnimationClip>();
    private List<AnimationClip> takeDamageClips = new List<AnimationClip>();
    private List<AnimationClip> attackClips = new List<AnimationClip>();
    private List<AnimationClip> walkClips = new List<AnimationClip>();
    private List<AnimationClip> runClips = new List<AnimationClip>();
    //private List<AnimationClip> carryClips = new List<AnimationClip>();

    public TowerAnimator() { }

    public TowerAnimator(TowerBase towerBase)
    {
        this.tower = towerBase;
        Init(tower);
    }


    public override void Init (AI ai)
    {
        base.Init(ai);

        InitAnimations();

        //this.SetAnimatorBool("Walk", true);

        tower.OnTakeDamage += PlayTakeDamgeAnimation;
        tower.OnPathEnd += SetToIdleState;
        tower.OnDie += PlayDeathAnimation;
    }


    protected override void InitAnimations ()
    {
        //attackClips = SplitClipsByName("Attack");
    }

    private void PlayTakeDamgeAnimation ()
    {
        //PlayAnimation(takeDamageClips);
    }

    protected void SetToIdleState ()
    {
        //enemy.AIController.navMeshAgent.isStopped = true;
        //if (animators != null)
        //{
        //    this.SetAnimatorBool("Attack", false);
        //    this.SetAnimatorBool("Walk", false);
        //    this.SetAnimatorTrigger("Idle");
        //}
    }

    private void PlayDeathAnimation ()
    {
        // losuj animacje
        //PlayAnimation(deathClips);

        //SetAnimatorBool("Walk", false);
        //SetAnimatorBool("Idle", false);

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class EnemyAnimator : AIAnimator
{
    private Enemy enemy;

    private List<AnimationClip> deathClips = new List<AnimationClip>();
    private List<AnimationClip> takeDamageClips = new List<AnimationClip>();
    private List<AnimationClip> attackClips = new List<AnimationClip>();
    private List<AnimationClip> walkClips = new List<AnimationClip>();
    private List<AnimationClip> runClips = new List<AnimationClip>();
    private List<AnimationClip> carryClips = new List<AnimationClip>();

    public float timeForAttackEvent; // ??
    public bool isAttackTimeSensitive = false;

    public EnemyAnimator () { }

    public EnemyAnimator(Enemy enemy)
    {
        this.enemy = enemy;
        Init(enemy);
    }

    public override void Init(AI ai) 
    {
        base.Init(ai);

        InitAnimations();

        this.SetAnimatorBool("Walk", true);
        this.SetAnimatorBool("Idle", false);

        enemy.OnTakeDamage += PlayTakeDamgeAnimation;
        enemy.OnPathEnd += SetToIdleState;
        enemy.OnDie += PlayDeathAnimation;
		enemy.OnAttack += PlayAttackAnimation;
    }

    protected override void InitAnimations ()
    {
        deathClips = SplitClipsByName("Dead");
        takeDamageClips = SplitClipsByName("Hit");
        attackClips = SplitClipsByName("Attack");
        runClips = SplitClipsByName("Run");
        walkClips = SplitClipsByName("Walk");
        carryClips = SplitClipsByName("Carries");
    }

    private void PlayDeathAnimation()
    {
        // losuj animacje
		Debug.Log("deathclo[s: "+ deathClips);
		CrossPlayAnimation(deathClips);

        SetAnimatorBool("Walk", false);
        SetAnimatorBool("Idle", false); 
		SetAnimatorBool("Dead", true); 
    }
    
    private void PlayTakeDamgeAnimation()
    {
		this.SetAnimatorTrigger("Hit");
    }

    public float PlayRunAnimation()
    {
        float animTime = 0;
        PlayAnimation(runClips, out animTime);
        return animTime;
    }

    public float PlayWalkAnimation(float speed = 1)
    {
        float animTime = 0;
        PlayAnimation(walkClips, speed);
        //PlayAnimation(walkClips, out animTime);
        return animTime;
    }

	protected void PlayAttackAnimation (string parameterName)
    {
        if (isAttackTimeSensitive)
        {
			this.SetAnimatorTrigger(parameterName);
            //Invoke("MakeAttack", timeForAttackEvent);
        }
        else
			this.SetAnimatorBool(parameterName, true);
    }

	public void SetAnimatorPauseState(bool isPaused)
	{
		List<Animator> animators = GetAnimators();
		foreach(Animator anim in animators)
		{
			anim.speed = isPaused ? 0f : 1f;
		}
	}

    protected void SetToIdleState ()
    {
        //enemy.AIController.navMeshAgent.isStopped = true;
        if (animators != null)
        {
            this.SetAnimatorBool("Attack", false);
            this.SetAnimatorBool("Walk", false);
            this.SetAnimatorTrigger("Idle");
        }
    }

    #region Setters

    #endregion Setters
}
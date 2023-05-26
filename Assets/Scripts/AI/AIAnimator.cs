using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AIAnimator
{
    protected List<Animator> animators = new List<Animator>();
    //protected UnityEditor.Animations.AnimatorController animatorController;
    protected AnimationClip[] clips;
    private float animationSpeed = 1;

    protected Dictionary<string, List<AnimationClip>> animations = new Dictionary<string, List<AnimationClip>>();
    public List<AnimationClip> GetAnimations(string animationKey)
    {
        // sprawdzic czy istnieje
        if (animations.ContainsKey(animationKey))
            return animations[animationKey];

        return null;
    }

    public AIAnimator()
    {

    }

    public virtual void Init(AI ai) // constructor zrobic
    {
        animators = new List<Animator>();
        Animator[] childrenAnimators = ai.GetComponentsInChildren<Animator>();
        animators.AddRange(childrenAnimators);
        animationSpeed = ai.walkSpeed;

        // rozwiazanie z KnightFall'a
        //animatorController = animators[0].runtimeAnimatorController as UnityEditor.Animations.AnimatorController; // moze przeleciec przez cala tablice
		clips = animators[0].runtimeAnimatorController.animationClips;// animatorController.animationClips;
    }

    protected virtual void InitAnimations () { }

    protected List<AnimationClip> SplitClipsByName(string name)
    {
        List<AnimationClip> newClips = new List<AnimationClip>();
        foreach (AnimationClip ac in clips)
        {
			if (ac.name.ToLower().Contains(name.ToLower()))
                newClips.Add(ac);
        }
        animations.Add(name, newClips);
        return newClips;
    }

    public void PlayAnimation(string name)
    {
        List<AnimationClip> clips = GetAnimations(name);
        PlayAnimation(clips);
    }

    protected void PlayAnimation (List<AnimationClip> clips)
    {
        if (clips.Count > 0)
        {
            AnimationClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
            if (clip != null)
            {
                animators[0].Play(clip.name);
            }
        }
    }

	protected void CrossPlayAnimation (List<AnimationClip> clips)
	{
		if (clips.Count > 0)
		{
			AnimationClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
			if (clip != null)
			{
				animators[0].CrossFade(clip.name,.2f);
			}
		}
	}

    protected void PlayAnimation (List<AnimationClip> clips, float speed)
    {
        if (clips.Count > 0)
        {
            AnimationClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
            if (clip != null)
            {
                clip.frameRate /= speed;
                animators[0].Play(clip.name);
            }
        }
    }

    protected void PlayAnimation (List<AnimationClip> clips, out float animTime)
    {
        animTime = 0.0f;
        if (clips.Count > 0)
        {
            AnimationClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
            
            if (clip != null)
            {
                animators[0].Play(clip.name);
                animTime = clip.length;
            }
        }
    }

    public void SetAnimationSpeed(List<AnimationClip> clips, float speed)
    {
        if (clips.Count > 0)
        {
            AnimationClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];

            if (clip != null)
            {
                clip.frameRate /= speed;
            }
        }
    }


    public void SetAnimatorBool (string boolName, bool value)
    {
        foreach (Animator anim in animators)
        {
            if (anim.runtimeAnimatorController != null)
                anim.SetBool(boolName, value);
        }
    }

    public void SetAnimatorTrigger (string triggerName)
    {
        foreach (Animator anim in animators)
        {
            if (anim.runtimeAnimatorController != null)
                anim.SetTrigger(triggerName);
        }
    }
		
    protected void SetAnimatorRandomSpeed (float speed)
    {
        foreach (Animator anim in animators)
        {
            if (anim.runtimeAnimatorController != null)
                anim.speed = speed;
        }
    }

    public void SetAnimatorWalkRunBlend ()
    {
        float walkBlend = Mathf.InverseLerp(1.4f, 3.5f, 10.0f /*enemy.speed*/);

        foreach (Animator anim in animators)
        {
            if (anim.runtimeAnimatorController != null)
                anim.SetFloat("WalkRunBlend", walkBlend);
        }
    }

    public void SetAnimatorWalkBlend(float speed)
    {
        foreach (Animator anim in animators)
        {
            if (anim.runtimeAnimatorController != null)
                anim.SetFloat("WalkRunBlend", speed);
        }
    }

	public void SetAnimatorSpeedFactor(float speedFactor)
	{
		foreach (Animator anim in animators)
		{
			if (anim.runtimeAnimatorController != null)
            {
				anim.SetFloat("SpeedFactor", speedFactor);
            }
		}
	}

	public float GetCurrentClipLength()
	{
		foreach (Animator anim in animators)
		{
			if (anim.runtimeAnimatorController != null)
			{
				return  anim.GetCurrentAnimatorStateInfo (0).length;
			}
		}
		return 0;
	}

	public float GetCurrentClipSpeed()
	{
		foreach (Animator anim in animators)
		{
			if (anim.runtimeAnimatorController != null)
			{
				return  anim.GetCurrentAnimatorStateInfo (0).speed;
			}
		}


		return 0;
	}

	public List<Animator> GetAnimators()
	{
		return animators;
	}
}
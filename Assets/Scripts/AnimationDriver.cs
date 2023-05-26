using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDriver : MonoBehaviour 
{
	public float animOffset = 0f;
	public float animSpeed = -100f;
	private Animator animator;
	private float tempTime;
	private int initAnimNameHash;
	private float crossFadeSpeed = 0;

	private float time = 0;
	private float currentAnimValue =0;
	private float tempAnimatorTime;




	// Use this for initialization
	void Awake ()
	{
		animator = this.gameObject.GetComponent<Animator>();
		SetParams ();
	}
		
	private void SetParams()
	{
		foreach (AnimatorControllerParameter param in animator.parameters)
		{
			if (param.name == "Offset") 
				animator.SetFloat ("Offset", animOffset);
			else if(param.name == "Speed")
			{
				if (this.animSpeed > -95f) 
					this.gameObject.GetComponent<Animator> ().SetFloat ("Speed", animSpeed);
			}
		}
	}

	public void SetAnimationSpeed(float speed)
	{
		if (animator != null) {
			foreach (AnimatorControllerParameter param in animator.parameters) {
				if (param.name == "Speed") {
					this.gameObject.GetComponent<Animator> ().SetFloat ("Speed", speed);
				}
			}
		}
	}
	public float GetAnimationSpeed()
	{
		if (animator != null) {
			foreach (AnimatorControllerParameter param in animator.parameters) {
				if (param.name == "Speed") {
					return this.gameObject.GetComponent<Animator> ().GetFloat ("Speed");
				}
			}
		}
		return 0;
	}

	public void MoveNormalState()
	{
		StopCoroutine ("OnAnimation");
		//float duration = (30 / crossFadeSpeed) * 1.5f;
		crossFadeSpeed = 0;
		this.SetAnimationSpeed (animSpeed);

		animator.enabled = false;
		animator.enabled = true;

	//	animator.CrossFade (initAnimNameHash, duration, 0,tempTime);
	}
		
	public void MoveToInitState(float speed)
	{
		/*
		crossFadeSpeed += speed;
		initAnimNameHash = animator.GetCurrentAnimatorStateInfo (0).shortNameHash;
	//	animator.CrossFade(initAnimNameHash, duration, 0, 1);
		tempAnimatorTime = Mathf.Ceil(animator.GetCurrentAnimatorStateInfo (0).normalizedTime) - 1;
		tempTime =  animator.GetCurrentAnimatorStateInfo (0).normalizedTime%1;
		float animLength = animator.GetCurrentAnimatorStateInfo (0).length;

		float timeToEnd = ( animLength - (animLength * tempTime)) /60;
		this.SetAnimationSpeed (timeToEnd * crossFadeSpeed);

		StartCoroutine ("OnAnimation");
		*/
		this.SetAnimationSpeed (1);
		animator.PlayInFixedTime (animator.GetCurrentAnimatorStateInfo (0).shortNameHash, 0, 1);
		animator.Update (Time.deltaTime);
		this.OnStopAnimator ();
	}

	IEnumerator OnAnimation()
	{
		/*
		yield return null;
		while (animator.IsInTransition(0)) 
		{
			Debug.Log ("anim:" + animator.GetCurrentAnimatorStateInfo (0).normalizedTime % 1);
			yield return null;
		};
		Debug.Log ("end anim:" + animator.GetCurrentAnimatorStateInfo (0).normalizedTime % 1);
		this.OnStopAnimator ();
		*/

		yield return null;
		while (currentAnimValue - tempAnimatorTime < 1 + animOffset) 
		{
			currentAnimValue = animator.GetCurrentAnimatorStateInfo (0).normalizedTime;
			yield return null;
		};
	//	Debug.Log ("end anim:" + animator.GetCurrentAnimatorStateInfo (0).normalizedTime % 1);
		this.OnStopAnimator ();

	}
		
	public void OnStopAnimator()
	{
		animator.enabled = false;
	}


	public void Reset()
	{
		if(animator != null) animator.enabled = true;
		crossFadeSpeed = 0; 
		SetAnimationSpeed (animSpeed);
	}

	public void clearCrossFadeSpeed()
	{
		crossFadeSpeed = 0;
	}


}

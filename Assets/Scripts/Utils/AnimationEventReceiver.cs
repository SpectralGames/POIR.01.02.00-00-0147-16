using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventReceiver : MonoBehaviour {

	public UnityEvent AttackAnimationEvent;
	public UnityEvent WalkAnimationEvent;
	public UnityEvent CatchAnimationEvent;
	public UnityEvent OnFinishAnimationEvent;

	public void OnWalkEvent()
	{
		if(WalkAnimationEvent != null)
			WalkAnimationEvent.Invoke ();	
	}

	public void OnAttackEvent()
	{
		if(AttackAnimationEvent != null)
			AttackAnimationEvent.Invoke ();	
	}

	public void OnCatchEvent()
	{
		if(CatchAnimationEvent != null)
			CatchAnimationEvent.Invoke ();	
	}

	public void OnAnimationFinishEvent()
	{
		if(OnFinishAnimationEvent != null)
			OnFinishAnimationEvent.Invoke ();	
	}
    
    /// <summary>
    /// Ma na ceu wyłączenie wyjątków zgłaszanych przez animator podczas eventów. 
    /// </summary>
    public void Invoke() { }
}
	

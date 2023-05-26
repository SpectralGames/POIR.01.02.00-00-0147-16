using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UIAnimatorEase
{
	LINEAR,
	EASEOUT,
	EASEIN,
	EASE_OUT_BACK
};

public class UIAnimationEngine : MonoBehaviour {
	
	public UIAnimatorEase ease;
	[Header("Time Params")]
	public float timeInSeconds;
	public float delay;
	[Header("Behaviour Params")]
	public bool isReverse = false;
	public bool isLoop = false;
	public bool autoStart = false;

	protected float currentAnimationTime;

	protected bool isPlaying = false;
	protected bool isReverseMoving = false;
	protected bool isPlayingNormalMove = false;
	protected bool isReverseAllowed = false;

	private GameObject eventReciever;
	private string completeEvent;

    public UnityEvent OnAnimationEnd = new UnityEvent();

	void OnEnable()
	{
		PrepareAnimation ();
		if (autoStart) {
			StartAnimation ();
		}
	}

	public virtual void StartAnimation()
	{
		isPlayingNormalMove = true;
		isPlaying = true;
	}

	public void StopAnimation()
	{
		isPlaying = false;
		isReverseMoving = false;
	}

	void Update()
	{
		if (isPlaying) {
			OnAnimation ();
		}
	}

	public virtual void OnFinishAnimation()
	{
        OnAnimationEnd.Invoke();
		currentAnimationTime = 0;
		if (isPlayingNormalMove)
		{
			// jesli jest opcja powrotnej animacji to zamien wartosci docelowe i zmien flage normal move
			if (isReverse)
			{
				isPlayingNormalMove = false;
				ReverseValues ();
			} 
			// jest jest loopowana animacja wyczysc wartosci
			else if (isLoop) 
			{
				ClearValues ();
			} 
			// jesli niej jest powrotna i nie jest loop wyslij event i zatrzymaj animacje
			else
			{
				SendCompleteEvent ();
				StopAnimation ();
			}

		} 
		// jesli zakonczyl sie ruch powrotny, sprawdz czy jest opcja rewerse loop
		else 
		{
			if (isLoop) {
				isPlayingNormalMove = true;
				ReverseValues ();
			} else {
				StopAnimation ();
				SendCompleteEvent ();
			}
		}	
	}

	public virtual void ClearValues() { }

	public virtual void PrepareAnimation() {  } 

	public virtual void OnAnimation() { }

	public virtual void ReverseValues() { }

	public bool IsPlaying() {
		return isPlaying;
	}

	// MESSAGE --------------------------------------------------------------------------------------
	public void SetCompleteEvent(GameObject reciever, string message)
	{
		this.eventReciever = reciever;
		this.completeEvent = message;
	}

	protected void SendCompleteEvent()
	{
		if(eventReciever != null && completeEvent != null)
			eventReciever.SendMessage(completeEvent, gameObject, SendMessageOptions.DontRequireReceiver);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIAnimatorSequence : MonoBehaviour {

	public List<UIAnimationEngine> animatorsSequenceList;
	[Header("Behaviour Params")]
	public bool autoStart = false;

	private GameObject eventReciever;
	private string completeSequenceEvent;
	private string completeAnimationEvent;

	private int currentIndexAnimation = 0;


	void Awake()
	{
		if (autoStart) {
			StartSequence ();
		}
	}

	public void StartSequence()
	{
		StartNextAnim ();
	}

	private void StartNextAnim()
	{
		if (currentIndexAnimation < animatorsSequenceList.Count) {
			animatorsSequenceList [currentIndexAnimation].SetCompleteEvent (this.gameObject, "OnAnimationComplete");
			animatorsSequenceList [currentIndexAnimation].StartAnimation ();	
		} else {
			SendCompleteSequenceEvent ();
		}
	}
	
	private void OnAnimationComplete()
	{
		SendCompleteAnimationEvent ();
		currentIndexAnimation++;
		StartNextAnim ();
	}
		
	// MESSAGE --------------------------------------------------------------------------------------
	public void SetCompleteSequenceEvent(GameObject reciever, string message)
	{
		this.eventReciever = reciever;
		this.completeSequenceEvent = message;
	}

	protected void SendCompleteSequenceEvent()
	{
		if(eventReciever != null && completeSequenceEvent != null)
			eventReciever.SendMessage(completeSequenceEvent, SendMessageOptions.DontRequireReceiver);
	}

	public void SetCompleteAnimationEvent(GameObject reciever, string message)
	{
		this.eventReciever = reciever;
		this.completeAnimationEvent = message;
	}

	protected void SendCompleteAnimationEvent()
	{
		if(eventReciever != null && completeAnimationEvent != null)
			eventReciever.SendMessage(completeAnimationEvent, currentIndexAnimation, SendMessageOptions.DontRequireReceiver);
	}

}

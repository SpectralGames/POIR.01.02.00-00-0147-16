using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIAlphaAnimator : UIAnimationEngine {

	// scale
	public float valueFrom;
	public float valueTo;
	public bool drawBefore = true;

	private float tempValueFrom;
	private float tempValueTo;
	private float initValue = 0;
	private CanvasGroup canvasGroup;

	public override void PrepareAnimation()
	{
		canvasGroup = gameObject.GetComponent<CanvasGroup> ();
		if (canvasGroup == null) {
			canvasGroup = gameObject.AddComponent<CanvasGroup> ();
		}

		tempValueFrom = valueFrom;
		tempValueTo = valueTo;
		initValue = canvasGroup.alpha;
		currentAnimationTime = -delay;

	} 

	public override void StartAnimation()
	{
		base.StartAnimation ();
		DrawBeforeAnim ();
	}

	public void DrawBeforeAnim()
	{
		if (drawBefore) {
			canvasGroup = gameObject.GetComponent<CanvasGroup> ();
			if (canvasGroup == null) {
				canvasGroup = gameObject.AddComponent<CanvasGroup> ();
			}
			canvasGroup.alpha = valueFrom;
		}
	}
		
	public override void ClearValues()
	{
		if(canvasGroup == null) return;

		currentAnimationTime = -delay;
		valueFrom = tempValueFrom;
		valueTo = tempValueTo;
		isReverseMoving = false;
		canvasGroup.alpha = initValue;
		DrawBeforeAnim ();
	}

	public override void OnAnimation()
	{
		if (currentAnimationTime < timeInSeconds) 
		{
			currentAnimationTime += Time.deltaTime;
			if (currentAnimationTime >= 0) 
			{
				float currentValue = Mathf.Lerp (valueFrom, valueTo, (currentAnimationTime / timeInSeconds));
				float t = currentAnimationTime / timeInSeconds;

				if (ease == UIAnimatorEase.LINEAR) {
					currentValue = Mathf.Lerp (valueFrom, valueTo, t);
				} else if (ease == UIAnimatorEase.EASEOUT) {
					t = Mathf.Sin (t  * Mathf.PI * 0.5f);
					currentValue = Mathf.Lerp (valueFrom, valueTo, t);
				} else if (ease == UIAnimatorEase.EASEIN) {
					t = 1f - Mathf.Cos (t * Mathf.PI * 0.5f);
					currentValue = Mathf.Lerp (valueFrom, valueTo, t);
				} 
				canvasGroup.alpha = currentValue;
			}
		} else {
			canvasGroup.alpha = valueTo;
			OnFinishAnimation ();
		}
	}
		
	public override void ReverseValues()
	{
		float tempFrom = valueFrom;
		valueFrom = valueTo;
		valueTo = tempFrom;
	}


}

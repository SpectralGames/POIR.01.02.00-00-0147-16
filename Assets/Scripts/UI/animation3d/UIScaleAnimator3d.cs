using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIScaleAnimator3d : UIAnimationEngine {

	public Vector3 valueFrom;
	public Vector3 valueTo;

	private Vector3 tempValueFrom;
	private Vector3 tempValueTo;
	private Vector3 initValue = Vector3.one;

	public override void PrepareAnimation()
	{
		tempValueFrom = valueFrom;
		tempValueTo = valueTo;
		initValue = transform.localScale;
		currentAnimationTime = -delay;
	} 
		
	public override void ClearValues()
	{
		valueFrom = tempValueFrom;
		valueTo = tempValueTo;
		isReverseMoving = false;
		transform.localScale = initValue;
	}
		
	public override void OnAnimation()
	{
		if (currentAnimationTime < timeInSeconds) {
			
			currentAnimationTime += Time.deltaTime;
			if (currentAnimationTime >= 0) {
				Vector3 currentValue = Vector3.zero;
				float t = currentAnimationTime / timeInSeconds;
				if (ease == UIAnimatorEase.LINEAR) {
					currentValue = Vector3.Lerp (valueFrom, valueTo, t);
				} else if (ease == UIAnimatorEase.EASEOUT) {
					t = Mathf.Sin (t * Mathf.PI * 0.5f);
					currentValue = Vector3.Lerp (valueFrom, valueTo, t);
				} else if (ease == UIAnimatorEase.EASEIN) {
					t = 1f - Mathf.Cos (t * Mathf.PI * 0.5f);
					currentValue = Vector3.Lerp (valueFrom, valueTo, t);
				} else if (ease == UIAnimatorEase.EASE_OUT_BACK) {
					float f = (1f - t);
					t = 1f - (f * f * f - f * Mathf.Sin(f * Mathf.PI));
					currentValue = valueFrom * (1f-t) + valueTo * t; //Vector3.Lerp(valueFrom, valueTo, t);
				}
				transform.localScale = currentValue;
			}
		} else {
			transform.localScale = valueTo;
			OnFinishAnimation ();
		}
	}
		
	public override void ReverseValues()
	{
		Vector3 tempFrom = valueFrom;
		valueFrom = valueTo;
		valueTo = tempFrom;
	}
		
}

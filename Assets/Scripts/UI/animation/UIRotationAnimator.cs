using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotationAnimator : UIAnimationEngine {

	public Vector3 valueFrom;
	public Vector3 valueTo;

	private Vector3 tempValueFrom;
	private Vector3 tempValueTo;
	private Quaternion initValue = Quaternion.identity;

	private RectTransform rectTransform;

	public override void PrepareAnimation()
	{
		rectTransform = GetComponent<RectTransform> ();
		tempValueFrom = valueFrom;
		tempValueTo = valueTo;
		initValue = rectTransform.localRotation;
		currentAnimationTime = -delay;
	} 

	public override void ClearValues()
	{
		if(rectTransform == null) return;

		valueFrom = tempValueFrom;
		valueTo = tempValueTo;
		isReverseMoving = false;
		rectTransform.localRotation = initValue;
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
					t = Mathf.Sin (t  * Mathf.PI * 0.5f);
					currentValue = Vector3.Lerp (valueFrom, valueTo, t);
				} else if (ease == UIAnimatorEase.EASEIN) {
					t = 1f - Mathf.Cos (t * Mathf.PI * 0.5f);
					currentValue = Vector3.Lerp (valueFrom, valueTo, t);
				} 
				rectTransform.localEulerAngles = currentValue;
			}
		} else {
			rectTransform.localEulerAngles = valueTo;
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

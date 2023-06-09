using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIImageFillAmountAnimator : UIAnimationEngine {

	[Header("Modifier Params")]
	public float valueFrom;
	public float valueTo;
	public bool drawBefore = true;

	private float tempValueFrom;
	private float tempValueTo;
	private float initValue = 0;
	private Image image;


	public override void PrepareAnimation()
	{
		image = GetComponent<Image> ();
		tempValueFrom = valueFrom;
		tempValueTo = valueTo;
		initValue = image.fillAmount;
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
			
			image.fillAmount = valueFrom;
		}
	}
		
	public override void ClearValues()
	{
		currentAnimationTime = -delay;
		valueFrom = tempValueFrom;
		valueTo = tempValueTo;
		isReverseMoving = false;
		image.fillAmount = initValue;
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
				image.fillAmount = currentValue;
			}
		} else {
			image.fillAmount = valueTo;
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

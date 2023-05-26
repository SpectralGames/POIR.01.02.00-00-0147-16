using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChartStatsController : MonoBehaviour 
{
	public Image chartBase;
	public Image chartUpgrade;
	public Image chartSimulation;

	//private Coroutine coroutine;

	public void SetData(float maxValue, float currentValue, float upgradedValue = 0f, bool isSimulated = false)
	{
		if(isSimulated == false)
		{
			upgradedValue = currentValue;
		}
		chartBase.fillAmount = Mathf.Clamp01(currentValue / maxValue);
		if(chartUpgrade != null) chartUpgrade.fillAmount = Mathf.Clamp01(upgradedValue / maxValue);
		this.CleanSimulation();
	}

	public void SetDataAndAnimate(float maxValue, float currentValue, float upgradedValue = 0f,  bool isSimulated = false)
	{
		if(isSimulated == false)
		{
			upgradedValue = currentValue;
		}
		StartCoroutine(OnAnimation(chartBase, chartBase.fillAmount, Mathf.Clamp01(currentValue/maxValue)));
		StartCoroutine(OnAnimation(chartUpgrade, chartUpgrade.fillAmount, Mathf.Clamp01(upgradedValue/maxValue)));
		this.CleanSimulation();
	}

	public void SetSimulation(float maxValue, float simulatedValue)
	{
		chartSimulation.fillAmount = Mathf.Clamp01(simulatedValue / maxValue);
	}

	public void CleanSimulation()
	{
		chartSimulation.fillAmount = 0f;
	}

	public void ShowUpgradeAnimation()
	{
		//coroutine = StartCoroutine (OnAnimation ());
		StartCoroutine (OnAnimation (chartUpgrade, chartUpgrade.fillAmount, chartSimulation.fillAmount));
	}

	IEnumerator OnAnimation(Image imageToAnimate, float animationFrom, float animationTo)
	{
		float currentAnimationTime = 0f;
		float totalAnimationTIme = .5f;

		while(currentAnimationTime < totalAnimationTIme)
		{
			currentAnimationTime += Time.deltaTime;
			float currentValue = Mathf.Lerp (animationFrom, animationTo, (currentAnimationTime / totalAnimationTIme));
											
			imageToAnimate.fillAmount = Mathf.Clamp01(currentValue);
			yield return null;
		}
		//StopCoroutine (coroutine);
	}
}

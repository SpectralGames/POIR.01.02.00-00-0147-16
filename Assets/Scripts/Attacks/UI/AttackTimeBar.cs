using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackTimeBar : MonoBehaviour {

	private Image imageBackground;
	private Image image;
	private static GameObject healthsParent;

	private float maxTimePoints;
	[HideInInspector] public float timePoints;
	public float TimePercent
	{
		get 
		{
			return timePoints / maxTimePoints;
		}
		set { timePoints = value; }//?
	}

	public void Init(float maxTime, Vector3 position)
	{
		maxTimePoints = maxTime;

		imageBackground = gameObject.GetComponent<Image>();
		image = gameObject.FindComponentInChildWithName<Image>("Time");

		healthsParent = GameObject.Find("WorldCanvas/Healths");
		this.gameObject.transform.SetParent(healthsParent.transform);
		this.transform.position = position;//Vector3.up*3f;
	}

	public void SetAlpha (float alpha)
	{
		image.canvasRenderer.SetAlpha(alpha);
		imageBackground.canvasRenderer.SetAlpha(alpha);
	}

	void Update()
	{
		this.transform.LookAt(Camera.main.transform.position);
		image.fillAmount = (1 - TimePercent);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandIndicator : MonoBehaviour {

	public Text tfValue;
	public Image filledImage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateValue(int trueValue, float valuePrc)
	{
		tfValue.text = trueValue.ToString ();
		filledImage.fillAmount = valuePrc;
	}
}

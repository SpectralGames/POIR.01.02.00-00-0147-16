using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuInfoWindow : MonoBehaviour 
{
	public Text descriptionText;
	public GameObject OKButton, CancelButton;
	private Vector3 okButtonInitPosition, initPosition;

	private System.Action okButtonCallback = null;
	private System.Action cancelButtonCallback = null;

	public static MainMenuInfoWindow Instance;
	// Use this for initialization
	void Awake () 
	{
		Instance = this;
		okButtonInitPosition = OKButton.transform.localPosition;
		initPosition = this.transform.position;

		this.gameObject.SetActive(false);
	}

	public void ShowInfoWindow(string descriptionText, System.Action okCallback, System.Action cancelCallback, bool hideCancelButton = false, EMainMenuState menuState = EMainMenuState.NONE)
	{
		this.ShowInfoWindow(descriptionText, okCallback, cancelCallback, hideCancelButton);

		float yAngle = 0f;
		if(menuState == EMainMenuState.SpellsLayer || menuState == EMainMenuState.SpellsLayerStats)
			yAngle = 60f;
		else if(menuState == EMainMenuState.SummonsLayer || menuState == EMainMenuState.SummonsLayerStats)
			yAngle = -60f;

		if(Mathf.Abs(yAngle) < 1f) //odsun w pozycji na wprost
		{
			this.transform.position = initPosition - this.transform.forward*2f;
		}else{
			this.transform.position = initPosition;
		}

		this.transform.localEulerAngles = new Vector3(0f, yAngle, 0f);
	}

	public void ShowInfoWindow(string descriptionText, System.Action okCallback, System.Action cancelCallback, bool hideCancelButton = false)
	{
		this.descriptionText.text = descriptionText;
		if(hideCancelButton)
		{
			CancelButton.gameObject.SetActive(false);
			OKButton.transform.localPosition = new Vector3(0f, OKButton.transform.localPosition.y, OKButton.transform.localPosition.z); //wysrodkuj ok
		}else{
			CancelButton.gameObject.SetActive(true);
			OKButton.transform.localPosition = okButtonInitPosition;
		}

		okButtonCallback = okCallback;
		cancelButtonCallback = cancelCallback;
		this.gameObject.SetActive(true);
	}

	public void HideInfoWindow()
	{
		this.gameObject.SetActive(false);
		okButtonCallback = null;
		cancelButtonCallback = null;
	}
	
	public void OnOkButtonClicked()
	{
		if(okButtonCallback != null)
			okButtonCallback.Invoke();
		
		this.HideInfoWindow();
	}

	public void OnCancelButtonClicked()
	{
		if(cancelButtonCallback != null)
			cancelButtonCallback.Invoke();
		
		this.HideInfoWindow();
	}
}

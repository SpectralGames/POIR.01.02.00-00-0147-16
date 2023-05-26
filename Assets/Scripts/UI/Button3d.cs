using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3d : MonoBehaviour {

	public float clickDistance = .02f;
  public float moveDistance = 0.04f;
    public UnityEvent touchOnEvent;
    public UnityEvent touchOffEvent;
    public UnityEvent clickEvent;

	private UnityIntEvent clickIntEvent;
	private int clickIntParam;
	//public UnityEvent<int> intClickEvent;

	private Button3dBody body;

	void Awake()
	{
		this.clickDistance = Mathf.Max(this.clickDistance, 0.02f); //min 2cm
		body = GetComponentInChildren<Button3dBody> ();
		body.OnSetButton3d (this);
	}

	public void OnButtonClick()
	{
		if(clickEvent != null)
			clickEvent.Invoke ();

		if (clickIntEvent != null)
			clickIntEvent.Invoke (clickIntParam);
	}

	public void EnableButton(bool enable)
	{
		if (enable)
			body.EnableButton ();
		else
			body.DisableButton ();
	}

	public void SetClickEvent(UnityIntEvent clickIntEvent, int param)
	{
		this.clickIntEvent = clickIntEvent;
		this.clickIntParam = param;
	}
}

[System.Serializable]
public class UnityIntEvent : UnityEvent<int> {}

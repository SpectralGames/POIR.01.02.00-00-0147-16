using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : MonoBehaviour {

	public float duration = 2;
	public Text textField;

	private float timeCounter = 0;
	private Transform pivotTransform;


	public static void CreateToast(string text)
	{
		ToastMessage toast = Instantiate (Resources.Load ("UI/ToastMessage") as GameObject).GetComponent<ToastMessage>();
		toast.SetText (text);
	}

	void Start () {
		pivotTransform = GameObject.FindGameObjectWithTag ("FloatingMenuPivot").transform;
	}

	public void SetText(string text)
	{
		textField.text = text;
	}
		

	void Update () 
	{
		timeCounter += Time.deltaTime;
		if (timeCounter > duration) {
			Destroy (gameObject);
		}

		this.transform.position = pivotTransform.position;
		this.transform.LookAt (new Vector3 (Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));
		this.transform.rotation *= Quaternion.Euler (new Vector3 (0f, 180f, 0f));
	}
}

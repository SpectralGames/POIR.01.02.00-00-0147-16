using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTouchSensor : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{

		if (other.tag.Equals ("ControllerTouchSensor")) {
			GameObject.FindGameObjectWithTag ("FloatingMenu").GetComponent<FloatingMenuCanvas> ().TogglePauseMenu ();
		}
	}

}

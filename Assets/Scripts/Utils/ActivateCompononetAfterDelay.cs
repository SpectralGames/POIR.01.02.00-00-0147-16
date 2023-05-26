using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCompononetAfterDelay : MonoBehaviour 
{
	public Collider colliderToActivate;
	public MonoBehaviour scriptToActivate;
	public float activationTime = 3f;
	// Use this for initialization
	void Start () 
	{
		Invoke("ActivateComponent", activationTime);
	}
	
	private void ActivateComponent()
	{
		if(colliderToActivate != null)
			colliderToActivate.enabled = true;
		if(scriptToActivate != null)
			this.scriptToActivate.enabled = true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNullParentAfterDelay : MonoBehaviour 
{
	public float delayTime = 5f;
	// Use this for initialization
	void Start () 
	{
		Invoke("DetachFromParent", delayTime);
	}
	
	private void DetachFromParent()
	{
		this.transform.SetParent(null);
	}
}

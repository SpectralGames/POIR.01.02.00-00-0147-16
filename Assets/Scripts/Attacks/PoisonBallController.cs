﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBallController : AttackBaseController
{
	RFX4_TransformMotion transformMotion;
	public GameObject[] objectsToDeactivateOnActivation;
	// Use this for initialization
	protected override void Awake () 
	{
		base.Awake();
		//transformMotion = this.GetComponentInChildren<RFX4_TransformMotion>();
		//transformMotion.CollisionEnter += this.CollisionEnter;
	}

	/*private void CollisionEnter(object collisionObject, RFX4_TransformMotion.RFX4_CollisionInfo collisionInfo)
	{
		if(isActive == false)
			this.OnActivate();
	}*/

	protected override void OnActivate ()
	{
		base.OnActivate ();
		foreach(GameObject obj in objectsToDeactivateOnActivation)
		{
			obj.SetActive(false);
		}
	}


}

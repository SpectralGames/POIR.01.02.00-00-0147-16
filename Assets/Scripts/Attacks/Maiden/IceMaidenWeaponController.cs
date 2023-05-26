using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMaidenWeaponController : MaidenWeaponBaseController 
{
	// Use this for initialization
	protected override void Awake () 
	{
		base.Awake();
		applyKickForceToEnemies = false;
	}

	public override void OnInit (Vector3 target, Vector3 targetNormal, int level = 0) 
	{
		base.OnInit(target, targetNormal, level);
		this.transform.rotation = Quaternion.identity;
	}
}

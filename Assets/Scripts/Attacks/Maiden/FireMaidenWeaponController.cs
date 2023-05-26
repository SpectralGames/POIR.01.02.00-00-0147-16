using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMaidenWeaponController : MaidenWeaponBaseController
{
	public override void OnInit (Vector3 target, Vector3 targetNormal, int level = 0) 
	{
		base.OnInit<CapsuleCollider>(target, targetNormal, level);
	}

	public override void OnSetValues(int level)
	{
		base.OnSetValues(level);
		CapsuleCollider myCapsuleCollider = myCollider as CapsuleCollider;
		myCapsuleCollider.radius = transform.InverseTransformVector(this.transform.forward * attackRadius).magnitude; //przelicz wymiary z world space na local space, kompensacja mniejszej skali
		myCapsuleCollider.height = transform.InverseTransformVector(this.transform.forward * XMLItemsReader.GetCustomStatValue(weaponType.ToString(), level, "capsule_height") ).magnitude;
		myCapsuleCollider.center = new Vector3(0f, 0f, myCapsuleCollider.height * 0.5f);
		myCapsuleCollider.direction = 2;
	}
}

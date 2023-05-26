using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public interface ICollisionTrigger
{
	void OnTriggerEnter (Collider other);
	void OnTriggerExit (Collider other);
}*/

public class MovingObjectColliderProxy : MonoBehaviour 
{
	//protected ICollisionTrigger parentBaseController;
	protected System.Action<Collider> onTriggerEnterAction, onTriggerExitAction;

	public virtual void OnInit(System.Action<Collider> triggerEnter, System.Action<Collider> triggerExit) //ICollisionTrigger baseController)
	{
		this.onTriggerEnterAction += triggerEnter;
		this.onTriggerExitAction += triggerExit;
		//this.parentBaseController = baseController;
	}

	public virtual void OnInit(System.Action<Collider> triggerEnter)
	{
		this.onTriggerEnterAction += triggerEnter;
	}

	/*void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		float collisionDot = Vector3.Dot(this.transform.forward, contact.normal);
		float collisionForce = Mathf.Abs(collisionDot)*(this.GetComponent<Rigidbody>().velocity.magnitude);
		if(collisionForce > 2f)
		{
			
		}
	}*/

	void OnTriggerEnter(Collider other) 
	{
        DamageReceiver.Projectile = this.transform;
        onTriggerEnterAction?.Invoke(other);
	}

	void OnTriggerExit(Collider other)
	{
		onTriggerExitAction?.Invoke(other);
	}
}

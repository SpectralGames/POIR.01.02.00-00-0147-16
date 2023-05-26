using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObjectColliderProxy : MonoBehaviour
{
	protected DestroyableObject parentDestroyableObject;
	protected float currentHealth;
	protected bool isDestroyed;

	public virtual void OnInit(DestroyableObject destroyableObject, float health)
	{
		this.parentDestroyableObject = destroyableObject;
		currentHealth = health;
		isDestroyed = false;
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

	public virtual void OnTakeDamage(Vector3 impactPosition, Vector3 impactDirection, float impactForce, float impactRadius)
	{
		currentHealth -= impactForce;
		if(currentHealth <= 0)
		{
			isDestroyed = true;
			parentDestroyableObject.OnIntactObjectCracked(impactPosition, impactDirection, impactForce, impactRadius);
		}
	}

	public bool IsDestroyed()
	{
		return this.isDestroyed;
	}
}

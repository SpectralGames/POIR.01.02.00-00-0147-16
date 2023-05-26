using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDestructor : MonoBehaviour {


	public float attackRate = 2f;

	public float attackPowerStaticObjects = 10f;
	public float attackPowerDestroyable = 5f;

	private float timeCounter;

	private SphereCollider attackCollider;

	// Use this for initialization
	void Awake()
	{
		attackCollider = GetComponent<SphereCollider> ();
		attackCollider.enabled = false;
	}
		
	void Update () {
		
		timeCounter += Time.deltaTime;
		if (timeCounter >= attackRate) {
			timeCounter = 0;
			EnableCollider ();
		}
	}

	private void EnableCollider()
	{
		attackCollider.enabled = true;
		Invoke("OnDisableCollider", 0.25f);
	}

	private void OnDisableCollider()
	{
		attackCollider.enabled = false;
	}
		
	void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<DestroyableObjectColliderProxy>() != null)
		{
			other.GetComponent<DestroyableObjectColliderProxy>().OnTakeDamage(transform.position, GetCollisionDirection(other.transform.position) , attackPowerDestroyable, attackCollider.radius);
		}
		if(other.GetComponent<Rigidbody>() != null)
		{
			other.GetComponent<Rigidbody>().AddForce(GetCollisionDirection(other.transform.position) * attackPowerStaticObjects);
		}
 		
	}

	private Vector3 GetCollisionDirection(Vector3 positionToCheck)
	{
		Vector3 distanceVector = positionToCheck - transform.position;
		float distance = distanceVector.magnitude;
		if (distance == 0)
			return Vector3.zero;
		else
			return distanceVector / distance;
	}

}

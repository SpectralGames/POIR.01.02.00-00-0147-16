using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour 
{
	public int attackPower;
	public bool stopTargetAfterHit = false;
	public GameObject selectionObject;
	public Vector3 localSnapPosition, localSnapRotation;
	public float throwOffset = 1.7f;

	private bool isTaken = false;
	private bool isSelected = false;

	private Rigidbody rigidBodyComponent;

	// Use this for initialization
	void Start () {
		ObjectPool.Instance.throwableList.Add (this);
		rigidBodyComponent = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	public Rigidbody GetRigidBody()
	{
		return rigidBodyComponent;
	}
		
	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
			other.gameObject.GetComponent<Enemy>().TakeDamage(attackPower, stopTargetAfterHit);
		}
	}

	public void SetIsTaken(bool taken)
	{
		isTaken = taken;

        rigidBodyComponent.isKinematic = false;

    }

	public void OnThrowObject(Vector3 velocity, Vector3 angularVelocity)
	{
		rigidBodyComponent.useGravity = true;
		
		if(velocity.magnitude > 0.01f)
			rigidBodyComponent.velocity = velocity;
		//if(angularVelocity.magnitude > 0.01f)
			//rigidBodyComponent.angularVelocity = angularVelocity;

		if (!this.GetComponent<MagicGrenadeController>().Equals(null))
		{
			this.GetComponent<MagicGrenadeController>().OnThrowObject();
		}
			
	}

	public bool GetIsTaken()
	{
		return isTaken;
	}

	public void SetIsSelected(bool selected)
	{
		isSelected = selected;
	}

	public bool GetIsSelected()
	{
		return isSelected;
	}

	public void SelectObject(bool selected)
	{
		isSelected = selected;
		selectionObject.SetActive (selected);
	}
}

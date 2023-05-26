using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBoost : EnvironmentBaseObject 
{
	private List<Rigidbody> rigidbodiesInRange;
	private Vector3 forceDirection;

	// Use this for initialization
	void Awake ()
	{
		this.rigidbodiesInRange = new List<Rigidbody>();
		this.forceDirection = this.transform.GetChild(0).transform.forward;

	}

	void Start()
	{
		init ();
	}
	protected override void Work()
	{
		for(int i=rigidbodiesInRange.Count-1; i>=0; i--)
		{
			if(this.rigidbodiesInRange[i] != null)
				this.rigidbodiesInRange[i].AddForce(forceDirection*forceStrength, ForceMode.Force);
			else
				this.rigidbodiesInRange.RemoveAt(i);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		Rigidbody otherRigidbody = other.GetComponentInParent<Rigidbody>();
		if(otherRigidbody != null && this.rigidbodiesInRange.Contains(otherRigidbody) == false)
		{
			this.rigidbodiesInRange.Add(otherRigidbody);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Rigidbody otherRigidbody = other.GetComponentInParent<Rigidbody>();
		if(otherRigidbody != null)
		{
			if(this.rigidbodiesInRange.Contains(otherRigidbody))
				this.rigidbodiesInRange.Remove(otherRigidbody);
		}
	}


}

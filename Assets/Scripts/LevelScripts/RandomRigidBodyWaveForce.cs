using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRigidBodyWaveForce : MonoBehaviour 
{
	private Rigidbody myRigidbody;
	public float waveFrequency = 1f;
	public float waveAmplitude = 1f;
	public Vector3 localForceForward = Vector3.forward;

	void Awake () 
	{
		myRigidbody = this.GetComponent<Rigidbody>();	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		myRigidbody.AddForce(this.transform.TransformDirection(localForceForward) * (Mathf.PerlinNoise(0f, Time.time * waveFrequency) - 0.5f) * Time.deltaTime * 60f * waveAmplitude);
	}
}

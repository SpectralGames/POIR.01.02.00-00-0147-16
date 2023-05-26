using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletBallController : AttackBaseController 
{
	RFX4_ParticleCollisionHandler particleCollisionHandler;
	public ParticleSystem[] particlesToPauseOnActivation;
	public ParticleSystem[] trailParticles;
	// Use this for initialization
	protected override void Awake () 
	{
		base.Awake();
		particleCollisionHandler = this.GetComponentInChildren<RFX4_ParticleCollisionHandler>();
		//particleCollisionHandler.OnParticleCollisionEvent += this.ParticleCollision;
	}

	private void ParticleCollision(Vector3 collisionPosition)
	{
		this.transform.position = collisionPosition;
		if(isActive == false)
			this.OnActivate();
	}
		
	protected override void OnActivate ()
	{
		base.OnActivate ();
		foreach(ParticleSystem particle in trailParticles)
		{
			ParticleSystem.MainModule mainModule = particle.main;
			mainModule.gravityModifier = 0f;
		}
		foreach(ParticleSystem particle in particlesToPauseOnActivation)
		{
			ParticleSystem.EmissionModule emission = particle.emission;
			emission.enabled = false;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBaseObject : MonoBehaviour {

	public ParticleSystem particles;

	public AnimationDriver animationDriver;

	public float forceStrength = 1f;
	public float forceDuration = -1f;
	public float forceInterval = -1f;
	public float forceDamage = .3f;

	private bool isWorking;
	private bool isStopping;
	private float animResumeSpeed = 1f;
	private float animSpeed = 1f;
	private float boostPower = 1f;
	private float tempBoostPower = 0f;
	private float currentTime = 0.0f;
	private float forceTimer;
	// force 
	private float initForceStrength;
	private float currentStrengthValue = 0;
	// damage 
	private float initForceDamage;
	private float currentDamageValue = 0;

	private float timeOffset;
	// animation
	private float initAnimationSpeed;
	private float currentAnimationSpeed;

	public List<ParticleHolder> listParticlesHolder = new List<ParticleHolder>();

	protected void init()
	{
		isWorking = true;

		initForceStrength = forceStrength;
		currentStrengthValue = forceStrength;

		initForceDamage = forceDamage;
		currentDamageValue = forceDamage;

		if (particles != null) {
			foreach (ParticleSystem particleSystem in particles.GetComponentsInChildren<ParticleSystem>()) {
				ParticleHolder holder = new ParticleHolder ();
				holder.particles = particleSystem;
				holder.particleMainModule = particleSystem.main;
				holder.initParticleStartSize = holder.particleMainModule.startSizeMultiplier;
				holder.currentParticleStartSize = holder.initParticleStartSize;
				holder.particlesArray = new ParticleSystem.Particle[holder.particleMainModule.maxParticles]; 
				listParticlesHolder.Add (holder);
			}
		}

		if (animationDriver != null) {
			initAnimationSpeed = animationDriver.animSpeed;
			currentAnimationSpeed = initAnimationSpeed;
		}
	}
		
	void FixedUpdate () 
	{
		if(forceDuration > -0.1f && forceInterval > -0.1f)
		{
			forceTimer += Time.deltaTime;
			if(isWorking)
			{
				if(forceTimer > forceDuration)
				{ 
					isWorking = false; forceTimer = 0f; 
					if(particles != null) particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
			}else{
				if(forceTimer > forceInterval)
				{ 
					isWorking = true; forceTimer = 0f; 
					if(particles != null) particles.Play(true);
				}
			}
		}
		if(isWorking)
		{
			Work ();
		}
	}
	protected virtual void Work()	{}

	// set state to stop/resume object, reduce particles and force (wind, fire etc)
	public void OnChangeState(bool isStopping, float power)
	{
		this.isStopping = isStopping;
	

		if (this.isStopping)
		{
			boostPower += power;
			tempBoostPower = boostPower;
			animSpeed = 30 / boostPower;
		}
		else 
		{
			animSpeed=  30 / tempBoostPower * 1.5f;
			boostPower = 0;
			timeOffset = 1.5f;
		}
			
		currentStrengthValue = forceStrength;
		currentDamageValue = forceDamage;

		if (particles != null) {
			foreach(ParticleHolder particleHolder in listParticlesHolder)
				particleHolder.currentParticleStartSize  = particleHolder.particleMainModule.startSizeMultiplier;
		}

		if (animationDriver != null)
			currentAnimationSpeed = animationDriver.GetAnimationSpeed ();

		currentTime = 0;
	}
	//  reduce particles and force (wind, fire etc)
	void Update()
	{
		if (isStopping) {  // stop smoothly object
			forceStrength =  Mathf.Lerp (currentStrengthValue, 0, (currentTime / animSpeed));
			forceDamage =  Mathf.Lerp (currentDamageValue, 0, (currentTime / animSpeed));
			FadeOutParticles ();

			if (animationDriver != null) {
				animationDriver.SetAnimationSpeed(Mathf.Lerp (currentAnimationSpeed, 0, (currentTime / animSpeed)));
			}
		} 
		else {  // resume smoothly object
			forceStrength = Mathf.Lerp (currentStrengthValue, initForceStrength, (currentTime / animSpeed));
			forceDamage = Mathf.Lerp (currentDamageValue, initForceDamage, (currentTime / animSpeed));

			FadeInParticles ();
			if (animationDriver != null) {
				animationDriver.SetAnimationSpeed(Mathf.Lerp (currentAnimationSpeed, initAnimationSpeed, (currentTime / animSpeed)));
			}
		}
	
		// timeoffset ustawiony jest by wznowienie dzialania elementu nastapila po okreslonej chwily bo zatrzymaniu boosta
		if (timeOffset <= 0) {
			currentTime += Time.deltaTime;
		} else {
			timeOffset -= Time.deltaTime;
		}
	}

	private void FadeOutParticles()
	{
		if (particles != null) {
			foreach (ParticleHolder particleHolder in listParticlesHolder) {
				particleHolder.particleMainModule.startSizeMultiplier = Mathf.Lerp (particleHolder.currentParticleStartSize, 0, (currentTime / animSpeed));			
				int numOfParticles =	particleHolder.particles.GetParticles (particleHolder.particlesArray);
				for (int i = 0; i < numOfParticles; i++) {
					particleHolder.particlesArray [i].remainingLifetime = Mathf.Lerp (particleHolder.particlesArray [i].remainingLifetime, 0, (currentTime / (animSpeed * 20)));
				}
				particleHolder.particles.SetParticles (particleHolder.particlesArray, numOfParticles);
			}
		}
	}

	private void FadeInParticles()
	{
		if (particles != null) {
			foreach (ParticleHolder particleHolder in listParticlesHolder)
				particleHolder.particleMainModule.startSizeMultiplier = Mathf.Lerp (particleHolder.currentParticleStartSize, particleHolder.initParticleStartSize, (currentTime / animSpeed));
		}
	}

}
public class ParticleHolder
{
	// particles
	public ParticleSystem particles;
	public float currentParticleStartSize = 0;
	public float initParticleStartSize = 0;
	public ParticleSystem.MainModule particleMainModule;
	public ParticleSystem.Particle[] particlesArray;
}

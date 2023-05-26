using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MaidenLightningController : MaidenWeaponBaseController 
{
	public GameObject[] lightningFaggots;
	private Vector3[] lightningFaggotsInitForward;
	private Projector[] decalProjectors;

	public override void OnInit (Vector3 target, Vector3 targetNormal, int level) 
	{
		base.OnInit(target, targetNormal, level);
		this.transform.rotation = Quaternion.identity;

		lightningFaggotsInitForward = new Vector3[lightningFaggots.Length];
		for(int i=0; i<lightningFaggots.Length; i++)
		{
			lightningFaggotsInitForward[i] = lightningFaggots[i].transform.forward;
		}
	}

	protected override void Awake () 
	{
		base.Awake();
	}

	protected override void Update () 
	{
		base.Update();

		if(isActive)
		{
			for(int i=enemiesInRange.Count-1; i>=0; i--)
			{
				if(enemiesInRange[i] == null)
					enemiesInRange.RemoveAt(i);
			}

			enemiesInRange.OrderBy(x => Vector3.Distance(x.transform.position, this.transform.position));
			for(int i=0; i<lightningFaggots.Length; i++)
			{
				if(i<enemiesInRange.Count && enemiesInRange[i] != null)
				{
					lightningFaggots[i].transform.forward = (enemiesInRange[i].transform.position + Vector3.up * enemiesInRange[i].GetAIHeight()*0.5f) - lightningFaggots[i].transform.position;
				}else{
					lightningFaggots[i].transform.forward = lightningFaggotsInitForward[i];
				}
			}
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		ParticleSystem[] particles = this.GetComponentsInChildren<ParticleSystem>(false);
		foreach(ParticleSystem particle in particles)
		{
			ParticleSystem.EmissionModule emissionModule = particle.emission;
			emissionModule.enabled = false;
		}

		decalProjectors = new Projector[lightningFaggots.Length];
		for(int i=0; i<lightningFaggots.Length; i++)
		{
			decalProjectors[i] = lightningFaggots[i].GetComponentInChildren<Projector>();
		}

		StartCoroutine(FadeAwayLightsAndSounds());
	}

	IEnumerator FadeAwayLightsAndSounds()
	{
		Light[] lights = this.GetComponentsInChildren<Light>();
		foreach(Light light in lights)
		{
			light.GetComponent<RFX4_LightCurves>().enabled = false;
		}
		AudioSource[] audios = this.GetComponentsInChildren<AudioSource>();

		float currentTime = 0f;
		float fadeTime = 0.4f;
		while(currentTime < fadeTime)
		{
			float factor = 1f - Mathf.Clamp01(currentTime/fadeTime);

			foreach(Light light in lights)
			{
				light.intensity *= factor;
			}
			foreach(AudioSource audio in audios)
			{
				audio.volume *= factor;
			}
			foreach(Projector projector in decalProjectors)
			{
				projector.orthographicSize *= factor;
			}

			currentTime += Time.deltaTime;
			yield return null;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParticlesContainer : MonoBehaviour 
{
	[SerializeField]
	private GameObject deathParticles, burnSideEffect, frozenSideEffect, teleportParticles;
	// Use this for initialization
	void Start () {
		
	}
	
	public GameObject GetDeathParticles()
	{
		return deathParticles;
	}

	public GameObject GetBurnSideEffect()
	{
		return burnSideEffect;
	}

	public GameObject GetFrozenSideEffect()
	{
		return frozenSideEffect;
	}

	public GameObject GetTeleportEffect()
	{
		return teleportParticles;
	}
}

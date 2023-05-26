using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeShelfController : MonoBehaviour 
{
	public GameObject[] grenadePivots;
	public GameObject grenadePrefab;
	// Use this for initialization
	void Awake () 
	{
		this.SetupGrenades();
	}

	private void SetupGrenades()
	{
		for(int i=0; i<grenadePivots.Length; i++)
		{
			GameObject grenadeInstance = GameObject.Instantiate(grenadePrefab, grenadePivots[i].transform.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), null);
			//grenadeInstance.transform.SetParent(this.transform);
		}
	}

}

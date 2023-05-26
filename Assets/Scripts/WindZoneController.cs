using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneController : MonoBehaviour 
{
	private WindZone windZone;
	private float windZoneWindMain;
	private float directionChangeFrequency, directionChangeTimer;
	// Use this for initialization
	void Awake () 
	{
		windZone = this.GetComponent<WindZone>();
		windZoneWindMain = windZone.windMain;
		windZone.windMain = 0f;

		directionChangeFrequency = directionChangeTimer = 15f;
	}

	void Start()
	{
		windZone.windMain = windZoneWindMain;
	}
		
	void Update()
	{
		directionChangeTimer += Time.deltaTime;
		if(directionChangeTimer > directionChangeFrequency)
		{
			directionChangeTimer = 0f;
			this.transform.rotation = Quaternion.Euler(new Vector3(0f, this.transform.rotation.eulerAngles.y + Random.Range(-50f, 50f), 0f));
		}
	}
}

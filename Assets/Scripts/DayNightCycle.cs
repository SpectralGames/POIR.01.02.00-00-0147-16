using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour 
{
	public Light directionalLight;
	public Gradient ambientGradient, directionalGradient, fogGradient;
	public AnimationCurve factorCurve;
	public MeshRenderer skyMesh;
	private Material skyMaterial;
	private float ambientCheckFrequency, currentAmbientCheckTimer;
	private float currentTime;
	private float directionalLightInitValue, ambientInitValue;
	//private float directionalLightIndirectInitValue;
	private float initGiUpdateTreshold;
	//private int lastAmbientChangeHour;
	private Quaternion noonRotation;
	private Material skyboxMaterial;
	private Color initAmbientSkyboxMaterialEmission;
	private ReflectionProbe[] reflectionProbes;
	private Vector3 initDirectionalUp, initDirectionalPosition;
	private float circleMovementRadius, circleMovementAngle, circleMovementSpeed;
	private TorchController[] torchControllers;

	// Use this for initialization
	void Awake () 
	{
		ambientCheckFrequency = 3f;
		currentAmbientCheckTimer = ambientCheckFrequency + 0.1f;
		currentTime = System.DateTime.Now.Hour;
		//lastAmbientChangeHour = System.DateTime.Now.Hour + 2; //przesun czas, aby pierwszy raz ustawic ambient
		noonRotation = Quaternion.Euler(60f, 180f, 0f); //directionalLight.transform.rotation; //Quaternion.Euler(35f, 0f, 0f);
		Quaternion tempDirectionalRotation = directionalLight.transform.rotation;
		directionalLight.transform.rotation = noonRotation;
		initDirectionalUp = directionalLight.transform.up;
		initDirectionalPosition = directionalLight.transform.position;
		directionalLight.transform.rotation = tempDirectionalRotation;
		circleMovementAngle = 0f;
		circleMovementRadius = 30f;
		circleMovementSpeed = 5f;

		skyboxMaterial = new Material(RenderSettings.skybox); //zrob backup, zeby nie nadpisywal materialu z projektu
		RenderSettings.skybox = skyboxMaterial;
		directionalLightInitValue = directionalLight.intensity;
		//directionalLightIndirectInitValue = directionalLight.bounceIntensity;

		ambientInitValue = RenderSettings.ambientIntensity;
		reflectionProbes = GameObject.FindObjectsOfType<ReflectionProbe>();

		// // Sky Mesh
		skyMaterial = Material.Instantiate(skyMesh.material);
		skyMesh.material = skyMaterial;
		initAmbientSkyboxMaterialEmission = skyMaterial.GetColor("_EmissionColor");

		// Torch Controllers
		torchControllers = GameObject.FindObjectsOfType<TorchController>();

		initGiUpdateTreshold = DynamicGI.updateThreshold;
		DynamicGI.synchronousMode = true;
		DynamicGI.UpdateEnvironment();
	}
	
	void Update()
	{
		//currentTime = (currentTime+Time.deltaTime*0.03f)%24f; //System.DateTime.Now.Hour + System.DateTime.Now.Minute/60f;
		//this.UpdateTimeOfDay();
	}

	public void SetTimeOfDay(float time)
	{
		StartCoroutine(SetTimeOfDayAsynchronous(time));
	}

	IEnumerator SetTimeOfDayAsynchronous(float time)
	{
		currentTime = time;
		this.UpdateTimeOfDay();
	
		yield return new WaitForSeconds(3f);
		DynamicGI.synchronousMode = false;
		DynamicGI.updateThreshold = initGiUpdateTreshold * 2f;
	}

	void UpdateTimeOfDay () 
	{
		float angleFactor;
		if(currentTime > 18f) 
		{
			angleFactor = 12f-(currentTime-12f);
		}else if(currentTime < 6f)
		{
			angleFactor = (12f-currentTime)-12f;
		}else{
			angleFactor = (currentTime-12f);
		}

		float directionalIntensityFactor = (12f-Mathf.Abs(currentTime-12f))/12f; // 12 godzina - 1f, 24 godzina - 0f  /// 0f-1f
		float curveCurrentFactor = factorCurve.Evaluate(directionalIntensityFactor);
		//Debug.Log(currentTime + " factor " + curveCurrentFactor);
		//Directional Light
		directionalLight.transform.rotation = noonRotation * Quaternion.AngleAxis(angleFactor*13f, Vector3.up); //Quaternion.AngleAxis(angleFactor*15f, new Vector3(0f, 0.5f, 0.5f) ); //ustaw kat swiatla
		directionalLight.intensity = Mathf.Clamp(curveCurrentFactor*directionalLightInitValue, 0.2f, directionalLightInitValue);
		directionalLight.color = directionalGradient.Evaluate(curveCurrentFactor);
		directionalLight.shadowStrength = Mathf.Clamp(curveCurrentFactor*1.4f, 0.3f, 1f);
		//Circle Movement //
		circleMovementAngle = Mathf.Repeat(circleMovementAngle + circleMovementSpeed * Time.deltaTime, 360f);
		directionalLight.transform.position = initDirectionalPosition + new Vector3(circleMovementRadius * Mathf.Cos(Mathf.Deg2Rad * circleMovementAngle), 0f, circleMovementRadius * Mathf.Sin(Mathf.Deg2Rad * circleMovementAngle));

		//Scene Ambient
		currentAmbientCheckTimer += Time.deltaTime;
		if(currentAmbientCheckTimer >= ambientCheckFrequency)
		{
			currentAmbientCheckTimer = 0f;
			this.ChangeAmbient(curveCurrentFactor);
		}
	}

	public void ChangeAmbient(float intensityFactor = -1f)
	{
		/*if(Mathf.FloorToInt(currentTime) == lastAmbientChangeHour || directionalFactor < -0.5f) //zmieniaj co godzine
		{
			return;
		}*/

		RenderSettings.ambientIntensity = Mathf.Lerp(0.5f, 1f, intensityFactor*intensityFactor) * ambientInitValue;
		RenderSettings.reflectionIntensity = Mathf.Clamp(intensityFactor, 0.1f, 1f);
		skyboxMaterial.SetColor("_Tint", ambientGradient.Evaluate(intensityFactor));

		skyMaterial.SetColor("_EmissionColor", initAmbientSkyboxMaterialEmission * Mathf.Clamp(intensityFactor, 0.6f, 1f));
		skyMaterial.SetFloat("_DayNightBlend", 1f-intensityFactor);
		skyMaterial.SetVector("_SunForwardVector", new Vector4(directionalLight.transform.forward.x, directionalLight.transform.forward.y, directionalLight.transform.forward.z, 1f));

		RenderSettings.fogColor = fogGradient.Evaluate(intensityFactor);

		//lastAmbientChangeHour = Mathf.FloorToInt(currentTime);

		//Reflection Probes
		this.ChangeReflectionProbesIntensity(intensityFactor);

		//Torches
		this.ChangeTorches(intensityFactor);

		DynamicGI.UpdateEnvironment();
	}

	private void ChangeReflectionProbesIntensity(float intensityFactor)
	{
		foreach(ReflectionProbe probe in reflectionProbes)
		{
			probe.intensity = Mathf.Clamp(intensityFactor, 0.1f, 1f);
		}
	}

	private void ChangeTorches(float intensityFactor)
	{
		foreach(TorchController torch in torchControllers)
		{
			torch.OnUpdateTorchIntensity(1f-intensityFactor);
		}
	}
		
}

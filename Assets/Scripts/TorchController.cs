using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class OnTrochStatusChange : UnityEvent<bool> { }

public class TorchController : MonoBehaviour
{
    public Transform particleParent;
    public Light torchLight;
    public Gradient torchGradient;
    public AnimationCurve torchIntensity;
    public float animSpeed;

    public bool isTorchBurning = false;
    public OnTrochStatusChange OnTrochStatusChange = new OnTrochStatusChange();
    private bool isAnimatingForward;
    private float currentAnimTime;
    private float initIntensity;
    private float torchIntensityCutoff;
    private Vector3 initTorchLightPosition;

    // Use this for initialization
    void Awake()
    {
        currentAnimTime = UnityEngine.Random.Range(0f, 1f);
        initIntensity = torchLight.intensity;
        isAnimatingForward = UnityEngine.Random.Range(0, 2) == 0 ? false : true;

        torchIntensityCutoff = UnityEngine.Random.Range(0.45f, 0.55f);
        animSpeed += UnityEngine.Random.Range(-0.02f, 0.02f);

        initTorchLightPosition = torchLight.transform.position;

        this.OnSetTorchLight(isTorchBurning);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTorchBurning)
            this.AnimateTorchLight();
    }

    public void OnUpdateTorchIntensity(float torchIntensityFactor)
    {
        bool torchShouldBeBurning = torchIntensityFactor < torchIntensityCutoff ? false : true;
        if (isTorchBurning != torchShouldBeBurning)
            this.OnSetTorchLight(torchShouldBeBurning);
    }

    private void OnSetTorchLight(bool isBurning)
    {
        OnTrochStatusChange.Invoke(isTorchBurning = isBurning);
        for (int i = 0; i < particleParent.childCount; i++)
        {
            ParticleSystem particle = particleParent.GetChild(i).GetComponent<ParticleSystem>();
            ParticleSystem.EmissionModule emissionModule = particle.emission;
            emissionModule.enabled = isTorchBurning;
        }
        torchLight.enabled = isBurning;
    }


    private void AnimateTorchLight()
    {
        currentAnimTime += animSpeed * Time.deltaTime * (isAnimatingForward ? 1f : -1f);
        //if(currentAnimTime < 0f || currentAnimTime > 1f)
        //{
        //	isAnimatingForward = Random.Range(0, 2) == 0 ? false : true;
        //currentAnimTime = currentAnimTime < 0f ? 1f : 0f;
        //}
        currentAnimTime = Mathf.Repeat(currentAnimTime, 1f);
        //if(this.gameObject.name == "PochodniaScianaPrefab")
        //Debug.Log(currentAnimTime);

        Color currentColor = torchGradient.Evaluate(currentAnimTime);
        float currentIntensity = torchIntensity.Evaluate(currentAnimTime);

        torchLight.color = currentColor;
        torchLight.intensity = currentIntensity * initIntensity;
        torchLight.transform.position = initTorchLightPosition + new Vector3((Mathf.PerlinNoise(Time.timeSinceLevelLoad, 0f) - 0.5f) * 0.2f, Mathf.Sin(Time.timeSinceLevelLoad) * 0.07f, (Mathf.PerlinNoise(0f, Time.timeSinceLevelLoad) - 0.5f) * 0.2f);
    }
}

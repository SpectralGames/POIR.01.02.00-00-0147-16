using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StatusBar : MonoBehaviour // StatusBar? // podzielic osobno na level text i health
{
	private AI aiObject;
    private float maxHealthPoints;
    [HideInInspector] public float healthPoints;
	private float maxTimePoints;
	[HideInInspector] public float timePoints;
    
    private float spawnHeight;
	private bool showTimeBar = true;
	private bool alwaysVisible = false;

    private Camera mainCamera;


    private HealthBar health;
    private LevelBar level;
    private TimeSpawnBar timeSpawnBar;

    public float HealthPercent
    {
        get 
        {
            if (health.IsVisible) return healthPoints / maxHealthPoints;
            else return -1f;
        }
        set { healthPoints = value; }//?
    }

	public float TimePercent
	{
		get 
		{
			if (showTimeBar) return timePoints / maxTimePoints;
			else return -1f;
		}
		set { timePoints = value; }//?
	}

    private static GameObject healthsParent;

	public void Init (AI aiObject, float hp, float spawnHeight, bool alwaysVisible = false)
    {
		this.aiObject = aiObject;
        this.spawnHeight = spawnHeight;
		this.alwaysVisible = alwaysVisible;
        maxHealthPoints = hp;
        healthPoints = maxHealthPoints;

        mainCamera = Camera.main;
        healthsParent = GameObject.Find("WorldCanvas/Healths");

        OnHealthSpawn();

		aiObject.OnDie += OnDie; // potrzebne? jesli  i tak w update wywoluje ta akcje
		aiObject.OnDamageRecorded += ShowStatusBar;
    }
		
	public void EnableTimeBar(float maxTime)
	{
		maxTimePoints = maxTime;

        timeSpawnBar.ShowElement();
	}

    private void OnHealthSpawn()
    {
		this.gameObject.name = aiObject.name + "_health";

        health = this.gameObject.FindComponentInChild<HealthBar>();
        health.Init(aiObject, this.gameObject);

        level = this.gameObject.FindComponentInChild<LevelBar>();
        level.Init(aiObject, this.gameObject);

        timeSpawnBar = this.gameObject.FindComponentInChild<TimeSpawnBar>();
        timeSpawnBar.Init(aiObject, this.gameObject);

        this.gameObject.transform.SetParent(healthsParent.transform);

		if (alwaysVisible) {

			ShowStatusBar ();

		} else {
			HideStatusBar ();
		}
    }
      
    public virtual void Tick ()
    {
		if (aiObject.isAlive)
        {
            float currentAlpha = health.GetAlpha();// healthImage.canvasRenderer.GetAlpha();

            if (HealthPercent < 0)
            {
                if (currentAlpha > 0.05f)
                {
                    health.SetAlpha(Mathf.Max(0f, currentAlpha - 5f * Time.deltaTime));
                    level.SetAlpha(Mathf.Max(0f, currentAlpha - 7f * Time.deltaTime));
                }
                //else
                //    healthImageBackground.gameObject.SetActive(false);
            }
            else
            {
                if (currentAlpha < 0.95f)
                {
                    health.ShowElement();
                    health.SetAlpha(1f);
                    level.SetAlpha(1f);
                }
                health.Fill(HealthPercent);
                //ustaw nad postacia i patrz w kamere
				this.transform.position = aiObject.transform.position + Vector3.up * spawnHeight;//Vector3.up*3f;
                this.transform.LookAt(mainCamera.transform.position);
                //ustaw kolor
                health.Color(new Color(0.5f + 0.5f * (1f - HealthPercent), HealthPercent, 0.07f));
            }

            float currentTimeAlpha = timeSpawnBar.GetAlpha();// timeImage.canvasRenderer.GetAlpha();
			if (TimePercent < 0)
			{
                if (currentTimeAlpha > 0.05f)
                {
                    timeSpawnBar.SetAlpha(Mathf.Max(0f, currentTimeAlpha - 5f * Time.deltaTime));
                }
                else
                    timeSpawnBar.HideBackground();
				//else
				//	timeImageBackground.gameObject.SetActive(false);
			}
			else
			{
				if (currentTimeAlpha < 0.95f)
				{
                    timeSpawnBar.ShowElement();
                    timeSpawnBar.SetAlpha(1f);

				}
                timeSpawnBar.Fill(1 - TimePercent);
			}
        }
    }

    public void ShowStatusBar ()
    {
        if (healthPoints > 0f)
		{
            level.ShowElement();
            health.ShowElement();

			if (alwaysVisible == false) {
				CancelInvoke ("HideStatusBar");
				Invoke ("HideStatusBar", 1f);
			}
        }
    }

    private void HideStatusBar ()
    {
        level.HideElement();
        health.HideElement();
    }

    public void OnDie ()
    {
        HideStatusBar();
        this.gameObject.SetActive(false); // destroy?
        Destroy(this.gameObject);
    }
}

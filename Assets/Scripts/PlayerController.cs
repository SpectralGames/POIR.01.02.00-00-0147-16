using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour 
{
	public Transform headPivot;
	public Transform centerEye;
	public int[] healthLevels;
	public int[] healthRechargeRateLevels;
	public int[] manaLevels;
	public int[] manaRechargeRateLevels;
	public int[] teleportCosts;

	public HandIndicator healthIndicator;
	public HandIndicator manaIndicator;
	public AttacksController attackController;
	public PlayerCanvas playerCanvas;

	private int currentLevel;
	private int raycastLayerMask;
	private Transform localRoomArea;
	//private GameplayController gameController;
	private Transform playerHead;
	private int currentHealth;
	private float rechargeHealthCounter;

	private int currentMana;
	private float rechargeManaCounter;

	private bool isDying = false;
	private bool isAlive = true;
	private float[] xAxisDeviation = new float[2] {0f, 0f};
	private Coroutine hitAnimationCoroutine; 
	private Coroutine dyingAnimationCoroutine;

	
	private GameplayController gameplayController;

    private TowerTeleport selectedTeleport;
    private bool gripLeftWasDown = false;
    private bool gripRightWasDown = false;
    public event System.Action OnPlayerDied;
    public static bool canCast = true;
    private void OnPlayerDied_Invoke()
	{
		OnPlayerDied?.Invoke();
	}

	void Awake()
	{
		currentLevel = 0;
		currentHealth = healthLevels[currentLevel];
		currentMana = manaLevels[currentMana];

		localRoomArea = this.transform.parent;
		raycastLayerMask = LayerMask.GetMask("Default", "TowerTeleport", "Enemy");
		playerHead = GetComponentInChildren<Camera> ().transform;
		attackController.OnSetPlayerController (this);
		gameplayController = GameObject.FindObjectOfType<GameplayController> ();
		
	}

	void Start ()
	{
		if (SceneManager.GetActiveScene().name == "BruteVsMaidenVR_Loading")
			canCast = false;
		else
		{
			canCast = true;
		}

		SpellCaster.OnSpellCast += CastCooldown;
		//gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameplayController>();

		this.UpdateLodBias();
	}

	public void CastCooldown()
	{
		canCast = false;
		Debug.Log(canCast);
		StartCoroutine(TickDownCooldownTime());
	}

	private IEnumerator TickDownCooldownTime()
	{
		yield return new WaitForSeconds(1f);
		canCast = true;
	}
	void UpdateLodBias()
	{
		QualitySettings.lodBias = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView/2f) / Mathf.Tan(Mathf.Deg2Rad * 90f/2f);
	}
		
	// Update is called once per frame
	void Update () 
	{
		if (isAlive == false)
			return;
		
		if(Input.GetMouseButtonDown(0))
		{
			Ray newRay = new Ray(centerEye.transform.position, centerEye.transform.forward); //Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane)); //new Ray(Camera.main.transform.position, Camera.main.transform.forward); //Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
			//Debug.DrawRay(newRay.origin, newRay.direction*5000f, Color.red, 4f);
			//newRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if(Physics.Raycast(newRay, out hitInfo, 5000f, raycastLayerMask))
			{
				Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
				if (enemy != null)
				{
					enemy.TakeDamage(10.0f, false);
				}
					
				if(hitInfo.collider.GetComponent<DestroyableObjectColliderProxy>() != null)
				{
					hitInfo.collider.GetComponent<DestroyableObjectColliderProxy>().OnTakeDamage(hitInfo.point, hitInfo.normal, 20f, 1f);
				}
				if(hitInfo.collider.GetComponent<Rigidbody>() != null)
				{
					hitInfo.collider.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 500f);
				}
			}
			if (gameplayController != null) {
				if (gameplayController.IsGameplayActive ()) {
					CheckTeleportRaycast ();
				}
			}
		}

		RechargeMana ();
		RechargeHealth ();
        EnemyHitRaycast();
        
        if (Input.GetAxis("RightVRGrip") > 0)
            CheckControllerAxis();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
		{
			this.OnTeleportDetected();
		}
#endif
		
    }//update

	private void CheckControllerAxis()
	{
		
	
	}

	private void RotateFpsController(bool rotateToRight)
	{
		float angle = rotateToRight ? 45f : -45f;
	}

    public void EnemyHitRaycast ()
    {
        Ray newRay = new Ray(centerEye.transform.position, centerEye.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(newRay, out hitInfo, 5000f, LayerMask.GetMask("Enemy")))
        {
            Enemy e = hitInfo.collider.GetComponentInParent<Enemy>();
            if (e != null)
            {
                e.statusBar?.ShowStatusBar();
            }
        }
    }

	private void RechargeMana()
	{
		if (currentMana < manaLevels [currentLevel]) {
			if (rechargeManaCounter >= 30f / (float) manaRechargeRateLevels[currentLevel])
			{
				rechargeManaCounter = 0f;
				currentMana += 1;
				UpdateManaUI ();
			}
			rechargeManaCounter += Time.deltaTime;
		}
	}

	private void RechargeHealth()
	{
		if (currentHealth < healthLevels [currentLevel]) {
			if (rechargeHealthCounter >= 60f / (float) healthRechargeRateLevels[currentLevel])
			{
				rechargeHealthCounter = 0f;
				currentHealth++;
				UpdateHealthUI ();
			}
			rechargeHealthCounter += Time.deltaTime;
		}
	}

	public void OnTeleportDetected()
	{
		if (gameplayController != null && gameplayController.IsGameplayActive ()) 
		{
			if (teleportCosts [currentLevel] > GetCurrentMana ()) {
				// nie można użyć, nie ma wystarczająco many
				ToastMessage.CreateToast ("LOW MANA");
			} else {
                //if (CheckTeleportRaycast () == false)
                //	ToastMessage.CreateToast ("TELEPORT FAILED");
                CheckTeleportRaycast();
                
			}
		}
	}

	private bool CheckTeleportRaycast()
	{
        selectedTeleport = attackController.GetSelectedTower();
        if (selectedTeleport != null)
        {
            if (Vector3.Distance(new Vector3(selectedTeleport.transform.position.x, 0f, selectedTeleport.transform.position.z), new Vector3(Camera.main.transform.position.x, 0f, Camera.main.transform.position.z)) > 5f)
            {
                StartCoroutine(AnimationTeleportIn());
                return true;
            }
            return false;
        }
        return false;

        /*
		Ray newRay = new Ray(centerEye.transform.position, centerEye.transform.forward); //Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane)); //new Ray(Camera.main.transform.position, Camera.main.transform.forward); //Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
		RaycastHit hitInfo;

		if(Physics.Raycast(newRay, out hitInfo, 5000f, raycastLayerMask))
		{
			if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer ("TowerTeleport")) {
                selectedTeleport = hitInfo.collider.GetComponent<TowerTeleport> ();

				if (selectedTeleport != null && Vector3.Distance (new Vector3 (selectedTeleport.transform.position.x, 0f, selectedTeleport.transform.position.z), new Vector3 (Camera.main.transform.position.x, 0f, Camera.main.transform.position.z)) > 5f) {
                    StartCoroutine(AnimationTeleportIn());
                   // towerTeleport.TeleportPlayer (this.localRoomArea, fpsController.transform);
					//OnUseMana (teleportCosts [currentLevel]);
				} 
				return true;
			} else {
				return false;
			}
		}
		return false;
        */

    }

	public void OnTeleportPlayerToTeleportNumber(int teleportNumber)
	{
		foreach(TowerTeleport teleport in ObjectPool.Instance.towerTeleportList)
		{
			Debug.Log("teleport:" + teleport.towerNumber + " " +teleportNumber);
			if(teleport.towerNumber == teleportNumber)
			{
				return;
			}
		}
	}
		
	public void TakeDamage(int damage, GameObject enemy)
	{
		if (isAlive == false)
			return;

	
		currentHealth -= damage;
		if (currentHealth < 0) {
			currentHealth = 0;
			
			isAlive = false;
			OnPlayerDied_Invoke();
			Debug.Log ("player die");
			//gameController.GameOver ();		
		} 
		else if (currentHealth < 20) 
		{ // prawie umiera, pokaż efekt
			isDying = true;
			if(hitAnimationCoroutine != null)
				StopCoroutine (hitAnimationCoroutine);
		} 
		else // dostał hit, pokaż efekt i powoli go ukryj
		{
			
		}

		playerCanvas.ShowDamageAnimation (enemy);
		UpdateHealthUI ();

	}

	IEnumerator AnimationTeleportIn()
    {
	    float destVignetteFactor = 1;
        float destBlurFactor = 0.35f; 
        

        float timeAnim = .3f;
        float currentTime = 0;
        while (currentTime < timeAnim)
        {
            currentTime += Time.deltaTime;
            float time = (currentTime / timeAnim);
            time = Mathf.Sin(time * Mathf.PI * 0.5f);

            yield return null;
        }

        
        OnUseMana (teleportCosts [currentLevel]);
        
        currentTime = 0;
        timeAnim = .3f;
        while (currentTime < timeAnim)
        {
            currentTime += Time.deltaTime;
            float time = (currentTime / timeAnim);
            time = 1f - Mathf.Cos(time * Mathf.PI * 0.5f);
           
            yield return null;
        }
    }

    public void OnUseMana(int value)
	{
		currentMana -= value;
		if (currentMana < 0) {
			currentMana = 0;
		}
		UpdateManaUI ();
	}

	private void UpdateManaUI()
	{
		manaIndicator.UpdateValue (currentMana, (float)currentMana / manaLevels[currentLevel]);
	}

	private void UpdateHealthUI()
	{
		if (isDying && currentHealth > 20) { // jesli wyszedl ze stanu umierania to schowaj damage efekt
			isDying = false;
		}
		healthIndicator.UpdateValue (currentHealth,(float) currentHealth / healthLevels[currentLevel]);
	}
		
	public Vector3 GetPlayerPosition()
	{
		return this.localRoomArea.position;
	}

	public float GetCurrentMana()
	{
		return currentMana;
	}

	public Vector3 GetPlayerHeadPosition()
	{
		return playerHead.position;
	}
}

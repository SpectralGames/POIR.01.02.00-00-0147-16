using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AttacksController : MonoBehaviour 
{
	public Transform centerEye, vrCrosshair;
	public Color vrCrosshairActiveColor;
	public LayerMask attackLayerMask;
//	public GameObject fireBallPrefab, fireBallBigPrefab, violetBallPrefab, lightningPrefab, meteorPrefab;
	[SerializeField]
	public AttackItemHolder[] attackItems;
	[SerializeField]
	public Dictionary<EAttackType, GameObject> attackItemsList;
    public bool debugMode = false;
    public LineRenderer lineCrosshair;

    private TowerManager towerManager;
	//private ControllerModelHolder leftHolder, rightHolder;
	public PlayerController _playerController;
	private GameObject towersParent;
	private MaidenSpawnTower currentMaidenSpawnTower;

	private float lastCrosshairZRotation;
	private Material vrCrosshairMaterial;

    private bool isSpellAttacksEnabled = true;
    private TowerTeleport selectedTower;
    private AttackBaseController tempAttackController;
    
    public struct SpawnStruct
	{
		public Vector3 spawnPosition;
		public Quaternion spawnRotation;
		public Vector3 targetPosition;
		public Vector3 targetNormal;
	}

	// Use this for initialization
	void Awake () 
	{
		//leftHolder = ControllerModelHolder.GetControllerModel(ControllerType.LeftController);
		//rightHolder = ControllerModelHolder.GetControllerModel(ControllerType.RightController);
		towerManager = GameObject.FindObjectOfType<TowerManager> ();
		towersParent = new GameObject("AttackTowersParent");
		//podmien material w celowniku
		Projector vrCrosshairProjector = vrCrosshair.GetComponent<Projector>();
		vrCrosshairMaterial = new Material(vrCrosshairProjector.material);
		vrCrosshairProjector.material = vrCrosshairMaterial;


        lineCrosshair.startWidth = .01f;
        lineCrosshair.endWidth = .1f;
    }
	

	public void OnSetPlayerController(PlayerController playerController)
	{
		this._playerController = playerController;
	}


	public GameObject CreateAttack(string attackType, SpawnStruct spawnStruct, bool addToObjectPool = true)
	{
		GameObject attackObject = null;
		if (XMLItemsReader.GetItemType (attackType).Equals ("Spell"))
        {
            if (isSpellAttacksEnabled)
            {
                attackObject = GameObject.Instantiate(GetPrefabByAttackType(attackType), spawnStruct.spawnPosition, spawnStruct.spawnRotation);
                if (attackObject.GetComponent<AttackBaseController>() != null)
                    attackObject.GetComponent<AttackBaseController>().OnInit(spawnStruct.targetPosition, spawnStruct.targetNormal, GetCurrentAttackLevel(attackType));

                tempAttackController = attackObject.GetComponent<AttackBaseController>();
            }
            else
            {
                isSpellAttacksEnabled = true; // odblokuj strzelanie czarami, zablokowane było ponieważ gracz wykonywał gest.
            }
		} 
		else
		{
			attackObject = Instantiate (GetPrefabByAttackType (attackType), spawnStruct.targetPosition, Quaternion.identity, towersParent.transform);
			if (attackObject.GetComponent<TowerBase> () != null) {
				attackObject.GetComponent<TowerBase> ().OnSetValues (GetCurrentAttackLevel (attackType));
				if(currentMaidenSpawnTower != null)
					currentMaidenSpawnTower.OnTowerSpawned (attackObject.GetComponent<TowerBase>());
				if(addToObjectPool)
					ObjectPool.Instance.towerList.Add (attackObject.GetComponent<TowerBase> ());
			}
		}
      
       
		return attackObject;
	}

	public MaidenSpawnTower CheckTowerRaycast()
	{
		Ray newRay = new Ray(centerEye.transform.position, centerEye.transform.forward); //Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane)); //new Ray(Camera.main.transform.position, Camera.main.transform.forward); //Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
		RaycastHit hitInfo;
		int towerSpawnLayerMask = LayerMask.GetMask("TowerMaidenSpawnPoint");
		if (Physics.Raycast (newRay, out hitInfo, 5000f, towerSpawnLayerMask)) {			
			MaidenSpawnTower towerSpawn = hitInfo.collider.GetComponent<MaidenSpawnTower> ();
			return towerSpawn;
		} else {
			ToastMessage.CreateToast ("Nie trafiłeś czarem w Tower Spawn");
		}
		return null;
	}

    private LookAtObjectEventHandler lookAtObjectEventHandler = null;
    private LookAtObjectEventHandler lookAtObjectWithHeadEventHandler = null;
    // Update is called once per frame
    
    void Update () 
	{
		/*
		Ray eyeRay = new Ray(centerEye.transform.position, centerEye.transform.forward); 
		RaycastHit hitInfo;
		Vector3 crosshairPosition = -Vector3.up * 10000f;
		Vector3 crosshairForward = Vector3.down;

		Vector3[] positions = new Vector3[2];
		positions[0] = lineCrosshair.transform.position;

		if (Physics.Raycast(eyeRay, out hitInfo, 5000f, attackLayerMask))
		{
			crosshairPosition = hitInfo.point + hitInfo.normal * 0.75f;
			crosshairForward = Vector3.RotateTowards(Vector3.down, -hitInfo.normal, Mathf.PI*0.5f, 0f);
		}

		positions[1] = positions[0] + centerEye.transform.forward * 20;//crosshairPosition;
		lineCrosshair.SetPositions(positions);

		vrCrosshair.transform.position = crosshairPosition;
		vrCrosshair.transform.forward = crosshairForward;
		lastCrosshairZRotation = Mathf.Repeat(lastCrosshairZRotation + 40f * Time.deltaTime, 360f);
		vrCrosshair.transform.localEulerAngles = new Vector3(vrCrosshair.transform.localEulerAngles.x, vrCrosshair.transform.localEulerAngles.y, lastCrosshairZRotation);

		//zmien kolor celownika
		if (Physics.Raycast(eyeRay, out hitInfo, 500f))
		{
		    if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("TowerTeleport") || hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("TowerMaidenSpawnPoint"))
		    {
		        if (lookAtObjectEventHandler == null)
		        {
		            lookAtObjectEventHandler = hitInfo.collider.gameObject.GetComponent<LookAtObjectEventHandler>();
		            lookAtObjectEventHandler?.LookAt();
		        }
		        vrCrosshairMaterial.color = vrCrosshairActiveColor;
		    }
		    else
		    {
		        if (lookAtObjectEventHandler != null)
		        {
		            lookAtObjectEventHandler.LookOff();
		            lookAtObjectEventHandler = null;
		        }
		        vrCrosshairMaterial.color = Color.white;
		    }
		}
		*/
       // left hand pointing object
       
       
        CheckHeadSelection();
	}

    private void CheckHandsSelection(Ray rayLeftHand, Ray rayRightHand)
    {
        RaycastHit hitInfo;
        bool isSelectedByOneHand = false;
        selectedTower = null;

        if (Physics.Raycast(rayLeftHand, out hitInfo, 500f))
        {
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("TowerTeleport"))
            {
                isSelectedByOneHand = true;
                selectedTower = hitInfo.collider.gameObject.GetComponent<TowerTeleport>();
            }
        }

        if (Physics.Raycast(rayRightHand, out hitInfo, 500f))
        {
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("TowerTeleport"))
            {
                isSelectedByOneHand = true;
                selectedTower = hitInfo.collider.gameObject.GetComponent<TowerTeleport>();
            }
        }

        if (isSelectedByOneHand)
        {
            if (lookAtObjectEventHandler == null)
            {
                lookAtObjectEventHandler = hitInfo.collider.gameObject.GetComponent<LookAtObjectEventHandler>();
                lookAtObjectEventHandler?.LookAt();
            }
            vrCrosshairMaterial.color = vrCrosshairActiveColor;
        }
        else
        {
            if (lookAtObjectEventHandler != null)
            {
                lookAtObjectEventHandler.LookOff();
                lookAtObjectEventHandler = null;
            }
            vrCrosshairMaterial.color = Color.white;
        }
        
    }

    private void CheckHeadSelection()
    {
        RaycastHit hitInfo;
        bool isSelected = false;
        Ray newRay = new Ray(centerEye.transform.position, centerEye.transform.forward);
        if (Physics.Raycast(newRay, out hitInfo, 500f))
        {
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("TowerMaidenSpawnPoint"))
            {
                isSelected = true;
            }
         
        }
        if (isSelected)
        {
            if (lookAtObjectWithHeadEventHandler == null)
            {
                lookAtObjectWithHeadEventHandler = hitInfo.collider.gameObject.GetComponent<LookAtObjectEventHandler>();
                lookAtObjectWithHeadEventHandler?.LookAt();
            }
            vrCrosshairMaterial.color = vrCrosshairActiveColor;
        }
        else
        {
            if (lookAtObjectWithHeadEventHandler != null)
            {
                lookAtObjectWithHeadEventHandler.LookOff();
                lookAtObjectWithHeadEventHandler = null;
            }
            vrCrosshairMaterial.color = Color.white;
        }

    }

    public GameObject GetPrefabByAttackType(string type)
	{
		int attackLevel = GetCurrentAttackLevel (type);

		foreach (AttackItemHolder item in attackItems) {
			if (item.type.ToString().Equals(type))
				return item.attackPrefabs[attackLevel-1];
		}
		Debug.LogWarning ("Not found attack prefab!!!");
		return null;
	}

	private int GetCurrentAttackLevel(string type)
	{
		int attackLevel = SaveGameController.instance.GetItemLevel (type);
		if (attackLevel <= 0) {
			Debug.LogWarning ("attack nie jest odblokowany, w trybie debug ustawiam na 1");
            if(debugMode)
		    	attackLevel = 1;
		}
		return attackLevel;
	}

	public void CreateMenuTower(MaidenSpawnTower spawnTower, string type)
	{
		currentMaidenSpawnTower = spawnTower;
		currentMaidenSpawnTower = null;
	}

	public GameObject CreateMenuAttack(Transform spawnTransform, string type, Vector3 target)
	{
		SpawnStruct spawnPoint = new SpawnStruct ();
		spawnPoint.spawnRotation = spawnTransform.rotation; //Quaternion.identity;
		spawnPoint.spawnPosition = spawnTransform.position; //rightHolder.GetPointerWorldPosition();
		spawnPoint.targetPosition = target;
		spawnPoint.targetNormal = (spawnTransform.position - target).normalized; //Vector3.zero;
		return CreateAttack (type, spawnPoint);
	}

	public Transform GetTowersParent()
	{
		return towersParent.transform;
	}

    public void DestroyTempAttack()
    {
        if(tempAttackController != null)
        {
            tempAttackController.Deactivate();
        }
    }

    public void DisableSpellAttacks()
    {
        isSpellAttacksEnabled = false;
    }

    public TowerTeleport GetSelectedTower()
    {
        return selectedTower;
    }

 

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigCrossbow : MonoBehaviour
{
    [SerializeField] private AttacksController attacksController;
    [SerializeField] private Transform shootingPoint = null;
    public EAttackType actuallyShottingType;
    [SerializeField] private int maxAmmo = 3;
    [SerializeField] private int currentAmmo = 3;
    [SerializeField] private GameObject crossBowObject = null;
    public MeshRenderer boltMesh = null;
    public Text ammoCounter = null;
    public bool canShoot = true;
    private SaveGameController _saveGameController;
    private ObjectPool _objectPool;
    private bool isHoldingWith2Hands = false;
    
    private void Awake()
    {
        _objectPool = ObjectPool.Instance;
        WarmBolts(shootingPoint);
    }

    private void Start()
    {
        _saveGameController = SaveGameController.instance;
    }

    private void OnEnable()
    {
        currentAmmo = maxAmmo;
        canShoot = true;
        ammoCounter.text = currentAmmo.ToString();
        ammoCounter.color = Color.yellow;
        boltMesh.material = attacksController.attackItems[0].boltMaterial;
        
    }

    private void Update()
    {
        if (isHoldingWith2Hands)
        {
            Vector3 temp = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(temp.x, temp.y, 0);

        }
    }

    public void WarmBolts(Transform spawningPoint)
    {
        
        for (int i = 0; i < maxAmmo; i++)
        {
            var bolt = Instantiate(attacksController.attackItems[0].attackPrefabs[0], Vector3.zero, Quaternion.identity);
            bolt.gameObject.SetActive(false);
            _objectPool.boltList.Add(bolt);
        }
    }
    public void Shoot()
    {
        if (currentAmmo > 0 && canShoot)
        {
            Bolt bolt;
            bolt = SetupBolt(actuallyShottingType);

            string boltType = bolt.BoltType.ToString();
            if (_saveGameController.GetItemLevel(boltType) == 1 && !bolt.Equals(null))
            {
                bolt.Init();
                bolt.transform.position = shootingPoint.transform.position;
                bolt.transform.rotation = transform.rotation * Quaternion.Euler(-90,0,0);
                bolt.AimDir = shootingPoint.forward;
                bolt.gameObject.SetActive(true);
                bolt.attackDamage *= 3;
                currentAmmo--;
                ammoCounter.text = currentAmmo.ToString();
                StartCoroutine(ThrowBackWeapon(bolt.throwBackStrength*10, bolt.upwardDrift));
            }
        }
        else
        {
            canShoot = true;
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        ammoCounter.text = currentAmmo.ToString();
    }

    private IEnumerator ThrowBackWeapon(float strength, float upwardDrift)
    {
        canShoot = false;
        ammoCounter.color = Color.gray;
        
        Vector3 tempLocalPos = crossBowObject.transform.localPosition;
        Quaternion localRot = crossBowObject.transform.localRotation;
        
        crossBowObject.transform.localPosition = new Vector3(crossBowObject.transform.localPosition.x, crossBowObject.transform.localPosition.y, crossBowObject.transform.localPosition.z - strength);
        
        if(isHoldingWith2Hands)
            crossBowObject.transform.localRotation *= Quaternion.Euler(upwardDrift*-1, 1, 1);
        else
            crossBowObject.transform.localRotation *= Quaternion.Euler(upwardDrift*-3, 1, 1);
        
        float maxDuration = .5f;
        float currentDuration = 0;
        while (currentDuration < maxDuration)
        {
            currentDuration += Time.deltaTime;
            crossBowObject.transform.localPosition = Vector3.Lerp(crossBowObject.transform.localPosition, tempLocalPos, currentDuration / maxDuration);
            crossBowObject.transform.localRotation = Quaternion.Slerp(crossBowObject.transform.localRotation, localRot, currentDuration / maxDuration);
            yield return null;
        }
        canShoot = true;
        ammoCounter.color = Color.yellow;
    }

    private Bolt SetupBolt(EAttackType attackType)
    {
        switch (actuallyShottingType)
        {
            case EAttackType.WoodenBolt:
                return _objectPool.GetBolt(_objectPool.boltList).GetComponent<Bolt>();
                
            case EAttackType.FireBolt:
                return _objectPool.GetBolt(_objectPool.fireBoltList).GetComponent<Bolt>();
        
            case EAttackType.IceBolt:
                return _objectPool.GetBolt(_objectPool.iceBoltList).GetComponent<Bolt>();

            default:
                return _objectPool.GetBolt(_objectPool.boltList).GetComponent<Bolt>();
                break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Crossbow : MonoBehaviour
{
    [SerializeField] private AttacksController attacksController;
    [SerializeField] private Transform shootingPoint = null;
    public EAttackType actuallyShottingType;
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private int currentAmmo = 10;
    [SerializeField] private float shootCooldown = 0.1f;
    [SerializeField] private List<AudioClip> shootingClips = null;
    [SerializeField] private List<AudioClip> critEffects = null;
    public MeshRenderer boltMesh = null;
    public Text ammoCounter = null;
    public bool canShoot = true;
    private SaveGameController _saveGameController;
    
    private ObjectPool _objectPool;

    private void Awake()
    {
        _objectPool = ObjectPool.Instance;
        WarmBolts(shootingPoint);
    }

    private void Start()
    {
        _saveGameController = SaveGameController.instance;
    }

    private void OnDestroy()
    {
        Debug.Log("Destroyed crossbow");
    }

    private void OnEnable()
    {
        currentAmmo = maxAmmo;
        canShoot = true;
        ammoCounter.text = currentAmmo.ToString();
        ammoCounter.color = Color.white;
        boltMesh.material = attacksController.attackItems[0].boltMaterial;
    }

    public void WarmBolts(Transform spawningPoint)
    {
        for (int i = 0; i < 10; i++)
        {
            var bolt = Instantiate(attacksController.attackItems[0].attackPrefabs[0], Vector3.zero, Quaternion.identity);
            bolt.gameObject.SetActive(false);
            _objectPool.boltList.Add(bolt);
        }

        for (int i = 0; i < 10; i++)
        {
            var bolt = Instantiate(attacksController.attackItems[2].attackPrefabs[0], Vector3.zero, Quaternion.identity);
            bolt.gameObject.SetActive(false);
            _objectPool.iceBoltList.Add(bolt);
        }
        for (int i = 0; i < 10; i++)
        {
            var bolt = Instantiate(attacksController.attackItems[1].attackPrefabs[0], Vector3.zero, Quaternion.identity);
            bolt.gameObject.SetActive(false);
            _objectPool.fireBoltList.Add(bolt);
        }
    }
    public void Shoot()
    {
        if (currentAmmo > 0 && canShoot)
        {
            canShoot = false;
            boltMesh.enabled = false;
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
                currentAmmo--;

                if(bolt.RecentlyCritted)
                    SoundManager.instance.PlaySceneEffect(critEffects[Random.Range(0, critEffects.Count)], gameObject.transform.position, .5f, 0f, .5f, 1.5f);
                else
                    SoundManager.instance.PlaySceneEffect(shootingClips[0], gameObject.transform.position, 0f, 0f, 0.5f, 1.5f);

                ammoCounter.text = currentAmmo.ToString();
                StartCoroutine(ThrowBackWeapon(bolt.throwBackStrength, bolt.upwardDrift));
            }

            StartCoroutine(CountDowntimer());
        }
        else
        {
            canShoot = true;
        }
    }

    private IEnumerator CountDowntimer()
    {
        float currentTime = 0f;
        while (currentTime < shootCooldown)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        boltMesh.enabled = true;
        canShoot = true;
    }
    public IEnumerator ShootSerie()
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentAmmo > 0 && canShoot && gameObject.activeInHierarchy)
            {

                Bolt bolt;
                bolt = SetupBolt(actuallyShottingType);

                string boltType = bolt.BoltType.ToString();
                if (_saveGameController.GetItemLevel(boltType) == 1 && !bolt.Equals(null))
                {
                    bolt.Init();
                    bolt.transform.position = shootingPoint.transform.position;
                    bolt.transform.rotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
                    bolt.AimDir = shootingPoint.forward;
                    bolt.gameObject.SetActive(true);
                    currentAmmo--;
                    ammoCounter.text = currentAmmo.ToString();
                    StartCoroutine(ThrowBackWeapon(bolt.throwBackStrength * (i+1), bolt.upwardDrift));
                    //TriggerVibrations(ctrl, 1.3f);
                    yield return new WaitForSeconds(.1f);
                    
                }
            }
            else
            {
                canShoot = true;
            }
        }
    }

    private void OnDisable()
    {
        StopCoroutine(ThrowBackWeapon(0f, 0f));
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        ammoCounter.text = currentAmmo.ToString();
    }

    private IEnumerator ThrowBackWeapon(float strength, float upwardDrift)
    {
        Vector3 tempLocalPos = transform.localPosition;
        Quaternion localRot = transform.localRotation;
        
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + strength);
        transform.localRotation *= Quaternion.Euler(upwardDrift, 1, 1);

        float maxDuration = .1f;
        float currentDuration = 0;
        while (currentDuration < maxDuration)
        {
            currentDuration += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, tempLocalPos, currentDuration / maxDuration);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, localRot, currentDuration / maxDuration);
            yield return null;
        }
    }

    private Bolt SetupBolt(EAttackType attackType)
    {
        switch (actuallyShottingType)
        {
            case EAttackType.WoodenBolt:
            {
                return _objectPool.GetBolt(_objectPool.boltList).GetComponent<Bolt>();
            }
            case EAttackType.FireBolt:
                return _objectPool.GetBolt(_objectPool.fireBoltList).GetComponent<Bolt>();
        
            case EAttackType.IceBolt:
                return _objectPool.GetBolt(_objectPool.iceBoltList).GetComponent<Bolt>();

            default:
                return _objectPool.GetBolt(_objectPool.boltList).GetComponent<Bolt>();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoltBaseController : MonoBehaviour
{
    public EAttackType attackType;
    public bool stopTargetAfterAttack;
    public LayerMask CollisionLayers;
    public GameObject boltParticlesOnHit;
    public float attackDamage;
    public float throwBackStrength;
    public float upwardDrift;
    
    protected bool applyKickForceToEnemies = true;
    protected float critChance;
    protected int attackLevel;
    protected float speed;
    
    private TrailRenderer _trail;
    private int level;
    private bool recentlyCritted;
    private float currentDestructionTime = 0f;
    private float selfDestructionTime = 3f;
    private Vector3 aimDir;
    private EAttackType type = EAttackType.WoodenBolt;
    public EAttackType BoltType { 
        get => type;
        set { type = value; }
    } 
    public Vector3 AimDir { 
        get => aimDir;
        set { aimDir = value; }
    }
    public float BoltSpeed { 
        get => speed;
        set { speed = value; }
    }
    public float UpwardDrift
    { 
        get => upwardDrift;
        set { upwardDrift = value; }
    }
    public float SelfDestructionTime { 
        get => selfDestructionTime;
        set { selfDestructionTime = value; }
    }
    public float CurrentDestructionTime { 
        get => currentDestructionTime;
        set { currentDestructionTime = value; }
    }

    public bool RecentlyCritted => recentlyCritted;

    private Gradient startingBoltTrailColor;
    public void Init()
    {
        SetValues(GetCurrentAttackLevel(attackType.ToString()));
        
        int testCrit = Random.Range(1, 100);
        Debug.Log(testCrit);
        if (testCrit <= critChance)
        {
            recentlyCritted = true;
            _trail.startColor = Color.yellow;
            _trail.endColor = Color.yellow;
        }
        else
        {
            _trail.colorGradient = startingBoltTrailColor;
        }

    }
    
    private void SetValues(int level = -1)
    {
        attackLevel = -1;
        if (level > -1)
        {
            attackLevel = level;
        }
        
        if(attackLevel > -1) //gdy level=-1 -> nieodblokowany, nie sciagaj wartosci
        {
            //int currentLevel = SaveGameController.instance.GetItemLevel(currentItemCodeName);
            string currentItemCodeName = attackType.ToString();
            
            attackDamage = XMLItemsReader.GetDamageValue(currentItemCodeName, attackLevel);
            speed = XMLItemsReader.GetSpeedValue(currentItemCodeName, attackLevel);
            throwBackStrength = XMLItemsReader.GetThrowBackValue(currentItemCodeName, attackLevel);
            upwardDrift = XMLItemsReader.GetUpwardDriftValue(currentItemCodeName, attackLevel);
            critChance = XMLItemsReader.GetCritChanceValue(currentItemCodeName, attackLevel);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        float tempDamage = attackDamage;
        Enemy baseController = other.GetComponent<Enemy>();

        if (recentlyCritted)
        {
            tempDamage *= 2;
        }
        
            
        if (other.gameObject.layer == 0 || other.gameObject.layer == 14)
        {
            _trail.enabled = false;
            _trail.Clear();
            ObjectPool.Instance.ReturnBolt(this.gameObject);
        }
        
        if (baseController != null)
        {
            baseController.TakeDamage(tempDamage, stopTargetAfterAttack, attackType, EAttackSideEffect.NONE, attackType.ToString(), attackLevel, recentlyCritted);
            var particles = Instantiate(boltParticlesOnHit, transform.position, Quaternion.identity);
            
            if (applyKickForceToEnemies)
            {
                Rigidbody[] childRigidbodies = baseController.GetComponentsInChildren<Rigidbody>();
                Vector3 kickVector = baseController.transform.position - transform.position + Vector3.up * 0.5f;
                foreach (Rigidbody currentRigid in childRigidbodies)
                {
                    currentRigid.AddForce(kickVector.normalized * tempDamage * (10f / Mathf.Max(kickVector.magnitude, 1f)));
                    currentRigid.AddTorque(Random.onUnitSphere * tempDamage * 10f);
                }
            }
        }

        DamageReceiver damageReceiver = other.GetComponent<DamageReceiver>();
        if (damageReceiver != null)
        {
            damageReceiver.TakeDamage(tempDamage, stopTargetAfterAttack, EAttackSideEffect.NONE, attackType.ToString(), attackLevel, recentlyCritted);
        }
        if (baseController == null && damageReceiver == null)
        {
            other.gameObject.SendMessage("TakeDamage", tempDamage/2, SendMessageOptions.DontRequireReceiver);
            var particles = Instantiate(boltParticlesOnHit, transform.position, Quaternion.identity);
        }

        if (other.GetComponent<DestroyableObjectColliderProxy>() != null)
        {
            other.GetComponent<DestroyableObjectColliderProxy>().OnTakeDamage(transform.position, (other.transform.position - transform.position).normalized, 20f, 1f);
            var particles = Instantiate(boltParticlesOnHit, transform.position, Quaternion.identity);
        }

        if (other.GetComponent<Rigidbody>() != null && baseController == null)
        {
            Vector3 kickVector = other.transform.position - transform.position + Vector3.up * 0.5f;
            other.GetComponent<Rigidbody>().AddForce(kickVector.normalized * attackDamage * (10f / Mathf.Max(kickVector.magnitude, 1f)));
        }
        recentlyCritted = false;
    }
    
    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
        startingBoltTrailColor = _trail.colorGradient;
    }

    private void OnEnable()
    {
        currentDestructionTime = 0f;
        _trail.enabled = true;
        _trail.Clear();
        
    }

    public void SelfDestruct()
    {
        currentDestructionTime += Time.deltaTime;
        if (currentDestructionTime >= selfDestructionTime)
        {
            _trail.enabled = false;
            _trail.Clear();
            ObjectPool.Instance.ReturnBolt(this.gameObject);
        }
    }
    
    private void Update()
    {
        SelfDestruct();
        transform.Translate(aimDir * (Time.deltaTime * speed), Space.World);
    }
    private int GetCurrentAttackLevel(string type)
    {
        int attackLevel = SaveGameController.instance.GetItemLevel (type);

        return attackLevel;
    }
}
public class Bolt : BoltBaseController
{


}

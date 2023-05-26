using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMaidenWeaponType
{
	ArcherMaidenWeapon,
	ThunderMaidenWeapon,
	FireMaidenWeapon,
	IceMaidenWeapon
}
	
public class MaidenWeaponBaseController : MonoBehaviour //, ICollisionTrigger
{
	public EMaidenWeaponType weaponType;
	//public int[] manaCosts;
	private bool usesActivationDelay = false;
	private float activationDelay;
	private float damageStep;
	//public int[] damageLevels;
	//public int[] radiusLevels;
	public bool instantTransition = false;
	public ETransitionInterpolation transitionInterpolation = ETransitionInterpolation.Linear;
	public float transitionSpeed = 5f;
	public float transitionArc = 1f;

	private bool instantDamage;
	//public float[] durationLevels;
	private float destroyTime;

	public GameObject movingObject = null;
	public GameObject effectOnActivate;

	public bool stopTargetAfterAttack;

	public EAttackSideEffect attackSideEffect = EAttackSideEffect.NONE;

	protected int weaponLevel;
	protected float attackRadius;
	protected float attackDamage;
	protected float attackDuration;
	protected int attackManaCost;

	private float currentDamageStep;

	protected List<Enemy> enemiesInRange, instantDamageEnemiesHit;
	protected List<GameObject> objectsInRange;

	protected Collider myCollider;
	protected bool isActive;
	protected bool isTransitioning;
	protected Vector3 targetPosition, targetNormal, initPosition;
	protected float lerpFactor, initDistance;

	protected GameObject colliderActiveWhileInTransition;

	protected bool applyKickForceToEnemies = true;

    [SerializeField] private MagicType magicType = null;

	// Use this for initialization
	protected virtual void Awake () 
	{
		enemiesInRange = new List<Enemy>();
		instantDamageEnemiesHit = new List<Enemy>();
		objectsInRange = new List<GameObject>();

		isActive = false;
		//currentDuration = 0f;
	}

	public virtual void OnInit (Vector3 target, Vector3 targetNormal, int level = 0)
	{
		this.OnInit<SphereCollider>(target, targetNormal, level);
	}


	protected virtual void OnInit<T> (Vector3 target, Vector3 targetNormal, int level = 0) where T : Collider
	{
		Rigidbody myRigidbody;
		if(movingObject != null)
		{
			myCollider = movingObject.AddComponent<T>();
			myRigidbody = movingObject.AddComponent<Rigidbody>();
			movingObject.AddComponent<MovingObjectColliderProxy>().OnInit(this.OnTriggerEnter, this.OnTriggerExit);
		}else{
			myCollider = this.gameObject.AddComponent<T>();
			myRigidbody = this.gameObject.AddComponent<Rigidbody>();
		}

		if (myRigidbody == null) { // jesli juz w obiekcie był rigitbody
			myRigidbody = GetComponent<Rigidbody> ();
		}
		myCollider.isTrigger = true;
		myCollider.enabled = false;
		myRigidbody.useGravity = false;
		myRigidbody.isKinematic = true;

		this.OnSetValues(level);

		this.targetPosition = target;
		this.targetNormal = targetNormal;
		initPosition = this.transform.position;
		if(instantTransition)
		{
			if(movingObject != null)
				movingObject.transform.position = targetPosition;
			else
				this.transform.position = targetPosition;

			//Debug.Log(this.transform.position + "  " + targetPosition);
		}else{
			isTransitioning = true;
			//dodaj collider do kolizji w trakcie ruchu
			GameObject transitioningObject = movingObject != null ? movingObject : this.gameObject;
			colliderActiveWhileInTransition = new GameObject("TransitioningCollider");
			colliderActiveWhileInTransition.layer = this.gameObject.layer;
			colliderActiveWhileInTransition.transform.SetParent(transitioningObject.transform, false);
			colliderActiveWhileInTransition.AddComponent<Rigidbody>().isKinematic = true;
			SphereCollider colliderInTransition = colliderActiveWhileInTransition.AddComponent<SphereCollider>();
			colliderInTransition.radius = 0.25f;
			colliderInTransition.isTrigger = true;
			colliderActiveWhileInTransition.AddComponent<MovingObjectColliderProxy>().OnInit(this.OnInTransitionTriggerEnter);

			initDistance = Vector3.Distance(initPosition, targetPosition);
			lerpFactor = 0f;
		}

		//playBeforeHitAudioClip();

		if (usesActivationDelay)
			Invoke ("OnActivate", activationDelay);
		else if(instantTransition)
			this.OnActivate();
	//	else
	//		OnActivate ();

	}

	public virtual void OnSetValues(int level)
	{
		weaponLevel = level;
		string currentItemCodeName = weaponType.ToString();

		attackDamage = XMLItemsReader.GetDamageValue(currentItemCodeName, weaponLevel);
		attackRadius = XMLItemsReader.GetRadiusValue(currentItemCodeName, weaponLevel);
		instantDamage = XMLItemsReader.GetInstantDamageValue(currentItemCodeName);
		destroyTime = XMLItemsReader.GetDestroyTimeValue(currentItemCodeName);
		activationDelay = XMLItemsReader.GetActivationDelayValue(currentItemCodeName);
		usesActivationDelay = activationDelay > 0.01f;

		if(myCollider is SphereCollider)
			(myCollider as SphereCollider).radius = attackRadius;

		attackDuration = instantDamage == true ? 0f : XMLItemsReader.GetDurationValue(currentItemCodeName, weaponLevel);
		damageStep = instantDamage == true ? 0f : XMLItemsReader.GetDamageStepValue(currentItemCodeName);
		currentDamageStep = damageStep - 0.05f;

	}


	protected virtual void OnActivate()
	{
		myCollider.enabled = true;
		if(instantDamage == true) 
			Invoke("OnDeactivate", 0.25f);// 0.25
		else
			Invoke("OnDeactivate", attackDuration);
		isActive = true;

		if(effectOnActivate != null)
		{
			GameObject effect = GameObject.Instantiate(effectOnActivate, targetPosition + targetNormal * 0.15f, Quaternion.LookRotation(targetNormal)) as GameObject;
			Destroy(effect, 5f);
		}
		//playHitAudioClip();
		//odpal swiatlo
		//if(light) StartCoroutine(OnLightAnimation());
		Destroy (this.gameObject, attackDuration + destroyTime);
	}

	protected virtual void OnDeactivate()
	{
		myCollider.enabled = false;
		isActive = false;
	}


	// Update is called once per frame
	protected virtual void Update () 
	{
		if(isActive)
		{
            //Debug.Log ("instant:" + instantDamage);
            if (instantDamage == false)
            {
                if (currentDamageStep >= damageStep)
                {
                    Debug.Log("current seteP: " + currentDamageStep + " " + damageStep);
                    currentDamageStep = currentDamageStep - damageStep; //zachowaj nadmiar
                    for (int i = 0; i < enemiesInRange.Count; i++)
                    {
                        if (enemiesInRange[i] != null)
                        {
                            var modyfier = 0f;
                            if (MagicTypeDamageMap.Instance != null && enemiesInRange[i].MagicType != null && magicType)
                                modyfier = MagicTypeDamageMap.Instance.GetModyfier(magicType, enemiesInRange[i].MagicType);

                            var attackDamage = this.attackDamage + (this.attackDamage * modyfier);
                            enemiesInRange[i].TakeDamage(attackDamage, stopTargetAfterAttack); //, attackType);
                        }
                    }
                    for (int i = 0; i < objectsInRange.Count; i++)
                        if (objectsInRange[i] != null) objectsInRange[i].SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
                }
                currentDamageStep += Time.deltaTime;
            }
        }//isActive

        if (isTransitioning)
		{
			if(lerpFactor < 0.95f)
			{
				lerpFactor = Mathf.Clamp01(lerpFactor + (transitionSpeed / initDistance)*Time.deltaTime);
				float finalLerpFactor = transitionInterpolation == ETransitionInterpolation.SinOut ? Mathf.Sin(lerpFactor * Mathf.PI * 0.5f) : lerpFactor;

				Vector3 interpolatedPosition = Vector3.Lerp(initPosition, targetPosition, finalLerpFactor);
				Vector3 currentPosition = interpolatedPosition + Vector3.up * Mathf.Sin(Mathf.PI * finalLerpFactor) * transitionArc;
				if(movingObject != null)
					movingObject.transform.position = currentPosition;
				else
					this.transform.position = currentPosition;
			}else{
				isTransitioning = false;
				if(usesActivationDelay == false)
					this.OnActivate();
			}
		}
	}
		


	public void OnTriggerEnter(Collider other) 
	{
		Enemy baseController = other.GetComponentInChildren<Enemy>();
		if (baseController == null) {
			baseController = other.GetComponentInParent<Enemy>();
		}

		if(instantDamage == false) //jesli efekt czasowy, zbieraj wrogow/obiekty do kupy
		{
			if(baseController != null && enemiesInRange.Contains(baseController) == false)
				enemiesInRange.Add(baseController);
			else if(baseController == null)
				objectsInRange.Add(other.gameObject);
			
		}else{ //obrazenia natychmiastowe
			GameObject attackObject = movingObject != null ? movingObject : this.gameObject;
			if(baseController != null && instantDamageEnemiesHit.Contains(baseController) == false)
			{
				baseController.TakeDamage(attackDamage, stopTargetAfterAttack, EAttackType.WoodenBolt, attackSideEffect, weaponType.ToString(), weaponLevel);
				instantDamageEnemiesHit.Add(baseController);

				if(applyKickForceToEnemies)
				{
					Rigidbody[] childRigidbodies = baseController.GetComponentsInChildren<Rigidbody>();
					Vector3 kickVector = baseController.transform.position - attackObject.transform.position + Vector3.up*0.5f;
					foreach(Rigidbody currentRigid in childRigidbodies)
					{
						currentRigid.AddForce(kickVector.normalized * attackDamage * (10f/Mathf.Max(kickVector.magnitude,1f)) );
						currentRigid.AddTorque(Random.onUnitSphere * attackDamage * 10f);
					}
				}


			}else if(baseController == null){
				other.gameObject.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
			}

			if(other.GetComponent<DestroyableObjectColliderProxy>() != null)
			{
				other.GetComponent<DestroyableObjectColliderProxy>().OnTakeDamage(this.transform.position, (other.transform.position - this.transform.position).normalized, 20f, 1f);
			}
			if(other.GetComponent<Rigidbody>() != null)
			{
				Vector3 kickVector = other.transform.position - attackObject.transform.position + Vector3.up*0.5f;
				other.GetComponent<Rigidbody>().AddForce(kickVector.normalized * attackDamage * (10f/Mathf.Max(kickVector.magnitude,1f)) );
			}
		}
	}

	private void OnInTransitionTriggerEnter(Collider other)
	{
		if(isTransitioning)
		{
			GameObject myBody = movingObject != null ? movingObject : this.gameObject;
			if(Vector3.Distance(myBody.transform.position, targetPosition) > colliderActiveWhileInTransition.GetComponent<SphereCollider>().radius * 2f) //jesli nie dolecial do celu, wywolaj wczesna aktywacje
			{
				isTransitioning = false;
				targetPosition = other.ClosestPoint(myBody.transform.position); 
				targetNormal = (myBody.transform.position - targetPosition).normalized;

				if(usesActivationDelay == false)
					this.OnActivate();
			}
			Destroy(colliderActiveWhileInTransition);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if(instantDamage == false) //jesli efekt czasowy, usun wrogow/obiekty z kupy
		{
			Enemy baseController = other.GetComponent<Enemy>();

			if(baseController != null)
				enemiesInRange.Remove(baseController);
			else
				objectsInRange.Remove(other.gameObject);
		}
	}

	public int GetManaCost()
	{
		return attackManaCost;
	}

}

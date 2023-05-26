using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EEnemyType{
	Other, Archer, Axe, Ymir, Troll, Dragon, BarbarianMedium, IceShaman, Trex, Bomb
}

public abstract class Enemy : AI 
{
    #region Members

    protected FSMSystem finiteStateMachine;
	public void SetTransition (StateID s, FSMStateArgs additionalLeavingArgs = null, FSMStateArgs additionalEnteringArgs = null)
    {
        finiteStateMachine.PerformTransition(s, additionalLeavingArgs, additionalEnteringArgs);
    } //(Transition t) { finiteStateMachine.PerformTransition(t); }

    protected List<Virgin> virgins = new List<Virgin>();

    // osobna klasa?
    [SerializeField] protected float healthPoints = 100.0f;

    //[HideInInspector] public bool isPending = false; // czy oczekuje na usuniecie ze sceny
    [HideInInspector] public bool isEndMoving = false; // stany trzeba jakos inaczej przetrzymywac

    [HideInInspector] public StatusBar statusBar;

    protected EnemyStatePackage package;

    // noszona dziewica
    [HideInInspector] public List<Virgin> carryVirginList = null;
    [HideInInspector] public List<Transform> carryPivotList;
	[HideInInspector] public TowerBase targetTowerBase;

	public EEnemyType enemyType;
	public float attackRange;
	public int attackPower;
	public float attackInterval;
	public Transform hitPivot;
	public int killGoldBonus;
	public int killExpBonus = 5;

	private bool isAttackEnabled = true;
    public bool isAttackCharged { get; protected set; }
    [SerializeField] private bool waitForAttackAnimationToEnd = false;
    public bool WaitForAttackAnimationToEnd { get { return waitForAttackAnimationToEnd; } }
    public bool IsPlayingAttackAnimation { get; set; }
    public bool isCurrentHitAnimation { get; protected set; }
    public bool isCurrentAttackAnimation { get; protected set; }
    public bool allowAttackPlayer = true;
    protected float attackIntervalCounter;

    [Space]
    [SerializeField] private float animationTriggerDamageLevel = 20f;
    private float currentDamagaTriggerLevel = 0;

    [Space]
    public UnityEvent OnDeath = new UnityEvent();

    public bool BlockMovement { get; set; }

    [SerializeField] private EAttackType boltVulni;
    public EAttackType BoltVulni { get { return boltVulni; } }

    [SerializeField] private MagicType _magicType;
    public MagicType MagicType { get { return _magicType; } }

    public bool isSpawnedByHand = false;
    // startowa
    #endregion Members

    #region Inits
    private void Awake ()
    {
	    if(isSpawnedByHand)
			Init(null, null, NavigationType.None);
        
    }
    public virtual void Init (Pathpoint startPathPoint, BezierPath startBezierPath, NavigationType navigationType)
    {
        virgins = GameObject.FindObjectsOfType<Virgin>().ToList();

        
		InitHealth(GetAIHeight() + .9f);

        InitNavigation(startPathPoint, startBezierPath, navigationType);

        //aiController = new AIEnemyNavMeshController(this, start);

        animator = new EnemyAnimator(this);

        //OnTakeDamage += TakeDamage;
        OnDie += Died;
        OnTeleport += Teleport;
        isInit = true;


        
        animator.SetAnimatorWalkBlend(walkSpeed);

		attackIntervalCounter = attackInterval - .2f;

        carryPivotList = this.gameObject.FindComponentsInChildWithName<Transform>("CarryPivot");

        if(!isSpawnedByHand)
			CreateFSM();

		ShowTeleportEffect ();
		//Invoke("StopADF", 5f);
		//Invoke("Resume", 9f);
    }

	private void StopADF()
	{
		NavigationController.Disable();
	}

	public void Resume()
	{
		NavigationController.Enable();
	}

    private void InitNavigation (Pathpoint startPathPoint, BezierPath startBezierPath, NavigationType navigationType)
    {
        this.navigationType = navigationType;

        if (this.navigationType == NavigationType.NavMesh)
            NavigationController = new AIEnemyNavMesh(this, startPathPoint);
        else if (this.navigationType == NavigationType.Splines)
            NavigationController = new AIEnemySpline(this, startBezierPath);
    }

    protected virtual void CreateFSM ()
	{
		finiteStateMachine = new FSMSystem();
		// TODO: stworz mozliwe stany // podac stringa, moze dictionary
		EnemyMovementState movement = new EnemyMovementState(package, animator.GetAnimations("Walk"), animator.GetAnimations("Run"),
								StateID.AttackingPlayer, StateID.StopMovement, StateID.Frozen, StateID.TakingDamage, StateID.Death, StateID.AvoidingSpell, StateID.CarryVirign, StateID.AttackingTower); //CreateMovementState();
		//EnemyTakeDamageState takeDamage = CreateTakeDamageState();
		EnemyAvoidingSpellState avoid = new EnemyAvoidingSpellState(StateID.Movement, StateID.TakingDamage, StateID.Death); //CreateAvoidState();

		//EnemyCarryVirginState virgin = new EnemyCarryVirginState(package);
		EnemyDeathState death = new EnemyDeathState(package);

		EnemyFrozenState frozen = new EnemyFrozenState(this, finiteStateMachine, StateID.Movement, StateID.AttackingPlayer, StateID.AttackingTower);

		EnemyStopMovementState stopMovement = new EnemyStopMovementState(this, finiteStateMachine, StateID.Movement, StateID.Death, StateID.Frozen, StateID.AttackingPlayer, StateID.AttackingTower);

		EnemyAttackState attackPlayer = new EnemyAttackState(package, StateID.Frozen, StateID.TakingDamage, StateID.Death, StateID.Movement); //CreateAttackState();
		EnemyAttackTowerState attackTower = new EnemyAttackTowerState(package, StateID.Frozen, StateID.TakingDamage, StateID.Death, StateID.Movement); //CreateAttackTowerState ();

		finiteStateMachine.AddState(movement);
		//finiteStateMachine.AddState(takeDamage);
		finiteStateMachine.AddState(avoid);
		finiteStateMachine.AddState(death);
		finiteStateMachine.AddState(frozen);
		finiteStateMachine.AddState(stopMovement);
		//finiteStateMachine.AddState(virgin);
		finiteStateMachine.AddState(attackPlayer);
		finiteStateMachine.AddState(attackTower);
	}

	public StateID GetCurrentStateID()
	{
		return finiteStateMachine.CurrentStateID;
	}

    protected void InitHealth (float enemyHeight)
    {
        try
        {
            statusBar = Instantiate(Resources.Load<StatusBar>("StatusBar"), Vector3.zero, Quaternion.identity);
            Debug.Log("Status bar: " + statusBar);
            statusBar.Init(this, healthPoints , enemyHeight);
        }
        catch (Exception e)
        {
            Debug.LogError("Health nie znaleziony: " + e.Message);
        }
    }
    #endregion Inits

    #region Logic
    /*
    private void Update () // nie potrzebne
    {
        if (!isInit) return;
        if (!isAlive) return;
        //aiController?.Tick();
        NavigationController?.Tick(); // new
        statusBar?.Tick();
        finiteStateMachine.CurrentState.Reason();
        finiteStateMachine.CurrentState.Act();
		ChargeAttack ();
    }*/
    
    public virtual void ForceOnLookAt (Transform target = null)
    {
        StopCoroutine("OnLookAt");
        StartCoroutine(OnLookAt(target));
    }

	public override void SetSpeedFactor (float newSpeedFactor)
	{
		base.SetSpeedFactor(newSpeedFactor);

		NavigationController?.UpdateSpeedFactor();
		animator.SetAnimatorWalkBlend(walkSpeed * speedFactor);
		animator.SetAnimatorSpeedFactor(newSpeedFactor);
	}

    protected IEnumerator OnLookAt(Transform target)
    {
        yield return null;
    }

	public virtual void TakeDamage(float dmg, bool stopTargetAfterAttack, EAttackType attackType = EAttackType.WoodenBolt, EAttackSideEffect sideEffect = EAttackSideEffect.NONE, string attackCodeName = "", int attackLevel = -1, bool critted = false, int damageType = 0) // ?
    {
	    if (attackType == boltVulni)
	    {
		    dmg *= 2;
	    }
        dmg = Mathf.Floor(dmg);
        if (!isAlive) return;
        
        // CREATE FLOATING DAMAGE 
		//Debug.Log(this.gameObject.name + " damage: " + dmg + "  health: " + this.healthPoints);
        statusBar.healthPoints -= dmg;
        currentDamagaTriggerLevel += BlockMovement ? 0 : dmg;
        CreateDamageTakenText(dmg, critted, damageType);

        if (statusBar.healthPoints <= 0.0f)
        {
            //OnDie_Invoke ();
			SetTransition(StateID.Death); //Transition.DeadlyHit);
        }
        else
        {
            CancelInvoke("MakeAttack");
            OnDamageRecorded_Invoke();

            isCurrentAttackAnimation = false;
            if (finiteStateMachine.CurrentStateID != StateID.AttackingTower && finiteStateMachine.CurrentStateID != StateID.AttackingPlayer && stopTargetAfterAttack)
            {
				SetTransition(StateID.StopMovement, null, new FSMStopMovementStateArgs(animator.GetCurrentClipLength()));
				//Invoke("OnHitEndAfterStop", animator.GetCurrentClipLength());
            }
            /*else
            {
                Invoke("OnHitEnd", animator.GetCurrentClipLength());
            }*/
            if((currentDamagaTriggerLevel >= animationTriggerDamageLevel || stopTargetAfterAttack) && !isCurrentHitAnimation)
            {
                isCurrentHitAnimation = true;
                currentDamagaTriggerLevel = 0;
                OnTakeDamage_Invoke();
                if (finiteStateMachine.CurrentState is EnemyMovementState)
                    NavigationController.Disable();
                Invoke("OnHitEnd", animator.GetCurrentClipLength());
            }

            if (sideEffect != EAttackSideEffect.NONE)
				StartCoroutine(StartSideEffect(sideEffect, attackCodeName, attackLevel));
			
        }
    }

	protected virtual IEnumerator StartSideEffect(EAttackSideEffect sideEffect, string attackCodeName = "", int attackLevel = -1)
	{
		float sideEffectDamage = XMLItemsReader.GetSideEffectDamage(attackCodeName, attackLevel);
		int sideEffectDamageCount = XMLItemsReader.GetSideEffectDamageCount(attackCodeName, attackLevel);
		float sideEffectRateTime = XMLItemsReader.GetSideEffectRateTime(attackCodeName, attackLevel);

		EnemyParticlesContainer particlesContainer = (Resources.Load("EnemyParticlesContainer") as GameObject).GetComponent<EnemyParticlesContainer>();
		if (particlesContainer != null)
		{
			GameObject sideEffectRef = null;
			switch(sideEffect)
			{
			case EAttackSideEffect.Fire:
				sideEffectRef = particlesContainer.GetBurnSideEffect();
				break;
			case EAttackSideEffect.Ice:
				float attackDuration = XMLItemsReader.GetDurationValue(attackCodeName, attackLevel);
				this.SetTransition(StateID.Frozen, null, new FSMFrozenStateArgs(attackDuration));
				sideEffectRef = particlesContainer.GetFrozenSideEffect();
				break;
			case EAttackSideEffect.Poison:
				//TODO: effect otrucia
				break;
			}
			GameObject sideEffectGO = GameObject.Instantiate(sideEffectRef, this.transform.position, Quaternion.identity, null);
			sideEffectGO.transform.SetParent(baseMesh != null ? baseMesh.transform : this.transform);
		}

		while(sideEffectDamageCount > 0)
		{
			yield return new WaitForSeconds(sideEffectRateTime);
			this.TakeDamage(sideEffectDamage, false);
			sideEffectDamageCount--;
		}
	}

    private void CreateDamageTakenText(float dmg, bool crit, int damageType = 0)
    {
        TakenDamageText takenDamage = Instantiate(Resources.Load<TakenDamageText>("TakenDamage"), Vector3.zero, Quaternion.identity);
        takenDamage.Init(this, this.gameObject, dmg, GetAIHeight() + 0.4f);
        
        if (crit)
	        damageType = 2;
        
        takenDamage.SetDamageType(damageType);
    }

    protected void OnHitEnd()
	{
		isCurrentHitAnimation = false;
        if(finiteStateMachine.CurrentState is EnemyMovementState)
            NavigationController.Enable();
    }
		
	/*protected void OnHitEndAfterStop()
	{
		NavigationController.Enable ();
		finiteStateMachine.CurrentState.DoBeforeEntering ();
		isCurrentHitAnimation = false;
	}*/

    protected void Died() // odpalic w ktoryms ze stanow
    {
        // usunac zycie
        isAlive = false;

        foreach(var virgin in carryVirginList)
        {
            virgin.OnDrop_Invoke();
        }
     
        CancelInvoke ();
		StartCoroutine (OnStartTrashing (4f));
        // get all colliders and turn off
		TurnOffColliders();
        NavigationController.Disable();

		if(this.enemyType != EEnemyType.Dragon && this.enemyType != EEnemyType.Troll && this.enemyType != EEnemyType.Ymir && this.enemyType != EEnemyType.Trex)
		{
			this.RemoveChildRigidbodies();
			this.CreateSimpleRagdoll();
		}

		if (this.enemyType == EEnemyType.Dragon) {
			Invoke("EnableRigidbody", 0.1f);
			//this.EnableRigidbody();
			//print("dragon dead");
		}

        // dolaczyc skrypt ktory po czasie zeskaluje obiekt i usunie ze sceny jak i z listy
        Debug.LogFormat("Enemy {0} died.", this.name);
        OnDeath.Invoke();
    }

	protected virtual void EnableRigidbody() {
		var collider = GetComponentsInChildren<CapsuleCollider>();
		var animator = GetComponentInChildren<Animator>();
		if (animator != null)
			animator.enabled = false;
		foreach (var item in collider)
		{
			item.enabled = true;
			item.gameObject.AddComponent<Rigidbody>();
		}
			
	}

	protected virtual void CreateSimpleRagdoll()
	{
		Rigidbody myRigidbody = this.GetComponent<Rigidbody>();

		Transform b_root = null;
		for(int i=0; i<this.transform.childCount; i++)
		{
			if(b_root == null)
			{
				b_root = this.transform.GetChild(i).Find("b_root");
			}
		}

		PhysicMaterial physicMaterial = new PhysicMaterial("HighResistance");
		physicMaterial.dynamicFriction = physicMaterial.staticFriction = 0.85f;
		physicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
		physicMaterial.bounciness = 0.2f;
		physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;

		if(b_root != null)
		{
			Rigidbody b_rootRigid = b_root.gameObject.AddComponent<Rigidbody>();

			Transform b_klata = b_root.Find("b_klata");
			SetCharacterJoint(b_klata, b_rootRigid, -20f, 20f, 10f);

			BoxCollider b_klataCollider = b_klata.gameObject.AddComponent<BoxCollider>();
			b_klataCollider.material = physicMaterial;
			float b_klataToGlowaDistance = b_klata.InverseTransformVector(b_klata.transform.position-b_klata.GetChild(0).position).magnitude;
			b_klataCollider.size = new Vector3(b_klataToGlowaDistance*0.8f, 1f, 1f);

			Transform b_udo_l = b_root.transform.Find("b_udo_l");
			SetCharacterJoint(b_udo_l, b_rootRigid, -20f, 70f, 30f);
			SetCapsuleCollider(b_udo_l, 0.4f, physicMaterial, 2f);

			Transform b_udo_p = b_root.transform.Find("b_udo_p");
			SetCharacterJoint(b_udo_p, b_rootRigid, -20f, 70f, 30f);
			SetCapsuleCollider(b_udo_p, 0.4f, physicMaterial, 2f);

			Transform b_ramie_l = b_klata.Find("b_ramie_l");
			SetCharacterJoint(b_ramie_l, b_klata.gameObject.GetComponent<Rigidbody>(), -70f, 10f, 50f);
			SetCapsuleCollider(b_ramie_l, 0.35f, physicMaterial, 1.5f);

			Transform b_ramie_p = b_klata.Find("b_ramie_p");
			SetCharacterJoint(b_ramie_p, b_klata.gameObject.GetComponent<Rigidbody>(), -70f, 10f, 50f);
			SetCapsuleCollider(b_ramie_p, 0.35f, physicMaterial, 1.5f);

			Transform b_glowa = b_klata.Find("b_glowa");
			SetCharacterJoint(b_glowa, b_klata.gameObject.GetComponent<Rigidbody>(), -40f, 25f, 25f);
			SphereCollider glowaCollider = b_glowa.gameObject.AddComponent<SphereCollider>();
			glowaCollider.center = new Vector3(-0.4f, 0f, 0f);
			glowaCollider.radius = 0.5f;

			//wylacz animatora, zeby ragdoll mogl dzialac
			List<Animator> animators = animator.GetAnimators();
			foreach(Animator currentAnimator in animators)
				currentAnimator.enabled = false;

			if(myRigidbody != null)
				Destroy(myRigidbody);
		}else{
			GameObject colliderChild = new GameObject("CapsuleDeathCollider");
			colliderChild.transform.SetParent(this.transform, false);
			colliderChild.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			CapsuleCollider deathCollider = colliderChild.AddComponent<CapsuleCollider>();
			deathCollider.radius = this.GetAIHeight()*0.2f;
			deathCollider.height = this.GetAIHeight();
			deathCollider.material = physicMaterial;

			if(myRigidbody != null)
			{
				myRigidbody.isKinematic = false;
				myRigidbody.angularDrag = 4f;
			}
		}
			
	}

	private void SetCharacterJoint(Transform jointTransform, Rigidbody connectedBody, float lowTwistLimit, float highTwistLimit, float swing1Limit)
	{
		CharacterJoint joint = jointTransform.gameObject.AddComponent<CharacterJoint>();
		joint.connectedBody = connectedBody;
		SoftJointLimit lowTwist = joint.lowTwistLimit;
		lowTwist.limit = lowTwistLimit;
		SoftJointLimit highTwist = joint.highTwistLimit;
		highTwist.limit = highTwistLimit;
		SoftJointLimit swing1 = joint.swing1Limit;
		swing1.limit = swing1Limit;
	}

	private void SetCapsuleCollider(Transform jointTransform, float radius, PhysicMaterial material, float heightFactor = 1f)
	{
		CapsuleCollider capsuleCollider = jointTransform.gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.direction = 0;
		capsuleCollider.radius = radius;
		capsuleCollider.material = material;
		capsuleCollider.height = jointTransform.InverseTransformVector(jointTransform.transform.position-jointTransform.GetChild(0).position).magnitude * heightFactor;
		capsuleCollider.center = new Vector3(-capsuleCollider.height*0.5f, 0f, 0f);
	}

	protected virtual void RemoveChildRigidbodies()
	{
		for(int i=0; i<this.transform.childCount; i++) //usun area destruktory
		{
			Rigidbody[] childrenRigidbodies = this.transform.GetChild(i).GetComponentsInChildren<Rigidbody>();
			for(int j=0; j<childrenRigidbodies.Length; j++)
				Destroy(childrenRigidbodies[j].gameObject);
		}
	}

	protected override IEnumerator Trashing()
	{
		EnemyParticlesContainer particlesContainer = (Resources.Load("EnemyParticlesContainer") as GameObject).GetComponent<EnemyParticlesContainer>();
		if (particlesContainer != null && baseMesh != null) {
			GameObject deathParticlesGO = GameObject.Instantiate (particlesContainer.GetDeathParticles (), this.transform.position, Quaternion.identity, null);
			deathParticlesGO.transform.SetParent (this.transform);

			ParticleSystem deathParticles = deathParticlesGO.GetComponentInChildren<ParticleSystem> ();
			if (deathParticles != null) {
				ParticleSystem.ShapeModule shapeModule = deathParticles.shape;
				shapeModule.skinnedMeshRenderer = baseMesh.GetComponent<SkinnedMeshRenderer> ();

				yield return new WaitForSeconds (1f);

				deathParticlesGO.transform.SetParent (null);
				//schowaj kolesia, mozna przerobic na cos lepszego
				this.transform.position = Vector3.down * 10000f;
			}
		} 

		yield return new WaitForSeconds(1f);
		Destroy(this.gameObject, 3f);

		ObjectPool.Instance.RemoveEnemy(this, true);
	}

    public void Teleport()
    {
        isAlive = false;
        foreach (var virgin in carryVirginList)
        {
            virgin.OnDrop_Invoke();
        }
        CancelInvoke();
		ShowTeleportEffect ();
        statusBar.OnDie();
        TurnOffColliders();
		StartCoroutine(HideEffect());
		ObjectPool.Instance.RemoveEnemy(this, false);
    }

	public void ShowTeleportEffect()
	{
		EnemyParticlesContainer particlesContainer = (Resources.Load("EnemyParticlesContainer") as GameObject).GetComponent<EnemyParticlesContainer>();
		if (particlesContainer != null)
		{
			GameObject sideEffectGO = GameObject.Instantiate(particlesContainer.GetTeleportEffect(), this.transform.position + Vector3.up*GetAIHeight(), Quaternion.identity, null);
		}
	}

	private IEnumerator HideEffect()
	{
		//yield return new WaitForSeconds(0.4f);

		//float initScaleFactor = transform.localScale.x;
		//float trashingTime = 0.25f;
		//while (trashingTime > 0f)
		//{
		//	trashingTime -= Time.deltaTime;
		//	transform.localScale = new Vector3(initScaleFactor, initScaleFactor * trashingTime * 4f, initScaleFactor);
		//	yield return null;
		//}

		yield return new WaitForSeconds(1f);
		Destroy(this.gameObject);
	}


    protected void TurnOffColliders ()
    {
        List<Collider> colliders = this.gameObject.FindComponentsInChild<Collider>();
        Collider parentCollider = this.gameObject.GetComponent<Collider>();
        if (parentCollider != null)
            colliders.Add(parentCollider);
        foreach (Collider c in colliders)
			c.enabled = false;

        //aiController.navMeshAgent.enabled = false;
    }

	protected void ChargeAttack()
	{
		if(attackIntervalCounter >= attackInterval && isAlive)
		{
			isAttackCharged = true;
		}
		attackIntervalCounter += Time.deltaTime;
	}
		
    public virtual void Attack()
    {
		if (isAttackCharged && isCurrentHitAnimation == false && isCurrentAttackAnimation == false) {
			isCurrentAttackAnimation = true;

			StartAnimationAttack ();
			Invoke ("MakeAttack", timeForAttackEvent);
		}
    }
	public virtual void StartAnimationAttack()
	{
		OnAttack_Invoke ();
	}

	public virtual void MakeAttack()
	{
		ResetAttack ();
		if(attackAudioClips.Length > 0)
		{
			int audioClipIndex = UnityEngine.Random.Range(0, attackAudioClips.Length);
			SoundManager.instance.PlaySceneEffect(attackAudioClips[audioClipIndex], gameObject.transform.position, -1f, 0f, 0.5f);
		}

		if (finiteStateMachine.CurrentStateID == StateID.AttackingTower) 
		{
			if (targetTowerBase != null)
				targetTowerBase.TakeDamage (attackPower);
		} 
		else if (finiteStateMachine.CurrentStateID == StateID.AttackingPlayer) {
			ObjectPool.Instance.player.TakeDamage (attackPower, gameObject);
		}	
	}

	public virtual void StopMovement()
	{
		NavigationController.Disable ();
		Animator.SetAnimatorBool("Walk", false);
		Animator.SetAnimatorBool("Run", false);
		Animator.SetAnimatorBool("Idle", true);
	}

	protected void ResetAttack()
	{
		attackIntervalCounter = 0f;
		isAttackCharged = false;
		isCurrentAttackAnimation = false;
	}

	public void EnableAttack(bool enable)
	{
		isAttackEnabled = enable;
	}
	public bool IsAttackEnabled()
	{
		return isAttackEnabled;
	}

	public int GetKillGoldBonus()
	{
		return killGoldBonus;
	}

	public int GetKillExpBonus()
	{
		return killExpBonus;
	}

	public void StopAllowAttackPlayer()
	{
		allowAttackPlayer = false;
	}

	public bool AllowAttackPlayer()
	{
		return allowAttackPlayer;
	}

	public void OnWalkEvent()
	{
		if(moveAudioClips.Length > 0)
		{			
			int audioClipIndex = UnityEngine.Random.Range(0, moveAudioClips.Length);
			SoundManager.instance.PlaySceneEffect(moveAudioClips[audioClipIndex], gameObject.transform.position, -1f, 0f, 0.5f);
		}
	}

	public void OnAttackAnimationEvent()
	{
		Debug.Log ("attack evnet");
	}

    #endregion Logic

    protected override void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.up * 3, Color.blue);
#if UNITY_EDITOR
        if (finiteStateMachine != null && finiteStateMachine.CurrentState != null)
            UnityEditor.Handles.Label(transform.position + Vector3.up * 3, string.Format("State: {0}", finiteStateMachine.CurrentState.GetType().Name));
#endif
        var forwardPosition = transform.position + transform.forward;
        Debug.DrawLine(transform.position + transform.right, forwardPosition, Color.yellow);
        Debug.DrawLine(transform.position - transform.right, forwardPosition, Color.yellow);

        var color = Gizmos.color;
        if(NavigationController is AISplinePath)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * 2), .5f);
        Gizmos.color = color;
    }

    public bool CheckHasEmptyPivotsForVirgin()
    {
        bool hasEnemyEmptyPivotForVirgin = false;
        foreach (var pivot in carryPivotList)
        {
            if (pivot.childCount == 0)
                hasEnemyEmptyPivotForVirgin = true;
        }
        return hasEnemyEmptyPivotForVirgin;
    }
}
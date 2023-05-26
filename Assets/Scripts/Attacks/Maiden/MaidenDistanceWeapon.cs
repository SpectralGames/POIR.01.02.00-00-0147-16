using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaidenDistanceWeapon : MonoBehaviour
{
    public AudioClip[] hitAudioClips;
	public float quadraticOffsetFactor = 20f;
	public EMaidenWeaponType weaponType;
	public bool stopTargetAfterAttack;

	protected int damage;
	protected int weaponLevel;
	private float speed = 10;
    private Vector3 targetPosition;
	private Collider mainCollider;
	private List<Enemy> enemiesHit;
	private bool soundPlayed;
	protected GameObject shooterReference;

	void Awake()
	{
		//Init ();
	}

	public void Init(int level)
	{
		weaponLevel = level;
		string currentItemCodeName = weaponType.ToString();
		speed = XMLItemsReader.GetSpeedValue(currentItemCodeName, weaponLevel);
		damage = (int) XMLItemsReader.GetDamageValue(currentItemCodeName, weaponLevel);	
	}


	public void OnShoot(Vector3 targetPosition,  float delay, float quadraticOffset = 20) // 
    {
        this.targetPosition = targetPosition;
		this.quadraticOffsetFactor = quadraticOffset;
		this.enemiesHit = new List<Enemy> ();
		mainCollider = GetComponent<Collider> ();
		Invoke("ShootAfterDelay", delay);
		soundPlayed = false;
    }

	private void ShootAfterDelay()
	{
		StartCoroutine("ShootAnimation");
	}

    IEnumerator ShootAnimation()
    {
        float currentFlyTime = 0f;
        Vector3 initDistance = this.targetPosition - this.transform.position;
        Vector3 initArrowPosition = this.transform.position;


		float distanceFlyTime =Vector3.Distance (this.targetPosition, this.transform.position) / speed;  //flyTime * Mathf.Clamp(initDistance.magnitude, 50f, 140f) * 0.015f;
		while (currentFlyTime < distanceFlyTime)
        {
            currentFlyTime += Time.deltaTime;
			float quadraticFactor = currentFlyTime / distanceFlyTime - 0.5f;
			Vector3 quadraticOffset = Vector3.zero;

			bool lookRotation = true;
			quadraticOffset = -(quadraticFactor * quadraticFactor) * (Vector3.up*2) * quadraticOffsetFactor + (Vector3.up*2) * (quadraticOffsetFactor * 0.25f); //ruch po paraboli

			Vector3 newPosition = initArrowPosition + (initDistance * currentFlyTime / distanceFlyTime) + quadraticOffset;
		
			if(lookRotation)
				this.transform.rotation = Quaternion.LookRotation(newPosition - this.transform.position); //* Quaternion.Euler(90f, 0f, -90f);
			else
				this.transform.localRotation *= Quaternion.Euler(new Vector3(0f, 0f, distanceFlyTime*1.5f) * 90f * Time.deltaTime);
            this.transform.position = newPosition;

            yield return null;
        }

		yield return new WaitForFixedUpdate ();

		//StartCoroutine (TurnOffColliderAfterDelay(0.25f));
		this.OnTargetHit(); //wywolaj trafienie strzala
    }

    void OnTriggerEnter(Collider other)
    {
		bool hitPositive = CheckCollistionWithTarget (other);

		if(hitPositive && enemiesHit.Count == 1)
       		this.OnTargetHit(); //wywolaj trafienie strzala po pierwszej kolizji z wrogiem
    }

	private bool CheckCollistionWithTarget(Collider other) 
	{ 
		Enemy enemy = other.GetComponent<Enemy>();
		if (enemy == null) {
			enemy = other.GetComponentInParent<Enemy> ();
		}

		if (enemy != null)
		{
			if (enemiesHit.Contains (enemy))
				return false;
			else
				enemiesHit.Add (enemy);

			enemy.TakeDamage(this.damage, stopTargetAfterAttack);
			return true;
		}
		return false;
	}


    private void OnTargetHit()
    {
		for(int i=0; i<this.transform.childCount; i++)
		{
			MeshRenderer childMesh = this.transform.GetChild(i).GetComponent<MeshRenderer>();
			if(childMesh != null)
				childMesh.enabled = false;
		}

       	PlayHitAudioClip();

		this.Invoke("DeactivateCollider", 0.25f);
		Invoke("OnDestroy", 2.5f);
		StopCoroutine("ShootAnimation");
    }

	private void DeactivateCollider()
	{
		this.mainCollider.enabled = false;
	}

    public void OnDestroy()
    {
       Destroy(this.gameObject);
    }

    private void PlayHitAudioClip()
    {
		if(soundPlayed) 
			return; 
		else 
			soundPlayed = true;
		
        if (hitAudioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, hitAudioClips.Length);
     //       SoundManager.instance.PlaySceneEffect(hitAudioClips[randomIndex], gameObject.transform.position);
        }
    }

	public void SetShooterReference(GameObject shooter)
	{
		shooterReference = shooter;
	}



}

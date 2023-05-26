using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDistanceWeapon : MonoBehaviour {

	public AudioClip[] hitAudioClips;
	public float quadraticOffsetFactor = 20f;
	public float rotationFactor = 90f;
	public bool stopTargetAfterAttack;
	public Vector3 offset = Vector3.zero;
	public int damage;
	public float speed = 10;
	public GameObject effectOnActivate;
	public bool lookRotation = true;
	private Vector3 targetPosition;
	private Collider mainCollider;
	private List<Collider> collidersChecked;
	private bool soundPlayed;
	protected GameObject shooterReference;


	public void OnShoot(Vector3 targetPosition, float delay, float quadraticOffset = 20) // 
	{
		this.targetPosition = targetPosition;
		//this.quadraticOffsetFactor = quadraticOffset;
		this.collidersChecked = new List<Collider> ();
		mainCollider = GetComponent<Collider> ();
		Invoke("ShootAfterDelay", delay);
		soundPlayed = false;
	}

	private void ShootAfterDelay()
	{
		StartCoroutine(ShootAnimation());
	}

	IEnumerator ShootAnimation()
	{
		float currentFlyTime = 0f;
		Vector3 initDistance = (this.targetPosition - this.transform.position) + offset;
		Vector3 initArrowPosition = this.transform.position;

		float distanceFlyTime = Vector3.Distance (this.targetPosition, this.transform.position) / speed;

		while (currentFlyTime < distanceFlyTime)
		{
			currentFlyTime += Time.deltaTime;
			float quadraticFactor = currentFlyTime / distanceFlyTime -0.5f;
			Vector3 quadraticOffset = Vector3.zero;

			quadraticOffset = -(quadraticFactor * quadraticFactor) * (Vector3.up) * quadraticOffsetFactor + (Vector3.up) * (quadraticOffsetFactor * 0.25f); //ruch po paraboli

			//quadraticOffset = Vector3.up*1 * quadraticFactor * quadraticOffsetFactor; //ruch po paraboli

			Vector3 newPosition = initArrowPosition + (initDistance * currentFlyTime / distanceFlyTime) + quadraticOffset;

			if(lookRotation)
				this.transform.rotation = Quaternion.LookRotation(newPosition - this.transform.position); //* Quaternion.Euler(90f, 0f, -90f);
			else
				this.transform.localRotation *= Quaternion.Euler(new Vector3(0f, 0f, distanceFlyTime*1.5f) * rotationFactor * Time.deltaTime);
			this.transform.position = newPosition;

			yield return null;
		}

		yield return new WaitForFixedUpdate ();

		//StartCoroutine (TurnOffColliderAfterDelay(0.25f));
		this.OnTargetHit(); //wywolaj trafienie strzala
	}

	void OnTriggerEnter(Collider other)
	{
		if (collidersChecked.Contains (other)) {
			return;
		} else
			collidersChecked.Add (other);

		CheckCollistionWithTarget (other);

		this.OnTargetHit(); //wywolaj trafienie strzala
	}

	private void CheckCollistionWithTarget(Collider other)
	{
		PlayerController player = other.GetComponent<PlayerController>();
		if (player == null) 
			player = other.GetComponentInParent<PlayerController> ();		

		if (player != null)
			player.TakeDamage(this.damage, shooterReference);


		TowerBase tower = other.GetComponent<TowerBase>();
		if (tower == null) 
			tower = other.GetComponentInParent<TowerBase> ();

		if (tower != null)
			tower.TakeDamage(this.damage);

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

		Invoke("OnDestroy", 2.5f);
		this.Invoke("DeactivateCollider", 0.25f);

		if (effectOnActivate != null) {
			GameObject effect = GameObject.Instantiate (effectOnActivate, transform.position, Quaternion.identity) as GameObject;
			Destroy (effect, 5f);
		}
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

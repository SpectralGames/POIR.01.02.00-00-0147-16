using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObjectFractureElement : DestroyableObjectColliderProxy 
{
	public override void OnTakeDamage(Vector3 impactPosition, Vector3 impactDirection, float impactForce, float impactRadius)
	{
		if(isDestroyed) return;

		currentHealth -= impactForce;
		if(currentHealth <= 0f)
		{
			this.StartCoroutine(Destroy(impactPosition, impactDirection, impactForce));
		}

		if(impactRadius > 0f)
			parentDestroyableObject.OnFractureElementTakeDamage(impactPosition, impactDirection, impactForce, impactRadius);
	}

	private IEnumerator Destroy(Vector3 impactPosition, Vector3 impactDirection, float impactForce)
	{
		isDestroyed = true;
		yield return null;

		parentDestroyableObject.OnFractureElementDestroyed(this); //zobacz, czy nad elementem sa inne elementy wiszace do zniszczenia

		Rigidbody elementRigid = this.GetComponent<Rigidbody>();
		elementRigid.isKinematic = false;
		elementRigid.AddForceAtPosition(impactDirection * impactForce * 10f + Vector3.up * impactForce * 7f, impactPosition);

		this.GetComponent<Collider>().enabled = false;
		this.Invoke("OnReEnableCollider", 0.5f);
		StartCoroutine(StartTrashing());
	}

	private void OnReEnableCollider()
	{
		this.GetComponent<Collider>().enabled = true;
	}

	IEnumerator StartTrashing()
	{
		yield return new WaitForSeconds(5f);
		this.GetComponent<Rigidbody>().isKinematic = true;
		this.GetComponent<Collider>().enabled = false;

		Vector3 initScale = this.transform.localScale;

		float currentTime = 0f;
		float animTime = 0.3f;
		while(currentTime <= animTime)
		{
			currentTime += Time.deltaTime;
			float animFactor = Mathf.Clamp01(currentTime/animTime);

			this.transform.localScale = Vector3.Lerp(initScale, Vector3.zero, animFactor);
			yield return null;
		}

		Destroy(this.gameObject);
	}

	public void OnElementCollapsed()
	{
		isDestroyed = true;
		parentDestroyableObject.OnFractureElementDestroyed(this);

		Rigidbody elementRigid = this.GetComponent<Rigidbody>();
		elementRigid.isKinematic = false;
		elementRigid.AddForceAtPosition(new Vector3(Random.insideUnitSphere.x, 0.6f, Random.insideUnitSphere.z) * 50f, this.GetComponent<Collider>().bounds.center);
		elementRigid.AddTorque(Random.insideUnitCircle * 7f);

		this.GetComponent<Collider>().enabled = false;
		this.Invoke("OnReEnableCollider", 1f);
		StartCoroutine(StartTrashing());
	}
}

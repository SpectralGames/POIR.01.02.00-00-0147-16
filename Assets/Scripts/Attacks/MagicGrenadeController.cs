using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicGrenadeController : AttackBaseController 
{
	bool isBeingThrown = false;

    public Transform colliderTransitionParent;
    public float colliderInTransitionSize;
	protected override void Awake () 
	{
		base.Awake();
		this.OnInit(Vector3.zero, Vector3.zero, 1);

	}

	public override void OnInit (Vector3 target, Vector3 targetNormal, int level = 0) 
	{
		base.OnInit(target, targetNormal, level);
		isBeingThrown = false;
		isTransitioning = false;
		colliderActiveWhileInTransition.SetActive(false);
		/*
		colliderActiveWhileInTransition.layer = LayerMask.NameToLayer("Attacks");
		colliderActiveWhileInTransition.SetActive(false);
		myCollider.radius = 0.01f;
     //   colliderActiveWhileInTransition.transform.SetParent(colliderTransitionParent);
         if(colliderTransitionParent != null)
             colliderActiveWhileInTransition.transform.localPosition = colliderTransitionParent.transform.localPosition;
         
        colliderActiveWhileInTransition.GetComponent<SphereCollider>().radius = colliderInTransitionSize;


        this.GetComponent<Rigidbody>().isKinematic = true;
		this.GetComponent<Rigidbody>().useGravity = true;
		*/
	}

	public void OnThrowObject()
	{
		myCollider.enabled = false;
		if (myBoxCollider)
			myBoxCollider.enabled = false;
		
		Invoke("ActivateGrenadeCollider", .1f);
	}

	private void ActivateGrenadeCollider()
	{
		isBeingThrown = true;
		if(colliderActiveWhileInTransition != null)
			colliderActiveWhileInTransition.SetActive(true);
	}
	protected override void OnInTransitionTriggerEnter(Collider other)
	{
		if(isBeingThrown)
		{
			GameObject myBody = movingObject != null ? movingObject : this.gameObject;
			isTransitioning = false;
			targetPosition = other.ClosestPoint(myBody.transform.position); 
			targetNormal = (myBody.transform.position - targetPosition).normalized;

			this.OnActivate();
			isBeingThrown = false;
			Destroy(colliderActiveWhileInTransition);
		}
	}

	/*
	protected override void OnTriggerEnter(Collider other) 
	{
        if (isActive == false)
			return;

        base.OnTriggerEnter(other);
	}*/

	protected override void OnActivate()
	{
        myCollider.radius = attackRadius;
		base.OnActivate();
	}
}

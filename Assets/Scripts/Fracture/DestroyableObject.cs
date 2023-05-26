using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableObject : MonoBehaviour 
{
	public GameObject intactObject, destroyedObject;
	public GameObject parentFracture;
	public float intactObjectHealth, fractureElementHealth;

    public UnityEvent OnCrackCallback = new UnityEvent();

	// Use this for initialization
	void Awake () 
	{
		intactObject.SetActive(true);
		parentFracture.SetActive(false);
		if(destroyedObject != null) destroyedObject.SetActive(false);

		DestroyableObjectColliderProxy destroyableProxy = intactObject.AddComponent<DestroyableObjectColliderProxy>();
		destroyableProxy.OnInit(this, intactObjectHealth);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    public void DestoryTrigger()
    {
        intactObject.GetComponent<DestroyableObjectColliderProxy>().OnTakeDamage(transform.position, Vector3.zero, 1000,1);
    }


	//Glowny kawalek zostal uszkodzony, rozbij go na mniejsze kawalki, dodaj collidery, rigidbody, itd
	public void OnIntactObjectCracked(Vector3 impactPosition, Vector3 impactDirection, float impactForce, float impactRadius)
	{
        OnCrackCallback.Invoke();

        intactObject.SetActive(false);
		parentFracture.SetActive(true);
		if(destroyedObject != null)
			destroyedObject.SetActive(true);

		for(int i=0; i<parentFracture.transform.childCount; i++)
		{
			GameObject element = parentFracture.transform.GetChild(i).gameObject;
			Rigidbody elementRigid = element.AddComponent<Rigidbody>();
			elementRigid.isKinematic = true;
			elementRigid.Sleep();

			MeshCollider elementCollider = element.AddComponent<MeshCollider>();
			elementCollider.convex = true;

			DestroyableObjectFractureElement destroyableFractureElement = element.AddComponent<DestroyableObjectFractureElement>();
			destroyableFractureElement.OnInit(this, fractureElementHealth);
		}

		for(int i=0; i<parentFracture.transform.childCount; i++)
		{
			this.DistributeElementDamage(parentFracture.transform.GetChild(i).GetComponent<DestroyableObjectFractureElement>(), impactPosition, impactDirection, impactForce, impactRadius);
		}
	}

	//jeden z kawalkow oberwal, sprobuj rozlozyc damage po innych kawalkach w promieniu uderzenia
	public void OnFractureElementTakeDamage(Vector3 impactPosition, Vector3 impactDirection, float impactForce, float impactRadius)
	{
		for(int i=0; i<parentFracture.transform.childCount; i++)
		{
			DestroyableObjectFractureElement destroyableFractureElement = parentFracture.transform.GetChild(i).GetComponent<DestroyableObjectFractureElement>();
			this.DistributeElementDamage(destroyableFractureElement, impactPosition, impactDirection, impactForce, impactRadius);
		}
	}

	//rozloz damage, ktory wynika z trafienia w inny kawalek w promieniu impactRadius
	private void DistributeElementDamage(DestroyableObjectFractureElement element, Vector3 impactPosition, Vector3 impactDirection, float impactForce, float impactRadius)
	{
		float distanceToElement = Vector3.Distance(element.GetComponent<Collider>().bounds.center, impactPosition);
		if(distanceToElement < impactRadius)
		{
			float distanceFactor = Mathf.Clamp01(1f/distanceToElement);
			element.OnTakeDamage(impactPosition, impactDirection, impactForce * distanceFactor, -1f); //nie wywoluj dalszych zniszczen wokolo
		}
	}


	//sprawdz, czy nad zniszczonym elementem sa jakies wiszace elementy
	public void OnFractureElementDestroyed(DestroyableObjectFractureElement destroyedElement)
	{
		Collider destroyedElementCollider = destroyedElement.GetComponent<Collider>();
		List<GameObject> elementsTouching = new List<GameObject>();

		for(int i=0; i<parentFracture.transform.childCount; i++)
		{
			GameObject currentElement = parentFracture.transform.GetChild(i).gameObject;
			Collider currentElementCollider = currentElement.GetComponent<Collider>();

			if(currentElement.GetComponent<DestroyableObjectFractureElement>().IsDestroyed() || destroyedElement.transform == currentElement.transform)// || 
				//destroyedElementCollider.bounds.center.y > currentElementCollider.bounds.center.y) //jesli zniszczony element jest wyzej, to ignoruj kawalek
				continue;
			
			Vector3 positionOnDestroyedSurface = destroyedElementCollider.ClosestPoint(currentElementCollider.bounds.center);
			Vector3 positionOnCurrentElement = currentElementCollider.ClosestPoint(positionOnDestroyedSurface);
			Vector3 diff = positionOnCurrentElement - positionOnDestroyedSurface;
			if(Vector3.Angle(diff, Vector3.up) < 70f && diff.magnitude < 0.8f)
			{
				elementsTouching.Add(currentElement);
				//Debug.Log(destroyedElement.name + " touches " + currentElement.name + " distance " + Vector3.Distance(positionOnCurrentElement, positionOnDestroyedSurface));
			}
		}

		foreach(GameObject element in elementsTouching)
		{
			element.GetComponent<DestroyableObjectFractureElement>().OnElementCollapsed();
		}
	}

}

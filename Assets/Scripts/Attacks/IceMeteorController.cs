using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMeteorController : AttackBaseController 
{
	public Transform rock2Parent;

	public override void OnInit (Vector3 target, Vector3 targetNormal, int level = 0) 
	{
		base.OnInit(target, targetNormal, level);
		this.transform.forward = new Vector3(this.transform.forward.x, 0f, this.transform.forward.z);

		bool frontNormal = false;
		if(Vector3.Angle(Camera.main.transform.forward, targetNormal) > 120f) //jesli celujesz w sciane, przekrec mocniej atak
			frontNormal = true;

		this.transform.rotation *= frontNormal ? Quaternion.Euler(0f, Mathf.Lerp(150f, 275f, Random.value), 0f) : Quaternion.Euler(0f, Random.value*110f, 0f);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		StartCoroutine(Trashing());
	}

	IEnumerator Trashing()
	{
		yield return new WaitForSeconds(3f);
		float currentTime = 0f;
		float animTime = 0.5f;
		for(int i=0; i<rock2Parent.childCount; i++)
		{
			Collider collider = rock2Parent.GetChild(i).GetComponent<Collider>();
			if(collider != null)
				collider.enabled = false;
		}

		while(currentTime <= animTime)
		{
			currentTime += Time.deltaTime;
			float animFactor = Mathf.Clamp01(currentTime/animTime);

			for(int i=0; i<rock2Parent.childCount; i++)
			{
				MeshRenderer renderer = rock2Parent.GetChild(i).GetComponent<MeshRenderer>();
				if(renderer != null)
					renderer.transform.localScale *= 1f-animFactor;
			}
			yield return null;
		}
	}
}

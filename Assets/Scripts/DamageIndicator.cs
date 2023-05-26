using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour {

	public RectTransform canvasRect;
	public UIAlphaAnimator alphaAnimator;


	public void ShowPointer(Transform pointerTarget)
	{
		gameObject.SetActive(true);
		alphaAnimator.ClearValues ();
		alphaAnimator.StartAnimation ();

		SetPointerPosition (pointerTarget);
	}


	private void  SetPointerPosition(Transform pointerTarget, bool pulsingAnimation = false)
	{
		RectTransform pointerRect = GetComponent<RectTransform>();
		pointerRect.anchoredPosition = Vector2.zero;
		//MeshRenderer pointerTargetMesh = pointerTarget.GetComponent<MeshRenderer>();
	//	if(pointerTargetMesh == null) pointerTargetMesh = pointerTarget.GetComponentInChildren<MeshRenderer>();
		RectTransform targetRect = pointerTarget.GetComponent<RectTransform>();
		if(targetRect != null && targetRect.GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace) //jesli worldspace, traktuj jak normalny obiekt na scenie
			targetRect = null;
		Camera canvasCamera = Camera.main; //tutorialCanvasRect.GetComponent<Canvas>().worldCamera;

		Vector2 initScreenPosition = targetRect != null ? (Vector2)targetRect.position : Vector2.one;
		Vector2 initViewportPosition = targetRect != null ? new Vector2(initScreenPosition.x/canvasRect.sizeDelta.x, initScreenPosition.y/canvasRect.sizeDelta.y) :
			(Vector2)canvasCamera.WorldToViewportPoint(pointerTarget.transform.position);
		float initAngle = 60f; //initViewportPosition.x < 0.5f ? 60f : 0f;
		if(initViewportPosition.y < 0.5f) initAngle = -initAngle - 75f;
		pointerRect.localEulerAngles = new Vector3(0f, 0f, initAngle); //ustal kat

		float xFactor = initViewportPosition.x < 0.5f ? 7f : -7f; //parametry, ktore kontroluja kierunek animacji pointera w zaleznosci od pozycji na ekranie
		float yFactor = initViewportPosition.y < 0.5f ? 7f : -7f;

		if(pulsingAnimation == false) xFactor = yFactor = 0f;

		Vector3 viewportTargetPosition = Camera.main.WorldToViewportPoint(pointerTarget.transform.position);	
		if ( (viewportTargetPosition.x > 0f &&  viewportTargetPosition.x < 1f) && ( viewportTargetPosition.y > 0f &&  viewportTargetPosition.y < 1f) && viewportTargetPosition.z > 0f) //jesli przede mna 
		{
			gameObject.SetActive(false); 
		}
	
			
		//ustaw pozycje
		if(targetRect != null)
		{
			pointerRect.position = targetRect.position;
			pointerRect.anchoredPosition -= new Vector2(pointerRect.transform.up.x, pointerRect.transform.up.y) * Mathf.Sin(Time.realtimeSinceStartup*xFactor)*5f;//new Vector2(Mathf.Sin(Time.realtimeSinceStartup*xFactor), Mathf.Sin(Time.realtimeSinceStartup*yFactor))*4f;
		}else{
			Vector3 viewportPosition = canvasCamera.WorldToViewportPoint(pointerTarget.transform.position);

			Matrix4x4 cameraMatrix = canvasCamera.transform.worldToLocalMatrix;
			Vector3 cameraLocalObjectPosition = cameraMatrix.MultiplyPoint3x4(pointerTarget.transform.position);

			if(viewportPosition.z > 0f)
			{
				viewportPosition = new Vector3(Mathf.Clamp(viewportPosition.x, 0.35f, 0.65f), Mathf.Clamp(viewportPosition.y, 0.35f, 0.65f), viewportPosition.z);

			}else{
				Vector3 cameraLocalPosition = cameraLocalObjectPosition*0.5f + Vector3.one*0.5f; 
				cameraLocalPosition = new Vector3(Mathf.Clamp( cameraLocalPosition.x*2f, 0.35f, 0.65f), Mathf.Clamp( cameraLocalPosition.y*2f, 0.35f, 0.65f), viewportPosition.z);
				viewportPosition = cameraLocalPosition;
			}
	
			//ustal kat wzgledem kierunku
		Vector3 dir = (Vector2)viewportPosition - new Vector2 (0.5f, 0.5f);
			float angle = Mathf.Atan2 (dir.y * Mathf.Sign (dir.z), dir.x * Mathf.Sign (dir.z)) * Mathf.Rad2Deg;
			pointerRect.localEulerAngles = new Vector3 (0f, 0f, Mathf.LerpAngle (pointerRect.localEulerAngles.z, angle + 90, 1)); //ustal kat

			pointerRect.anchoredPosition = new Vector2 ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
				(viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) - new Vector2 (pointerRect.transform.up.x, pointerRect.transform.up.y) * Mathf.Sin (Time.realtimeSinceStartup * xFactor) * 5f; //new Vector2(Mathf.Sin(Time.realtimeSinceStartup*xFactor), Mathf.Sin(Time.realtimeSinceStartup*yFactor))*4f;

		}
	}


}

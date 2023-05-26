using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;

public class Szturchanko : MonoBehaviour 
{
    //public static void Invoke (int delay)
    //{
    //    Debug.Log("Started...");
    //    Action t = Test;
    //    Action delayAction = () => Thread.Sleep(delay);
    //    delayAction.BeginInvoke(IAsyncResult => t(), null);
    //}
    //private static void Test ()
    //{
    //    Debug.Log("WORKS");
    //}
	public Transform localRoomArea;
	public Transform centerEye;
	private int raycastLayerMask;

    // Use this for initializatio
    void Start ()
    {
		raycastLayerMask = LayerMask.GetMask("Default", "TowerTeleport", "Enemy");
		//centerEye = Camera.main.transform.GetChild(0);
        //Invoke(5000);
	}
		

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			//Vector3 worldPosition = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.CenterEye)) * InputTracking.GetLocalPosition(XRNode.CenterEye);
			//Vector3 worldDirection = InputTracking.GetLocalRotation(XRNode.CenterEye) * Camera.main.transform.forward;
			//Matrix4x4 m = Camera.main.cameraToWorldMatrix;
			//worldPosition = m.MultiplyPoint(worldPosition);

			Ray newRay = new Ray(centerEye.transform.position, centerEye.transform.forward); //Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane)); //new Ray(Camera.main.transform.position, Camera.main.transform.forward); //Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
			//Debug.DrawRay(newRay.origin, newRay.direction*5000f, Color.red, 4f);
			//newRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if(Physics.Raycast(newRay, out hitInfo, 5000f, raycastLayerMask))
			{
                Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(10.0f, false);
                }
					

				if(hitInfo.collider.GetComponent<DestroyableObjectColliderProxy>() != null)
				{
					hitInfo.collider.GetComponent<DestroyableObjectColliderProxy>().OnTakeDamage(hitInfo.point, hitInfo.normal, 20f, 1f);
				}
				if(hitInfo.collider.GetComponent<Rigidbody>() != null)
				{
					hitInfo.collider.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 500f);
				}
			}

			if(Physics.Raycast(newRay, out hitInfo, 5000f, raycastLayerMask))
			{
				if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("TowerTeleport"))
				{
					//Debug.LogWarning("Dupa" + hitInfo.collider.name);
					TowerTeleport towerTeleport = hitInfo.collider.GetComponent<TowerTeleport>();
					if(towerTeleport != null)
					{
						//towerTeleport.TeleportPlayer(this.localRoomArea, this.gameObject.GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().transform);
					}
				}
			}
		}
	}


}

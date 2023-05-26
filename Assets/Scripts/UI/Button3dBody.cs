using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button3dBody : MonoBehaviour {
	private Material materialToModify;

	private Button3d button3d;
	private Vector3 initLocalPosition = Vector3.zero;
	private Vector3 initForward;
	private bool isTouching = false;
	private bool isPushed = false;
	private bool isActive = true;
	private bool isEnabled = true;
	private Rigidbody myRigidbody;

	void Awake()
	{
		myRigidbody = this.GetComponent<Rigidbody>();
		initLocalPosition = this.transform.localPosition;
		initForward = this.transform.forward;
		//materialToModify = GetComponent<Renderer> ().material;// transform.Find("Bg").GetComponent<Renderer> ().material;
	}

	// Update is called once per frame
	public void OnSetButton3d(Button3d button3d)
	{
		this.button3d = button3d;
	}

	void OnDisable()
	{
		this.ResetButton();
	//	materialToModify.color = Color.white;
		this.transform.localPosition = initLocalPosition;
		//isActive = true;
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 5000)) {
				if (hit.collider.gameObject == gameObject) {
					if(isPushed == false)
						this.OnButtonDown();
				}			
			}

		}
		if (Input.GetMouseButtonUp (0)) 
		{
			if(isPushed == true)
			{
				this.OnButtonUp();
				this.ResetButton();
			}
			/*Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 5000)) {
				if (hit.collider.gameObject == gameObject) {
					materialToModify.color = Color.white;
					button3d.OnButtonClick ();
					this.ResetButton();
				}			
			}*/
		}
			

		if (isTouching == false) 
		{
			transform.localPosition += (initLocalPosition - transform.localPosition) * Time.unscaledDeltaTime * 10f;
			if(isActive == true && isPushed == true && transform.TransformVector(transform.localPosition - initLocalPosition).magnitude/Mathf.Abs(transform.localScale.z) < (button3d.clickDistance-0.007f) )
			{
				this.OnButtonUp();
			}
		} else {
			if (isActive == true && isPushed == false && transform.TransformVector(transform.localPosition - initLocalPosition).magnitude/Mathf.Abs(transform.localScale.z) > (button3d.clickDistance-0.007f) ) 
			{
				this.OnButtonDown();
			}
		}
	}

	void ResetButton()
	{
		isPushed = false;
		isTouching = false;
	}

	private void OnButtonDown()
	{
		isPushed = true;
		isTouching = true;
	//	materialToModify.color = Color.green;
	}

	private void OnButtonUp()
	{
		isPushed = false;
	//	materialToModify.color = Color.white;
		button3d.OnButtonClick();
		this.OnDeactivateButton();
	}

	void FixedUpdate()
	{
		this.SetLocalPosition();
	}
	void LateUpdate()
	{
		this.SetLocalPosition();
	}

	private void OnDeactivateButton()
	{
		isActive = false;
		myRigidbody.isKinematic = true;
		Invoke("OnActivateButton", 0.5f);
	}

	private void OnActivateButton()
	{
		isActive = true;
		myRigidbody.isKinematic = false;
	}

	private void SetLocalPosition()
	{
		// freeze position in x and y and check max z distance
		float newPosZ = transform.localPosition.z;
//		if (newPosZ > initPosition.z + button3d.clickDistance)
//			newPosZ = initPosition.z + button3d.clickDistance;

		newPosZ = Mathf.Clamp(newPosZ, initLocalPosition.z, initLocalPosition.z + this.transform.InverseTransformVector(this.initForward * button3d.moveDistance).magnitude * Mathf.Abs(transform.localScale.z));

		transform.localPosition = new Vector3 (initLocalPosition.x, initLocalPosition.y, newPosZ);
	}


	void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.gameObject.layer == LayerMask.NameToLayer("UI")) //nie koliduj z UI
			return;
		//isPushed = false;
		isTouching = true;
        button3d.touchOnEvent.Invoke();
//		materialToModify.color = Color.yellow;
    }

    void OnCollisionExit(Collision collision)
	{
		if(collision.collider.gameObject.layer == LayerMask.NameToLayer("UI")) //nie koliduj z UI
			return;
		
		isTouching = false;
        button3d.touchOffEvent.Invoke();
    //	materialToModify.color = Color.white;
    }

	public void EnableButton()
	{
	//	materialToModify.color = Color.white;
		isEnabled = true;
		GetComponent<Collider> ().enabled = true;
	}

	public void DisableButton()
	{
	//	materialToModify.color = Color.gray;
		isEnabled = false;
		GetComponent<Collider> ().enabled = false;
	}
}

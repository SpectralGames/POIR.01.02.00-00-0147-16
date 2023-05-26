using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderThumb3d : MonoBehaviour {

	private Material materialToModify;
	private Vector3 initLocalPosition = Vector3.zero;
	private bool isMouseTouching = false;
    private bool isControllerDrag = false;
	private bool isActive = true;
	private bool isEnabled = true;
	private Rigidbody myRigidbody;
	private FixedJoint joint;
    private float tempPosX;
    private Collider tempCollider;
    private Slider3d slider;

    private float currentSliderValue = 1;

	void Awake()
	{
		myRigidbody = this.GetComponent<Rigidbody>();
		initLocalPosition = new Vector3(0, this.transform.localPosition.y, this.transform.localPosition.z) ;
		
		//materialToModify = GetComponent<Renderer> ().material;// transform.Find("Bg").GetComponent<Renderer> ().material;
	}

    public void SetValue(float value)
    {
        currentSliderValue = value;
        CalcThumbPos();
    }

    public float GetValue()
    {
        return currentSliderValue;
    }

    public void CalcThumbPos()
    {
        float newValue = slider.size * Mathf.Clamp01(currentSliderValue) - slider.size / 2;
        transform.localPosition = new Vector3(newValue, transform.localPosition.y, transform.localPosition.z);
    }

    public void OnSetSider3dReference(Slider3d sliderRef)
    {
        slider = sliderRef;
    }

	void Update () 
	{
		if (Input.GetMouseButtonDown (0)) 
		{
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 5000)) {
                if (hit.collider.gameObject == gameObject) {

                    if (isMouseTouching == false)
						this.OnButtonDown();
				}			
			}
		}
		if (Input.GetMouseButtonUp (0)) 
		{
			if(isMouseTouching)
			{
				this.OnButtonUp();
			}
		}
			
		if (isMouseTouching) 
		{     
            float deltaPos = tempPosX - Input.mousePosition.x;
            currentSliderValue -= deltaPos * 0.01f;
            currentSliderValue = Mathf.Clamp01(currentSliderValue);
            CalcThumbPos();
            tempPosX = Input.mousePosition.x;
            slider.OnSliderChangeCallback(currentSliderValue);
        } 
	}

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint( gameObject.transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

	private void OnButtonDown()
	{
        tempPosX = Input.mousePosition.x;
        isMouseTouching = true;
	}

	private void OnButtonUp()
	{
        isMouseTouching = false;
        slider.OnSliderChangeEndCallback();
    }

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer != LayerMask.NameToLayer("Hands"))
			return;

		if (collider.isTrigger == false) // todo rozpoznac rece po tagu
		{
            isControllerDrag = true;
            tempCollider = collider;
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collider.GetComponentInChildren<Rigidbody>();
		}
    }

    void OnTriggerExit(Collider collider)
	{
		if(collider.gameObject.layer != LayerMask.NameToLayer("Hands")) 
			return;
		if (collider.isTrigger == false) // todo rozpoznac rece po tagu
		{
            isControllerDrag = false;
            Destroy(joint);
            slider.OnSliderChangeEndCallback();
            SetPositionForControllerDrag();
        }
    }

	void FixedUpdate()
	{
        if (isControllerDrag)
        {
            this.SetPositionForControllerDrag();
        }
	}

	void LateUpdate()
	{
        if (isControllerDrag)
        {
            this.SetPositionForControllerDrag();
        }
	}

	private void SetPositionForControllerDrag()
	{
        float newXPos = Mathf.Clamp(transform.localPosition.x, initLocalPosition.x - slider.size * .5f, initLocalPosition.x + slider.size * .5f);
        transform.localPosition = new Vector3(newXPos, transform.localPosition.y, transform.localPosition.z);
        currentSliderValue = newXPos / slider.size + slider.size;
        slider.OnSliderChangeCallback(currentSliderValue);   
	}

	public void EnableButton()
	{
        isEnabled = true;
		GetComponent<Collider> ().enabled = true;
	}

	public void DisableButton()
	{
        Debug.Log("@@@ disable button");
        isEnabled = false;
		GetComponent<Collider> ().enabled = false;
	}
}

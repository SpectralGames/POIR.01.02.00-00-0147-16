using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProxy : MonoBehaviour {

    public bool enabledInput = false;
    public float speedPosition = 1f;
    public float speedMouse = 1f;
    public bool lockRay = false;
    //public Vector2 mousePosition;
    //Vector3 centerPosition = Vector3.zero;

    private Quaternion currentRotation;

    void Start()
    {
#if UNITY_EDITOR
        if (enabledInput)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);
            currentRotation = transform.rotation;
        }
#endif
    }

    void Update()
    {
#if UNITY_EDITOR

        if (enabledInput)
        {
            UpdateInput();
        }
#endif
        }

    void UpdateInput()
    {
        float translationVertical = Input.GetAxis("Vertical") * speedPosition;
        float translationHorizontal = Input.GetAxis("Horizontal") * speedPosition;
        float translationZoom = Input.GetAxis("Mouse ScrollWheel") * speedPosition * 10;
        transform.Translate(translationHorizontal, translationZoom, translationVertical);
        //transform.Translate()



        if (Input.GetMouseButton(0))
        {
            currentRotation *= Quaternion.Euler(new Vector3(Input.GetAxis("Mouse Y") * -speedMouse, Input.GetAxis("Mouse X") * speedMouse, 0f));
            currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, 0f);
            transform.rotation = currentRotation;
            //transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * speed, Vector3.up);
          // transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * speed, Vector3.left);
        

          //      transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * speed, Space.World);
           // transform.Rotate(Vector3.right * Input.GetAxis("Mouse Y") * speed, Space.World);
            /*if (Input.GetAxis("Mouse X") > 0)
                transform.Rotate(Vector3.up * speed, Space.World);
            if (Input.GetAxis("Mouse Y") < 0)
                transform.Rotate(Vector3.right * -speed, Space.World);
            if (Input.GetAxis("Mouse Y") > 0)
                transform.Rotate(Vector3.right * speed, Space.World);  */
        }
        



        if (Input.GetKeyUp(KeyCode.R))
            transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);
        if (Input.GetKey(KeyCode.E))
            transform.RotateAround(Vector3.zero, Vector3.up, -0.5f);
        if (Input.GetKey(KeyCode.Q))
            transform.RotateAround(Vector3.zero, Vector3.up, 0.5f);
        if(lockRay)
            transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);
        if (Input.GetKeyUp(KeyCode.X))
            lockRay = !lockRay;



    }

}

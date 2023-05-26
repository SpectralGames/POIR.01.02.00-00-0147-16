using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerInput : MonoBehaviour
{
    [SerializeField] private string axisName = string.Empty;

    public static bool IgnoreNextButtonDown = false;
    public UnityEvent OnButtonDown = new UnityEvent();

    public static bool IgnoreNextButtonUp = false;
    public UnityEvent OnButtonUp = new UnityEvent();

    private float value = 0;
    private float oldValue = 0;


    private void Update()
    {
        value = Input.GetAxis(axisName);
        if (Mathf.RoundToInt(value) != 0 && Mathf.RoundToInt(oldValue) == 0)
        {
            if (IgnoreNextButtonDown)
                IgnoreNextButtonDown = false;
            else
                OnButtonDown.Invoke();
        }

        if (Mathf.RoundToInt(value) == 0 && Mathf.RoundToInt(oldValue) != 0)
        {
            if (IgnoreNextButtonUp)
                IgnoreNextButtonUp = false;
            else
                OnButtonUp.Invoke();
        }

        oldValue = value;
    }
}

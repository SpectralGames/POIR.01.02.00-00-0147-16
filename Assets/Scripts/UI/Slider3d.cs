using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Slider3d : MonoBehaviour {

	public float size = .5f; // <-0.25, 0.25>
 
    public UnityFloatEvent onSliderChange;
    public UnityEvent onSliderChangeEnd;

    private float eventValue;
	private SliderThumb3d thumb;

	void Awake()
	{
        Init();
    }

    private void Init()
    {
        thumb = GetComponentInChildren<SliderThumb3d>();
        thumb.OnSetSider3dReference(this);
    }

    public void SetValue(float value)
    {
        if (thumb == null) Init();
            thumb.SetValue(value);
    }

    public void OnSliderChangeCallback(float value)
    {
        if (onSliderChange != null)
            onSliderChange.Invoke(value);
    }

    public void OnSliderChangeEndCallback()
    {
        if (onSliderChangeEnd != null)
            onSliderChangeEnd.Invoke();
    }

    public float GetValue()
    {
        return thumb.GetValue();
    }


    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }
}


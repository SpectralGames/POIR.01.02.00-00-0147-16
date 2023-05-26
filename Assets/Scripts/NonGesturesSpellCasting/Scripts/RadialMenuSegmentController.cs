using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadialMenuSegmentController : MonoBehaviour, IRadialMenuSegmentController
{
    [SerializeField] private Image _image = null;
    public float FillAmount
    {
        get { return _image.fillAmount; }
        set { _image.fillAmount = value; }
    }

    [SerializeField] private Transform _offsetTransform = null;
    [SerializeField] private Transform _iconOffsetTransform = null;
    public float Offset
    {
        get { return _offsetTransform.localRotation.eulerAngles.z; }
        set { _offsetTransform.transform.localRotation = Quaternion.Euler(0, 0, value); }
    }

    public float IconOffset
    {
        get { return _iconOffsetTransform.localRotation.eulerAngles.z; }
        set { _iconOffsetTransform.transform.localRotation = Quaternion.Euler(0, 0, value); }
    }

    [SerializeField] private Transform _display = null;
    public Transform Display { get { return _display; } }

    [SerializeField] private bool _isSelected = false;
    public bool IsSelected { get { return _isSelected; } }

    private void Reset()
    {
        _image = GetComponent<Image>();
        _image.fillMethod = Image.FillMethod.Radial360;
        _image.fillClockwise = true;
        _image.fillOrigin = 0;
    }

    public void ResetImageRotation(Transform transform)
    {
        _display.rotation = Quaternion.LookRotation(-transform.forward, transform.up);
    }

    public void OnSelected()
    {
        Debug.Log(string.Format("{0} selected.", gameObject.name));
        if (!_isSelected)
        {
            _isSelected = true;
            OnSelectedCallback.Invoke();
        }
    }

    public void OnActivated()
    {
        Debug.Log(string.Format("{0} activated.", gameObject.name));
        OnActivatedCallback.Invoke();
    }

    public void OnDeselected()
    {
        Debug.Log(string.Format("{0} deselected.", gameObject.name));
        if (_isSelected)
        {
            _isSelected = false;
            OnDeselectedCallback.Invoke();
        }
    }

    public UnityEvent OnSelectedCallback = new UnityEvent();
    public UnityEvent OnActivatedCallback = new UnityEvent();
    public UnityEvent OnDeselectedCallback = new UnityEvent();
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IRiadialMenu
{
    void Initialize(RadialMenuSegment[] segments);
    void UpdateMenu();
}

public class RadialMenuController : MonoBehaviour
{

    [SerializeField] private Vector2 _input = Vector2.zero;
    public Vector2 Input { get { return _input; } set { _input = value; } }

    [SerializeField] private RadialMenuSegment[] _menuSegments = null;
    [SerializeField] private RadialMenuSegment _selectedSegment = null;
    [SerializeField] private float curentAngle = 0f;
    private float maxDelta = 0f;

    [SerializeField] private Transform _transformToRatoate = null;

    private void Awake()
    {
        float angle = (360 / _menuSegments.Length);
        maxDelta = angle;
        float zAngle = 0;
        for (int i = 0; i < _menuSegments.Length; i++)
        {
            if (_menuSegments.Length % 2 == 0)
            {
                _menuSegments[i].MinAngle = zAngle;
                _menuSegments[i].MaxAngle = zAngle + angle;
                zAngle += angle;
            }
            else
            {
                _menuSegments[i].MinAngle = zAngle - (angle/2);
                _menuSegments[i].MaxAngle = zAngle + (angle/2);
                zAngle += angle;
            }

            CheckIsLockedSegment(_menuSegments[i]);
          
        }
        IRiadialMenu initializer = GetComponent<IRiadialMenu>();
        initializer.Initialize(_menuSegments);    
    }

    private void Start()
    {
        // activate first unlocked item from list
        for (int i = 0; i < _menuSegments.Length; i++)
        {
            if (_menuSegments[i].IsLocked == false)
            {
                _menuSegments[i].OnActivated.Invoke();
                break;
            }
        }     
    }

    public void UpdateMenu()
    {
        for (int i = 0; i < _menuSegments.Length; i++)
        {
            CheckIsLockedSegment(_menuSegments[i]);
        }

        IRiadialMenu initializer = GetComponent<IRiadialMenu>();
        initializer.UpdateMenu();
    }

    private void CheckIsLockedSegment(RadialMenuSegment segment)
    {
        var data = segment.GetAdditionalData() as UnityUIRadialMenuSegment.UnityUIRadialMenuSegmentData;
        if (SaveGameController.instance.GetItemLevel(data.AttackType.ToString()) > 0)
        {
            segment.IsLocked = false;
        }
        else
        {
            segment.IsLocked = true;
        }
    }

    private void Update()
    {
        if (Input.magnitude > .1f)
        {
            curentAngle = Vector2.SignedAngle(Vector2.up, Input);
            curentAngle = curentAngle > 0 ? curentAngle : 360 + curentAngle;
            if (_transformToRatoate != null)
                _transformToRatoate.localRotation = Quaternion.Euler(0f, 0f, curentAngle);

            for (int i = 0; i < _menuSegments.Length; i++)
            {
                if (_menuSegments[i].IsLocked == false)
                {
                    float deltaToMin = Mathf.DeltaAngle(curentAngle, _menuSegments[i].MinAngle);
                    float deltaToMax = Mathf.DeltaAngle(curentAngle, _menuSegments[i].MaxAngle);

                    if (Mathf.Abs(deltaToMax) + Math.Abs(deltaToMin) == maxDelta)
                    {
                        if (_selectedSegment != _menuSegments[i])
                        {
                            if (_selectedSegment != null) _selectedSegment.OnDeselected.Invoke();
                            _selectedSegment = _menuSegments[i];
                            _selectedSegment.OnSelected.Invoke();
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            if (_selectedSegment != null)
            {
                _selectedSegment.OnDeselected.Invoke();
                _selectedSegment.OnActivated.Invoke();
            }
            _selectedSegment = null;
        }
    }
}

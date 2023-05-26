using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class HandsInput : MonoBehaviour
{
    [SerializeField] private string _horizontal = "RightHandHorizontal";
    [SerializeField] private string _vertical = "RightHandVertical";

    private RadialMenuController controller = null;
    private MenuStatusManager manager = null;

    private Vector2 _input = Vector2.zero;

    private void Update()
    {
        _input.x = Input.GetAxis(_horizontal);
        _input.y = Input.GetAxis(_vertical);

        if (controller != null) controller.Input = _input;
        if (manager != null) manager.Input = _input;
    }

    public void GetMenuObject(GameObject gameObject )
    {
        controller = gameObject.GetComponentInChildren<RadialMenuController>();
        manager = gameObject.GetComponentInChildren<MenuStatusManager>();
    }
}

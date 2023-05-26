using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStatusManager : MonoBehaviour
{
    public Vector2 Input { get; set; }
    [SerializeField] private Animator animator = null;
    [SerializeField] private string _showParametrName = "Show"; 

    private void Update()
    {
        animator.SetBool(_showParametrName, Input.magnitude > .1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RadialMenuSegmentController))]
public class SegmentAnimationController : MonoBehaviour
{
    [SerializeField] private RadialMenuSegmentController _radialMenuSegmentController = null;
    [SerializeField] private string _showSegmentName = "Show";
    [SerializeField] private Animator _animator = null;

    private void Update()
    {
        if (_animator != null)
            _animator.SetBool(_showSegmentName, _radialMenuSegmentController.IsSelected);
    }

    private void Reset()
    {
        _radialMenuSegmentController = gameObject.GetComponent<RadialMenuSegmentController>();
    }
}

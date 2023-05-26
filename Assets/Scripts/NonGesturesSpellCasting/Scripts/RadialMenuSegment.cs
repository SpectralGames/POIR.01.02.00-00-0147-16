using UnityEngine;
using UnityEngine.Events;

public abstract class RadialMenuSegment : MonoBehaviour
{
    [SerializeField] private float _minAngle = 0f;
    public float MinAngle
    {
        get { return _minAngle; }
        set { _minAngle = value; }
    }

    [SerializeField] private float _maxAngle = 0f;
    public float MaxAngle
    {
        get { return _maxAngle; }
        set { _maxAngle = value; }
    }

    private bool _isLocked;
    public bool IsLocked
    {
        get { return _isLocked; }
        set { _isLocked = value; }
    }

    public UnityEvent OnSelected = new UnityEvent();
    public UnityEvent OnActivated = new UnityEvent();
    public UnityEvent OnDeselected = new UnityEvent();

    public void SetSegment(IRadialMenuSegmentController controller)
    {
        OnSelected.AddListener(controller.OnSelected);
        OnActivated.AddListener(controller.OnActivated);
        OnDeselected.AddListener(controller.OnDeselected);
    }

    public abstract object GetAdditionalData();
}

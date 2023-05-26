using System;
using UnityEngine;
using UnityEngine.Events;


[DisallowMultipleComponent]
public class UnityUIRadialMenuSegment : RadialMenuSegment, ISpellTypeProvider
{
    [Serializable]
    public class UnityUIRadialMenuSegmentData
    {
        [SerializeField] private EAttackType _attackType = EAttackType.VioletBall;
        public EAttackType AttackType
        {
            get { return _attackType; }
            set { _attackType = value; }
        }

        [SerializeField] private GameObject _objectToShow = null;
        public GameObject ObjectToShow { get { return _objectToShow; } }

        [SerializeField] private Sprite _icon = null;
        public Sprite Icon { get { return _icon; } }

        [SerializeField] private Sprite _iconLocked = null;
        public Sprite IconLocked { get { return _iconLocked; } }

    }

    [SerializeField] private UnityUIRadialMenuSegmentData unityUIRadialMenuSegmentData = new UnityUIRadialMenuSegmentData();

    public UnityUIRadialMenuSegmentData UnityUIRadialMenuSegmentData1
    {
        get { return unityUIRadialMenuSegmentData; }
        set { unityUIRadialMenuSegmentData = value; }
    }


    public event Action<EAttackType> OnSetSpellType;

    private void Awake()
    {
        OnActivated.AddListener(() =>
        {
            OnSetSpellType?.Invoke(unityUIRadialMenuSegmentData.AttackType);
        });


    }

    public override object GetAdditionalData()
    {
        return unityUIRadialMenuSegmentData;
    }
}

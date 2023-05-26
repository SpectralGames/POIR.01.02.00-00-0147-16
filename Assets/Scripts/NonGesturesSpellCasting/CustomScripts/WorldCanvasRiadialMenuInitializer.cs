using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvasRiadialMenuInitializer : MonoBehaviour, IRiadialMenu
{
    [SerializeField] private Transform _menuRoot = null;
    [SerializeField] private RadialMenuSegmentController _menuSegmentPrefab = null;
    [SerializeField, Range(0f, 360f)] private float _angleSpacing = 5f;
    [SerializeField] private Sprite _defaultIcon = null;
    [SerializeField] private Image _selectedSpell = null;


    private RadialMenuSegment[] segments = null;
    private List<RadialMenuSegmentController> segmentsViewList = new List<RadialMenuSegmentController>();

    private void Awake()
    {
        if (_selectedSpell != null) _selectedSpell.gameObject.SetActive(false);
    }

    public void Initialize(RadialMenuSegment[] segments)
    {
        this.segments = segments;
        _menuSegmentPrefab.gameObject.SetActive(false);
        _menuRoot.transform.localRotation = segments.Length % 2 == 0 ? Quaternion.Euler(0, 0, -(_angleSpacing / segments.Length)) : Quaternion.identity;

        float angle = (360 / segments.Length);
        float fillAmount = (angle - _angleSpacing / 2f) / 360;
        float zAngle = 0;
        float offset = segments.Length % 2 == 0 ? angle : angle / 2f;
        float iconOffset = -(angle / 2f);

        for (int i = 0; i < segments.Length; i++)
        {
            RadialMenuSegmentController segment = Instantiate(_menuSegmentPrefab, _menuRoot, false);
            segmentsViewList.Add(segment);
            //_selectedSpell.transform.rotation = Quaternion.LookRotation(-transform.forward, transform.up);
            segments[i].SetSegment(segment);
            segment.transform.localRotation = Quaternion.Euler(0, 0, zAngle);
            segment.gameObject.SetActive(true);
            segment.gameObject.name = string.Format("Menu Segment {0}", i);
            segment.FillAmount = fillAmount;
            segment.Offset = offset;
            segment.IconOffset = iconOffset;
            segment.ResetImageRotation(transform);
            zAngle += angle;

            int index = i;
            UpdateSegmentState(segment, index);

        }
    }

    public void UpdateMenu()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            int index = i;
            UpdateSegmentState(segmentsViewList[i], index);
        }
    }

    private void UpdateSegmentState(RadialMenuSegmentController segmentController, int segmentIndex)
    {
        object data = segments[segmentIndex].GetAdditionalData();
        if (data != null)
        {
            var custonData = data as UnityUIRadialMenuSegment.UnityUIRadialMenuSegmentData;
            var image = segmentController.Display.GetComponent<Image>();
            if (image == null)
                Debug.LogError(string.Format("There is no {1} component on Icon GameObject in {0} hierarchy!", segmentController.name, typeof(Image).Name), segmentController);
            else
            {
                // check if spell is unlocked
                if (segments[segmentIndex].IsLocked)
                {
                    image.sprite = custonData.IconLocked != null ? custonData.IconLocked : _defaultIcon;
                    segmentController.GetComponentInChildren<SegmentAnimationController>().enabled = false;         
                }
                else
                {
                    image.sprite = custonData.Icon != null ? custonData.Icon : _defaultIcon;
                    segmentController.OnActivatedCallback.AddListener(() => SetActiveSpellObject(segmentIndex));

                    segmentController.GetComponentInChildren<SegmentAnimationController>().enabled = true;
                }
            }
        }
    }

    private void SetActiveSpellIcon(int index)
    {
        if (_selectedSpell != null && !_selectedSpell.gameObject.activeSelf) _selectedSpell.gameObject.SetActive(true);

        var data = (segments[index].GetAdditionalData() as UnityUIRadialMenuSegment.UnityUIRadialMenuSegmentData);
        _selectedSpell.sprite = data.Icon != null ? data.Icon : _selectedSpell.sprite;
    }

    private void SetActiveSpellObject(int index)
    {
        if (_selectedSpell != null && !_selectedSpell.gameObject.activeSelf) _selectedSpell.gameObject.SetActive(true);

        var data = (segments[index].GetAdditionalData() as UnityUIRadialMenuSegment.UnityUIRadialMenuSegmentData);
      
        if(data.Icon != null)
        {
            foreach(Transform child in _selectedSpell.transform)
            {
                Destroy(child.gameObject);
            }

            var gameObject = Instantiate(data.ObjectToShow, _selectedSpell.transform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

        }  
    }
}

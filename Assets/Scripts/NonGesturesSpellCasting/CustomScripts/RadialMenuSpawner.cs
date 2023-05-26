using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class RadialMenuSpawnCallback : UnityEvent<GameObject> { }

public class RadialMenuSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _radialMenuPrefab = null;
    public RadialMenuSpawnCallback RadialMenuSpawnCallback = new RadialMenuSpawnCallback();

    private void Awake()
    {
        var menu = Instantiate(_radialMenuPrefab, transform, false);
        menu.transform.localPosition = Vector3.zero;
        menu.transform.localRotation = Quaternion.identity;
        RadialMenuSpawnCallback.Invoke(menu);
    }                                                        
}

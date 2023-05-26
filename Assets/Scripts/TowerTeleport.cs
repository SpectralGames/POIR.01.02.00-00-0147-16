using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TowerTeleport : MonoBehaviour
{
    public int towerNumber;
    public Transform towerTeleportPoint;
    public GameObject particle;

    public static event Action OnGlobalTeleport = null;
    public UnityEvent OnTeleport = new UnityEvent();
    public UnityEvent OnTeleportOff = new UnityEvent();


    void Awake()
    {
        ObjectPool.Instance.towerTeleportList.Add(this);
    }

    public void TeleportPlayer(Transform localRoomArea, Transform player, bool setForwardToTeleport = false)
    {
        localRoomArea.position = towerTeleportPoint.position;
        player.transform.localPosition = Vector3.zero;

        // włącz particle dla pozostałych towerów 
        foreach (TowerTeleport otherTeleport in ObjectPool.Instance.towerTeleportList)
        {
            if (otherTeleport != this)
            {
                if (otherTeleport.particle.activeInHierarchy == false)
                {
                    otherTeleport.particle.SetActive(true);
                    otherTeleport.OnTeleportOff.Invoke();
                }
            }
        }
        // wyłącz particle dla towera w którym jest player
        particle.SetActive(false);
        OnTeleport.Invoke();

        OnGlobalTeleport?.Invoke();

        if (setForwardToTeleport)
        {
            localRoomArea.forward = towerTeleportPoint.forward;
        }
    }
}

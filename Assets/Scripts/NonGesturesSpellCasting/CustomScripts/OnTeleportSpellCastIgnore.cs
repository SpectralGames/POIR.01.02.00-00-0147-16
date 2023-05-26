using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTeleportSpellCastIgnore : MonoBehaviour
{
    private void Awake()
    {
        TowerTeleport.OnGlobalTeleport += IgnoreSpellClast;
    }

    private void OnDestroy()
    {
        TowerTeleport.OnGlobalTeleport -= IgnoreSpellClast;
    }

    private void IgnoreSpellClast()
    {
        TriggerInput.IgnoreNextButtonUp = true;
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolster : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Right Hand") || other.gameObject.CompareTag("Left Hand"))
        {
            var holsterSupport = other.GetComponent<WeaponHolsterSupport>();
            if (holsterSupport)
            {
                
            }
            
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Left Hand") || other.gameObject.CompareTag("Right Hand") || other.gameObject.CompareTag("Crossbow"))
        {
 
        }
        
    }

}

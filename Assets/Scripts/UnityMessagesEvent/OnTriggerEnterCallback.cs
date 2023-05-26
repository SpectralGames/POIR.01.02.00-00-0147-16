using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEnterCallback : OnTriggerMessageCallback
{
    private void OnTriggerEnter(Collider other)
    {
        Callback.Invoke(other);
    }
}

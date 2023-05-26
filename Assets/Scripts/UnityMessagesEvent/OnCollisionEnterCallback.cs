using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionEnterCallback : OnCollisionMessageCallback
{
    private void OnCollisionEnter(Collision collision)
    {
        Callback.Invoke(collision);
    }
}

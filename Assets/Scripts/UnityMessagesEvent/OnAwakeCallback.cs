using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnAwakeCallback : OnUnityMessageCallbakc
{
    private void Awake()
    {
        Callback.Invoke();
    }
}

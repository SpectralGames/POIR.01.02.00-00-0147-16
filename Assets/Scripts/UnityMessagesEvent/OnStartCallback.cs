using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartCallback : OnUnityMessageCallbakc
{
    void Start()
    {
        Callback.Invoke();
    }
}

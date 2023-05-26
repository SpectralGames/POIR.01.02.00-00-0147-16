using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDestoryCallback : OnUnityMessageCallbakc
{
    private void OnDestroy()
    {
        Callback.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableCallback : OnUnityMessageCallbakc
{
    private void OnEnable()
    {
        Callback.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;

public class OnDisableCallback : OnUnityMessageCallbakc
{
    private void OnDisable()
    {
        Callback.Invoke();
    }
}

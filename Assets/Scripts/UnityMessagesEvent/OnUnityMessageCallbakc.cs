using UnityEngine;
using UnityEngine.Events;

public abstract class OnUnityMessageCallbakc : MonoBehaviour
{
    public UnityEvent Callback = new UnityEvent();
}

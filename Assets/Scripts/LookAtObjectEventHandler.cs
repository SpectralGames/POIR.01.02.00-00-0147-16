using UnityEngine;
using UnityEngine.Events;

public class LookAtObjectEventHandler : MonoBehaviour
{
    public UnityEvent OnLookAtCallback = new UnityEvent();
    public UnityEvent OnLookOffCallback = new UnityEvent();

    public void LookAt()
    {
        OnLookAtCallback.Invoke();
    }

    public void LookOff()
    {
        OnLookOffCallback.Invoke();
    }
}

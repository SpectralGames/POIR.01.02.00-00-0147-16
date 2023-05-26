using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayCallbackInvoke : OnUnityMessageCallbakc
{
    [SerializeField] private float minTime = 0f;
    [SerializeField] private float maxTime = 1f;

    public void Invoke()
    {
        StartCoroutine(DelayCororutine());
    }

    private IEnumerator DelayCororutine()
    {
        yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        Callback.Invoke();
    }
}

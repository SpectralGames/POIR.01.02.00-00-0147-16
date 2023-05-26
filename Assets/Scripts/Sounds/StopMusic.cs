using UnityEngine;

public class StopMusic : MonoBehaviour
{
    [SerializeField] private FadeInfo fadeInfo = new FadeInfo();

    public void Stop()
    {
        ChannelManager.Instance.Stop(fadeInfo);
    }
}
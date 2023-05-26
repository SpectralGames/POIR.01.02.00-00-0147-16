using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField] private string playListName = string.Empty;
    [SerializeField] private FadeInfo fadeInfo = new FadeInfo();

    public void Play()
    {
        ChannelManager.Instance.Play(playListName, fadeInfo);
    }
}
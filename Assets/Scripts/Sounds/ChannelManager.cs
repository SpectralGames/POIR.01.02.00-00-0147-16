using System.Collections.Generic;
using UnityEngine;
using Utilities.Singleton;

public class ChannelManager : MonoBehaviorSingletion<ChannelManager>
{
    [SerializeField] private int maxChannels = 2;
    [SerializeField] private List<Channel> channels = new List<Channel>();

    [SerializeField] private Channel currentChannel = null;

    protected override void Awake()
    {
        base.Awake();
        if (currentChannel != null) currentChannel.Play();
    }

    protected override void Reset()
    {
        base.Reset();
        for (int i = 0; i < maxChannels; i++)
            AddChannel();
    }

    public void AddChannel()
    {
        GameObject channelObject = new GameObject("New Chanel", typeof(Channel));
        channelObject.transform.SetParent(this.transform, false);
        channels.Add(channelObject.GetComponent<Channel>());
    }

    public void Play(string name, FadeInfo fadeInfo)
    {
        Stop(fadeInfo);
        foreach (var item in channels)
            if (item.name == name)
            {
                currentChannel = item;
                currentChannel.Play();
                currentChannel.FadeIn(fadeInfo.FadeIn);
            }
    }

    public void Resume(FadeInfo fadeInfo)
    {

    }

    public void Stop(FadeInfo fadeInfo)
    {
        if (currentChannel != null) currentChannel.FadeOut(fadeInfo.FadeOut);
    }
}

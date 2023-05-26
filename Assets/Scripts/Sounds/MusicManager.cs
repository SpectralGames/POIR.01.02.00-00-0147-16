using UnityEngine;
using Utilities.Singleton;

[RequireComponent(typeof(IChannelManager))]
public class MusicManager : MonoBehaviorSingletion<MusicManager>
{
    private ChannelManager channelManager = null;

    protected override void Awake()
    {
        base.Awake();
        channelManager = GetComponent<ChannelManager>();
    }
}
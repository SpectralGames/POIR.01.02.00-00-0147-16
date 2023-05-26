using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InOrderSoundFxSelector : MonoBehaviour, IPlaySoundFxSelector
{
    [SerializeField] private int index = 0;
    private List<AudioClip> fxAudioClips = null;

    public void Init(List<AudioClip> fxAudioClips)
    {
        this.fxAudioClips = fxAudioClips;
    }

    public AudioClip Select()
    {
        if (index == fxAudioClips.Count)
            index = 0; 
        return fxAudioClips[index++];
    }
}

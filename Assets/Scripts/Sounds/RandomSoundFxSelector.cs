using System.Collections.Generic;
using UnityEngine;

public class RandomSoundFxSelector : MonoBehaviour, IPlaySoundFxSelector
{
    private List<AudioClip> fxAudioClips = new List<AudioClip>();
    public void Init(List<AudioClip> fxAudioClips)
    {
        this.fxAudioClips = fxAudioClips;
    }

    public AudioClip Select()
    {
        return fxAudioClips[Random.Range(0, fxAudioClips.Count)];
    }
}
using System.Collections.Generic;
using UnityEngine;

public interface IPlaySoundFxSelector
{
    void Init(List<AudioClip> fxAudioClips);
    AudioClip Select();
}

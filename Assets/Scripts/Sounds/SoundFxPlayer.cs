using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundFxPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource = null;
    public AudioSource AudioSource { get { return audioSource; } }

    public void StopFx()
    {
        audioSource.Stop();
    }

    [SerializeField] private List<AudioClip> fxAudioClips = new List<AudioClip>();

    private IPlaySoundFxSelector playAudioFx = null;

    [Space]
    public bool Owerwrite = true;

    private void Awake()
    {
        playAudioFx = GetComponent<IPlaySoundFxSelector>();
        playAudioFx.Init(fxAudioClips);
    }

    public void PlayFx()
    {
        if ((!Owerwrite && audioSource.isPlaying) || !gameObject.activeSelf) return;
        audioSource.clip = playAudioFx.Select();
        audioSource.Play();
    }

    private void Reset()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
}
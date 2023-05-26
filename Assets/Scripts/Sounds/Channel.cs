using System;
using System.Collections;
using UnityEngine;

public class Channel : MonoBehaviour
{
    [SerializeField] private int index = 0;
    [SerializeField, Range(0f, 1f)] private float clipTime = 0f;
    [SerializeField] private PlayList playList = null;

    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioSource additionAudioSource = null;
    public bool IsPlaying { get { return audioSource.isPlaying; } }

    private void Awake()
    {
        audioSource.volume = additionAudioSource.volume = 0;
    }

    private void OnEnable()
    {
        //Play();
    }

    private void OnDisable()
    {
        audioSource.Stop();
        additionAudioSource.Stop();
    }

    public void Play()
    {
        audioSource.clip = playList.Clips[index];
        audioSource.Play();
        StartCoroutine(FadeCoroutine(audioSource, playList.Clips[index].Fade.FadeIn));
    }

    private void Reset()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();
        additionAudioSource = this.gameObject.AddComponent<AudioSource>();
    }

    public void FadeOut(AnimationCurve fadeOut)
    {
        StartCoroutine(FadeCoroutine(audioSource, fadeOut, null, () => enabled = false));
    }

    public void FadeIn(AnimationCurve fadeIn)
    {
        StartCoroutine(FadeCoroutine(audioSource, fadeIn, () => enabled = true));
    }

    private void Update()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            enabled = false;
            return;
        }

        // Przenieść do interfejsu
        clipTime = audioSource.time / audioSource.clip.length;
        if ((audioSource.clip.length - audioSource.time) < playList.Clips[index].Fade.FadeOutTime)
        {
            var nextIndex = index + 1 > playList.Clips.Length - 1 ? 0 : index + 1;

            var cac = audioSource;
            additionAudioSource.clip = playList.Clips[nextIndex];
            additionAudioSource.Stop();
            audioSource = additionAudioSource;
            additionAudioSource = cac;

            audioSource.Play();
            StartCoroutine(FadeCoroutine(audioSource, playList.Clips[nextIndex].Fade.FadeIn));
            StartCoroutine(FadeCoroutine(additionAudioSource, playList.Clips[index].Fade.FadeOut, null, () => additionAudioSource.Stop()));
            index++;
            if (index == playList.Clips.Length) index = 0;
        }
    }

    private IEnumerator FadeCoroutine(AudioSource source, AnimationCurve fade, Action onFadeStart = null, Action onFadeEnd = null)
    {
        float counter = 0;
        float time = fade[fade.length - 1].time;

        onFadeStart?.Invoke();

        while (time > counter)
        {
            source.volume = fade.Evaluate(counter);
            counter += Time.deltaTime;
            yield return null;
        }
        source.volume = fade.Evaluate(counter);

        onFadeEnd?.Invoke();
    }

    private void OnValidate()
    {
        if (audioSource != null && audioSource.clip != null)
            audioSource.time = clipTime * audioSource.clip.length;
    }
}

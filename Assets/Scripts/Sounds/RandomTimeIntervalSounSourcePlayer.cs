using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTimeIntervalSounSourcePlayer : MonoBehaviour
{
    [SerializeField] private float min;
    [SerializeField] private float max;

    private void Awake()
    {
        var audioSources = GetComponentsInChildren<AudioSource>();
        foreach (var item in audioSources)
            StartCoroutine(PlayCoroutine(item, Random.Range(min, max)));
    }

    private IEnumerator PlayCoroutine(AudioSource source, float time)
    {
        source.playOnAwake = false;
        source.Stop();
        yield return new WaitForSeconds(time);
        source.Play();
    }
}

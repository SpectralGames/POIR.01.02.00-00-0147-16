using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayList", menuName = "Music/PlayList")]
public class PlayList : ScriptableObject
{
    [Serializable]
    public class AudioClipHolder
    {
        [SerializeField] private AudioClip audioClip = null;
        public AudioClip AudioClip { get { return audioClip; } }

        [SerializeField] private FadeInfo fade = new FadeInfo();
        public FadeInfo Fade { get { return fade; } }

        public static implicit operator AudioClip (AudioClipHolder holder)
        {
            return holder.audioClip;
        }
    }

    [SerializeField] private AudioClipHolder[] clips = null;
    public AudioClipHolder[] Clips { get { return clips; } }
}
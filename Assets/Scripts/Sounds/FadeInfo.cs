using System;
using UnityEngine;

[Serializable]
public class FadeInfo
{
    [SerializeField] private AnimationCurve fadeIn = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.1f, 1f));
    public AnimationCurve FadeIn { get { return fadeIn; } }
    public float FadeInTime { get { return fadeIn[fadeIn.length - 1].time; } }

    [SerializeField] private AnimationCurve fadeOut = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(.1f, 0));
    public AnimationCurve FadeOut { get { return fadeOut; } }
    public float FadeOutTime { get { return fadeOut[fadeOut.length - 1].time; } }
}

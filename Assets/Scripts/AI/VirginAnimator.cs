using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VirginAnimator : AIAnimator
{
    private Virgin virgin;

    private List<AnimationClip> carryClips = new List<AnimationClip>();
    private List<AnimationClip> runClips = new List<AnimationClip>();
	private List<AnimationClip> fatRunClips = new List<AnimationClip>();
    private List<AnimationClip> sitClips = new List<AnimationClip>();
    private List<AnimationClip> balconyClips = new List<AnimationClip>();
    private List<AnimationClip> flyClips = new List<AnimationClip>();

    public VirginAnimator(Virgin virgin)
    {
        this.virgin = virgin;
        Init(virgin);
    }

    public override void Init (AI ai)
    {
        base.Init(ai);
        InitAnimations();
    }

    protected override void InitAnimations()
    {
        sitClips = SplitClipsByName("sit");
        carryClips = SplitClipsByName("catch");
        balconyClips = SplitClipsByName("balocony");
        runClips = SplitClipsByName("run");
        flyClips = SplitClipsByName("fly");
		fatRunClips = SplitClipsByName("fatRun");
    }
}
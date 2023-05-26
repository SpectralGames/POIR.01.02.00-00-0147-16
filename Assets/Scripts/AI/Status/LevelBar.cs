using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LevelBar : BarElement
{
    private Image image;
    private Text text;

    public LevelBar(AI ai, GameObject go)
    {
        Init(ai, go);
    }

    public override void Init (AI ai, GameObject go)
    {
        this.aiObject = ai;
        image = go.FindComponentInChildWithName<Image>("LevelHolder");
        text = go.FindComponentInChildWithName<Text>("LevelText");
        text.text = aiObject.Level.ToString();

        SetAlpha(0f);
    }
    
    public override void SetAlpha (float alpha)
    {
        image.canvasRenderer.SetAlpha(alpha);
        text.canvasRenderer.SetAlpha(alpha);
    }

    public override float GetAlpha ()
    {
        return image.canvasRenderer.GetAlpha();
    }

    public override void HideElement ()
    {
        IsVisible = false;
        SetVisibility();
    }

    public override void ShowElement ()
    {
        IsVisible = true;
        SetVisibility();
    }

    private void SetVisibility ()
    {
        image.gameObject.SetActive(IsVisible);
        text.gameObject.SetActive(IsVisible); // chyba zawsze jest widoczne, dopoiero jak health 0 to wylacz
    }
}
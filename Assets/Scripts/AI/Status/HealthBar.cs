using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : BarElement
{
    private Image imageBackground;
    private Image image;

    public HealthBar(AI ai, GameObject go)
    {
        Init(ai, go);
    }

    public override void Init (AI ai, GameObject go)
    {
        this.aiObject = ai;
        IsVisible = false;

        imageBackground = go.FindComponentInChildWithName<Image>("HealthBackground");
        image = go.FindComponentInChildWithName<Image>("Health");

        SetAlpha(0f);
    }

    public override void SetAlpha (float alpha)
    {
        image.canvasRenderer.SetAlpha(alpha);
        imageBackground.canvasRenderer.SetAlpha(alpha);
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

    public void Fill(float percent)
    {
        image.fillAmount = percent;
    }

    public void Color(Color newColor)
    {
        image.color = newColor;
    }

    private void SetVisibility()
    {
        image.gameObject.SetActive(IsVisible);
        imageBackground.gameObject.SetActive(IsVisible); // chyba zawsze jest widoczne, dopoiero jak health 0 to wylacz
    }
}
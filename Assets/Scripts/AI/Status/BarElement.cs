using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class BarElement : MonoBehaviour
{
    protected AI aiObject;
    public bool IsVisible
    {
        get; set;
    }
    protected CanvasRenderer canvasRenderer;

    public virtual void Init (AI ai) { }
    public virtual void Init (AI ai, GameObject go) { }
    public abstract void SetAlpha (float alpha);
    public abstract float GetAlpha ();
    public abstract void HideElement ();
    public abstract void ShowElement ();
}

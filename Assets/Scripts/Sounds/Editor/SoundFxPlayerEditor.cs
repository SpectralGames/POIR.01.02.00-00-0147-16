using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundFxPlayer))]
public class SoundFxPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Play"))
            (target as SoundFxPlayer).PlayFx();
    }
}

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StopMusic))]
public class StopMusicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Stop"))
            (target as PlayMusic).Play();
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ActorAttach))]
public class ActorAttachEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ActorAttach aa = target as ActorAttach;

        if(aa!=null)
        {
            if(GUILayout.Button("Find and reattach"))
            {
                FindAndAttach(aa);
            }
            if(GUILayout.Button("Set position"))
            {
                Reattach();
            }
        }
    }

    public static void FindAndAttach(ActorAttach aa)
    {
        GameObject go = GameObject.Find(aa.attachTo.baseActorName);
        if(go)
        {
            Transform[] transforms = go.GetComponentsInChildren<Transform>();

            if( !string.IsNullOrEmpty(aa.attachTo.boneName) )
            {
                foreach( Transform t in transforms )
                    if( t.name == aa.attachTo.boneName )
                    {
                        aa.transform.parent = t;
                        Reattach(aa);
                        break;
                    }
            }
            else
            {
                aa.transform.parent = go.transform;
                Reattach(aa);
            }            
        }
    }

    public static void Reattach(ActorAttach aa)
    {
        aa.transform.localPosition = aa.attachTo.offset;
        aa.transform.localRotation = aa.attachTo.rotation;
    }

    private void Reattach()
    {
        ActorAttach aa = target as ActorAttach;
        Reattach(aa);
    }
}

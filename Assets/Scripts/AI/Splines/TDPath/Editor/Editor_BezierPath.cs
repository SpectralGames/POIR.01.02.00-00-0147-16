using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(BezierPath))]
[CanEditMultipleObjects()]
public class Editor_BezierPath : Editor
{
    public override void OnInspectorGUI()
    {
        //force bake on select
        Object[] beziers = targets;
        for( int i = 0; i < beziers.Length; i++ )
        {
            (beziers[i] as BezierPath).BakePath(true);
        }

        GUILayout.BeginHorizontal();
        {
            if( GUILayout.Button("SelectAll") )
            {
                List<GameObject> ControlPoints = new List<GameObject>();
                SelectThemAll(ref ControlPoints, target as BezierPath);
                Selection.objects = ControlPoints.ToArray();
            }
            if( GUILayout.Button("ShowOnlySelected") )
            {
                foreach( var bezier in GameObject.FindObjectsOfType(typeof(BezierPath)) as BezierPath[] )
                {
                    bezier.DrawSpline = false;
                }

                List<GameObject> ControlPoints = new List<GameObject>();
                SelectThemAll(ref ControlPoints, target as BezierPath);
                foreach( var bezier in ControlPoints )
                {
                    bezier.GetComponent<BezierPath>().DrawSpline = true;
                }
            }
            if( GUILayout.Button("ShowAll") )
            {
                foreach( var bezier in GameObject.FindObjectsOfType(typeof(BezierPath)) as BezierPath[] )
                {
                    bezier.DrawSpline = true;
                }
            }
        }
        GUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }

    void SelectThemAll(ref List<GameObject> ControlPoints, BezierPath point)
    {
        AddPointToSelection(ref ControlPoints, point);   

        if( point.controlPoint != null )
        {
            foreach( BezierPath prevPoint in point.controlPoint.prevPoints )
            {
                if( !ControlPoints.Contains(prevPoint.gameObject) )
                    SelectThemAll(ref ControlPoints, prevPoint);
            }

			foreach( BezierPath nextPoint in point.controlPoint.nextPoints )
			{
				if( !ControlPoints.Contains(nextPoint.gameObject) )
					SelectThemAll(ref ControlPoints, nextPoint);
			}
        }        
    }

    void AddPointToSelection(ref List<GameObject> ControlPoints, BezierPath point)
    {
        //Debug.Log("Adding Point: " + point.gameObject);
        ControlPoints.Add(point.gameObject);
    }


    void OnSceneGUI()
    {
        Matrix4x4 old = Handles.matrix;

        EditorGUI.BeginChangeCheck();

        BezierPath point = target as BezierPath;
        
        //Handles.Label(point.transform.position, point.name);

        Handles.matrix = point.transform.localToWorldMatrix;

        if( Event.current.modifiers == EventModifiers.Control )
        {
            //hide tools widget
            ToolsHidden = true;

            Vector3 tangentPos = Handles.FreeMoveHandle(point.controlPoint.tangentPos1, Quaternion.identity, 0.1f, Vector3.one * 0.01f, Handles.SphereHandleCap);
            if( tangentPos != point.controlPoint.tangentPos1 )
            {
                Undo.RecordObject(point, "BezierPath Tangent Change");                
                point.controlPoint.tangentPos1 = tangentPos;
                point.controlPoint.tangentPos2 = -tangentPos;
                EditorUtility.SetDirty(point);
            }
            tangentPos = Handles.FreeMoveHandle(point.controlPoint.tangentPos2, Quaternion.identity, 0.1f, Vector3.one * 0.01f, Handles.SphereHandleCap);
            if( tangentPos != point.controlPoint.tangentPos2 )
            {
                Undo.RecordObject(point, "BezierPath Tangent Change");
                point.controlPoint.tangentPos2 = tangentPos;
                point.controlPoint.tangentPos1 = -tangentPos; 
                EditorUtility.SetDirty(point);
            }

            Vector3 p = Handles.FreeMoveHandle(point.controlPoint.forward * Vector3.forward, Quaternion.identity, 0.05f, Vector3.one * 0.01f, Handles.CubeHandleCap);
            Quaternion forward = Quaternion.LookRotation(p);
            if( forward != point.controlPoint.forward )
            {
                Undo.RecordObject(point, "BezierPath Forward Change");                
                point.controlPoint.forward = Quaternion.LookRotation(p);
                EditorUtility.SetDirty(point);
            }
        }
        else
        {
            //restore tools widget
            ToolsHidden = false;
        }

        if( Event.current.modifiers == EventModifiers.Control )
        {
            Handles.DrawLine(point.controlPoint.tangentPos1, Vector3.zero);
            Handles.DrawLine(point.controlPoint.tangentPos2, Vector3.zero);

            Color c = Handles.color;
            Handles.color = Color.red;
            Handles.DrawLine(Vector3.zero, point.controlPoint.forward * Vector3.forward);
            Handles.color = c;
        }

        Handles.matrix = old;  
    }

    public static bool ToolsHidden
    {
        get
        {
            System.Type type = typeof(Tools);
            FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
            return ((bool)field.GetValue(null));
        }
        set
        {
            System.Type type = typeof(Tools);
            FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, value);
        }
    }
}

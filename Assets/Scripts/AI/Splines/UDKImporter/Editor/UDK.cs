/// <summary>
/// UDK spline importer.
/// Szybki, draftowy, tylko pod FlowerFenzy
/// Ma zrobione "aby bylo" parsowanie plikow
/// ale moze posluzyc do rozbudowy o importowanie dalszych rzeczy
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class UDK
{

    [MenuItem("Fury/Import Spline from file...")]
    static void ImportSpline ()
    {
        string pathToFile = EditorUtility.OpenFilePanel("Open spline", "", "txt");

        if (pathToFile != null && pathToFile.Length > 0)
        {
            Selection.activeObject = CreatePathFromFile(pathToFile).gameObject;
        }
    }

    [MenuItem("Fury/Import Spline from file to selected object...")]
    static void ImportSplineToSelectedObject ()
    {
        string pathToFile = EditorUtility.OpenFilePanel("Open spline", "", "txt");

        if (pathToFile != null && pathToFile.Length > 0)
        {
            Transform t = Selection.activeTransform;
            if (t != null)
            {
                ImportPathToTDPath(pathToFile, t.gameObject);

            }
        }
    }

    [MenuItem("Fury/Import Splines from dir...")]
    static void ImportSplinesFromDir ()
    {
        string pathDir = EditorUtility.OpenFolderPanel("Open dir with splines", "", "");
        if (pathDir != null && pathDir.Length > 0)
        {
            GameObject rootObject = new GameObject(System.IO.Path.GetDirectoryName(pathDir));


            string[] files = System.IO.Directory.GetFiles(pathDir);

            foreach (string file in files)
            {
                if (System.IO.Path.GetExtension(file) == ".txt")
                {
                    //TDPath path = CreatePathFromFile(file);
                    //path.transform.parent = rootObject.transform;
                    ImportPathToTDPath(file, rootObject);
                }
                else
                    Debug.Log("Skipping non txt file: " + file);
            }

            Selection.activeGameObject = rootObject;
        }
    }

    [MenuItem("Fury/Import Hierarchy...")]
    static void ImportHierarchy ()
    {
        string pathToFile = EditorUtility.OpenFilePanel("Open object", "", "txt");

        if (pathToFile != null && pathToFile.Length > 0)
        {
            UDKImporter importer = new UDKImporter(pathToFile);

            List<UDKActor> actors = importer.listOfMaps[0].levels[0].actors;

            GameObject go = GameObject.Find(System.IO.Path.GetFileNameWithoutExtension(pathToFile));
            if (go == null)
                go = new GameObject(System.IO.Path.GetFileNameWithoutExtension(pathToFile));

            foreach (UDKActor actor in actors)
            {
                //Debug.Log(actor);

                GameObject goActor = null;
                Transform Child = go.transform.Find(actor.name);
                if (Child != null)
                    goActor = Child.gameObject;
                else
                    goActor = new GameObject(actor.name);

                Undo.RecordObject(goActor.transform, "UDK Importer Change");
                goActor.transform.parent = go.transform;
                goActor.transform.position = actor.UnityPosition;
                goActor.transform.rotation = actor.UnityRotation;
                goActor.transform.localScale = actor.UnityScale;
            }
        }
    }

    static void SetPropertyCurveValue (AnimationClip clip, string path, System.Type type, string property, float value, float time)
    {
        if (clip == null)
            return;

        AnimationClipCurveData[] datas = AnimationUtility.GetAllCurves(clip, true);
        bool bFound = false;
        for (int i = 0; i < datas.Length; i++)
        {
            AnimationClipCurveData data = datas[i];
            if (data.path == path)
            {
                if (data.propertyName == property)
                {
                    //Debug.Log("Adding Key at path: " + path + " to property: " + property + " with value: " + value + " at time: " + time);
                    bFound = true;
                    if (data.curve.AddKey(time, value) == -1)
                    {
                        for (int j = 0; j < data.curve.keys.Length; j++)
                        {
                            if (data.curve.keys[j].time == time)
                            {
                                data.curve.RemoveKey(j);
                                data.curve.AddKey(time, value);
                                break;
                            }
                        }
                    }
                    clip.SetCurve(path, type, property, data.curve);
                    break;
                }
            }
        }

        if (!bFound)
        {
            //Debug.Log("Adding new curve at path: " + path + " to property: " + property + " with value: " + value + " at time: " + time);
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(time, value);
            curve.AddKey(clip.length, value);
            clip.SetCurve(path, type, property, curve);
        }
    }

    static void RecursiveImportAnimationKeys (AnimationClip clip, System.IO.DirectoryInfo Dir)
    {
        DirectoryInfo[] DIs = Dir.GetDirectories();
        if (DIs.Length > 0)
        {
            foreach (var DI in DIs)
            {
                RecursiveImportAnimationKeys(clip, DI);
            }
        }

        FileInfo[] FIs = Dir.GetFiles();
        if (FIs.Length > 0)
        {
            float time;
            if (float.TryParse(Dir.Name, out time))
            {
                foreach (var FI in FIs)
                {
                    //string fulldir = System.IO.Path.GetDirectoryName(pathToFile);
                    //Debug.Log("Path: " + fulldir);
                    //string dir = fulldir.Substring(fulldir.LastIndexOf(System.IO.Path.AltDirectorySeparatorChar) + 1);
                    //Debug.Log("Dirname: " + dir + " float: " + float.Parse(dir));

                    string pathToFile = FI.FullName;
                    Debug.Log("applying data from file: " + pathToFile);
                    UDKImporter importer = new UDKImporter(pathToFile);
                    List<UDKActor> actors = importer.listOfMaps[0].levels[0].actors;
                    foreach (UDKActor actor in actors)
                    {
                        string curvePath = System.IO.Path.GetFileNameWithoutExtension(pathToFile) + "/" + actor.name;

                        Vector3 position = actor.UnityPosition;
                        Quaternion rotation = actor.UnityRotation;
                        Vector3 scale = actor.UnityScale;

                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalPosition.x", position.x, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalPosition.y", position.y, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalPosition.z", position.z, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalRotation.x", rotation.x, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalRotation.y", rotation.y, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalRotation.z", rotation.z, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalRotation.w", rotation.w, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalScale.x", scale.x, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalScale.y", scale.y, time);
                        SetPropertyCurveValue(clip, curvePath, typeof(Transform), "m_LocalScale.z", scale.z, time);
                    }
                }
            }
        }
    }

    [MenuItem("Fury/Import Animation Keys...")]
    static void ImportAnimationKeys ()
    {
        AnimationClip selectedClip = Selection.activeObject as AnimationClip;
        if (selectedClip == null)
        { EditorUtility.DisplayDialog("Import Animation Keys...", "Please select animation clip", "OK"); return; }


        //string clipPath = AssetDatabase.GetAssetPath(selectedClip);
        //string newPath = clipPath.Insert(clipPath.IndexOf(".anim"), "_new");
        //make copy of this scene
        //AssetDatabase.CopyAsset(clipPath, newPath);
        //AssetDatabase.Refresh();

        //AnimationClip clip = AssetDatabase.LoadAssetAtPath(newPath, typeof(AnimationClip)) as AnimationClip;
        /*
        AnimationClipCurveData[] datas = AnimationUtility.GetAllCurves(clip, true);
        for( int i = 0; i < datas.Length; i++ )
        {
            AnimationClipCurveData data = datas[i];
            Debug.Log("curve, path: " + data.path + ", property: " + data.propertyName);
         
        }
        return;
        */

        string pathDir = EditorUtility.OpenFolderPanel("Open object", "", "dupa");

        if (!string.IsNullOrEmpty(pathDir))
        {
            Debug.Log("Processing clip: " + selectedClip);
            RecursiveImportAnimationKeys(selectedClip, new System.IO.DirectoryInfo(pathDir));
        }

        AssetDatabase.Refresh();
    }

    private static Transform ImportPathToTDPath (string pathToFile, GameObject rootGameObject)
    {
        UDKImporter importer = new UDKImporter(pathToFile);

        List<UDKActor> actors = importer.listOfMaps[0].levels[0].actors;

        if (rootGameObject == null)
            rootGameObject = new GameObject(pathToFile);

        Dictionary<string, BezierPath> dictionaryOfObjects = new Dictionary<string, BezierPath>();

        for (int a = 0; a < actors.Count; a++)
        {
            UDKActor actor = actors[a];

            GameObject goControlPoint = new GameObject(actor.name);
            BezierPath bezierPath = goControlPoint.AddComponent<BezierPath>();
            dictionaryOfObjects[actor.name] = bezierPath;

            goControlPoint.transform.parent = rootGameObject.transform;
            goControlPoint.transform.localPosition = UDKImporter.UDKPositionToUnityPosition(actor.udkPosition);




            bezierPath.controlPoint = new BezierPath.BezierControlPoint();
            bezierPath.controlPoint.forward = Quaternion.LookRotation(UDKImporter.UDKRotationToUnityRotation(actor.udkRotation) * Vector3.left);

            foreach (UDKObject obj in actor.listOfObjects)
            {
                if (obj.GetType() == typeof(UDKObjectSplineComponent))
                {
                    UDKObjectSplineComponent spline = (UDKObjectSplineComponent)obj;

                    bezierPath.controlPoint.tangentPos1 = 0.5f * UDKImporter.UDKPositionToUnityPosition(-spline.splineInfo.point1.leaveTangent);
                    bezierPath.controlPoint.tangentPos2 = 0.5f * UDKImporter.UDKPositionToUnityPosition(spline.splineInfo.point1.leaveTangent);
                }
            }
        }

        for (int a = 0; a < actors.Count; a++)
        {
            UDKActor actor = actors[a];

            BezierPath actualPath = dictionaryOfObjects[actor.name];

            foreach (UDKActor.Connection connection in actor.listOfConnections)
            {
                if (connection.name == UDKObjectSplineComponent.splineComponentName)
                {
                    BezierPath start = dictionaryOfObjects[actor.name];
                    BezierPath dest = dictionaryOfObjects[connection.toActorName];
                    //start.controlPoint.nextPoints[0] = dest;
                    if (dest.controlPoint.prevPoints == null)
                        dest.controlPoint.prevPoints = new BezierPath[0];
                    if (start.controlPoint.nextPoints == null)
                        start.controlPoint.nextPoints = new BezierPath[0];

                    ArrayUtility.Add<BezierPath>(ref start.controlPoint.nextPoints, dest);
                    ArrayUtility.Add<BezierPath>(ref dest.controlPoint.prevPoints, start);
                }
            }

            if (actor.attach != null)
            {
                ActorAttach aa = actualPath.gameObject.AddComponent<ActorAttach>();
                ActorAttach.AttachTo attachTo = new ActorAttach.AttachTo();
                attachTo.baseClassName = actor.attach.baseClassName;
                attachTo.baseActorName = actor.attach.baseActorName;
                attachTo.boneName = actor.attach.boneName;
                attachTo.offset = UDKImporter.UDKPositionToUnityPosition(actor.attach.udkOffset);
                attachTo.rotation = UDKImporter.UDKRotationToUnityRotation(actor.attach.udkRotation);
                aa.attachTo = attachTo;

                BezierPath path = aa.gameObject.GetComponent<BezierPath>();
                if (path == null)
                    Debug.LogError("Failed to get BezierPath");
                else
                    path.isActive = true;

                ActorAttachEditor.FindAndAttach(aa);
            }
        }

        foreach (KeyValuePair<string, BezierPath> key in dictionaryOfObjects)
        {
            key.Value.BakePath(true);
        }

        return rootGameObject.transform;
        /*
        path.pathType = TDPath.EPathType.Bezier;
        
        path.points = new TDPath.TDPathPoint[actors.Count];
        path.links = new TDPath.Links[0];

        for(int a=0; a<actors.Count; a++)
        {
            UDKActor actor = actors[a];


            path.points[a] = new TDPath.TDPathPoint();
            path.points[a].position = UDKImporter.UDKPositionToUnityPosition( actor.udkPosition);   
            path.points[a].forward = Quaternion.LookRotation( UDKImporter.UDKRotationToUnityRotation( actor.udkRotation )*Vector3.left);
            path.points[a].pointName = actor.name;
            
            foreach(UDKObject obj in actor.listOfObjects)
            {
                if(obj.GetType()==typeof(UDKObjectSplineComponent))
                {
                    UDKObjectSplineComponent spline = (UDKObjectSplineComponent)obj;
                    
                    path.points[a].tangentPos1 = 0.5f*UDKImporter.UDKPositionToUnityPosition( -spline.splineInfo.point1.leaveTangent );
                    path.points[a].tangentPos2 = 0.5f*UDKImporter.UDKPositionToUnityPosition( spline.splineInfo.point1.leaveTangent );
                    
                    
                }
            }
            
            foreach(UDKActor.Connection connection in actor.listOfConnections)
            {
                if(connection.name==UDKObjectSplineComponent.splineComponentName)
                {
                    //Debug.Log(" "+actor.name+" "+connection);
                    int num = -1;
                    UDKActor nextActor = importer.listOfMaps[0].levels[0].GetActorByName(connection.toActorName, out num);
                    if(nextActor!=null)
                    {
                        path.points[a].nextPoint = num;
                    }
                }
            }

            if(actor.attach!=null)
            {
                GameObject go = new GameObject(string.Format("{0}.{1}",actor.attach.baseClassName,actor.attach.baseActorName));
                go.transform.parent = path.transform;

                ActorAttach aa = go.AddComponent<ActorAttach>();
                ActorAttach.AttachTo attachTo = new ActorAttach.AttachTo();
                attachTo.baseClassName = actor.attach.baseClassName;
                attachTo.baseActorName = actor.attach.baseActorName;
                attachTo.boneName = actor.attach.boneName;
                attachTo.offset = UDKImporter.UDKPositionToUnityPosition(actor.attach.udkOffset);
                attachTo.rotation = UDKImporter.UDKRotationToUnityRotation(actor.attach.udkRotation);
                aa.attachTo = attachTo;

                go.transform.localPosition = aa.attachTo.offset;
                go.transform.localRotation = aa.attachTo.rotation;

                TDPath.Links link = new TDPath.Links();
                link.pointName = actor.name;
                link.target = aa.transform;

                ArrayUtility.Add<TDPath.Links>(ref path.links, link);

            }
        }
        path.BakePath();
        */
    }

    private static Transform CreatePathFromFile (string pathToFile)
    {
        //string objName = System.IO.Path.GetFileNameWithoutExtension(pathToFile);


        return ImportPathToTDPath(pathToFile, null);

    }


}

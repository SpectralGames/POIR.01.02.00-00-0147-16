using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TDPath))]
public class TDPath_Editor : Editor
{
    private TDPath currentRoad;
    private bool needBake = false;

    float weight = 0.0f;

    void OnEnable ()
    {
        currentRoad = target as TDPath;
        if (currentRoad.points == null)
            currentRoad.points = new TDPath.TDPathPoint[0];
    }

    void OnDisable ()
    {
        currentRoad = null;
    }

    void OnSceneGUI ()
    {
        Matrix4x4 old = Handles.matrix;

        //Handles.matrix = currentRoad.transform.localToWorldMatrix;

        Handles.matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.25f);

        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < currentRoad.points.Length; i++)
        {
            TDPath.TDPathPoint point = currentRoad.points[i];
            Vector3 pos = currentRoad.transform.TransformPoint(point.position);
            Vector3 posNew = Handles.PositionHandle(pos / 0.25f, Quaternion.identity) * 0.25f;

            if (pos != posNew)
            {
                point.position = currentRoad.transform.InverseTransformPoint(posNew);
                UpdatePointPosition(currentRoad, point);
                Undo.RecordObject(currentRoad, "TDPath Change");
                EditorUtility.SetDirty(currentRoad);
            }

            if (currentRoad.pathType == TDPath.EPathType.Bezier)
            {
                Matrix4x4 m = Handles.matrix;

                Matrix4x4 mat = currentRoad.transform.localToWorldMatrix;
                mat *= Matrix4x4.TRS(point.position, Quaternion.identity, Vector3.one);

                Handles.matrix = mat;

                Vector3 test1 = point.tangentPos1;

                if (Event.current.modifiers == EventModifiers.Control)
                {
                    point.tangentPos1 = Handles.FreeMoveHandle(point.tangentPos1, Quaternion.identity, 0.05f, Vector3.one * 0.1f, Handles.SphereHandleCap);
                    point.tangentPos2 = -point.tangentPos1;
                    point.tangentPos2 = Handles.FreeMoveHandle(point.tangentPos2, Quaternion.identity, 0.05f, Vector3.one * 0.1f, Handles.SphereHandleCap);
                    point.tangentPos1 = -point.tangentPos2;

                    Vector3 p = Handles.FreeMoveHandle(point.forward * Vector3.forward, Quaternion.identity, 0.05f, Vector3.one * 0.1f, Handles.CubeHandleCap);
                    point.forward = Quaternion.LookRotation(p);
                }


                //Handles.Label(point.tangentPos1,""+point.tangentPos1);
                Handles.matrix = m;

                if (Vector3.SqrMagnitude(test1 - point.tangentPos1) > Mathf.Epsilon)
                {
                    Undo.RecordObject(currentRoad, "TDPath Tangent Change");
                    EditorUtility.SetDirty(currentRoad);
                }
            }
        }
        if (EditorGUI.EndChangeCheck())
        {

            needBake = true;
        }


        Handles.matrix = currentRoad.transform.localToWorldMatrix;

        for (int a = 0; a < currentRoad.points.Length; a++)
        {
            TDPath.TDPathPoint point = currentRoad.points[a];
            if (point.pointName != null && point.pointName.Length > 0)
                Handles.Label(point.position, point.pointName);
            else
                Handles.Label(point.position, "Point " + a);
        }


        switch (currentRoad.pathType)
        {
            default:
            {
                if (currentRoad.bakedPath != null && currentRoad.bakedPath.Length > 1)
                {
                    for (int a = 0; a < currentRoad.bakedPath.Length - 1; a++)
                    {
                        Vector3 p1 = currentRoad.bakedPath[a];
                        Vector3 p2 = currentRoad.bakedPath[a + 1];

                        Handles.DrawLine(p1, p2);
                    }
                }
            }
            break;
            case TDPath.EPathType.Bezier:
            {
                for (int a = 0; a < currentRoad.points.Length; a++)
                {
                    Matrix4x4 m = Handles.matrix;

                    Handles.matrix *= Matrix4x4.TRS(currentRoad.points[a].position, Quaternion.identity, Vector3.one);

                    if (Event.current.modifiers == EventModifiers.Control)
                    {
                        Handles.DrawLine(currentRoad.points[a].tangentPos1, Vector3.zero);
                        Handles.DrawLine(currentRoad.points[a].tangentPos2, Vector3.zero);

                        Color c = Handles.color;
                        Handles.color = Color.red;
                        Handles.DrawLine(Vector3.zero, currentRoad.points[a].forward * Vector3.forward);
                        Handles.color = c;
                    }

                    Handles.matrix = m;

                    Color oldColor = Handles.color;
                    Handles.color = Color.magenta;
                    if (a < currentRoad.listOfBakedPaths.Count)
                        RenderLines(currentRoad.listOfBakedPaths[a].bakedPath);
                    Handles.color = oldColor;

                }
            }
            break;
            /*
            case TDPath.EPathType.CustomChainCatmullRom:
            {

                for(int a=0; a<currentRoad.points.Length; a++)
                {


                    Vector3[] tab = currentRoad.listOfBakedCustomPaths[a];


                    for(int p=0; p<tab.Length-1; p++)
                    {
                        Vector3 p1 = tab[p];
                        Vector3 p2 = tab[p+1];

                        Handles.DrawLine(p1,p2);
                    }
                }
            }
                break;
                */
        }

        Handles.matrix = old;

        Vector3 position;
        Quaternion rotation;
        currentRoad.GetPositionFromWeight(weight, out position, out rotation);
        

    }

    static void UpdatePointPosition (TDPath road, TDPath.TDPathPoint point)
    {

        switch (road.pathType)
        {
            default:
            case TDPath.EPathType.Square:
            {
                float squareSize = Mathf.Max(road.squareSize, 0.05f);
                float x = 1.0f / squareSize;
                float y = 1.0f / squareSize;
                float z = 1.0f / squareSize;

                Vector3 position = point.position;

                position.x = Mathf.Round(position.x * x) / x;
                position.y = Mathf.Round(position.y * y) / y;
                position.z = Mathf.Round(position.z * z) / z;

                point.position = position;

            }
            break;
        }

    }

    private enum EPointOperation
    {
        None,
        Add,
        Insert,
        MoveUp,
        MoveDown,
        Remove
    }

    private EPointOperation pointOperation = EPointOperation.None;
    private int pointOperationIndex = 0;


    public override void OnInspectorGUI ()
    {
        TDPath.EPathType current = currentRoad.pathType;

        currentRoad.pathType = (TDPath.EPathType)EditorGUILayout.EnumPopup("Road type", currentRoad.pathType);

        if (current != currentRoad.pathType)
        {
            EditorUtility.SetDirty(currentRoad);
            currentRoad.BakePath();
        }

        EditorGUI.BeginChangeCheck();
        currentRoad.squareSize = EditorGUILayout.FloatField("Snap size", currentRoad.squareSize);
        currentRoad.precission = Mathf.Clamp(EditorGUILayout.FloatField("Precision", currentRoad.precission), 0.1f, 1.0f);

        if (GUILayout.Button("Bake"))
            needBake = true;

        if (EditorGUI.EndChangeCheck())
            needBake = true;

        switch (currentRoad.pathType)
        {
            case TDPath.EPathType.CatmullRom:
            case TDPath.EPathType.Square:
            default:
            DrawDefault();
            break;
            case TDPath.EPathType.Bezier:
            DrawInspectorBezier();
            break;
            /*
            case TDPath.EPathType.CustomChainCatmullRom:
                DrawCustomChanCatmullRomPoint();
                break;
                */
        }

        if (GUILayout.Button("Add point"))
        {
            pointOperation = EPointOperation.Add;
        }

        EditorGUI.BeginChangeCheck();
        weight = EditorGUILayout.Slider(weight, 0.0f, 1.0f);
        if (EditorGUI.EndChangeCheck())
            SceneView.RepaintAll();

        switch (pointOperation)
        {
            default:
            break;
            case EPointOperation.Add:
            {
                TDPath.TDPathPoint point = new TDPath.TDPathPoint();
                if (currentRoad.points.Length > 0)
                {
                    point.position = currentRoad.points[currentRoad.points.Length - 1].position;
                }
                ArrayUtility.Add<TDPath.TDPathPoint>(ref currentRoad.points, point);

                EditorUtility.SetDirty(currentRoad);
            }
            break;
            case EPointOperation.Insert:
            {
                TDPath.TDPathPoint point = new TDPath.TDPathPoint();
                if (currentRoad.points.Length > 0)
                {
                    point.position = currentRoad.points[currentRoad.points.Length - 1].position;
                }

                ArrayUtility.Insert<TDPath.TDPathPoint>(ref currentRoad.points, pointOperationIndex, point);

                for (int a = 0; a < currentRoad.points.Length; a++)
                {
                    if (currentRoad.points[a].nextPoint >= pointOperationIndex)
                    {
                        currentRoad.points[a].nextPoint += 1;
                    }

                }

                EditorUtility.SetDirty(currentRoad);
            }
            break;
            case EPointOperation.MoveDown:
            {
                TDPath.TDPathPoint p1 = currentRoad.points[pointOperationIndex];
                currentRoad.points[pointOperationIndex] = currentRoad.points[pointOperationIndex + 1];
                currentRoad.points[pointOperationIndex + 1] = p1;

                for (int a = 0; a < currentRoad.points.Length; a++)
                {
                    if (currentRoad.points[a].nextPoint == pointOperationIndex)
                    {
                        currentRoad.points[a].nextPoint += 1;
                    }

                }

                EditorUtility.SetDirty(currentRoad);
            }
            break;
            case EPointOperation.MoveUp:
            {
                TDPath.TDPathPoint p1 = currentRoad.points[pointOperationIndex];
                currentRoad.points[pointOperationIndex] = currentRoad.points[pointOperationIndex - 1];
                currentRoad.points[pointOperationIndex - 1] = p1;

                for (int a = 0; a < currentRoad.points.Length; a++)
                {
                    if (currentRoad.points[a].nextPoint == pointOperationIndex)
                    {
                        currentRoad.points[a].nextPoint -= 1;
                    }

                }
                EditorUtility.SetDirty(currentRoad);
            }
            break;
            case EPointOperation.Remove:
            {
                ArrayUtility.RemoveAt<TDPath.TDPathPoint>(ref currentRoad.points, pointOperationIndex);

                for (int a = 0; a < currentRoad.points.Length; a++)
                {
                    if (currentRoad.points[a].nextPoint >= pointOperationIndex)
                    {
                        currentRoad.points[a].nextPoint -= 1;
                    }

                }
                EditorUtility.SetDirty(currentRoad);
            }
            break;
        }

        if (pointOperation != EPointOperation.None)
            needBake = true;

        pointOperation = EPointOperation.None;

        if (needBake)
        {
            currentRoad.BakePath();
            SceneView.RepaintAll();
            EditorUtility.SetDirty(currentRoad);
        }
        needBake = false;

        //base.DrawDefaultInspector();
    }


    private void DrawDefault ()
    {
        for (int a = 0; a < currentRoad.points.Length; a++)
        {
            TDPath.TDPathPoint point = currentRoad.points[a];
            GUILayout.Label("Point " + a);

            GUILayout.BeginHorizontal();

            {
                GUILayout.BeginHorizontal();

                point.position = EditorGUILayout.Vector3Field("Position", point.position);

                GUILayout.EndHorizontal();
            }

            {
                GUILayout.BeginHorizontal(GUILayout.MaxWidth(200));

                bool disableButtonUp = false;
                bool disableButtonDown = false;
                if (a == 0)
                    disableButtonUp = true;

                if (a == currentRoad.points.Length - 1)
                    disableButtonDown = true;

                EditorGUI.BeginDisabledGroup(disableButtonUp);
                if (GUILayout.Button("Up"))
                {
                    pointOperation = EPointOperation.MoveUp;
                    pointOperationIndex = a;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(disableButtonDown);
                if (GUILayout.Button("Down"))
                {
                    pointOperation = EPointOperation.MoveDown;
                    pointOperationIndex = a;
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Insert"))
                {
                    pointOperation = EPointOperation.Insert;
                    pointOperationIndex = a;
                }

                if (GUILayout.Button("Remove"))
                {
                    pointOperation = EPointOperation.Remove;
                    pointOperationIndex = a;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndHorizontal();
        }
    }

    private void DrawCustomChanCatmullRomPoint ()
    {

        for (int a = 0; a < currentRoad.points.Length; a++)
        {
            string[] tabNames = new string[currentRoad.points.Length];
            int[] tabValues = new int[currentRoad.points.Length];

            tabNames[0] = "None";
            tabValues[0] = -1;

            int index = 1;
            for (int p = 0; p < currentRoad.points.Length; p++)
            {
                if (p != a)
                {

                    tabNames[index] = "Point " + p;
                    tabValues[index] = p;

                    index += 1;
                }
            }

            TDPath.TDPathPoint point = currentRoad.points[a];
            GUILayout.Label("Point " + a);

            GUILayout.BeginVertical();


            point.position = EditorGUILayout.Vector3Field("Position", point.position);

            point.nextPoint = EditorGUILayout.IntPopup("Next point:", point.nextPoint, tabNames, tabValues);

            GUILayout.EndVertical();
        }
    }

    private void DrawInspectorBezier ()
    {
        for (int a = 0; a < currentRoad.points.Length; a++)
        {
            TDPath.TDPathPoint point = currentRoad.points[a];

            GUILayout.BeginHorizontal();

            {
                GUILayout.BeginVertical();

                {
                    GUILayout.Label("Point " + a);
                }

                {
                    GUILayout.BeginVertical();

                    string[] tabNames = new string[currentRoad.points.Length];
                    int[] tabValues = new int[currentRoad.points.Length];

                    tabNames[0] = "None";
                    tabValues[0] = -1;

                    int index = 1;
                    for (int p = 0; p < currentRoad.points.Length; p++)
                    {
                        if (p != a)
                        {

                            tabNames[index] = "Point " + p;
                            tabValues[index] = p;

                            index += 1;
                        }
                    }


                    Vector3 newPosition = EditorGUILayout.Vector3Field("Position", point.position);
                    if (newPosition != point.position)
                    {
                        point.position = newPosition;
                        EditorUtility.SetDirty(currentRoad);
                    }

                    int newNextPoint = EditorGUILayout.IntPopup("Next point:", point.nextPoint, tabNames, tabValues);
                    if (newNextPoint != point.nextPoint)
                    {
                        point.nextPoint = newNextPoint;
                        EditorUtility.SetDirty(currentRoad);
                    }

                    point.pointName = EditorGUILayout.TextField("Point name:", point.pointName);

                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();
            }

            {
                GUILayout.BeginVertical();

                bool disableButtonUp = false;
                bool disableButtonDown = false;
                if (a == 0)
                    disableButtonUp = true;

                if (a == currentRoad.points.Length - 1)
                    disableButtonDown = true;

                EditorGUI.BeginDisabledGroup(disableButtonUp);
                if (GUILayout.Button("Up"))
                {
                    pointOperation = EPointOperation.MoveUp;
                    pointOperationIndex = a;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(disableButtonDown);
                if (GUILayout.Button("Down"))
                {
                    pointOperation = EPointOperation.MoveDown;
                    pointOperationIndex = a;
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Insert"))
                {
                    pointOperation = EPointOperation.Insert;
                    pointOperationIndex = a;
                }

                if (GUILayout.Button("Remove"))
                {
                    pointOperation = EPointOperation.Remove;
                    pointOperationIndex = a;
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
    }

    private void RenderLines (Vector3[] array)
    {
        for (int a = 0; a < array.Length - 1; a++)
        {
            Handles.DrawLine(array[a], array[a + 1]);
        }
    }
}

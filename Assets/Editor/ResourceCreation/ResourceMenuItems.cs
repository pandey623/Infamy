using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public class ResourceMenuItems : MonoBehaviour {

    [MenuItem("Assets/Create/Resource/Nav Point")]
    public static void CreateNavPoint() {
        var obj = Build("Nav", "Nav Points", "sv_label_0", typeof(NavPoint));
        Selection.activeObject = obj;
        Undo.RegisterCreatedObjectUndo(obj, "Create Nav Point");
    }

    [MenuItem("GameObject/Create Other/Nav Point")]
    public static void CreateNavPoint2() {
        CreateNavPoint();
    }

    [MenuItem("Assets/Create/Resource/Area")]
    public static void CreateArea() {
        var obj = Build("Area", "Areas", "sv_icon_1", typeof(Area));
        Selection.activeObject = obj;
        Undo.RegisterCreatedObjectUndo(obj, "Create Area");
    }

    [MenuItem("GameObject/Create Other/Area")]
    public static void CreateArea2() {
        CreateArea();
    }

    [MenuItem("Assets/Create/Resource/Patrol Path")]
    public static void CreatePatrolPath() {
        var obj = Build("Patrol Path", "Patrol Paths", "sv_icon_2", typeof(PatrolPath));
        PatrolPath path = obj.GetComponent<PatrolPath>();
        CurvySpline spline = CurvySpline.Create();
        spline.Interpolation = CurvyInterpolation.CatmulRom;
        spline.AutoEndTangents = true;
        spline.Closed = true;
        spline.Add(new Vector3(-2, -1, 0), new Vector3(0, 2, 0), new Vector3(2, -1, 0));
        path.spline = spline;
        Undo.RegisterCreatedObjectUndo(obj, "Create Patrol Path");
        Selection.activeObject = obj;
    }

    [MenuItem("GameObject/Create Other/Patrol Path")]
    public static void CreatePatrolPath2() {
        CreatePatrolPath();
    }

    [MenuItem("Assets/Create/Resource/Waypoint Path")]
    public static void CreateWaypointPath() {
        var obj = Build("Waypoint Path", "Waypoint Paths", "sv_icon_3", typeof(WaypointPath));
        WaypointPath path = obj.GetComponent<WaypointPath>();
        CurvySpline spline = CurvySpline.Create();
        spline.Interpolation = CurvyInterpolation.CatmulRom;
        spline.AutoEndTangents = true;
        spline.Closed = true;
        spline.Add(new Vector3(-2, -1, 0), new Vector3(0, 2, 0), new Vector3(2, -1, 0));
        path.spline = spline;
        Undo.RegisterCreatedObjectUndo(obj, "Create Waypoint Path");
        Selection.activeObject = obj;
    }

    [MenuItem("GameObject/Create Other/Waypoint Path")]
    public static void CreateWaypointPath2() {
        CreateWaypointPath();
    }
    private static GameObject Build(string name, string parentName, string labelName, Type componentType) {
        GameObject obj = new GameObject(name + " - " + ~~(UnityEngine.Random.Range(1, 9999)));
        GameObject parent = GameObject.Find(parentName);
        if (parent == null) {
            parent = new GameObject(parentName);
        }
        obj.AddComponent(componentType);
        obj.transform.position = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.parent = parent.transform;
        AssignLabel(obj, "sv_label_1");
        return obj;
    }

    private static void AssignLabel(GameObject g, string label) {
        Texture2D tex = EditorGUIUtility.IconContent(label).image as Texture2D;
        Type editorGUIUtilityType = typeof(EditorGUIUtility);
        BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
        object[] args = new object[] { g, tex };
        editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
    }

}
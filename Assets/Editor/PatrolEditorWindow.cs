using UnityEngine;
using UnityEditor;

public class PatrolEditorWindow : EditorWindow
{
    [MenuItem("Tools/Path Editor")]
    public static void Open()
    {
        GetWindow<PatrolEditorWindow>("Path Editor");
    }

    private PatrolPath path;

    private void OnGUI()
    {
        path = (PatrolPath)EditorGUILayout.ObjectField("Patrol Path", path, typeof(PatrolPath), true);

        if (path == null) return;

        EditorGUILayout.LabelField("Points: ", path.points.Count.ToString());

        EditorGUILayout.Space(10f);

        path.showNumbers = EditorGUILayout.Toggle("Show Numbers", path.showNumbers);

        EditorGUILayout.Space(10f);

        if (GUILayout.Button("Add Point"))
        {
            Undo.RecordObject(path, "Add Patrol Point");

            SplinePoint newPoint = new SplinePoint();
            newPoint.position = Vector3.forward;

            path.points.Add(newPoint);
        }

        EditorGUILayout.Space(10f); EditorGUILayout.Space(10f);

        if (GUILayout.Button("Remove last"))
        {
            if (path.points.Count > 0)
            {
                Undo.RecordObject(path, "Remove Patrol Point");

                path.points.RemoveAt(path.points.Count - 1);
            }
        }

        EditorGUILayout.Space(10f);

        path.pathColor = EditorGUILayout.ColorField("Color", path.pathColor);
    }
}

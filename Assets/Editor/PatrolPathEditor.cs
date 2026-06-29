using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrolPath))]
public class PatrolPathEditor : Editor
{
    private void OnSceneGUI()
    {
        PatrolPath path = (PatrolPath)target;

        if (path.points == null) return;

        for (int i = 0; i < path.points.Count; i++)
        {
            Vector3 worldPos = path.transform.TransformPoint(path.points[i]); // local to world

            EditorGUI.BeginChangeCheck();

            Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(path, "Move Patrol Point");

                path.points[i] = path.transform.InverseTransformPoint(newPos); // world to local
            }
            if(path.showNumbers) Handles.Label(worldPos, "Point " + i);
        }
    }
}

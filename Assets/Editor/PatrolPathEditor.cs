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
            Vector3 worldPos = path.transform.TransformPoint(path.points[i].position); // local to world

            EditorGUI.BeginChangeCheck();

            Vector3 worldOut = path.transform.TransformPoint(path.points[i].position + path.points[i].outTangent);

            Vector3 newOut = Handles.PositionHandle(worldOut, Quaternion.identity);

            Vector3 worldIn = path.transform.TransformPoint(path.points[i].position + path.points[i].inTangent);

            Vector3 newIn = Handles.PositionHandle(worldIn, Quaternion.identity);

            Handles.color = Color.yellow;

            Handles.DrawLine(worldPos, worldOut);
            Handles.DrawLine(worldPos, worldIn);

            Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(path, "Move Patrol Point");

                path.points[i].position = path.transform.InverseTransformPoint(newPos); // world to local
                path.points[i].outTangent = path.transform.InverseTransformPoint(newOut) - path.points[i].position; // world to local
                path.points[i].inTangent = path.transform.InverseTransformPoint(newIn) - path.points[i].position; // world to local
            }
            if(path.showNumbers) 
                Handles.Label(worldPos, "Point " + i);
        }
    }
}

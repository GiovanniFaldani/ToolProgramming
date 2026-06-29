using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GizmosTest))]
public class HandlesTestEditor : Editor
{
    private void OnSceneGUI()
    {
        GizmosTest giz = (GizmosTest)target;

        Handles.color = Color.blue;

        Undo.RecordObject(giz, "Change info");

        giz.range = Handles.RadiusHandle(Quaternion.identity, giz.transform.position, giz.range);

        // giz.startPos = Handles.PositionHandle(giz.startPos, Quaternion.identity);
        // giz.endPos = Handles.PositionHandle(giz.endPos, Quaternion.identity);

        giz.startCubeScale = Handles.ScaleHandle(giz.startCubeScale, giz.startPos, Quaternion.identity);
        giz.endCubeScale = Handles.ScaleHandle(giz.endCubeScale, giz.endPos, Quaternion.identity);

        Handles.ArrowHandleCap(0, giz.transform.position, giz.transform.rotation, giz.range * 2, EventType.Repaint);

        Handles.DrawDottedLine(giz.transform.position, giz.startPos, 5f);

        Handles.Label(giz.transform.position + Vector3.up*10, "This GizmoObject");
    }
}

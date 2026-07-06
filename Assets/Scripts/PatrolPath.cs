using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class PatrolPath : MonoBehaviour
{
    public List<SplinePoint> points = new List<SplinePoint>();

    public Color pathColor = Color.green;

    [SerializeField] float pointsRadius = 2f;

    public bool showNumbers = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = pathColor;

        for(int i = 0; i < points.Count; i++)
        {
            Vector3 point = transform.TransformPoint(points[i].position);

            Gizmos.DrawSphere(point, pointsRadius);

            if(i < points.Count - 1)
            {
                Vector3 next = transform.TransformPoint(points[i + 1].position);

                // Gizmos.DrawLine(point, next);

                Vector3 direction = next - point;

                Vector3 startTangent = transform.TransformPoint(points[i].position + points[i].outTangent);
                Vector3 endTangent = transform.TransformPoint(points[i].position + points[i].inTangent);

                Handles.DrawBezier(point, next, startTangent, endTangent, Color.cyan, null, 2f);
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

public class PatrolPath : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();

    public Color pathColor = Color.green;

    [SerializeField] float pointsRadius = 2f;

    public bool showNumbers = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = pathColor;

        for(int i = 0; i < points.Count; i++)
        {
            Vector3 point = transform.TransformPoint(points[i]);

            Gizmos.DrawSphere(point, pointsRadius);

            if(i < points.Count - 1)
            {
                Vector3 next = transform.TransformPoint(points[i + 1]);

                Gizmos.DrawLine(point, next);
            }
        }
    }
}

using UnityEngine;

public class GizmosTest : MonoBehaviour
{
    public float range;
    public Vector3 startPos;
    public Vector3 endPos;

    public Vector3 startCubeScale;
    public Vector3 endCubeScale;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, range - 0.2f);

        Gizmos.DrawLine(startPos, endPos);

        Gizmos.DrawRay(startPos, Vector3.up * 5f);

        Gizmos.DrawWireCube(endPos, endCubeScale);

        Gizmos.DrawCube(startPos, startCubeScale);
    }
}

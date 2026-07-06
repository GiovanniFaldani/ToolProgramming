using UnityEngine;

[System.Serializable]
public class SplinePoint
{
    public Vector3 position;

    public Vector3 inTangent = new Vector3(-1, 0, 0);

    public Vector3 outTangent = new Vector3(1, 0, 0);
    
}

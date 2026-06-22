using UnityEngine;

public class ClampRangeAttribute : PropertyAttribute
{
    public float min;
    public float max;

    public ClampRangeAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

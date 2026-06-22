using UnityEngine;

public class ProgressBarAttribute : PropertyAttribute
{
    public float maxValue;

    public ProgressBarAttribute(float max)
    {
        this.maxValue = max;
    }
}

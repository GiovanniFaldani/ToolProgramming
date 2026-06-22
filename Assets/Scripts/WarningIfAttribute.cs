using UnityEngine;

public class WarningIfAttribute : PropertyAttribute
{
    public int limit;
    public string message;

    public WarningIfAttribute(int limit, string message = "")
    {
        this.limit = limit;
        this.message = message;
    }
}

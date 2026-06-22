using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EditorStartup
{
    // questo viene fatto all'apertura dell'engine
    static EditorStartup()
    {
        Debug.Log("Ho aperto l'engine!");
    }
}

using UnityEngine;
using UnityEditor;

public class TransformResetTool
{
    [MenuItem("Tools/Reset Transform %t")]   // % = ctrl, # = shift, & = alt
    public static void ResetTransform()
    {
        GameObject[] obj = Selection.gameObjects;    // activeGameObject (singolo), gameObjects(multiplo)
        if (obj == null)
        {
            Debug.LogWarning("Nessun oggetto selezionato");
            return;
        }

        foreach (GameObject go in obj)
        {
            Undo.RecordObject(go.transform, "Reset Transform");

            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }
    }
}

using UnityEngine;
using UnityEditor;

public class EnemyCreatorTool
{
    [MenuItem("Tools/Create Enemy")]
    public static void CreateEnemy()
    {
        GameObject enemy = new GameObject("Enemy");
        enemy.name = "enemy";
        enemy.AddComponent<Enemy>();
        enemy.transform.position = Vector3.zero;
        Selection.activeGameObject = enemy;
    }

    [MenuItem("Tools/Create/Create Enemy Prefab")]
    public static void CreatePrefab()
    {
        GameObject selected = Selection.activeGameObject;

        if(selected == null)
        {
            Debug.LogWarning("Nessun oggetto selezionato");
            return;
        }

        PrefabUtility.SaveAsPrefabAsset(selected, "Assets/Enemy.prefab");
    }
}

using UnityEngine;
//#if UNITY_EDITOR
using UnityEditor;
//#endif

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    private Texture2D previewTexture;

    public override void OnInspectorGUI()
    {
        Enemy enemy = (Enemy)target;

        //DrawDefaultInspector();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.Space(10);

        GUI.backgroundColor = Color.white;
        enemy.type = (EnemyType)EditorGUILayout.EnumPopup("Enemy Type", enemy.type);

        DisplayEnemyUI(enemy, enemy.type);
    }

    private void DisplayEnemyUI(Enemy enemy, EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Normal:
                DisplayHPInfo(ref enemy.health);
                DisplaySpeedInfo(ref enemy.speed);
                DisplayNormalExclusiveInfo(enemy);
                DisplayPrefabPreview(ref enemy.normalPrefab);
                break;

            case EnemyType.Elite:
                DisplayHPInfo(ref enemy.eliteHealth);
                DisplaySpeedInfo(ref enemy.eliteSpeed);
                DisplayEliteExclusiveInfo(enemy);
                DisplayPrefabPreview(ref enemy.elitePrefab);
                break;

            case EnemyType.Boss:
                DisplayHPInfo(ref enemy.bossHealth);
                DisplaySpeedInfo(ref enemy.bossSpeed);
                DisplayBossExclusiveInfo(enemy);
                DisplayPrefabPreview(ref enemy.bossPrefab);
                break;
        }

    }

    private void DisplayHPInfo(ref int enemyHP)
    {
        #region HealthUI
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUI.backgroundColor = Color.gray1;

        EditorGUILayout.BeginVertical("box");

        GUILayout.Space(10f);

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.white;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 18;

        GUILayout.Label("Enemy HP", titleStyle);

        GUILayout.Space(5f);

        GUI.backgroundColor = Color.white;

        GUI.color = Color.orange;

        EditorGUILayout.BeginHorizontal();

        // rimuovi 10hp
        if (GUILayout.Button("Remove 10HP", GUILayout.ExpandWidth(true)))
        {
            if (enemyHP > 0)
                enemyHP -= 10;
        }

        GUILayout.Space(5);

        GUIStyle HPStyle = new GUIStyle();
        HPStyle.fontSize = 12;
        HPStyle.normal.textColor = Color.yellow;
        HPStyle.alignment = TextAnchor.MiddleRight;

        GUILayout.Label($" = {enemyHP - 10}", HPStyle, GUILayout.Width(40), GUILayout.ExpandHeight(true));

        GUILayout.Space(20);

        GUILayout.EndHorizontal();

        GUILayout.Space(5f);

        // TODO rendere l'intfield a sfondo normale
        GUI.color = Color.white;
        GUI.backgroundColor = Color.gray1;
        EditorGUIUtility.labelWidth = 50;
        EditorGUILayout.IntField("Health", enemyHP, GUILayout.ExpandWidth(true));
        EditorGUIUtility.labelWidth = 0;
        GUI.backgroundColor = Color.white;

        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.red;

        // kill --> vita a 0
        if (GUILayout.Button("Kill"))
        {
            enemyHP = 0;
        }

        GUILayout.Space(5f);

        GUI.color = Color.green;

        if (GUILayout.Button("Reset Health"))
        {
            enemyHP = 100;
        }

        GUI.color = Color.white;

        GUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        GUILayout.Space(10f);
        #endregion
    }

    private void DisplaySpeedInfo(ref float enemySpeed)
    {
        #region SpeedUI
        // TODO verde - rosso
        // speed x2, speed /2
        // speed +1, speed -1
        // speed +10, speed -10

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUI.backgroundColor = Color.gray1;

        EditorGUILayout.BeginVertical("box");

        GUILayout.Space(10f);

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.white;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 18;

        GUILayout.Label("Enemy Speed", titleStyle);

        GUILayout.Space(5);

        GUI.color = Color.white;
        EditorGUIUtility.labelWidth = 50;
        EditorGUILayout.FloatField("Speed", enemySpeed, GUILayout.ExpandWidth(true));
        EditorGUIUtility.labelWidth = 0;

        GUILayout.Space(5);

        GUIStyle speedStyleGreen = new GUIStyle();
        speedStyleGreen.fontSize = 12;
        speedStyleGreen.normal.textColor = Color.green;
        speedStyleGreen.alignment = TextAnchor.MiddleRight;

        GUIStyle speedStyleRed = new GUIStyle();
        speedStyleRed.fontSize = 12;
        speedStyleRed.normal.textColor = Color.red;
        speedStyleRed.alignment = TextAnchor.MiddleRight;

        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.green;

        if (GUILayout.Button("Speed x2"))
        {
            enemySpeed *= 2;
        }

        GUILayout.Label($" = {enemySpeed * 2}", speedStyleGreen, GUILayout.Width(40), GUILayout.ExpandHeight(true));

        GUILayout.Space(5);

        GUI.color = Color.red;

        if (GUILayout.Button("Speed /2"))
        {
            enemySpeed /= 2;
        }

        GUILayout.Label($" = {enemySpeed / 2}", speedStyleRed, GUILayout.Width(40), GUILayout.ExpandHeight(true));

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.green;

        if (GUILayout.Button("Speed +1"))
        {
            enemySpeed += 1;
        }

        GUILayout.Label($" = {enemySpeed + 1}", speedStyleGreen, GUILayout.Width(40), GUILayout.ExpandHeight(true));

        GUILayout.Space(5);

        GUI.color = Color.red;

        if (GUILayout.Button("Speed -1"))
        {
            enemySpeed -= 1;
        }

        GUILayout.Label($" = {enemySpeed - 1}", speedStyleRed, GUILayout.Width(40), GUILayout.ExpandHeight(true));

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.green;

        if (GUILayout.Button("Speed +10"))
        {
            enemySpeed += 10;
        }

        GUILayout.Label($" = {enemySpeed + 10}", speedStyleGreen, GUILayout.Width(40), GUILayout.ExpandHeight(true));

        GUILayout.Space(5);

        GUI.color = Color.red;

        if (GUILayout.Button("Speed -10"))
        {
            enemySpeed -= 10;
        }

        GUILayout.Label($" = {enemySpeed - 10}", speedStyleRed, GUILayout.Width(40), GUILayout.ExpandHeight(true));

        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        #endregion

    }

    private void DisplayNormalExclusiveInfo(Enemy enemy)
    {
        //Event e = Event.current;

        //if (e.control)
        //{
        //    GUILayout.Label("Secret Label", titleStyle);
        //    Repaint();  // forza aggiornamento
        //}

        GUILayout.Space(5f);

        enemy.canFly = EditorGUILayout.Toggle("Can Fly", enemy.canFly);

        if (enemy.canFly)
        {
            enemy.flySpeed = EditorGUILayout.IntSlider("Fly Speed", enemy.flySpeed, 0, 100);
            EditorGUILayout.HelpBox("Flying enemies need minimum 0 speed", MessageType.Info);
        }

        enemy.debugColor = EditorGUILayout.ColorField("Debug Color", enemy.debugColor);
    }

    private void DisplayEliteExclusiveInfo(Enemy enemy)
    {
        GUILayout.Space(5f);

        enemy.canSwim = EditorGUILayout.Toggle("Can Swim", enemy.canSwim);

        if (enemy.canSwim)
        {
            enemy.swimSpeed = EditorGUILayout.IntSlider("Swim Speed", enemy.swimSpeed, 0, 100);
            EditorGUILayout.HelpBox("Swimming enemies need minimum 0 speed", MessageType.Info);
        }

        enemy.eliteDebugColor = EditorGUILayout.ColorField("Elite Debug Color", enemy.eliteDebugColor);

    }

    private void DisplayBossExclusiveInfo(Enemy enemy)
    {
        GUILayout.Space(5f);

        enemy.canDig = EditorGUILayout.Toggle("Can Dig", enemy.canDig);

        if (enemy.canDig)
        {
            enemy.digDepth = EditorGUILayout.IntSlider("Dig Depth", enemy.digDepth, 0, 100);
            EditorGUILayout.HelpBox("Digging enemies need minimum 0 depth", MessageType.Info);
        }

        enemy.bossDebugColor = EditorGUILayout.ColorField("Boss Debug Color", enemy.bossDebugColor);

    }

    private void DisplayPrefabPreview(ref GameObject prefabToPreview)
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.Space(10);

        prefabToPreview = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToPreview, typeof(GameObject), false);

        EditorGUILayout.Space(10);

        if (prefabToPreview != null)
        {
            previewTexture = AssetPreview.GetAssetPreview(prefabToPreview);

            if (previewTexture != null)
            {
                GUILayout.Label("Prefab Preview");

                Rect rect = GUILayoutUtility.GetRect(128, 128, GUILayout.ExpandWidth(false));

                GUI.DrawTexture(rect, previewTexture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUILayout.Label("Generating Preview...");
            }
        }

        EditorGUILayout.Space(10);
    }
}
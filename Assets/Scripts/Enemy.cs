using UnityEngine;

public enum EnemyType
{
    Normal,
    Elite,
    Boss
}

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public EnemyType type;
    public GameObject prefabToPreview;

    //--------NORMAL----------
    public int health = 100;
    public float speed = 5f;

    public bool canFly;
    public int flySpeed = 10;

    public Color debugColor;
    public GameObject normalPrefab;

    //--------ELITE-----------
    public int eliteHealth = 100;
    public float eliteSpeed = 5f;

    public bool canSwim;
    public int swimSpeed = 10;

    public Color eliteDebugColor;
    public GameObject elitePrefab;

    //--------BOSS------------
    public int bossHealth = 100;
    public float bossSpeed = 5f;

    public bool canDig;
    public int digDepth = 50;

    public Color bossDebugColor;
    public GameObject bossPrefab;
}

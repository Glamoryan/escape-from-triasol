using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public GameObject prefab;
    }

    public EnemyType[] enemyTypes;
    public float spawnInterval = 3f;
    public float spawnDistance = 10f;
    public int maxEnemies = 20;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (enemyTypes.Length == 0 || player == null) return;

        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemyCount >= maxEnemies) return;

        Vector2 spawnDir = Random.insideUnitCircle.normalized;
        Vector2 spawnPos = (Vector2)player.position + spawnDir * spawnDistance;

        EnemyType chosen = enemyTypes[Random.Range(0, enemyTypes.Length)];
        Instantiate(chosen.prefab, spawnPos, Quaternion.identity);
    }
}

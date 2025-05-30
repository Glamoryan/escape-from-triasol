using UnityEngine;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public GameObject prefab;
    }

    public EnemyType[] enemyTypes;
    public Transform player;

    public float spawnDistance = 10f;
    public float spawnInterval = 1f;
    public float spawnDuration = 15f;
    public float restDuration = 10f;

    private int currentLevel = 1;
    private int spawnedEnemies = 0;
    private int maxEnemiesForLevel = 5;
    private bool isSpawning = false;

    private Coroutine spawnRoutine;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        StartCoroutine(LevelLoop());
    }

    IEnumerator LevelLoop()
    {
        while (true)
        {
            spawnedEnemies = 0;
            maxEnemiesForLevel = 5 + (currentLevel - 1) * 3;

            Debug.Log($"Level {currentLevel} Başladı. Maksimum düşman: {maxEnemiesForLevel}");

            isSpawning = true;
            spawnRoutine = StartCoroutine(SpawnEnemies());

            yield return new WaitForSeconds(spawnDuration);

            isSpawning = false;
            StopCoroutine(spawnRoutine);

            Debug.Log($"Level {currentLevel} spawn bitti. {restDuration} saniye bekleniyor.");

            yield return new WaitForSeconds(restDuration);
            currentLevel++;
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (isSpawning && spawnedEnemies < maxEnemiesForLevel)
        {
            SpawnOneEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneEnemy()
    {
        if (enemyTypes.Length == 0 || player == null) return;

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector2 pos = (Vector2)player.position + dir * spawnDistance;

        EnemyType chosen = enemyTypes[Random.Range(0, enemyTypes.Length)];
        GameObject enemy = Instantiate(chosen.prefab, pos, Quaternion.identity);
        spawnedEnemies++;

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += () => {
                // Debug.Log("Enemy death");
            };
        }
    }
}

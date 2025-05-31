using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;
    public static event Action<int> OnDayChanged;
    public static event Action<float> OnLevelTimerChanged;
    public static event Action<float> OnRestTimerChanged;

    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public GameObject prefab;
        [Range(0, 100)]
        public float spawnPercentage;
    }

    [System.Serializable]
    public class LevelConfig
    {
        public float preparationTime = 10f; // Hazırlık süresi
        public float levelDuration = 180f;
        public float restDuration = 120f;
        public int maxEnemies = 5;
        public List<EnemyType> enemyTypes = new List<EnemyType>();
    }

    public List<LevelConfig> levelConfigs = new List<LevelConfig>();
    public Transform player;
    public float spawnDistance = 10f;
    public float spawnInterval = 1f;
    public float initialPreparationTime = 10f; // Oyun başlangıcı hazırlık süresi

    private int currentLevel = 1;
    private int spawnedEnemies = 0;
    private bool isSpawning = false;
    private Coroutine spawnRoutine;
    private float levelTimer = 0f;
    private float restTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (levelConfigs.Count == 0)
        {
            Debug.LogError("Level konfigürasyonları boş! Lütfen Inspector'dan level ayarlarını yapın.");
            return;
        }

        OnDayChanged?.Invoke(currentLevel);
        StartCoroutine(InitialPreparation());
    }

    IEnumerator InitialPreparation()
    {
        Debug.Log($"Oyun başlıyor! Hazırlık süresi: {initialPreparationTime} saniye");
        
        float prepTimer = 0f;
        while (prepTimer < initialPreparationTime)
        {
            prepTimer += Time.deltaTime;
            OnLevelTimerChanged?.Invoke(prepTimer / initialPreparationTime);
            yield return null;
        }

        StartCoroutine(LevelLoop());
    }

    IEnumerator LevelLoop()
    {
        while (currentLevel <= levelConfigs.Count)
        {
            LevelConfig currentConfig = levelConfigs[currentLevel - 1];
            spawnedEnemies = 0;
            levelTimer = 0f;

            Debug.Log($"Day {currentLevel} Hazırlık Süresi: {currentConfig.preparationTime} saniye");
            
            // Hazırlık süresi
            float prepTimer = 0f;
            while (prepTimer < currentConfig.preparationTime)
            {
                prepTimer += Time.deltaTime;
                OnLevelTimerChanged?.Invoke(prepTimer / currentConfig.preparationTime);
                yield return null;
            }

            Debug.Log($"Day {currentLevel} Başladı. Maksimum düşman: {currentConfig.maxEnemies}");
            OnDayChanged?.Invoke(currentLevel);

            isSpawning = true;
            spawnRoutine = StartCoroutine(SpawnEnemies());

            while (levelTimer < currentConfig.levelDuration)
            {
                levelTimer += Time.deltaTime;
                OnLevelTimerChanged?.Invoke(levelTimer / currentConfig.levelDuration);
                yield return null;
            }

            isSpawning = false;
            if (spawnRoutine != null)
                StopCoroutine(spawnRoutine);

            Debug.Log($"Day {currentLevel} bitti. {currentConfig.restDuration} saniye dinlenme süresi.");

            restTimer = 0f;
            while (restTimer < currentConfig.restDuration)
            {
                restTimer += Time.deltaTime;
                OnRestTimerChanged?.Invoke(restTimer / currentConfig.restDuration);
                yield return null;
            }

            currentLevel++;
        }

        Debug.Log("Tüm günler tamamlandı!");
    }

    IEnumerator SpawnEnemies()
    {
        while (isSpawning && spawnedEnemies < levelConfigs[currentLevel - 1].maxEnemies)
        {
            SpawnOneEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneEnemy()
    {
        if (player == null) return;

        LevelConfig currentConfig = levelConfigs[currentLevel - 1];
        if (currentConfig.enemyTypes.Count == 0) return;

        Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 pos = (Vector2)player.position + dir * spawnDistance;

        float totalPercentage = 0f;
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        EnemyType chosenEnemy = currentConfig.enemyTypes[0];

        foreach (var enemyType in currentConfig.enemyTypes)
        {
            totalPercentage += enemyType.spawnPercentage;
            if (randomValue <= totalPercentage)
            {
                chosenEnemy = enemyType;
                break;
            }
        }

        GameObject enemy = Instantiate(chosenEnemy.prefab, pos, Quaternion.identity);
        spawnedEnemies++;

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += () => {
                spawnedEnemies--;
                if (GetActiveEnemyCount() == 0)
                {
                    DestroyAllBullets();
                }
            };
        }
    }

    int GetActiveEnemyCount()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void DestroyAllBullets()
    {
        var bullets = GameObject.FindObjectsOfType<Bullet>();
        foreach (var bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
        Debug.Log("Tüm mermiler temizlendi!");
    }
}

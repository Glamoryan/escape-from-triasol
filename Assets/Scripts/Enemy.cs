using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform target;
    public float attackRange = 4f;
    public float stopDistance = 1.5f;
    public float slowSpeed = 3f;
    public float fastSpeed = 12f;

    [Header("Drop Settings")]
    public EnemyDropConfig dropConfig;

    [Header("Burst Fire Settings")]
    public int burstCount = 3;
    public float timeBetweenShots = 0.2f;
    public float burstCooldown = 1.5f;

    [Header("Death Effect")]
    public GameObject[] explosionParticlePrefabs;
    public float explosionDuration = 0.5f;
    public float maxShakeDistance = 10f;
    public float maxShakeIntensity = 0.5f;
    public float shakeDuration = 0.3f;

    [Header("Ses Ayarları")]
    public AudioClip shootSound;
    public float shootVolume = 0.5f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    public Action OnDeath;

    private int shotsLeftInBurst;
    private float shotTimer;
    private float cooldownTimer;
    private bool isBursting = false;
    private Health health;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private static readonly Dictionary<ItemType, Queue<GameObject>> itemPool = new Dictionary<ItemType, Queue<GameObject>>();
    private static readonly int poolSize = 5;

    void Start()
    {
        if (target == null)
            FindNearestTarget();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += Die;
        }

        // AudioSource bileşenini oluştur
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = shootVolume;
        audioSource.spatialBlend = 0f; // 2D ses
        audioSource.outputAudioMixerGroup = null; // Doğrudan ses çıkışı
        audioSource.bypassEffects = true; // Efektleri bypass et
        audioSource.bypassListenerEffects = true; // Listener efektlerini bypass et
        audioSource.bypassReverbZones = true; // Reverb bölgelerini bypass et
        audioSource.dopplerLevel = 0f; // Doppler efektini kapat
        audioSource.spread = 0f; // Ses yayılımını kapat
        audioSource.priority = 128; // Normal öncelik
        audioSource.mute = false; // Sesi aç
        audioSource.loop = false; // Tekrarlamayı kapat
    }

    void FindNearestTarget()
    {
        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        // Player'ı kontrol et
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distToPlayer < shortestDistance)
            {
                shortestDistance = distToPlayer;
                nearestTarget = player.transform;
            }
        }

        // Turret'leri kontrol et
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Structure");
        foreach (GameObject turret in turrets)
        {
            float distToTurret = Vector2.Distance(transform.position, turret.transform.position);
            if (distToTurret < shortestDistance)
            {
                shortestDistance = distToTurret;
                nearestTarget = turret.transform;
            }
        }

        target = nearestTarget;
    }

    void Die()
    {
        // Patlama efekti oluştur
        GameObject explosionObj = new GameObject("Explosion");
        explosionObj.transform.position = transform.position;
        
        ExplosionEffect explosion = explosionObj.AddComponent<ExplosionEffect>();
        explosion.explosionParticlePrefabs = explosionParticlePrefabs;
        explosion.explosionDuration = explosionDuration;
        explosion.maxShakeDistance = maxShakeDistance;
        explosion.maxShakeIntensity = maxShakeIntensity;
        explosion.shakeDuration = shakeDuration;

        // Item drop
        if (dropConfig != null)
        {
            foreach (var dropConfig in dropConfig.dropConfigs)
            {
                if (dropConfig.prefab != null && UnityEngine.Random.value <= dropConfig.dropChance)
                {
                    int amount = UnityEngine.Random.Range(dropConfig.minAmount, dropConfig.maxAmount + 1);
                    SpawnItem(dropConfig.prefab, dropConfig.itemType, amount);
                }
            }
        }

        OnDeath?.Invoke();
    }

    private GameObject GetPooledItem(GameObject prefab, ItemType type)
    {
        if (!itemPool.ContainsKey(type))
        {
            itemPool[type] = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                itemPool[type].Enqueue(obj);
            }
        }

        if (itemPool[type].Count > 0)
        {
            GameObject item = itemPool[type].Dequeue();
            item.SetActive(true);
            return item;
        }

        return Instantiate(prefab);
    }

    private void ReturnToPool(GameObject item, ItemType type)
    {
        item.SetActive(false);
        if (!itemPool.ContainsKey(type))
        {
            itemPool[type] = new Queue<GameObject>();
        }
        itemPool[type].Enqueue(item);
    }

    void SpawnItem(GameObject prefab, ItemType type, int amount)
    {
        float randomOffset = UnityEngine.Random.Range(0.2f, 0.5f);
        Vector2 randomDirection = new Vector2(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
        ).normalized;
        
        Vector3 spawnPosition = transform.position + (Vector3)(randomDirection * randomOffset);
        
        GameObject item = GetPooledItem(prefab, type);
        item.transform.position = spawnPosition;
        
        CollectibleItem collectible = item.GetComponent<CollectibleItem>();
        if (collectible == null)
        {
            collectible = item.AddComponent<CollectibleItem>();
        }
        collectible.itemType = type;
        collectible.amount = amount;
        
        item.layer = LayerMask.NameToLayer("Gear");
        item.tag = "Item";
        
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = item.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
        rb.isKinematic = true;
        
        Collider2D col = item.GetComponent<Collider2D>();
        if (col == null)
        {
            col = item.AddComponent<CircleCollider2D>();
        }
        col.isTrigger = true;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            FindNearestTarget();
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            MoveTowardsTarget(fastSpeed);
        }
        else if (distance > stopDistance)
        {
            MoveTowardsTarget(slowSpeed);
        }
    }

    void MoveTowardsTarget(float speed)
    {
        Vector2 dir = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (target == null)
        {
            FindNearestTarget();
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            if (!isBursting && cooldownTimer <= 0f)
            {
                isBursting = true;
                shotsLeftInBurst = burstCount;
                shotTimer = 0f;
            }

            if (isBursting)
            {
                shotTimer -= Time.deltaTime;

                if (shotTimer <= 0f && shotsLeftInBurst > 0)
                {
                    Shoot();
                    shotsLeftInBurst--;
                    shotTimer = timeBetweenShots;
                }

                if (shotsLeftInBurst <= 0)
                {
                    isBursting = false;
                    cooldownTimer = burstCooldown;
                }
            }
            else
            {
                cooldownTimer -= Time.deltaTime;
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || target == null) return;

        Vector2 dir = (target.position - firePoint.position).normalized;
        GameObject bullet = Bullet.GetBullet(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.direction = dir;
        bulletScript.owner = gameObject;

        // Ateş etme sesini çal
        if (shootSound != null && audioSource != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            Vector2 escapeDir = (transform.position - col.transform.position).normalized;
            
            if (escapeDir == Vector2.zero)
            {
                escapeDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            }
            
            transform.position += (Vector3)(escapeDir * 0.5f);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            Vector2 escapeDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            transform.position += (Vector3)(escapeDir * 0.5f);
        }
    }
}

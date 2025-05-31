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

    public Action OnDeath;

    private int shotsLeftInBurst;
    private float shotTimer;
    private float cooldownTimer;
    private bool isBursting = false;
    private Health health;
    private Rigidbody2D rb;
    private static readonly Dictionary<ItemType, Queue<GameObject>> itemPool = new Dictionary<ItemType, Queue<GameObject>>();
    private static readonly int poolSize = 20;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += Die;
        }
    }

    void Die()
    {
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
        if (target == null) return;

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
        if (target == null) return;

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

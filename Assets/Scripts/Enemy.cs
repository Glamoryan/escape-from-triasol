using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform target;
    public GameObject gearPrefab;
    public float attackRange = 4f;
    public float stopDistance = 1.5f;
    public float slowSpeed = 3f;
    public float fastSpeed = 12f;

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
        // Gear düşürme şansı ve kontrolü
        if (gearPrefab != null)
        {
            // %50 şansla gear düşür
            if (UnityEngine.Random.value <= 0.5f)
            {
                // Gear'ı düşmanın pozisyonunda oluştur
                GameObject gear = Instantiate(gearPrefab, transform.position, Quaternion.identity);
                
                // Gear'ın layer'ını ayarla
                gear.layer = LayerMask.NameToLayer("Gear");
                
                // Gear'a tag ekle
                gear.tag = "Item";
                
                // Rigidbody2D ekle ve ayarla
                Rigidbody2D rb = gear.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = gear.AddComponent<Rigidbody2D>();
                }
                rb.gravityScale = 0f;
                rb.isKinematic = true;
                
                // Collider2D ekle ve ayarla
                Collider2D col = gear.GetComponent<Collider2D>();
                if (col == null)
                {
                    col = gear.AddComponent<CircleCollider2D>();
                }
                col.isTrigger = true;
            }
        }

        OnDeath?.Invoke();
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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.direction = dir;
        bulletScript.owner = gameObject;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            Vector2 escapeDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;

            transform.position += (Vector3)(escapeDir * 0.5f);
        }
    }
}

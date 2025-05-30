using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform target;
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

    private Rigidbody2D rb;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
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

    void OnDestroy()
    {
        OnDeath?.Invoke();
    }
}

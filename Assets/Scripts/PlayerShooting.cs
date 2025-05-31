using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    public float range = 10f;
    public string enemyTag = "Enemy";

    private float fireCountdown = 0f;
    private Transform target;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindNearestEnemy();
            if (target != null)
            {
                Shoot();
            }
        }

        if (fireCountdown > 0)
        {
            fireCountdown -= Time.deltaTime;
        }
    }

    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Shoot()
    {
        if (fireCountdown <= 0f && bulletPrefab != null && firePoint != null && target != null)
        {
            Vector2 dir = (target.position - firePoint.position).normalized;
            GameObject bulletGO = Bullet.GetBullet(bulletPrefab, firePoint.position, Quaternion.identity);
            
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.direction = dir;
                bullet.owner = gameObject;
            }

            fireCountdown = 1f / fireRate;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
} 
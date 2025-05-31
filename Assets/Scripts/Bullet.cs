using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 25f;
    public float lifetime = 5f;

    [HideInInspector] public Vector2 direction;
    [HideInInspector] public GameObject owner;
    [HideInInspector] public GameObject prefab;

    private static readonly Dictionary<GameObject, Queue<GameObject>> bulletPools = new Dictionary<GameObject, Queue<GameObject>>();
    private static readonly int poolSize = 50;
    private float currentLifetime;

    private void OnEnable()
    {
        currentLifetime = lifetime;
    }

    public static void InitializePool(GameObject bulletPrefab)
    {
        if (!bulletPools.ContainsKey(bulletPrefab))
        {
            bulletPools[bulletPrefab] = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.SetActive(false);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.prefab = bulletPrefab;
                }
                bulletPools[bulletPrefab].Enqueue(bullet);
            }
        }
    }

    public static GameObject GetBullet(GameObject bulletPrefab, Vector3 position, Quaternion rotation)
    {
        if (!bulletPools.ContainsKey(bulletPrefab))
        {
            InitializePool(bulletPrefab);
        }

        if (bulletPools[bulletPrefab].Count > 0)
        {
            GameObject bullet = bulletPools[bulletPrefab].Dequeue();
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.SetActive(true);
            return bullet;
        }

        GameObject newBullet = Instantiate(bulletPrefab, position, rotation);
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.prefab = bulletPrefab;
        }
        return newBullet;
    }

    public static void ReturnToPool(GameObject bullet, GameObject prefab)
    {
        if (bullet == null) return;
        
        if (prefab == null)
        {
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                prefab = bulletScript.prefab;
            }
        }

        if (prefab == null)
        {
            Debug.LogWarning("Bullet prefab referansı bulunamadı, mermi yok ediliyor.");
            Destroy(bullet);
            return;
        }

        bullet.SetActive(false);
        if (!bulletPools.ContainsKey(prefab))
        {
            bulletPools[prefab] = new Queue<GameObject>();
        }
        bulletPools[prefab].Enqueue(bullet);
    }

    void Update()
    {
        if (direction != Vector2.zero)
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0)
        {
            ReturnToPool(gameObject, prefab);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // MapBounds'dan geçişe izin ver
        if (other.CompareTag("MapBounds"))
        {
            return;
        }

        // Kendi sahibine çarpmasını engelle
        if (owner != null && other.gameObject == owner)
        {
            return;
        }

        // Item veya Spaceship'e çarpmasını engelle
        if (other.CompareTag("Item") || other.CompareTag("Spaceship"))
        {
            return;
        }

        // Hasar verme mantığı
        if (owner != null)
        {
            // Player'dan gelen mermiler
            if (owner.CompareTag("Player"))
            {
                // Sadece düşmanlara hasar ver
                if (other.CompareTag("Enemy"))
                {
                    Health targetHealth = other.GetComponent<Health>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(damage);
                    }
                    ReturnToPool(gameObject, prefab);
                }
            }
            // Düşmanlardan gelen mermiler
            else if (owner.CompareTag("Enemy"))
            {
                // Player'a ve yapılara hasar ver
                if (other.CompareTag("Player") || other.CompareTag("Structure"))
                {
                    Health targetHealth = other.GetComponent<Health>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(damage);
                    }
                    ReturnToPool(gameObject, prefab);
                }
            }
            // Turret'lerden gelen mermiler
            else if (owner.CompareTag("Structure"))
            {
                // Sadece düşmanlara hasar ver
                if (other.CompareTag("Enemy"))
                {
                    Health enemyHealth = other.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damage);
                    }
                    ReturnToPool(gameObject, prefab);
                }
            }
        }
    }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform target;
    public float fireRate = 2f;

    private float fireTimer;

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        Vector2 dir = target.position - firePoint.position;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().direction = dir;
    }
}

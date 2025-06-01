using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    public float range = 10f;
    public string enemyTag = "Enemy";
    public AudioClip shootSound;
    public AudioClip upgradedShootSound; // Yükseltilmiş mermi için ses
    public float shootVolume = 1f;
    [Header("Ses Ayarları")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    private float fireCountdown = 0f;
    private Transform target;
    private AudioSource audioSource;
    private bool isUpgraded = false;

    void Start()
    {
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

    public void SetUpgraded(bool upgraded)
    {
        isUpgraded = upgraded;
    }

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

            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                AudioClip currentSound = isUpgraded && upgradedShootSound != null ? upgradedShootSound : shootSound;
                if (currentSound != null)
                {
                    audioSource.PlayOneShot(currentSound, shootVolume);
                }
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
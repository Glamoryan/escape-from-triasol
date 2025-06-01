using UnityEngine;

public class Turret2D : MonoBehaviour
{
    public float range = 5f;
    public string enemyTag = "Enemy";
    public Transform rotatingPart;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    [Header("Ses Ayarları")]
    public AudioClip shootSound;
    public float shootVolume = 0.5f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    private float fireCountdown = 0f;
    private Transform target;
    private AudioSource audioSource;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.3f);

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

    void UpdateTarget()
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

    void Update()
    {
        if (target == null)
            return;

        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotatingPart.rotation = Quaternion.Euler(0f, 0f, angle);

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || target == null) return;

        Vector2 dir = (target.position - firePoint.position).normalized;
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.direction = dir;
            bullet.owner = gameObject;
        }

        // Ateş etme sesini çal
        if (shootSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

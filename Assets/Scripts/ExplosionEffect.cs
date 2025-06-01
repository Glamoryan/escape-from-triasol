using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject[] explosionParticlePrefabs;
    public float explosionDuration = 0.5f;
    public float maxShakeDistance = 10f;
    public float maxShakeIntensity = 0.5f;
    public float shakeDuration = 0.3f;
    
    private GameObject currentExplosion;
    private float currentDuration;
    private Vector3 originalPosition;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = mainCamera.transform.position;
        
        // Rastgele bir particle efekti seç ve oluştur
        if (explosionParticlePrefabs != null && explosionParticlePrefabs.Length > 0)
        {
            GameObject selectedPrefab = explosionParticlePrefabs[Random.Range(0, explosionParticlePrefabs.Length)];
            currentExplosion = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
            
            // Particle sistemlerini durdur ve temizle
            ParticleSystem[] particleSystems = currentExplosion.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                
                // Yeni ayarları uygula
                var main = ps.main;
                main.duration = explosionDuration;
                main.loop = false;
                
                // Sistemi yeniden başlat
                ps.Play();
            }
        }
        
        // Kamera titremesini başlat
        StartCameraShake();
        
        // Patlamayı belirli süre sonra yok et
        Destroy(gameObject, explosionDuration);
    }
    
    void StartCameraShake()
    {
        if (mainCamera == null) return;
        
        float distance = Vector2.Distance(transform.position, mainCamera.transform.position);
        if (distance <= maxShakeDistance)
        {
            // Mesafeye göre titreme yoğunluğunu hesapla
            float intensity = Mathf.Lerp(maxShakeIntensity, 0f, distance / maxShakeDistance);
            StartCoroutine(ShakeCamera(intensity));
        }
    }
    
    System.Collections.IEnumerator ShakeCamera(float intensity)
    {
        float elapsed = 0f;
        Vector3 originalPos = mainCamera.transform.position;
        
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            mainCamera.transform.position = originalPos + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.transform.position = originalPos;
    }
} 
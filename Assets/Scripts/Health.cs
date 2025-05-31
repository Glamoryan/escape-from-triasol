using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; set; }

    [Header("Otomatik Can Yenileme")]
    public float regenDelay = 3f;        
    public float regenRate = 10f;        
    public bool canRegenerate = false;    // Varsayılan olarak can yenileme kapalı

    public event Action OnDeath;
    public event Action<float> OnHealthChanged;

    private float regenTimer = 0f;
    private HealthBar healthBar;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();

        // Eğer bu bir Player ise can yenilemeyi aktif et
        if (gameObject.CompareTag("Player"))
        {
            canRegenerate = true;
        }
    }

    void Update()
    {
        // Sadece can yenileme aktifse ve Player ise can yenile
        if (canRegenerate && currentHealth < maxHealth && regenTimer <= 0f)
        {
            currentHealth += regenRate * Time.deltaTime;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
        }
        else if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Player öldüyse game over tetikle
        if (gameObject.CompareTag("Player"))
        {
            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.TriggerGameOver(GameOverManager.GameOverType.PlayerDeath);
            }
        }

        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }

        Destroy(gameObject);
    }
}

using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    [Header("Otomatik Can Yenileme")]
    public float regenDelay = 3f;        
    public float regenRate = 10f;        

    public event Action OnDeath;
    public event Action<float> OnHealthChanged;

    private float regenTimer = 0f;
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        if (currentHealth < maxHealth && regenTimer <= 0f)
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

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);
        
        regenTimer = regenDelay;

        // Health bar'ı göster
        if (healthBar != null)
        {
            healthBar.ShowHealthBar();
        }

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}

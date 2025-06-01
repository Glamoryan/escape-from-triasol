using UnityEngine;
using System;
[RequireComponent(typeof(Animator))]

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; set; }

    [Header("Otomatik Can Yenileme")]
    public float regenDelay = 3f;
    public float regenRate = 10f;
    public bool canRegenerate = false;    // Varsayılan olarak can yenileme kapalı
    public Animator animator;

    public event Action OnDeath;
    public event Action<float> OnHealthChanged;

    private float regenTimer = 0f;
    private HealthBar healthBar;

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
            // Eğer bu bir turret ise BuildManager'a bildir
            if (gameObject.CompareTag("Structure"))
            {
                BuildManager.Instance?.RemoveTurret(gameObject);
            }

            OnDeath?.Invoke();
            animator.SetTrigger("IsDead");
        }

    }

    public void DestroyPlayer()
    {
            Destroy(gameObject);

    }
}

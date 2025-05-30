using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public GameObject healthBarPrefab;

    [Header("Otomatik Can Yenileme")]
    public float regenDelay = 3f;        
    public float regenRate = 10f;        

    public event Action OnDeath;

    private float currentHealth;
    private HealthBar healthBar;
    private float regenTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            
            GameObject bar = Instantiate(healthBarPrefab, transform);

            
            bar.transform.localPosition = new Vector3(0, 1f, 0);

            bar.transform.localScale = Vector3.one;

            healthBar = bar.GetComponent<HealthBar>();
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    void Update()
    {
        
        if (currentHealth < maxHealth && regenTimer <= 0f)
        {
            currentHealth += regenRate * Time.deltaTime;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            if (healthBar != null) healthBar.SetHealth(currentHealth);
        }
        else if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        
        regenTimer = regenDelay;

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}

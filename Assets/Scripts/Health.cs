using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public GameObject healthBarPrefab;

    public event Action OnDeath;

    private float currentHealth;
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            // Bar'ı objeye child olarak instantiate et
            GameObject bar = Instantiate(healthBarPrefab, transform);

            // Sabit pozisyonla biraz yukarı yerleştir
            bar.transform.localPosition = new Vector3(0, 1f, 0);

            // Bar'ın scale'ini prefab'takiyle aynı tut
            bar.transform.localScale = Vector3.one;

            // Bileşeni al ve başlangıç değerini ayarla
            healthBar = bar.GetComponent<HealthBar>();
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}

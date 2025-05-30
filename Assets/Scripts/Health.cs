using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public GameObject healthBarPrefab;

    private float currentHealth;
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            Vector3 offsetPos = transform.position + Vector3.up * 0.5f;
            GameObject bar = Instantiate(healthBarPrefab, offsetPos, Quaternion.identity, transform);
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
            Destroy(gameObject);
        }
    }
}

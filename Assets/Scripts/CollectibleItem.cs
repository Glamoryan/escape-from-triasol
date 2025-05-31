using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemType itemType;
    public int amount = 1;
    public GameObject visualPrefab;
    public float lifetime = 30f; // Item'ın yerde kalma süresi (saniye)

    private float currentLifetime;

    void Start()
    {
        if (visualPrefab != null)
        {
            Instantiate(visualPrefab, transform.position, Quaternion.identity, transform);
        }
        currentLifetime = lifetime;
    }

    void Update()
    {
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
} 
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemType itemType;
    public int amount = 1;
    public GameObject visualPrefab;
    public float lifetime = 30f; // Item'ın yerde kalma süresi (saniye)
    public float autoCollectRange = 1.5f; // Otomatik toplama mesafesi
    public float moveSpeed = 5f; // Oyuncuya doğru hareket hızı

    private float currentLifetime;
    private Transform player;
    private bool isMovingToPlayer = false;

    void Start()
    {
        if (visualPrefab != null)
        {
            Instantiate(visualPrefab, transform.position, Quaternion.identity, transform);
        }
        currentLifetime = lifetime;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Eğer oyuncu yakındaysa ve henüz hareket etmiyorsa
        if (distanceToPlayer <= autoCollectRange && !isMovingToPlayer)
        {
            isMovingToPlayer = true;
        }

        // Eğer oyuncuya doğru hareket ediyorsa
        if (isMovingToPlayer)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            // Eğer oyuncuya yeterince yaklaştıysa topla
            if (distanceToPlayer <= 0.1f)
            {
                CollectItem();
            }
        }

        // Lifetime kontrolü
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void CollectItem()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemType, amount);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Otomatik toplama mesafesini görselleştir
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, autoCollectRange);
    }
} 
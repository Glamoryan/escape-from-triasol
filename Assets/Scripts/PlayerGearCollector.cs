using UnityEngine;

public class PlayerGearCollector : MonoBehaviour
{
    public float collectionRange = 1.5f;
    public string itemTag = "Item";
    public LayerMask gearLayer;
    public float manualCollectionRange = 3f;

    void Update()
    {
        // F tuşuna basıldığında manuel toplama
        if (Input.GetKeyDown(KeyCode.F))
        {
            CollectGearsInRange(manualCollectionRange);
        }

        // Otomatik toplama
        CollectGearsInRange(collectionRange);
    }

    void CollectGearsInRange(float range)
    {
        // Karakterin etrafındaki item'ları bul
        Collider2D[] nearbyItems = Physics2D.OverlapCircleAll(transform.position, range, gearLayer);

        foreach (Collider2D item in nearbyItems)
        {
            if (item.CompareTag(itemTag))
            {
                // Item'ı karaktere doğru çek
                Vector2 direction = (transform.position - item.transform.position).normalized;
                item.transform.position = Vector2.MoveTowards(
                    item.transform.position,
                    transform.position,
                    Time.deltaTime * 10f
                );

                // Eğer item karaktere yeterince yakınsa topla
                if (Vector2.Distance(transform.position, item.transform.position) < 0.1f)
                {
                    CollectGear(item.gameObject);
                }
            }
        }
    }

    void CollectGear(GameObject item)
    {
        // InventoryManager üzerinden gear'ı ekle
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddGear(1);
            Destroy(item);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Otomatik toplama menzilini görselleştir
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRange);

        // Manuel toplama menzilini görselleştir
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, manualCollectionRange);
    }
} 
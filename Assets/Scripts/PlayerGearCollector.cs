using UnityEngine;

public class PlayerGearCollector : MonoBehaviour
{
    public float collectionRange = 1.5f;
    public string itemTag = "Item";
    public LayerMask itemLayer;
    public float manualCollectionRange = 3f;

    private CollectibleItem nearbyItem;

    void Start()
    {
        itemLayer = LayerMask.GetMask("Gear");
        Debug.Log($"Item Layer Mask: {itemLayer.value}");
    }

    void Update()
    {
        if (nearbyItem != null)
        {
            Debug.Log($"Yakında item var: {nearbyItem.itemType}, Miktar: {nearbyItem.amount}");
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F tuşuna basıldı, item toplanıyor...");
                InventoryManager.Instance.AddItem(nearbyItem.itemType, nearbyItem.amount);
                Destroy(nearbyItem.gameObject);
                nearbyItem = null;
            }
        }

        CollectItemsInRange(collectionRange);
    }

    void CollectItemsInRange(float range)
    {
        Collider2D[] nearbyItems = Physics2D.OverlapCircleAll(transform.position, range, itemLayer);
        Debug.Log($"Yakında {nearbyItems.Length} adet item bulundu");

        foreach (Collider2D item in nearbyItems)
        {
            if (item.CompareTag(itemTag))
            {
                Debug.Log($"Item bulundu: {item.name}, Tag: {item.tag}, Layer: {item.gameObject.layer}");
                
                Vector2 direction = (transform.position - item.transform.position).normalized;
                item.transform.position = Vector2.MoveTowards(
                    item.transform.position,
                    transform.position,
                    Time.deltaTime * 10f
                );

                if (Vector2.Distance(transform.position, item.transform.position) < 0.1f)
                {
                    Debug.Log("Item yeterince yakın, toplanıyor...");
                    CollectItem(item.gameObject);
                }
            }
        }
    }

    void CollectItem(GameObject item)
    {
        if (InventoryManager.Instance != null)
        {
            CollectibleItem collectible = item.GetComponent<CollectibleItem>();
            if (collectible != null)
            {
                Debug.Log($"Item toplanıyor: {collectible.itemType}, Miktar: {collectible.amount}");
                InventoryManager.Instance.AddItem(collectible.itemType, collectible.amount);
                Destroy(item);
            }
            else
            {
                Debug.LogWarning($"Item'da CollectibleItem bileşeni yok: {item.name}");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, manualCollectionRange);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(itemTag))
        {
            Debug.Log($"Trigger Enter: {other.name}, Tag: {other.tag}, Layer: {other.gameObject.layer}");
            nearbyItem = other.GetComponent<CollectibleItem>();
            if (nearbyItem == null)
            {
                Debug.LogWarning($"Item'da CollectibleItem bileşeni yok: {other.name}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(itemTag) && other.GetComponent<CollectibleItem>() == nearbyItem)
        {
            Debug.Log($"Trigger Exit: {other.name}");
            nearbyItem = null;
        }
    }
} 
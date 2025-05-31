using UnityEngine;

public class PlayerGearCollector : MonoBehaviour
{
    public float collectionRange = 1.5f;
    public string itemTag = "Item";
    private CollectibleItem nearbyItem;

    void Update()
    {
        // Otomatik toplama
        CollectItemsInRange(collectionRange);
    }

    void CollectItemsInRange(float range)
    {
        Collider2D[] nearbyItems = Physics2D.OverlapCircleAll(transform.position, range);
        Debug.Log($"Yakında {nearbyItems.Length} adet item bulundu");

        foreach (Collider2D item in nearbyItems)
        {
            if (item == null) continue;

            Debug.Log($"Item kontrol ediliyor: {item.name}, Tag: {item.tag}");
            
            if (item.CompareTag(itemTag))
            {
                Debug.Log($"Item bulundu ve toplanıyor: {item.name}");
                CollectItem(item.gameObject);
            }
            else
            {
                Debug.Log($"Item tag'i uyuşmuyor. Beklenen: {itemTag}, Bulunan: {item.tag}");
            }
        }
    }

    void CollectItem(GameObject item)
    {
        if (item == null) return;

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
        else
        {
            Debug.LogError("InventoryManager.Instance bulunamadı!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(itemTag))
        {
            CollectItem(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(itemTag) && other.GetComponent<CollectibleItem>() == nearbyItem)
        {
            nearbyItem = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
} 
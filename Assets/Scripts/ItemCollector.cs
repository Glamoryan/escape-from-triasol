using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private CollectibleItem nearbyItem;

    void Update()
    {
        if (nearbyItem != null && Input.GetKeyDown(KeyCode.F))
        {
            InventoryManager.Instance.AddItem(nearbyItem.itemType, nearbyItem.amount);
            Destroy(nearbyItem.gameObject);
            nearbyItem = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            nearbyItem = other.GetComponent<CollectibleItem>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item") && other.GetComponent<CollectibleItem>() == nearbyItem)
        {
            nearbyItem = null;
        }
    }
}

using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private Collider2D nearbyItem;

    void Update()
    {
        if (nearbyItem != null && Input.GetKeyDown(KeyCode.F))
        {
            InventoryManager.Instance.AddGear(1);
            Destroy(nearbyItem.gameObject);
            nearbyItem = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            nearbyItem = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == nearbyItem)
        {
            nearbyItem = null;
        }
    }
}

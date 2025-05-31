using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemType itemType;
    public int amount = 1;
    public GameObject visualPrefab;

    void Start()
    {
        if (visualPrefab != null)
        {
            Instantiate(visualPrefab, transform.position, Quaternion.identity, transform);
        }
    }
} 
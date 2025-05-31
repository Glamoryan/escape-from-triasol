using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class ItemUI
    {
        public ItemType type;
        public TMP_Text text;
        public string prefix = "x";
    }

    public List<ItemUI> itemUIs = new List<ItemUI>();
    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();

    void Awake()
    {
        Instance = this;
        InitializeItems();
    }

    void InitializeItems()
    {
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            items[type] = 0;
        }
        UpdateAllUI();
    }

    public bool TrySpendItem(ItemType type, int amount)
    {
        if (items.ContainsKey(type) && items[type] >= amount)
        {
            items[type] -= amount;
            UpdateUI(type);
            return true;
        }
        return false;
    }

    public void AddItem(ItemType type, int amount)
    {
        if (!items.ContainsKey(type))
            items[type] = 0;

        items[type] += amount;
        UpdateUI(type);
    }

    void UpdateUI(ItemType type)
    {
        var itemUI = itemUIs.Find(ui => ui.type == type);
        if (itemUI != null && itemUI.text != null)
        {
            itemUI.text.text = itemUI.prefix + items[type];
        }
    }

    void UpdateAllUI()
    {
        foreach (var itemUI in itemUIs)
        {
            UpdateUI(itemUI.type);
        }
    }
}

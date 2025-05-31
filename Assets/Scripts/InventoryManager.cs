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
    }

    [System.Serializable]
    public class StartingItem
    {
        public ItemType type;
        public int amount;
    }

    [Header("UI Elements")]
    public List<ItemUI> itemUIs = new List<ItemUI>();

    [Header("Starting Inventory")]
    public List<StartingItem> startingItems = new List<StartingItem>();

    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeItems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeItems()
    {
        // Önce tüm itemleri sıfırla
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            items[type] = 0;
        }

        // Başlangıç itemlerini ekle
        foreach (var startingItem in startingItems)
        {
            if (startingItem != null)
            {
                items[startingItem.type] = startingItem.amount;
            }
        }

        UpdateAllUI();
    }

    public bool TrySpendItem(ItemType type, int amount, bool checkOnly = false)
    {
        if (!items.ContainsKey(type) || items[type] < amount)
            return false;

        if (!checkOnly)
        {
            items[type] -= amount;
            UpdateUI(type);
        }
        return true;
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
        if (itemUIs == null) return;

        var itemUI = itemUIs.Find(ui => ui != null && ui.type == type);
        if (itemUI != null && itemUI.text != null)
        {
            itemUI.text.text = items[type].ToString();
        }
    }

    void UpdateAllUI()
    {
        if (itemUIs == null) return;

        foreach (var itemUI in itemUIs)
        {
            if (itemUI != null)
            {
                UpdateUI(itemUI.type);
            }
        }
    }

    void OnValidate()
    {
        // Editor'da değişiklik olduğunda UI'ı güncelle
        if (Application.isPlaying) return;
        
        if (itemUIs != null)
        {
            foreach (var itemUI in itemUIs)
            {
                if (itemUI != null && itemUI.text != null)
                {
                    itemUI.text.text = "0";
                }
            }
        }
    }
}

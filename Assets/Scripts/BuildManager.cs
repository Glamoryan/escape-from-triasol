using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [System.Serializable]
    public class BuildableItem
    {
        public string name;
        public GameObject prefab;
        public int gearCost;
        public int batteryCost;
        public int ironCost;
    }

    public BuildableItem[] buildableItems;
    public Transform buildPoint;
    public float buildRange = 5f;

    [Header("UI Elements")]
    public Canvas buildModeCanvas;
    public TextMeshProUGUI buildModeText;
    public TextMeshProUGUI selectedItemText;
    public TextMeshProUGUI resourceCostText;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;
    public Color insufficientColor = Color.red;

    private BuildableItem selectedItem;
    private bool isBuilding = false;
    private GameObject previewObject;
    private Camera mainCamera;
    private Vector3 mouseOffset = new Vector3(0.5f, 0.5f, 0);

    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        
        if (buildModeCanvas != null)
            buildModeCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode();
        }

        if (isBuilding)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            buildPoint.position = mousePosition + mouseOffset;

            if (Input.GetKeyDown(KeyCode.Alpha1) && buildableItems.Length > 0)
            {
                selectedItem = buildableItems[0];
                UpdatePreview();
                UpdateSelectedItemUI();
                UpdateResourceCostUI();
                Debug.Log($"Item seçildi: {selectedItem.name}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && buildableItems.Length > 1)
            {
                selectedItem = buildableItems[1];
                UpdatePreview();
                UpdateSelectedItemUI();
                UpdateResourceCostUI();
                Debug.Log($"Item seçildi: {selectedItem.name}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && buildableItems.Length > 2)
            {
                selectedItem = buildableItems[2];
                UpdatePreview();
                UpdateSelectedItemUI();
                UpdateResourceCostUI();
                Debug.Log($"Item seçildi: {selectedItem.name}");
            }

            if (selectedItem != null && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                TryBuild();
            }

            if (previewObject != null)
            {
                previewObject.transform.position = buildPoint.position;
            }
        }
    }

    void ToggleBuildMode()
    {
        isBuilding = !isBuilding;
        UpdateBuildModeUI();

        if (!isBuilding)
        {
            selectedItem = null;
            if (previewObject != null)
            {
                Destroy(previewObject);
                previewObject = null;
            }
            UpdateSelectedItemUI();
            UpdateResourceCostUI();
        }
    }

    void UpdateBuildModeUI()
    {
        if (buildModeCanvas != null)
        {
            buildModeCanvas.gameObject.SetActive(isBuilding);
            if (buildModeText != null)
            {
                buildModeText.text = isBuilding ? "Build Mode: Enabled" : "Build Mode: Disabled";
                buildModeText.color = isBuilding ? activeColor : inactiveColor;
            }
        }
    }

    void UpdateSelectedItemUI()
    {
        if (selectedItemText != null)
        {
            if (selectedItem != null)
            {
                selectedItemText.text = $"Selected Item: {selectedItem.name}";
                selectedItemText.color = activeColor;
            }
            else
            {
                selectedItemText.text = "Selected Item: None";
                selectedItemText.color = inactiveColor;
            }
        }
    }

    void UpdateResourceCostUI()
    {
        if (resourceCostText != null)
        {
            if (selectedItem != null)
            {
                string costText = "Required Resources:\n";
                bool hasInsufficientResources = false;

                if (selectedItem.gearCost > 0)
                {
                    bool hasEnoughGear = HasEnoughResource(ItemType.Gear, selectedItem.gearCost);
                    costText += $"Gear: {selectedItem.gearCost} {(hasEnoughGear ? "" : "(Insufficient)")}\n";
                    if (!hasEnoughGear) hasInsufficientResources = true;
                }
                if (selectedItem.batteryCost > 0)
                {
                    bool hasEnoughBattery = HasEnoughResource(ItemType.Battery, selectedItem.batteryCost);
                    costText += $"Battery: {selectedItem.batteryCost} {(hasEnoughBattery ? "" : "(Insufficient)")}\n";
                    if (!hasEnoughBattery) hasInsufficientResources = true;
                }
                if (selectedItem.ironCost > 0)
                {
                    bool hasEnoughIron = HasEnoughResource(ItemType.Iron, selectedItem.ironCost);
                    costText += $"Iron: {selectedItem.ironCost} {(hasEnoughIron ? "" : "(Insufficient)")}";
                    if (!hasEnoughIron) hasInsufficientResources = true;
                }
                
                resourceCostText.text = costText;
                resourceCostText.color = hasInsufficientResources ? insufficientColor : activeColor;
            }
            else
            {
                resourceCostText.text = "Required Resources:\nNone";
                resourceCostText.color = inactiveColor;
            }
        }
    }

    bool HasEnoughResource(ItemType type, int amount)
    {
        if (InventoryManager.Instance == null) return false;
        
        // Geçici olarak kaynakları harcamayı dene
        bool hasEnough = InventoryManager.Instance.TrySpendItem(type, amount);
        
        // Eğer yeterli kaynak varsa, harcanan kaynakları geri ekle
        if (hasEnough)
        {
            InventoryManager.Instance.AddItem(type, amount);
        }
        
        return hasEnough;
    }

    void UpdatePreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        if (selectedItem != null && selectedItem.prefab != null)
        {
            previewObject = Instantiate(selectedItem.prefab, buildPoint.position, Quaternion.identity);
            
            // Preview için gerekli ayarları yap
            SpriteRenderer[] renderers = previewObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                Color color = renderer.color;
                color.a = 0.5f;
                renderer.color = color;
            }

            // Fiziksel etkileşimi engelle
            Collider2D[] colliders = previewObject.GetComponentsInChildren<Collider2D>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            // Rigidbody varsa devre dışı bırak
            Rigidbody2D rb = previewObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = false;
            }

            // Tüm davranışları devre dışı bırak
            MonoBehaviour[] behaviours = previewObject.GetComponentsInChildren<MonoBehaviour>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour != null && !(behaviour is SpriteRenderer))
                {
                    behaviour.enabled = false;
                }
            }

            // Preview tag'ini ekle
            previewObject.tag = "Preview";

            previewObject.transform.position = buildPoint.position;
            Debug.Log("Preview güncellendi");
        }
    }

    void TryBuild()
    {
        if (selectedItem == null || buildPoint == null)
        {
            Debug.LogWarning("Build için gerekli bileşenler eksik!");
            return;
        }

        if (!HasEnoughResources(selectedItem))
        {
            Debug.Log("Yeterli kaynak yok!");
            return;
        }

        SpendResources(selectedItem);

        GameObject builtObject = Instantiate(selectedItem.prefab, buildPoint.position, Quaternion.identity);
        
        // İnşa edilen objenin tüm davranışlarını etkinleştir
        MonoBehaviour[] behaviours = builtObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (var behaviour in behaviours)
        {
            if (behaviour != null)
            {
                behaviour.enabled = true;
            }
        }

        Debug.Log($"{selectedItem.name} inşa edildi!");
    }

    bool HasEnoughResources(BuildableItem item)
    {
        if (item.gearCost > 0 && !InventoryManager.Instance.TrySpendItem(ItemType.Gear, item.gearCost))
        {
            Debug.Log($"Yeterli Gear yok! Gerekli: {item.gearCost}");
            return false;
        }
        if (item.batteryCost > 0 && !InventoryManager.Instance.TrySpendItem(ItemType.Battery, item.batteryCost))
        {
            Debug.Log($"Yeterli Battery yok! Gerekli: {item.batteryCost}");
            return false;
        }
        if (item.ironCost > 0 && !InventoryManager.Instance.TrySpendItem(ItemType.Iron, item.ironCost))
        {
            Debug.Log($"Yeterli Iron yok! Gerekli: {item.ironCost}");
            return false;
        }
        return true;
    }

    void SpendResources(BuildableItem item)
    {
        if (item.gearCost > 0)
            InventoryManager.Instance.TrySpendItem(ItemType.Gear, item.gearCost);
        if (item.batteryCost > 0)
            InventoryManager.Instance.TrySpendItem(ItemType.Battery, item.batteryCost);
        if (item.ironCost > 0)
            InventoryManager.Instance.TrySpendItem(ItemType.Iron, item.ironCost);
    }

    void OnDrawGizmosSelected()
    {
        if (buildPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(buildPoint.position, 0.5f);
        }
    }
}

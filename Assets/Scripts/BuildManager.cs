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
            isBuilding = !isBuilding;
            Debug.Log($"Build modu: {(isBuilding ? "Açık" : "Kapalı")}");
            
            UpdateBuildModeUI();
            
            if (!isBuilding)
            {
                selectedItem = null;
                if (previewObject != null)
                {
                    Destroy(previewObject);
                    previewObject = null;
                }
            }
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
                Debug.Log($"Item seçildi: {selectedItem.name}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && buildableItems.Length > 1)
            {
                selectedItem = buildableItems[1];
                UpdatePreview();
                UpdateSelectedItemUI();
                Debug.Log($"Item seçildi: {selectedItem.name}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && buildableItems.Length > 2)
            {
                selectedItem = buildableItems[2];
                UpdatePreview();
                UpdateSelectedItemUI();
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

    void UpdateBuildModeUI()
    {
        if (buildModeCanvas != null)
        {
            buildModeCanvas.gameObject.SetActive(isBuilding);
            if (buildModeText != null)
            {
                buildModeText.text = isBuilding ? "Build Mode: Enabled" : "Build Mode: Disabled";
            }
        }
    }

    void UpdateSelectedItemUI()
    {
        if (selectedItemText != null)
        {
            selectedItemText.text = selectedItem != null ? $"Selected Item: {selectedItem.name}" : "Selected Item: None";
        }
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
            SpriteRenderer[] renderers = previewObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                Color color = renderer.color;
                color.a = 0.5f;
                renderer.color = color;
            }

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

        Instantiate(selectedItem.prefab, buildPoint.position, Quaternion.identity);
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

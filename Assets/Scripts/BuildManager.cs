using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject ghostBlockPrefab;

    private GameObject ghostInstance;
    private bool isPlacing = false;

    void Update()
    {
        if (!isPlacing) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 snapped = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0);
        ghostInstance.transform.position = snapped;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (InventoryManager.Instance.TrySpendGear(5))
            {
                // Rotation'ı ghost üzerinden al
                Quaternion rotation = Quaternion.identity;
                var ghostScript = ghostInstance.GetComponent<GhostBlockController>();
                if (ghostScript != null)
                    rotation = ghostScript.GetCurrentRotation();

                Instantiate(blockPrefab, snapped, rotation);
            }
            else
            {
                Debug.Log("Yetersiz gear!");
            }
            CancelPlacement();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    public void StartPlacement()
    {
        if (ghostInstance != null) Destroy(ghostInstance);
        ghostInstance = Instantiate(ghostBlockPrefab);
        isPlacing = true;
    }

    private void CancelPlacement()
    {
        if (ghostInstance != null) Destroy(ghostInstance);
        isPlacing = false;
    }
}

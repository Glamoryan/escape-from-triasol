using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class RepairRequirement
{
    public ItemType itemType;
    public int requiredAmount;
    public int currentAmount;
}

public class SpaceshipRepairManager : MonoBehaviour
{
    public List<RepairRequirement> repairRequirements = new List<RepairRequirement>();
    public float interactionDistance = 3f;
    public GameObject repairButton;
    public Transform player;
    public Canvas repairCanvas;
    public TextMeshProUGUI repairStatusText;
    public CanvasGroup buttonCanvasGroup;
    public float disabledOpacity = 0.3f;
    public Color itemTextColor = new Color(1f, 0.84f, 0f); // Varsayılan altın sarısı

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        repairButton.SetActive(false);
        UpdateRepairStatusUI();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool isInRange = distance <= interactionDistance;
        repairButton.SetActive(isInRange);

        if (isInRange)
        {
            UpdateButtonOpacity();
        }
    }

    private void UpdateButtonOpacity()
    {
        if (buttonCanvasGroup == null) return;

        bool hasAnyItems = false;
        foreach (var requirement in repairRequirements)
        {
            if (requirement.currentAmount < requirement.requiredAmount)
            {
                int needed = requirement.requiredAmount - requirement.currentAmount;
                if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requirement.itemType, 1)) // En az 1 tane varsa yeterli
                {
                    hasAnyItems = true;
                    break;
                }
            }
        }

        buttonCanvasGroup.alpha = hasAnyItems ? 1f : disabledOpacity;
    }

    public void TryRepair()
    {
        // Önce Gear gereksinimini bul
        var gearRequirement = repairRequirements.Find(r => r.itemType == ItemType.Gear);
        if (gearRequirement != null)
        {
            // Envanterden Gear'ı al
            int available = InventoryManager.Instance.GetItemCount(ItemType.Gear);
            if (available > 0)
            {
                int needed = gearRequirement.requiredAmount - gearRequirement.currentAmount;
                int amountToUse = Mathf.Min(available, needed);
                if (InventoryManager.Instance.TrySpendItem(ItemType.Gear, amountToUse))
                {
                    gearRequirement.currentAmount += amountToUse;
                    Debug.Log($"Gear için {amountToUse} adet item eklendi. Toplam: {gearRequirement.currentAmount}/{gearRequirement.requiredAmount}");
                }
            }
        }

        // Gear gereksinimini kontrol et
        bool canRepair = gearRequirement != null && gearRequirement.currentAmount >= gearRequirement.requiredAmount;

        if (canRepair)
        {
            // Tamir işlemi tamamlandı
            Debug.Log("Spaceship tamir edildi!");
            repairButton.SetActive(false);
            repairCanvas.gameObject.SetActive(false);
            
            // Game over tetikle
            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.TriggerGameOver(GameOverManager.GameOverType.SpaceshipRepaired);
            }
        }
        else
        {
            UpdateRepairStatusUI();
            UpdateButtonOpacity();
        }
    }

    private void UpdateRepairStatusUI()
    {
        if (repairStatusText == null) return;

        string status = "Spaceship Repair Status:\n";
        foreach (var requirement in repairRequirements)
        {
            string itemName = requirement.itemType.ToString();
            string colorHex = ColorUtility.ToHtmlStringRGB(itemTextColor);
            string coloredItemName = $"<color=#{colorHex}>{itemName}</color>";
            status += $"{coloredItemName}: {requirement.currentAmount} / {requirement.requiredAmount}\n";
        }
        repairStatusText.text = status;
    }
} 
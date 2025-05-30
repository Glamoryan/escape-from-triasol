using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public TMP_Text gearText;
    private int gearCount = 0;

    void Awake() => Instance = this;

    public bool TrySpendGear(int amount)
    {
        if (gearCount >= amount)
        {
            gearCount -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddGear(int amount)
    {
        gearCount += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        gearText.text = "x" + gearCount;
    }
}

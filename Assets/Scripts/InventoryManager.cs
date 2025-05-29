using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int gearCount = 0;
    public TMP_Text gearText;

    void Awake()
    {
        Instance = this;
        UpdateUI();
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

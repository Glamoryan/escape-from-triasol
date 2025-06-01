using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Başlangıç Kaynakları")]
    public int startingResources = 100;

    [Header("UI Referansları")]
    public TextMeshProUGUI resourceText;

    private int currentResources;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentResources = startingResources;
        UpdateResourceUI();
    }

    public void AddResources(int amount)
    {
        currentResources += amount;
        UpdateResourceUI();
    }

    public bool TrySpendResources(int amount)
    {
        if (currentResources >= amount)
        {
            currentResources -= amount;
            UpdateResourceUI();
            return true;
        }
        return false;
    }

    public int GetCurrentResources()
    {
        return currentResources;
    }

    private void UpdateResourceUI()
    {
        if (resourceText != null)
        {
            resourceText.text = currentResources.ToString();
        }
    }

    public void ResetResources()
    {
        currentResources = startingResources;
        UpdateResourceUI();
    }
} 
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public static EnergyBar Instance { get; private set; }

    public Image fillImage;
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float regenerationRate = 10f; // Saniyede yenilenen enerji
    public float regenerationDelay = 1f; // Koşma bırakıldıktan sonra yenilenmenin başlaması için beklenecek süre

    private float regenerationTimer;

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
    }

    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateUI();
        Debug.Log($"EnergyBar başlatıldı. Başlangıç enerjisi: {currentEnergy}");
    }

    void Update()
    {
        if (regenerationTimer > 0)
        {
            regenerationTimer -= Time.deltaTime;
        }
        else if (currentEnergy < maxEnergy)
        {
            currentEnergy = Mathf.Min(currentEnergy + regenerationRate * Time.deltaTime, maxEnergy);
            UpdateUI();
        }
    }

    public bool TryUseEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            regenerationTimer = regenerationDelay;
            UpdateUI();
            Debug.Log($"Enerji kullanıldı. Kalan enerji: {currentEnergy}");
            return true;
        }
        Debug.Log($"Yetersiz enerji. Mevcut: {currentEnergy}, Gerekli: {amount}");
        return false;
    }

    void UpdateUI()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentEnergy / maxEnergy;
            Debug.Log($"UI güncellendi. Fill miktarı: {fillImage.fillAmount}");
        }
        else
        {
            Debug.LogWarning("Fill Image referansı eksik!");
        }
    }
} 
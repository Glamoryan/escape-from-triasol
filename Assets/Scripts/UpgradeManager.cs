using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public Button bulletUpgradeButton;
    public Button suitUpgradeButton;
    public Button turretUpgradeButton;

    public AudioSource upgradeBulletSound;
    public AudioSource upgradeSuitSound;
    public AudioSource upgradeTurretSound;

    // Upgrade için gerekli item miktarları
    public int bulletUpgradeGear = 5;
    public int suitUpgradeBattery = 3;
    public int turretUpgradeIron = 10;

    // Item miktarı göstergeleri
    public TextMeshProUGUI bulletUpgradeCostText;
    public TextMeshProUGUI suitUpgradeCostText;
    public TextMeshProUGUI turretUpgradeCostText;

    // Bullet upgrade için yeni prefab
    public GameObject upgradedBulletPrefab;

    // Suit upgrade için can artışı
    public float suitHealthBonus = 50f;

    // Turret upgrade için bonuslar
    public float turretHealthBonus = 50f;
    public float turretDamageBonus = 10f;
    public float turretFireRateBonus = 0.5f;

    // Sayaçlar
    private int bulletUpgradeCount = 0;
    private int suitUpgradeCount = 0;
    private int turretUpgradeCount = 0;

    // Sayaç UI'ları
    public TextMeshProUGUI bulletUpgradeCountText;
    public TextMeshProUGUI suitUpgradeCountText;
    public TextMeshProUGUI turretUpgradeCountText;

    // Canvas Group referansı
    public CanvasGroup upgradeCanvasGroup;

    void Start()
    {
        if (bulletUpgradeButton != null)
            bulletUpgradeButton.onClick.AddListener(UpgradeBullet);
        if (suitUpgradeButton != null)
            suitUpgradeButton.onClick.AddListener(UpgradeSuit);
        if (turretUpgradeButton != null)
            turretUpgradeButton.onClick.AddListener(UpgradeTurrets);
        UpdateUpgradeCountTexts();
        UpdateUpgradeCostTexts();
    }

    void Update()
    {
        // Kaynak kontrolü
        bool hasBulletItems = InventoryManager.Instance != null && InventoryManager.Instance.TrySpendItem(ItemType.Gear, bulletUpgradeGear, true);
        bool hasSuitItems = InventoryManager.Instance != null && InventoryManager.Instance.TrySpendItem(ItemType.Battery, suitUpgradeBattery, true);
        bool hasTurretItems = InventoryManager.Instance != null && InventoryManager.Instance.TrySpendItem(ItemType.Iron, turretUpgradeIron, true);

        if (bulletUpgradeButton != null)
            bulletUpgradeButton.interactable = hasBulletItems;
        if (suitUpgradeButton != null)
            suitUpgradeButton.interactable = hasSuitItems;
        if (turretUpgradeButton != null)
            turretUpgradeButton.interactable = hasTurretItems;

        // Opacity ayarı
        if (upgradeCanvasGroup != null)
        {
            bool anyAvailable = hasBulletItems || hasSuitItems || hasTurretItems;
            upgradeCanvasGroup.alpha = anyAvailable ? 1f : 0.5f;
        }
    }

    void UpgradeBullet()
    {
        if (InventoryManager.Instance.TrySpendItem(ItemType.Gear, bulletUpgradeGear))
        {
            PlayerShooting playerShooting = FindObjectOfType<PlayerShooting>();
            if (playerShooting != null && upgradedBulletPrefab != null)
            {
                playerShooting.bulletPrefab = upgradedBulletPrefab;
                playerShooting.SetUpgraded(true);
                bulletUpgradeCount++;
                UpdateUpgradeCountTexts();
                if (upgradeBulletSound != null)
                {
                    upgradeBulletSound.Play();
                }
                Debug.Log("Bullet upgrade başarılı!");
            }
        }
        else
        {
            Debug.Log("Yeterli Gear yok!");
        }
    }

    void UpgradeSuit()
    {
        if (InventoryManager.Instance.TrySpendItem(ItemType.Battery, suitUpgradeBattery))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Health health = player.GetComponent<Health>();
                if (health != null)
                {
                    health.maxHealth += suitHealthBonus;
                    health.currentHealth += suitHealthBonus;
                    suitUpgradeCount++;
                    UpdateUpgradeCountTexts();
                    if (upgradeSuitSound != null)
                    {
                        upgradeSuitSound.Play();
                    }
                    Debug.Log("Kıyafet upgrade başarılı!");
                }
            }
        }
        else
        {
            Debug.Log("Yeterli Battery yok!");
        }
    }

    void UpgradeTurrets()
    {
        if (InventoryManager.Instance.TrySpendItem(ItemType.Iron, turretUpgradeIron))
        {
            GameObject[] turrets = GameObject.FindGameObjectsWithTag("Structure");
            foreach (GameObject turret in turrets)
            {
                Health health = turret.GetComponent<Health>();
                if (health != null)
                {
                    health.maxHealth += turretHealthBonus;
                    health.currentHealth += turretHealthBonus;
                }
                Turret2D turretScript = turret.GetComponent<Turret2D>();
                if (turretScript != null)
                {
                    turretScript.fireRate += turretFireRateBonus;
                    // Eğer damage değişkeni varsa:
                    // turretScript.damage += turretDamageBonus;
                }
            }
            turretUpgradeCount++;
            UpdateUpgradeCountTexts();
            if (upgradeTurretSound != null)
            {
                upgradeTurretSound.Play();
            }
            Debug.Log("Tüm taretler upgrade edildi!");
        }
        else
        {
            Debug.Log("Yeterli Iron yok!");
        }
    }

    void UpdateUpgradeCountTexts()
    {
        if (bulletUpgradeCountText != null)
            bulletUpgradeCountText.text = bulletUpgradeCount + "x";
        if (suitUpgradeCountText != null)
            suitUpgradeCountText.text = suitUpgradeCount + "x";
        if (turretUpgradeCountText != null)
            turretUpgradeCountText.text = turretUpgradeCount + "x";
    }

    void UpdateUpgradeCostTexts()
    {
        if (bulletUpgradeCostText != null)
            bulletUpgradeCostText.text = "x" + bulletUpgradeGear;
        if (suitUpgradeCostText != null)
            suitUpgradeCostText.text = "x" + suitUpgradeBattery;
        if (turretUpgradeCostText != null)
            turretUpgradeCostText.text = "x" + turretUpgradeIron;
    }
} 
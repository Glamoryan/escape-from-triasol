using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI Bileşenleri")]
    public Image fillImage;
    public Image backgroundImage;
    public Canvas canvas;

    [Header("Renk Ayarları")]
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    [Header("Görünürlük Ayarları")]
    public float showDuration = 3f;
    public float fadeSpeed = 2f;
    public float yOffset = 1f;

    private Health health;
    private float currentShowTime;
    private bool isVisible = false;

    void Start()
    {
        health = GetComponentInParent<Health>();
        if (health == null)
        {
            Debug.LogError("Health bileşeni bulunamadı!");
            return;
        }

        // Canvas'ı dünya uzayında göster
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // UI ölçeğini ayarla

        // Başlangıçta gizli
        SetVisibility(false);

        // Health değişikliklerini dinle
        health.OnHealthChanged += OnHealthChanged;
        health.OnDeath += OnHealthDepleted;
    }

    void Update()
    {
        if (health == null) return;

        // Can barını güncelle
        float healthPercent = health.currentHealth / health.maxHealth;
        fillImage.fillAmount = healthPercent;

        // Rengi güncelle
        UpdateColor(healthPercent);

        // Pozisyonu güncelle
        UpdatePosition();

        // Görünürlük süresini kontrol et
        if (isVisible)
        {
            currentShowTime -= Time.deltaTime;
            if (currentShowTime <= 0)
            {
                SetVisibility(false);
            }
        }
    }

    void UpdateColor(float healthPercent)
    {
        if (healthPercent > 0.6f)
            fillImage.color = highHealthColor;
        else if (healthPercent > 0.3f)
            fillImage.color = mediumHealthColor;
        else
            fillImage.color = lowHealthColor;
    }

    void UpdatePosition()
    {
        // Canvas'ı karakterin üzerinde tut
        Vector3 targetPosition = transform.parent.position + Vector3.up * yOffset;
        canvas.transform.position = targetPosition;
    }

    void SetVisibility(bool visible)
    {
        isVisible = visible;
        canvas.enabled = visible;
        
        if (visible)
        {
            currentShowTime = showDuration;
        }
    }

    public void ShowHealthBar()
    {
        SetVisibility(true);
    }

    void OnHealthChanged(float currentHealth)
    {
        // Can değiştiğinde health bar'ı göster
        ShowHealthBar();
    }

    void OnHealthDepleted()
    {
        // Ölüm durumunda can barını gizle
        SetVisibility(false);
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthChanged -= OnHealthChanged;
            health.OnDeath -= OnHealthDepleted;
        }
    }
} 
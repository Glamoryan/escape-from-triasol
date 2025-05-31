using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [System.Serializable]
    public class GameOverMessage
    {
        public GameOverType type;
        [TextArea(3, 10)]
        public string message;
    }

    public enum GameOverType
    {
        PlayerDeath,
        SpaceshipRepaired,
        SolarFlare
    }

    [Header("Game Over Settings")]
    public List<GameOverMessage> gameOverMessages = new List<GameOverMessage>();
    public float solarFlareChanceBeforeDay5 = 0.1f; // 10%
    public float solarFlareChanceAfterDay5 = 0.5f; // 50%
    public int guaranteedSolarFlareDay = 7;

    [Header("UI Elements")]
    public Canvas gameOverCanvas;
    public TextMeshProUGUI gameOverText;
    public CanvasGroup canvasGroup;
    public float fadeInDuration = 1f;
    public float typingSpeed = 0.05f;
    public float delayBeforeRestart = 3f;

    [Header("Solar Flare Effect")]
    public GameObject solarFlareEffectPrefab;
    public Animator solarFlareAnimator;

    private bool isGameOver = false;

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

        if (gameOverCanvas != null)
            gameOverCanvas.gameObject.SetActive(false);
    }

    void Start()
    {
        // Event'leri dinle
        if (EnemySpawnManager.Instance != null)
        {
            EnemySpawnManager.OnDayChanged += CheckSolarFlare;
        }
    }

    void OnDestroy()
    {
        if (EnemySpawnManager.Instance != null)
        {
            EnemySpawnManager.OnDayChanged -= CheckSolarFlare;
        }
    }

    public void TriggerGameOver(GameOverType type)
    {
        if (isGameOver) return;
        isGameOver = true;

        if (type == GameOverType.SolarFlare)
        {
            StartCoroutine(PlaySolarFlareEffect());
        }
        else
        {
            ShowGameOver(type);
        }
    }

    private IEnumerator PlaySolarFlareEffect()
    {
        if (solarFlareEffectPrefab != null)
        {
            // Solar flare efektini oluştur ve GameOverCanvas'ın en üst child'ı yap
            GameObject flareEffect = Instantiate(solarFlareEffectPrefab, gameOverCanvas.transform);
            flareEffect.transform.SetAsFirstSibling();

            // Sadece RectTransform ayarları
            RectTransform rectTransform = flareEffect.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
            }

            // Animator'ı al ve animasyonu başlat
            Animator animator = flareEffect.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Play");
            }

            // Animasyonun tamamlanmasını bekle (animasyon süresi kadar)
            yield return new WaitForSeconds(1.5f);
        }

        // Game over ekranını göster
        ShowGameOver(GameOverType.SolarFlare);
    }

    private void ShowGameOver(GameOverType type)
    {
        if (gameOverCanvas != null)
        {
            // Game over canvas'ı en üstte göster
            Canvas canvas = gameOverCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 200; // Solar flare'den daha yüksek
            }

            gameOverCanvas.gameObject.SetActive(true);
            StartCoroutine(TypeGameOverMessage(type));
        }
    }

    private IEnumerator TypeGameOverMessage(GameOverType type)
    {
        string message = GetGameOverMessage(type);
        gameOverText.text = "";

        foreach (char c in message)
        {
            gameOverText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Oyunu durdur
        Time.timeScale = 0f;
    }

    private string GetGameOverMessage(GameOverType type)
    {
        var message = gameOverMessages.Find(m => m.type == type);
        return message != null ? message.message : null;
    }

    private void CheckSolarFlare(int currentDay)
    {
        if (isGameOver) return;

        if (currentDay >= guaranteedSolarFlareDay)
        {
            TriggerGameOver(GameOverType.SolarFlare);
            return;
        }

        float chance = currentDay >= 5 ? solarFlareChanceAfterDay5 : solarFlareChanceBeforeDay5;
        if (Random.value <= chance)
        {
            TriggerGameOver(GameOverType.SolarFlare);
        }
    }

    // Oyunu yeniden başlatmak için
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
} 
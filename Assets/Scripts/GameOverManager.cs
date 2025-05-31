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
    public float typingSpeed = 0.04f;

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

        string message = GetGameOverMessage(type);
        if (string.IsNullOrEmpty(message))
        {
            message = "Game Over!";
        }

        StartCoroutine(ShowGameOverMessage(message));
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

    IEnumerator ShowGameOverMessage(string message)
    {
        if (gameOverCanvas != null)
            gameOverCanvas.gameObject.SetActive(true);

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        // Type text
        if (gameOverText != null)
        {
            gameOverText.text = "";
            foreach (char c in message)
            {
                gameOverText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // Oyunu duraklat
        Time.timeScale = 0f;
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
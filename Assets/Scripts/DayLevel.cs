using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayLevel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Slider levelProgressSlider;
    [SerializeField] private Slider restProgressSlider;

    private void OnEnable()
    {
        EnemySpawnManager.OnDayChanged += UpdateDayText;
        EnemySpawnManager.OnLevelTimerChanged += UpdateLevelProgress;
        EnemySpawnManager.OnRestTimerChanged += UpdateRestProgress;
    }

    private void OnDisable()
    {
        EnemySpawnManager.OnDayChanged -= UpdateDayText;
        EnemySpawnManager.OnLevelTimerChanged -= UpdateLevelProgress;
        EnemySpawnManager.OnRestTimerChanged -= UpdateRestProgress;
    }

    private void UpdateDayText(int day)
    {
        if (dayText != null)
            dayText.text = $"Day {day}";
    }

    private void UpdateLevelProgress(float progress)
    {
        if (levelProgressSlider != null)
            levelProgressSlider.value = progress;
    }

    private void UpdateRestProgress(float progress)
    {
        if (restProgressSlider != null)
            restProgressSlider.value = progress;
    }
}

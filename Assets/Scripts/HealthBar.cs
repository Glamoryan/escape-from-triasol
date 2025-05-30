using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    private float maxHealth;

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        SetHealth(maxHealth);
    }

    public void SetHealth(float current)
    {
        float ratio = Mathf.Clamp01(current / maxHealth);
        fillImage.fillAmount = ratio;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}

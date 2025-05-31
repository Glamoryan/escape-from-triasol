using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SolarFlareEffect : MonoBehaviour
{
    public Image flareImage;
    public float startSize = 0.1f;
    public float endSize = 2f;
    public float duration = 1.5f;
    public Color startColor = new Color(1f, 0.5f, 0f, 0f); // Turuncu, başlangıçta şeffaf
    public Color endColor = new Color(1f, 0.5f, 0f, 0.8f); // Turuncu, yarı saydam

    private void Start()
    {
        if (flareImage == null)
        {
            Debug.LogError("Flare Image atanmamış!");
            return;
        }

        // Başlangıç ayarları
        flareImage.transform.localScale = Vector3.one * startSize;
        flareImage.color = startColor;
        flareImage.gameObject.SetActive(true);

        // Efekti başlat
        StartCoroutine(PlayFlareEffect());
    }

    private IEnumerator PlayFlareEffect()
    {
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.one * startSize;
        Vector3 targetScale = Vector3.one * endSize;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Scale'i güncelle
            flareImage.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            // Rengi güncelle
            flareImage.color = Color.Lerp(startColor, endColor, progress);

            yield return null;
        }

        // Efekti tamamla
        flareImage.transform.localScale = targetScale;
        flareImage.color = endColor;
    }
} 
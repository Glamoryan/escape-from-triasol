using System.Collections;
using TMPro;
using UnityEngine;

public class CinematicTextController : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.04f;
    public float messageDelay = 2f;
    public CanvasGroup canvasGroup;

    [TextArea(3, 10)]
    public string[] messages;

    private void Start()
    {
        StartCoroutine(PlayMessages());
    }

    IEnumerator PlayMessages()
    {
        foreach (var message in messages)
        {
            // Fade in
            yield return StartCoroutine(FadeCanvas(0f, 1f, 0.5f));

            // Type text
            yield return StartCoroutine(TypeText(message));

            // Bekle
            yield return new WaitForSeconds(messageDelay);

            // Fade out
            yield return StartCoroutine(FadeCanvas(1f, 0f, 0.5f));
        }
    }

    IEnumerator TypeText(string message)
    {
        textComponent.text = "";
        foreach (char c in message)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}

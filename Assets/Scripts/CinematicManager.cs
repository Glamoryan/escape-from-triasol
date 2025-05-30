using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CinematicManager : MonoBehaviour
{
    public GameObject spaceship;
    public RectTransform speechBubble;
    public TextMeshProUGUI speechText;
    public CanvasGroup fadeImage;
    public float shakeIntensity = 0.2f;
    public float shakeDuration = 1.5f;
    public float spaceshipSpeed = 5f;

    private Vector3 originalCameraPos;
    private bool isRunning = false;

    public void StartCinematic()
    {
        StartCoroutine(PlayCinematic());
    }

    IEnumerator PlayCinematic()
    {
        isRunning = true;
        yield return Fade(1, 0, 1f);

        originalCameraPos = Camera.main.transform.position;
        float timer = 0f;
        while (timer < shakeDuration)
        {
            Camera.main.transform.position = originalCameraPos + (Vector3)Random.insideUnitCircle * shakeIntensity;
            timer += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = originalCameraPos;

        spaceship.SetActive(true);
        Vector3 start = new Vector3(12, 0, 0);
        Vector3 end = new Vector3(2, 0, 0);
        spaceship.transform.position = start;

        while (Vector3.Distance(spaceship.transform.position, end) > 0.1f)
        {
            spaceship.transform.position = Vector3.MoveTowards(spaceship.transform.position, end, spaceshipSpeed * Time.deltaTime);
            yield return null;
        }

        speechBubble.gameObject.SetActive(true);
        speechText.text = "Sh*t I'm crashing!!";
        yield return new WaitForSeconds(2f);

        yield return Fade(0, 1, 1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            fadeImage.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.alpha = to;
    }
}

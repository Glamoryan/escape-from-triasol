using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance { get; private set; }

    public AudioClip backgroundMusic;
    public float volume = 0.5f;
    public bool playOnStart = true;
    public bool loop = true;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // AudioSource bileşenini oluştur
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
        audioSource.priority = 0; // En düşük öncelik
        audioSource.spatialBlend = 0f; // 2D ses
    }

    void Start()
    {
        if (playOnStart && backgroundMusic != null)
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void FadeOutAndStop(float duration)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0f, timer / duration);
            audioSource.volume = newVolume;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Orijinal ses seviyesini geri yükle
    }
} 
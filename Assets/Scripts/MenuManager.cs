using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main"); // Sahne ismini Unity'deki dosya adına göre yaz
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene"); // Sahne ismini doğru yaz
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatılıyor...");
    }
}

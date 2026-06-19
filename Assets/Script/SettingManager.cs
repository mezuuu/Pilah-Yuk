using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public GameObject panelSetting;
    public AudioSource musicSource;

    public void ToggleSetting()
    {
        bool isActive = panelSetting.activeSelf;

        panelSetting.SetActive(!isActive);
        Time.timeScale = isActive ? 1f : 0f;
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
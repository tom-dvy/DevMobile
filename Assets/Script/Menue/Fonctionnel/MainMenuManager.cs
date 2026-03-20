using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panneaux UI")]
    public GameObject mainPanel;
    public GameObject settingsPanel;

    [Header("Nom de la scène de jeu")]
    public string gameSceneName = "Track_Scene";

    private void Start()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);

        if (FontSizeManager.Instance != null)
        {
            FontSizeManager.Instance.RegisterAllTextsInScene();
        }
    }

    public void PlayGame() => SceneManager.LoadScene(gameSceneName);

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void OnFontSizeChanged(int index)
    {
        FontSizeManager.Instance.SetFontSizeByIndex(index);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
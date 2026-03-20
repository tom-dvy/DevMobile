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
    }


    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Fermeture du jeu...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        Debug.Log("Volume réglé sur : " + volume);
    }
}
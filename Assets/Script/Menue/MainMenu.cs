/*
using UnityEngine;
using UnityEngine.SceneManagement;

<<<<<<< Updated upstream
//Menu principal
public class MainMenu : MonoBehaviour
{

  [Header("Scène")]
  public string levelToLoad;
  public string skinScene;
  public GameObject SettingsWindow;


  //Boutton commencer le jeux
  public void StartGame()
  {
    SceneManager.LoadScene(levelToLoad);
  }

  public void Skin()
  {
    SceneManager.LoadScene(skinScene);
  }

  //Boutton ouvrire les paramètres
  public void SettingButton()
  {
    SettingsWindow.SetActive(true);
  }

  //Boutton quitter le jeux
  public void QuitGames()
  {
    Application.Quit();
  }

  //bouton fermer les paramètres
  public void CloseSettingsWindow()
  {
    SettingsWindow.SetActive(false);
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      SettingsWindow.SetActive(false);
    }
  }

  //bouton ouvrire les credits
  public void LoadCreditsScene()
  {
    SceneManager.LoadScene("Credits");
  }

  //bouton resets playerpref
  public void ResetPref()
  {
    PlayerPrefs.DeleteAll(); //supprime playerpref
    PlayerPrefs.Save(); // assure enregistrement sur disk
  }
}
=======
public class MainMenu : MonoBehaviour
//Menu principal
{

    [Header("Scène")]
    public string levelToLoad;
    public string skinScene;
    public GameObject SettingsWindow;


    //Boutton commencer le jeux
    public void StartGame()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void Skin()
    {
        SceneManager.LoadScene(skinScene);
    }

    //Boutton ouvrire les paramètres
    public void SettingButton()
    {
        SettingsWindow.SetActive(true);
    }

    //Boutton quitter le jeux
    public void QuitGames()
    {
        Application.Quit();
    }

    //bouton fermer les paramètres
    public void CloseSettingsWindow()
    {
        SettingsWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingsWindow.SetActive(false);
        }
    }

    //bouton ouvrire les credits
    public void LoadCreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }

    //bouton resets playerpref
    public void ResetPref()
    {
        PlayerPrefs.DeleteAll(); //supprime playerpref
        PlayerPrefs.Save(); // assure enregistrement sur disk
    }
}

>>>>>>> Stashed changes

*/

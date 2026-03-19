using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction choiceAction;
    public GameObject SettingsWindow;

    void Awake()
    {
        // Configuration pour le nouveau Input System
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
        }

        // Create skip action
        choiceAction = new InputAction("Skip", InputActionType.Button);
        choiceAction.AddBinding("<Touchscreen>/primaryTouch/press");
        choiceAction.AddBinding("<Mouse>/leftButton");
        choiceAction.Enable();
    }

    /// <summary>
    /// Change la scène avec le nom fourni en paramètre
    /// </summary>
    /// <param name="sceneName">Le nom de la scène à charger</param>
    public void ChangeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Le nom de la scène est vide !");
            return;
        }

        SceneManager.LoadScene(sceneName);
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

}

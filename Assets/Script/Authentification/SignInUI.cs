using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Interface de connexion Unity Player Accounts + demande de pseudo
/// Workflow : Sign In → Enter Username → Play
/// </summary>
public class SignInUI : MonoBehaviour
{
    [Header("Sign In Panel (pas connecté)")]
    [SerializeField] private GameObject signInPanel;
    [SerializeField] private Button signInButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject loadingIndicator;

    [Header("Username Panel (connecté mais pas de pseudo)")]
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button confirmUsernameButton;
    [SerializeField] private TextMeshProUGUI usernameFeedbackText;

    [Header("Game Panel (tout est OK)")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private Button signOutButton;
    
    private Coroutine checkAuthCoroutine;

    private void Start()
    {
        // Lier les boutons
        if (signInButton != null)
        {
            signInButton.onClick.AddListener(OnSignInButtonClicked);
        }

        if (confirmUsernameButton != null)
        {
            confirmUsernameButton.onClick.AddListener(OnConfirmUsernameClicked);
        }

        if (signOutButton != null)
        {
            signOutButton.onClick.AddListener(OnSignOutButtonClicked);
        }

        // Verifier l'etat initial avec un delai pour laisser AuthenticationManager s'initialiser
        Invoke(nameof(UpdateUI), 0.5f);
        
        // Demarrer la coroutine de verification (seulement sur SignInPanel)
        checkAuthCoroutine = StartCoroutine(CheckAuthenticationStatus());
    }
    
    private IEnumerator CheckAuthenticationStatus()
    {
        while (true)
        {
            // Verifier seulement si on est sur le SignInPanel
            if (signInPanel != null && signInPanel.activeSelf)
            {
                // Verifier toutes les 2 secondes si la connexion a reussi
                UpdateUI();
            }
            
            yield return new WaitForSeconds(2f);
        }
    }

    private async void UpdateUI()
    {
        bool isAuthenticated = AuthenticationManager.Instance != null && 
                               AuthenticationManager.Instance.IsAuthenticated;

        if (!isAuthenticated)
        {
            // Pas connecté → afficher SignInPanel
            ShowPanel(signInPanel);
        }
        else
        {
            // Connecté → vérifier si pseudo défini
            string playerName = await AuthenticationManager.Instance.GetPlayerNameAsync();

            if (playerName == "Player" || string.IsNullOrEmpty(playerName))
            {
                // Connecté mais pas de pseudo → afficher UsernamePanel
                ShowPanel(usernamePanel);
            }
            else
            {
                // Connecté ET pseudo → afficher GamePanel
                ShowPanel(gamePanel);
                UpdateWelcomeText(playerName);
            }
        }
    }

    private void ShowPanel(GameObject panelToShow)
    {
        // Cacher tous les panels
        if (signInPanel != null) signInPanel.SetActive(false);
        if (usernamePanel != null) usernamePanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);

        // Afficher le bon panel
        if (panelToShow != null) panelToShow.SetActive(true);
    }

    // ============================================================================
    // SIGN IN
    // ============================================================================

    private void OnSignInButtonClicked()
    {
        Debug.Log("[SIGN IN UI] Sign In button clicked");
        
        // Afficher le loading
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(true);
        }

        if (statusText != null)
        {
            statusText.text = "Opening browser...";
        }

        // Desactiver le bouton
        if (signInButton != null)
        {
            signInButton.interactable = false;
        }

        // Demarrer la connexion
        AuthenticationManager.Instance.StartSignIn();

        // Reactiver apres 3 secondes
        Invoke(nameof(ReenableSignInButton), 3f);
    }

    private void ReenableSignInButton()
    {
        if (signInButton != null)
        {
            signInButton.interactable = true;
        }

        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(false);
        }

        if (statusText != null)
        {
            statusText.text = "";
        }
        
        // Mettre a jour l'UI au cas ou la connexion a reussi
        UpdateUI();
    }

    // ============================================================================
    // USERNAME
    // ============================================================================

    private async void OnConfirmUsernameClicked()
    {
        string username = usernameInputField.text.Trim();

        // Validation
        if (string.IsNullOrEmpty(username))
        {
            ShowUsernameFeedback("Username cannot be empty!", Color.red);
            return;
        }

        if (username.Length < 3)
        {
            ShowUsernameFeedback("Minimum 3 characters", Color.red);
            return;
        }

        if (username.Length > 20)
        {
            ShowUsernameFeedback("Maximum 20 characters", Color.red);
            return;
        }

        // Sauvegarder
        try
        {
            await AuthenticationManager.Instance.SetPlayerNameAsync(username);
            
            ShowUsernameFeedback("Username saved!", Color.green);
            
            // Attendre un peu puis passer au jeu
            await System.Threading.Tasks.Task.Delay(1000);
            UpdateUI();
        }
        catch (System.Exception e)
        {
            ShowUsernameFeedback($"Error: {e.Message}", Color.red);
        }
    }

    private void ShowUsernameFeedback(string message, Color color)
    {
        if (usernameFeedbackText != null)
        {
            usernameFeedbackText.text = message;
            usernameFeedbackText.color = color;
        }
    }

    // ============================================================================
    // GAME PANEL
    // ============================================================================

    private void UpdateWelcomeText(string playerName)
    {
        if (welcomeText != null)
        {
            welcomeText.text = $"Welcome, {playerName}!";
        }
    }

    private void OnSignOutButtonClicked()
    {
        Debug.Log("[SIGN IN UI] Sign Out button clicked");
        
        AuthenticationManager.Instance.SignOut(true);
        
        // NE PAS supprimer PlayerPrefs - ils sont lies au PlayerID
        // Si le joueur se reconnecte avec le meme compte, il retrouvera son pseudo
        
        UpdateUI();
    }

    private void OnDestroy()
    {
        // Arreter la coroutine
        if (checkAuthCoroutine != null)
        {
            StopCoroutine(checkAuthCoroutine);
        }
        
        if (signInButton != null)
        {
            signInButton.onClick.RemoveAllListeners();
        }

        if (confirmUsernameButton != null)
        {
            confirmUsernameButton.onClick.RemoveAllListeners();
        }

        if (signOutButton != null)
        {
            signOutButton.onClick.RemoveAllListeners();
        }
    }
}
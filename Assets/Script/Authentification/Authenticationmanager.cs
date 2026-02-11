using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System;

/// <summary>
/// Gestionnaire d'authentification Unity Services
/// Permet aux joueurs de se connecter et de sauvegarder leur pseudo
/// Requis pour les leaderboards et autres services sociaux
/// </summary>
public class AuthenticationManager : MonoBehaviour
{
    private static AuthenticationManager instance;
    private bool isAuthenticated = false;
    private string playerName = "";

    public static AuthenticationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AuthenticationManager>();
                if (instance == null)
                {
                    UnityEngine.GameObject obj = new UnityEngine.GameObject("AuthenticationManager");
                    instance = obj.AddComponent<AuthenticationManager>();
                }
            }
            return instance;
        }
    }

    // Proprietes publiques pour acceder aux donnees
    public bool IsAuthenticated => isAuthenticated;
    public string PlayerID => AuthenticationService.Instance.PlayerId;
    public string PlayerName => playerName;

    private async void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeAuthenticationAsync();
    }

    // ============================================================================
    // INITIALISATION
    // ============================================================================

    private async Task InitializeAuthenticationAsync()
    {
        try
        {
            Debug.Log("[AUTH] Initializing Unity Services...");

            // Initialiser Unity Services (requis avant toute authentification)
            await UnityServices.InitializeAsync();

            Debug.Log("[AUTH] Unity Services initialized");

            // S'inscrire aux evenements d'authentification
            SetupAuthenticationEvents();

            // Tenter une connexion anonyme automatique
            await SignInAnonymouslyAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AUTH] Initialization failed: {e.Message}");
        }
    }

    private void SetupAuthenticationEvents()
    {
        // Event quand la connexion reussit
        AuthenticationService.Instance.SignedIn += OnSignedIn;

        // Event quand la deconnexion se produit
        AuthenticationService.Instance.SignedOut += OnSignedOut;

        // Event en cas d'expiration de session
        AuthenticationService.Instance.Expired += OnExpired;
    }

    // ============================================================================
    // CONNEXION ANONYME (par defaut)
    // ============================================================================

    /// <summary>
    /// Connexion anonyme automatique
    /// Cree un compte temporaire sans email/mot de passe
    /// Ideal pour commencer sans forcer l'inscription
    /// </summary>
    public async Task SignInAnonymouslyAsync()
    {
        try
        {
            Debug.Log("[AUTH] Signing in anonymously...");

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log("[AUTH] Anonymous sign-in successful");
            Debug.Log($"[AUTH] Player ID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"[AUTH] Anonymous sign-in failed: {e.Message}");
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"[AUTH] Network error: {e.Message}");
        }
    }

    // ============================================================================
    // GESTION DU PSEUDO
    // ============================================================================

    /// <summary>
    /// Definir le pseudo du joueur
    /// Stocke localement ET dans Unity Cloud (pour leaderboard)
    /// </summary>
    public async Task SetPlayerNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[AUTH] Player name cannot be empty");
            return;
        }

        try
        {
            Debug.Log($"[AUTH] Setting player name to: {name}");

            // Mettre a jour le nom dans Unity Authentication
            await AuthenticationService.Instance.UpdatePlayerNameAsync(name);

            // Stocker localement
            playerName = name;
            PlayerPrefs.SetString("PlayerName", name);
            PlayerPrefs.Save();

            Debug.Log("[AUTH] Player name updated successfully");
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"[AUTH] Failed to update player name: {e.Message}");
        }
    }

    /// <summary>
    /// Recuperer le pseudo du joueur
    /// D'abord depuis le cache local, sinon depuis Unity Cloud
    /// </summary>
    public async Task<string> GetPlayerNameAsync()
    {
        // Si deja en cache
        if (!string.IsNullOrEmpty(playerName))
        {
            return playerName;
        }

        // Sinon, verifier PlayerPrefs
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            return playerName;
        }

        // En dernier recours, recuperer depuis Unity Cloud
        try
        {
            var playerInfo = await AuthenticationService.Instance.GetPlayerInfoAsync();
            playerName = playerInfo.Username ?? "Player";
            return playerName;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[AUTH] Could not fetch player name: {e.Message}");
            return "Player"; // Nom par defaut
        }
    }

    // ============================================================================
    // DECONNEXION
    // ============================================================================

    /// <summary>
    /// Deconnecter le joueur
    /// Efface la session mais garde le PlayerID en cache
    /// </summary>
    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        Debug.Log("[AUTH] Player signed out");
    }

    // ============================================================================
    // EVENTS CALLBACKS
    // ============================================================================

    private void OnSignedIn()
    {
        isAuthenticated = true;
        Debug.Log($"[AUTH] Player signed in - ID: {AuthenticationService.Instance.PlayerId}");

        // Recuperer le pseudo automatiquement
        _ = GetPlayerNameAsync();
    }

    private void OnSignedOut()
    {
        isAuthenticated = false;
        Debug.Log("[AUTH] Player signed out");
    }

    private void OnExpired()
    {
        isAuthenticated = false;
        Debug.LogWarning("[AUTH] Session expired - reconnecting...");

        // Reconnexion automatique
        _ = SignInAnonymouslyAsync();
    }

    // ============================================================================
    // NETTOYAGE
    // ============================================================================

    private void OnDestroy()
    {
        if (AuthenticationService.Instance != null)
        {
            AuthenticationService.Instance.SignedIn -= OnSignedIn;
            AuthenticationService.Instance.SignedOut -= OnSignedOut;
            AuthenticationService.Instance.Expired -= OnExpired;
        }
    }
}
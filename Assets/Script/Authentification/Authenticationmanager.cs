using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using System.Threading.Tasks;
using System;

/// <summary>
/// Gestionnaire d'authentification Unity Player Accounts
/// Connexion via navigateur avec compte Unity persistant
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
                instance = FindFirstObjectByType<AuthenticationManager>();
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

            // S'inscrire aux evenements Unity Player Accounts
            SetupPlayerAccountsEvents();

            // IMPORTANT : Verifier si deja connecte (session precedente)
            await CheckExistingSession();

            Debug.Log("[AUTH] Authentication ready.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[AUTH] Initialization failed: {e.Message}");
        }
    }

    /// <summary>
    /// Verifie si une session existe deja (Player Accounts + Authentication)
    /// Permet de garder le joueur connecte entre les lancements
    /// </summary>
    private async Task CheckExistingSession()
    {
        try
        {
            // Verifier si Player Accounts a une session active
            if (PlayerAccountService.Instance.IsSignedIn)
            {
                Debug.Log("[AUTH] Existing Player Accounts session found!");

                // Se reconnecter automatiquement a Unity Authentication
                await SignInWithUnityAuth();
            }
            else
            {
                Debug.Log("[AUTH] No existing session - player needs to sign in");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[AUTH] Could not restore session: {e.Message}");
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

    private void SetupPlayerAccountsEvents()
    {
        // Event quand la connexion Player Accounts reussit
        PlayerAccountService.Instance.SignedIn += OnPlayerAccountsSignedIn;
    }

    // ============================================================================
    // CONNEXION UNITY PLAYER ACCOUNTS
    // ============================================================================

    /// <summary>
    /// Demarrer le processus de connexion Unity Player Accounts
    /// Ouvre le navigateur pour que le joueur se connecte
    /// APPELER CETTE METHODE depuis un bouton UI
    /// </summary>
    public async void StartSignIn()
    {
        // Si deja connecte aux Player Accounts, se connecter directement a Authentication
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            Debug.Log("[AUTH] Already signed in to Player Accounts, signing in to Authentication...");
            await SignInWithUnityAuth();
            return;
        }

        try
        {
            Debug.Log("[AUTH] Starting Unity Player Accounts sign-in...");

            // Ceci ouvre le navigateur systeme pour la connexion
            await PlayerAccountService.Instance.StartSignInAsync();

            // Le reste se passe dans OnPlayerAccountsSignedIn (event callback)
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"[AUTH] Player Accounts sign-in failed: {ex.Message}");
            Debug.LogError($"[AUTH] Error code: {ex.ErrorCode}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AUTH] Unexpected error during sign-in: {ex.Message}");
        }
    }

    /// <summary>
    /// Callback appele quand le joueur s'est connecte aux Player Accounts
    /// Se connecte ensuite automatiquement a Unity Authentication
    /// </summary>
    private async void OnPlayerAccountsSignedIn()
    {
        Debug.Log("[AUTH] Player Accounts sign-in successful!");
        await SignInWithUnityAuth();
    }

    /// <summary>
    /// Se connecter a Unity Authentication avec le token Player Accounts
    /// </summary>
    private async Task SignInWithUnityAuth()
    {
        try
        {
            Debug.Log("[AUTH] Signing in to Unity Authentication...");

            // Recuperer le token d'acces Player Accounts
            string accessToken = PlayerAccountService.Instance.AccessToken;

            // Se connecter a Unity Authentication avec ce token
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            Debug.Log("[AUTH] Unity Authentication sign-in successful!");
            Debug.Log($"[AUTH] Player ID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"[AUTH] Unity Authentication sign-in failed: {ex.Message}");
            Debug.LogError($"[AUTH] Error code: {ex.ErrorCode}");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"[AUTH] Network error: {ex.Message}");
        }
    }

    // ============================================================================
    // GESTION DU PSEUDO
    // ============================================================================

    /// <summary>
    /// Definir le pseudo du joueur
    /// Stocke localement ET dans Unity Cloud
    /// IMPORTANT : Lie le pseudo au PlayerID
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

            // Stocker localement AVEC le PlayerID comme cle
            string playerID = AuthenticationService.Instance.PlayerId;
            playerName = name;
            PlayerPrefs.SetString($"PlayerName_{playerID}", name);
            PlayerPrefs.Save();

            Debug.Log($"[AUTH] Player name updated successfully for PlayerID: {playerID}");
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"[AUTH] Failed to update player name: {e.Message}");
        }
    }

    /// <summary>
    /// Recuperer le pseudo du joueur
    /// Cherche d'abord localement avec le PlayerID, puis dans Unity Cloud
    /// </summary>
    public async Task<string> GetPlayerNameAsync()
    {
        // Si deja en cache
        if (!string.IsNullOrEmpty(playerName))
        {
            return playerName;
        }

        // Recuperer le PlayerID actuel
        string playerID = AuthenticationService.Instance.PlayerId;

        if (string.IsNullOrEmpty(playerID))
        {
            Debug.LogWarning("[AUTH] No PlayerID available yet");
            return "Player";
        }

        // Verifier PlayerPrefs avec le PlayerID comme cle
        string key = $"PlayerName_{playerID}";
        if (PlayerPrefs.HasKey(key))
        {
            playerName = PlayerPrefs.GetString(key);
            Debug.Log($"[AUTH] Player name loaded from cache for PlayerID {playerID}: {playerName}");
            return playerName;
        }

        // En dernier recours, recuperer depuis Unity Cloud
        try
        {
            var playerInfo = await AuthenticationService.Instance.GetPlayerInfoAsync();
            playerName = playerInfo.Username ?? "Player";

            // Sauvegarder dans le cache pour la prochaine fois
            if (playerName != "Player" && !string.IsNullOrEmpty(playerName))
            {
                PlayerPrefs.SetString(key, playerName);
                PlayerPrefs.Save();
            }

            Debug.Log($"[AUTH] Player name fetched from cloud for PlayerID {playerID}: {playerName}");
            return playerName;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[AUTH] Could not fetch player name: {e.Message}");
            return "Player";
        }
    }

    // ============================================================================
    // DECONNEXION
    // ============================================================================

    /// <summary>
    /// Deconnecter le joueur de Player Accounts ET Authentication
    /// </summary>
    public void SignOut(bool clearSessionToken = false)
    {
        // Recuperer le PlayerID avant de deconnecter
        string playerID = AuthenticationService.Instance.PlayerId;

        // Deconnexion de Unity Authentication
        AuthenticationService.Instance.SignOut(clearSessionToken);

        // Deconnexion de Unity Player Accounts
        PlayerAccountService.Instance.SignOut();

        // Vider le cache du pseudo
        playerName = "";

        Debug.Log($"[AUTH] Player signed out (PlayerID was: {playerID})");
        Debug.Log("[AUTH] Note: PlayerPrefs kept for future sign-in with same account");
    }

    // ============================================================================
    // EVENTS CALLBACKS
    // ============================================================================

    private void OnSignedIn()
    {
        isAuthenticated = true;

        // Vider le cache du pseudo pour forcer le rechargement
        playerName = "";

        Debug.Log($"[AUTH] Unity Authentication signed in - ID: {AuthenticationService.Instance.PlayerId}");

        // Recuperer le pseudo automatiquement
        _ = GetPlayerNameAsync();
    }

    private void OnSignedOut()
    {
        isAuthenticated = false;
        Debug.Log("[AUTH] Unity Authentication signed out");
    }

    private async void OnExpired()
    {
        isAuthenticated = false;
        Debug.LogWarning("[AUTH] Session expired - please sign in again");

        // Redemander la connexion
        StartSignIn();
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

        if (PlayerAccountService.Instance != null)
        {
            PlayerAccountService.Instance.SignedIn -= OnPlayerAccountsSignedIn;
        }
    }
}
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine.UnityConsent;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

/// <summary>
/// Gestionnaire Analytics pour Unity Services Analytics
/// Compatible Unity 6.2+ avec Analytics SDK 6.1+
/// </summary>
public class AnalyticsManager : MonoBehaviour
{
    private static AnalyticsManager instance;
    private bool isInitialized = false;

    // Propriete publique pour verifier l'etat
    public bool IsInitialized => isInitialized;

    public static AnalyticsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AnalyticsManager>();
                if (instance == null)
                {
                    UnityEngine.GameObject obj = new UnityEngine.GameObject("AnalyticsManager");
                    instance = obj.AddComponent<AnalyticsManager>();
                }
            }
            return instance;
        }
    }

    private async void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeAnalyticsAsync();
    }

    private async Task InitializeAnalyticsAsync()
    {
        try
        {
            Debug.Log("[ANALYTICS] Initializing Unity Services...");

            // Etape 1 : Initialiser Unity Services (requis)
            await UnityServices.InitializeAsync();

            Debug.Log("[ANALYTICS] Unity Services initialized");

            // Etape 2 : Definir le consentement utilisateur
            EndUserConsent.SetConsentState(new ConsentState
            {
                AnalyticsIntent = ConsentStatus.Granted,
                AdsIntent = ConsentStatus.Denied
            });

            Debug.Log("[ANALYTICS] Consent granted for Analytics");

            isInitialized = true;
            Debug.Log("[ANALYTICS] Analytics initialized successfully");

            // Envoyer un event de demarrage
            TrackEvent("app_started");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ANALYTICS] Failed to initialize: {e.Message}");
            isInitialized = false;
        }
    }

    // ============================================================================
    // METHODE SIMPLE - UTILISE CELLE-CI
    // ============================================================================

    /// <summary>
    /// Methode ultra simple pour tracker un event
    /// Utilisation : AnalyticsManager.Instance.TrackEvent("mon_event");
    /// </summary>
    public void TrackEvent(string eventName)
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"[ANALYTICS] Cannot track '{eventName}' - Analytics not initialized yet");
            return;
        }

        try
        {
            AnalyticsService.Instance.CustomData(eventName, new Dictionary<string, object>
            {
                { "timestamp", DateTime.UtcNow.ToString("O") }
            });

            Debug.Log($"[ANALYTICS] Event tracked: {eventName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ANALYTICS] Failed to track event '{eventName}': {e.Message}");
        }
    }

    /// <summary>
    /// Methode avec parametres personnalises
    /// </summary>
    public void TrackEventWithData(string eventName, Dictionary<string, object> customData)
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"[ANALYTICS] Cannot track '{eventName}' - Analytics not initialized yet");
            return;
        }

        try
        {
            // Ajouter timestamp automatiquement
            customData["timestamp"] = DateTime.UtcNow.ToString("O");

            AnalyticsService.Instance.CustomData(eventName, customData);

            Debug.Log($"[ANALYTICS] Event tracked: {eventName} with {customData.Count} parameters");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ANALYTICS] Failed to track event '{eventName}': {e.Message}");
        }
    }

    // ============================================================================
    // METHODES SPECIFIQUES POUR DIAGNOSTICS
    // ============================================================================

    public void TrackCrashAttempt(string crashType)
    {
        var data = new Dictionary<string, object>
        {
            { "crash_type", crashType }
        };
        TrackEventWithData("crash_attempt", data);
    }

    public void TrackException(string exceptionType, string message)
    {
        var data = new Dictionary<string, object>
        {
            { "exception_type", exceptionType },
            { "message", message }
        };
        TrackEventWithData("exception_logged", data);
    }

    public void TrackDebugMenuToggle(bool isVisible)
    {
        var data = new Dictionary<string, object>
        {
            { "menu_state", isVisible ? "opened" : "closed" }
        };
        TrackEventWithData("debug_menu_toggled", data);
    }

    // ============================================================================
    // METHODES DE FLUSH (pour forcer l'envoi)
    // ============================================================================

    public void Flush()
    {
        if (isInitialized)
        {
            AnalyticsService.Instance.Flush();
            Debug.Log("[ANALYTICS] Events flushed to server");
        }
    }

    private void OnApplicationQuit()
    {
        Flush();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            Flush();
        }
    }
}
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine.UnityConsent;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class AnalyticsManager : MonoBehaviour
{
    private static AnalyticsManager instance;
    private bool isInitialized = false;

    public bool IsInitialized => isInitialized;

    public static AnalyticsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AnalyticsManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("AnalyticsManager");
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

            await UnityServices.InitializeAsync();

            // --- CORRECTION ---
            // On supprime StartDataCollection()
            // On utilise EndUserConsent comme demandé par le warning

            // Note : Assure-toi d'ajouter 'using UnityEngine.UnityConsent;' tout en haut du fichier
            EndUserConsent.SetConsentState(new ConsentState
            {
                AnalyticsIntent = ConsentStatus.Granted,
                AdsIntent = ConsentStatus.Denied
            });

            isInitialized = true;
            Debug.Log("[ANALYTICS] Initialized successfully");

            TrackEvent("app_started");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ANALYTICS] Failed to initialize: {e.Message}");
            isInitialized = false;
        }
    }

    public void TrackEvent(string eventName)
    {
        if (!isInitialized) return;

        try
        {
            // CORRECTION ERREUR: Utilisation de RecordEvent
            var myEvent = new CustomEvent(eventName);
            myEvent.Add("timestamp", DateTime.UtcNow.ToString("O"));

            AnalyticsService.Instance.RecordEvent(myEvent);
            Debug.Log($"[ANALYTICS] Event tracked: {eventName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ANALYTICS] Error: {e.Message}");
        }
    }

    public void TrackEventWithData(string eventName, Dictionary<string, object> customData)
    {
        if (!isInitialized) return;

        try
        {
            // CORRECTION: Conversion du Dictionary vers CustomEvent
            var myEvent = new CustomEvent(eventName);

            // Ajout du timestamp
            myEvent.Add("timestamp", DateTime.UtcNow.ToString("O"));

            foreach (var item in customData)
            {
                // Unity Analytics est strict sur les types. 
                // On doit vérifier le type avant d'ajouter.
                if (item.Value is string s) myEvent.Add(item.Key, s);
                else if (item.Value is int i) myEvent.Add(item.Key, i);
                else if (item.Value is float f) myEvent.Add(item.Key, f);
                else if (item.Value is bool b) myEvent.Add(item.Key, b);
                else if (item.Value is double d) myEvent.Add(item.Key, d);
                else myEvent.Add(item.Key, item.Value.ToString()); // Fallback
            }

            AnalyticsService.Instance.RecordEvent(myEvent);
            Debug.Log($"[ANALYTICS] Event tracked: {eventName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ANALYTICS] Error: {e.Message}");
        }
    }

    // ... Le reste de tes méthodes de diagnostic (TrackCrashAttempt, etc.) reste inchangé ...
    // Assure-toi juste qu'elles appellent bien TrackEventWithData corrigée ci-dessus.

    public void TrackCrashAttempt(string crashType)
    {
        var data = new Dictionary<string, object> { { "crash_type", crashType } };
        TrackEventWithData("crash_attempt", data);
    }

    public void TrackException(string exceptionType, string message)
    {
        var data = new Dictionary<string, object> { { "exception_type", exceptionType }, { "message", message } };
        TrackEventWithData("exception_logged", data);
    }

    public void TrackDebugMenuToggle(bool isVisible)
    {
        var data = new Dictionary<string, object> { { "menu_state", isVisible ? "opened" : "closed" } };
        TrackEventWithData("debug_menu_toggled", data);
    }

    public void Flush()
    {
        if (isInitialized)
        {
            AnalyticsService.Instance.Flush();
        }
    }

    // ... OnApplicationQuit, etc ...
}
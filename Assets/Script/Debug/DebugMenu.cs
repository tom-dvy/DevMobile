using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Diagnostics; // Nécessaire pour Utils.ForceCrash

public class DebugMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private Button crashButton;
    [SerializeField] private Button exceptionButton;
    [SerializeField] private Button analyticsTestButton;
    [SerializeField] private Button closeButton; // Bouton "X" pour fermer

    private bool isVisible = false;
    private InputAction toggleAction; // Pour gérer la touche F12

    private void Awake()
    {
        // Configuration de la touche F12 via le nouveau Input System
        toggleAction = new InputAction(name: "ToggleDebugMenu", binding: "<Keyboard>/f12");
        toggleAction.performed += context => ToggleMenu();
        toggleAction.Enable();

        // On cache le menu au démarrage
        if (debugPanel != null) debugPanel.SetActive(false);

        SetupButtons();
    }

    private void SetupButtons()
    {
        // On attache les fonctions aux clics des boutons
        if (crashButton != null) crashButton.onClick.AddListener(TestCrash);
        if (exceptionButton != null) exceptionButton.onClick.AddListener(TestException);
        if (analyticsTestButton != null) analyticsTestButton.onClick.AddListener(TestAnalytics);

        // Le bouton fermer fait la même chose que F12
        if (closeButton != null) closeButton.onClick.AddListener(ToggleMenu);
    }

    public void ToggleMenu()
    {
        isVisible = !isVisible;
        if (debugPanel != null) debugPanel.SetActive(isVisible);

        // On envoie l'info aux analytics pour savoir si les joueurs utilisent le menu debug
        AnalyticsManager.Instance.TrackDebugMenuToggle(isVisible);
    }

    public void TestCrash()
    {
        // 1. On prévient Analytics qu'on va crasher (pour avoir une trace avant la mort de l'app)
        AnalyticsManager.Instance.TrackCrashAttempt("MonoAbort");

        // 2. On force le crash violent (l'app se ferme instantanément)
        Utils.ForceCrash(ForcedCrashCategory.MonoAbort);
    }

    public void TestException()
    {
        // Crée une fausse erreur
        var ex = new System.Exception("Test exception from DebugMenu");

        // L'affiche dans la console Unity (en rouge)
        Debug.LogException(ex);

        // L'envoie aux analytics
        AnalyticsManager.Instance.TrackException("TestException", ex.Message);
    }

    public void TestAnalytics()
    {
        // Test simple pour voir si les events arrivent sur le Dashboard Unity
        AnalyticsManager.Instance.TrackEvent("analytics_test_button_clicked");
    }

    // Nettoyage propre quand l'objet est détruit (changement de scène ou fermeture)
    private void OnDestroy()
    {
        if (toggleAction != null)
        {
            toggleAction.Disable();
            toggleAction.Dispose();
        }

        // Bonne pratique : enlever les listeners pour éviter les fuites de mémoire
        if (crashButton != null) crashButton.onClick.RemoveAllListeners();
        // ... (etc pour les autres boutons)
    }
}
using UnityEngine;
using TMPro;

public class RaceEndScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private GameObject mobileControlsHUD; // <--- AJOUTE CECI
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI bestLapText;
    
    private RaceTimer raceTimer;
    private CarController carController;

    void Awake()
    {
        raceTimer = FindFirstObjectByType<RaceTimer>();
        carController = FindFirstObjectByType<CarController>();
    }

    void OnEnable()
    {
        if (raceTimer != null)
            raceTimer.onRaceComplete.AddListener(ShowEndScreen);
    }

    void OnDisable()
    {
        if (raceTimer != null)
            raceTimer.onRaceComplete.RemoveListener(ShowEndScreen);
    }

    private void ShowEndScreen(float totalTime, int laps)
    {
        // 1. On affiche l'écran de fin
        if (endScreenPanel != null) endScreenPanel.SetActive(true);
        
        // 2. ON CACHE LES CONTRÔLES MOBILES <--- LOGIQUE ICI
        if (mobileControlsHUD != null) mobileControlsHUD.SetActive(false);

        // 3. On remplit les textes
        if (finalTimeText != null)
            finalTimeText.text = $"Total Time: {RaceTimer.FormatTime(totalTime)}";
            
        if (bestLapText != null)
            bestLapText.text = $"Best Lap: {RaceTimer.FormatTime(raceTimer.GetBestLapTime())}";

        // 4. On coupe la physique de la voiture
        if (carController != null) carController.enabled = false;
    }

    public void RestartRace() 
    {
        // Pas besoin de réactiver le HUD ici, car le chargement de scène 
        // remet tout à l'état initial de ton Prefab/Scène.
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
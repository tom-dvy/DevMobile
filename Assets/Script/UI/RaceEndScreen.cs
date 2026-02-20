using UnityEngine;
using TMPro;

/// <summary>
/// Displays the end-of-race screen when the race is completed.
/// Shows final time, best lap time, disables player controls,
/// and hides mobile HUD elements
/// </summary>
public class RaceEndScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endScreenPanel;      // End screen UI panel
    [SerializeField] private GameObject mobileControlsHUD;   // Mobile controls to hide at race end
    [SerializeField] private TextMeshProUGUI finalTimeText;  // Displays total race time
    [SerializeField] private TextMeshProUGUI bestLapText;    // Displays best lap time

    private RaceTimer raceTimer;
    private CarController carController;
    public Autosave save;
    private void Awake()
    {
        // Find required gameplay components in the scene
        raceTimer = FindFirstObjectByType<RaceTimer>();
        carController = FindFirstObjectByType<CarController>();
    }

    private void OnEnable()
    {
        // Subscribe to race completion event
        if (raceTimer != null)
        {
            raceTimer.onRaceComplete.AddListener(ShowEndScreen);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        if (raceTimer != null)
        {
            raceTimer.onRaceComplete.RemoveListener(ShowEndScreen);
        }
    }

    private void ShowEndScreen(float totalTime, int laps)
    {
        save.sendlist();
        // Show end screen UI
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }

        // Hide mobile controls HUD
        if (mobileControlsHUD != null)
        {
            mobileControlsHUD.SetActive(false);
        }

        // Display final race time
        if (finalTimeText != null)
        {
            finalTimeText.text =
                $"Total Time: {RaceTimer.FormatTime(totalTime)}";
        }

        // Display best lap time
        if (bestLapText != null && raceTimer != null)
        {
            bestLapText.text =
                $"Best Lap: {RaceTimer.FormatTime(raceTimer.GetBestLapTime())}";
        }

        // Disable car controls at race end
        if (carController != null)
        {
            carController.enabled = false;
        }
    }

    // Reloads the current scene to restart the race
    public void RestartRace()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}

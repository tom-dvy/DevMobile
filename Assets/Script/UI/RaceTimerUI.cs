using UnityEngine;
using TMPro;

/// <summary>
/// Displays race-related timing information in the UI
/// Shows total time, current lap time, best lap time, and lap count
/// </summary>
public class RaceTimerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI totalTimeText;   // Displays total race time
    [SerializeField] private TextMeshProUGUI lapTimeText;     // Displays current lap time
    [SerializeField] private TextMeshProUGUI bestLapText;     // Displays best lap time
    [SerializeField] private TextMeshProUGUI lapCounterText;  // Displays current lap / total laps

    private RaceTimer raceTimer;
    private FinishLine finishLine;

    private void Start()
    {
        // Automatically find required gameplay components in the scene
        raceTimer = FindFirstObjectByType<RaceTimer>();
        finishLine = FindFirstObjectByType<FinishLine>();

        if (raceTimer == null)
        {
            Debug.LogError("RaceTimer not found in scene!");
        }
    }

    private void Update()
    {
        if (raceTimer == null) return;

        // Total race time
        if (totalTimeText != null)
        {
            totalTimeText.text =
                $"Time: {RaceTimer.FormatTime(raceTimer.GetCurrentTime())}";
        }

        // Current lap time
        if (lapTimeText != null)
        {
            lapTimeText.text =
                $"Lap: {RaceTimer.FormatTime(raceTimer.GetLapTime())}";
        }

        // Best lap time (fallback if no lap recorded yet)
        if (bestLapText != null)
        {
            float bestLap = raceTimer.GetBestLapTime();

            bestLapText.text = bestLap > 0f
                ? $"Best: {RaceTimer.FormatTime(bestLap)}"
                : "Best: --:--.---";
        }

        // Lap counter display
        if (lapCounterText != null && finishLine != null)
        {
            lapCounterText.text =
                $"Lap: {finishLine.GetCurrentLap()}/{finishLine.GetRequiredLaps()}";
        }
    }
}
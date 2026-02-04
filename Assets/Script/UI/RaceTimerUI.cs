using UnityEngine;
using TMPro;

/// <summary>
/// Displays race timer in UI - requires TextMeshPro
/// </summary>
public class RaceTimerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI totalTimeText;
    [SerializeField] private TextMeshProUGUI lapTimeText;
    [SerializeField] private TextMeshProUGUI bestLapText;
    [SerializeField] private TextMeshProUGUI lapCounterText;
    
    [Header("Settings")]
    [SerializeField] private bool showMilliseconds = true;

    private RaceTimer raceTimer;
    private FinishLine finishLine;

    void Start()
    {
        raceTimer = FindObjectOfType<RaceTimer>();
        finishLine = FindObjectOfType<FinishLine>();
        
        if (raceTimer == null)
        {
            Debug.LogError("RaceTimer not found in scene!");
        }
    }

    void Update()
    {
        if (raceTimer == null) return;

        // Update total time
        if (totalTimeText != null)
        {
            totalTimeText.text = $"Time: {RaceTimer.FormatTime(raceTimer.GetCurrentTime())}";
        }

        // Update current lap time
        if (lapTimeText != null)
        {
            lapTimeText.text = $"Lap: {RaceTimer.FormatTime(raceTimer.GetLapTime())}";
        }

        // Update best lap time
        if (bestLapText != null)
        {
            float bestLap = raceTimer.GetBestLapTime();
            if (bestLap > 0)
            {
                bestLapText.text = $"Best: {RaceTimer.FormatTime(bestLap)}";
            }
            else
            {
                bestLapText.text = "Best: --:--.---";
            }
        }

        // Update lap counter
        if (lapCounterText != null && finishLine != null)
        {
            lapCounterText.text = $"Lap: {finishLine.GetCurrentLap()}/{finishLine.GetRequiredLaps()}";
        }
    }
}
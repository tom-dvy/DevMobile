using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages race timing and lap progression.
/// Tracks total race time, current lap time, best lap,
/// and dispatches events when the race starts, a lap is completed,
/// or the race ends.
/// </summary>
public class RaceTimer : MonoBehaviour
{
    [Header("Race Settings")]
    public int requiredLaps = 3; // Number of laps required to finish the race

    [Header("Timer State")]
    [SerializeField] private bool isRunning = false; // Is the race currently active
    private float totalRaceTime = 0f;                // Total elapsed race time
    private float currentLapTime = 0f;                // Elapsed time for the current lap
    private float bestLapTime = float.MaxValue;       // Best lap time recorded
    private int currentLap = 0;                        // Current lap index

    [Header("Events")]
    public UnityEvent onRaceStart;                     // Fired when the race starts
    public UnityEvent<float, int> onLapComplete;       // Lap time, new lap index
    public UnityEvent<float, int> onRaceComplete;     // Total time, total laps

    private void Update()
    {
        if (!isRunning) return;

        totalRaceTime += Time.deltaTime;
        currentLapTime += Time.deltaTime;
    }

    // Starts the race and resets all timers
    public void StartRace()
    {
        if (isRunning) return;

        isRunning = true;
        totalRaceTime = 0f;
        currentLapTime = 0f;
        currentLap = 1;

        onRaceStart?.Invoke();
    }

    // Called when the player crosses the finish line
    public void PassLine()
    {
        if (!isRunning) return;

        float lapTime = currentLapTime;

        // Last lap completed
        if (currentLap >= requiredLaps)
        {
            StopRace();
            return;
        }

        // Regular lap completion
        currentLap++;
        currentLapTime = 0f;

        if (lapTime < bestLapTime)
        {
            bestLapTime = lapTime;
        }

        onLapComplete?.Invoke(lapTime, currentLap);
    }

    // Stops the race and triggers completion event
    public void StopRace()
    {
        isRunning = false;
        onRaceComplete?.Invoke(totalRaceTime, currentLap);
    }

    // --- Getters ---

    public float GetCurrentTime() => totalRaceTime;
    public float GetLapTime() => currentLapTime;
    public float GetBestLapTime() => bestLapTime == float.MaxValue ? 0f : bestLapTime;
    public int GetCurrentLap() => currentLap;
    public int GetRequiredLaps() => requiredLaps;
    public bool IsRunning() => isRunning;

    // Formats time in MM:SS.mmm
    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000f) % 1000f);

        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}

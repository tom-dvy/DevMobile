using UnityEngine;
using UnityEngine.Events;

public class RaceTimer : MonoBehaviour
{
    [Header("Race Settings")]
    public int requiredLaps = 3;

    [Header("Timer State")]
    [SerializeField] private bool isRunning = false;
    private float totalRaceTime = 0f;
    private float currentLapTime = 0f;
    private float bestLapTime = float.MaxValue;
    private int currentLap = 0;

    [Header("Events")]
    public UnityEvent onRaceStart;
    public UnityEvent<float, int> onLapComplete; // Temps du tour, numéro du tour
    public UnityEvent<float, int> onRaceComplete; // Temps total, tours totaux

    void Update()
    {
        if (isRunning)
        {
            totalRaceTime += Time.deltaTime;
            currentLapTime += Time.deltaTime;
        }
    }

    public void StartRace()
    {
        if (isRunning) return;
        isRunning = true;
        totalRaceTime = 0f;
        currentLapTime = 0f;
        currentLap = 1;
        onRaceStart?.Invoke();
        Debug.Log("Race Started!");
    }

   public void PassLine()
{
    if (!isRunning) return;

    float lapTime = currentLapTime;
    Debug.Log($"[RaceTimer] Passage de ligne validé ! Tour actuel : {currentLap}/{requiredLaps}");

    if (currentLap >= requiredLaps)
    {
        Debug.Log("[RaceTimer] Dernier tour fini ! Arrêt de la course.");
        StopRace();
    }
    else
    {
        currentLap++;
        currentLapTime = 0f;
        if (lapTime < bestLapTime) bestLapTime = lapTime;
        onLapComplete?.Invoke(lapTime, currentLap);
    }
}

public void StopRace()
{
    isRunning = false;
    Debug.Log($"[RaceTimer] COURSE ARRÊTÉE ! Temps total : {totalRaceTime}");
    onRaceComplete?.Invoke(totalRaceTime, currentLap);
}
    // Getters
    public float GetCurrentTime() => totalRaceTime;
    public float GetLapTime() => currentLapTime;
    public float GetBestLapTime() => bestLapTime == float.MaxValue ? 0f : bestLapTime;
    public int GetCurrentLap() => currentLap;
    public int GetRequiredLaps() => requiredLaps;
    public bool IsRunning() => isRunning;

    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
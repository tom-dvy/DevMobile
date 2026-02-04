using UnityEngine;

/// <summary>
/// Manages race timing - attach to a GameObject in your scene
/// </summary>
public class RaceTimer : MonoBehaviour
{
    [Header("Timer State")]
    [SerializeField] private bool isRunning = false;
    
    private float totalRaceTime = 0f;
    private float currentLapTime = 0f;
    private float bestLapTime = float.MaxValue;

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
        isRunning = true;
        totalRaceTime = 0f;
        currentLapTime = 0f;
        bestLapTime = float.MaxValue;
        Debug.Log("Timer started!");
    }

    public void StopRace()
    {
        isRunning = false;
        Debug.Log($"Timer stopped! Final time: {totalRaceTime:F2}s");
    }

    public void ResetLapTimer()
    {
        // Store best lap
        if (currentLapTime < bestLapTime)
        {
            bestLapTime = currentLapTime;
        }
        
        currentLapTime = 0f;
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    // Getters
    public float GetCurrentTime() => totalRaceTime;
    public float GetLapTime() => currentLapTime;
    public float GetBestLapTime() => bestLapTime == float.MaxValue ? 0f : bestLapTime;
    public bool IsRunning() => isRunning;

    // Format time as MM:SS.mmm
    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000f) % 1000f);
        
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
using UnityEngine;

/// <summary>
/// Handles start line and finish line logic
/// Detects when the player crosses the line to start the race
/// </summary>
public class FinishLine : MonoBehaviour
{
    [SerializeField] private bool isStartLine = true;           // Defines if this line can start the race
    [SerializeField] private float minTimeBetweenPasses = 2f;   // Anti double-trigger delay (seconds)
    private RaceTimer raceTimer;
    private float lastPassTime;

    private void Start()
    {
        // Find the race timer in the scene
        raceTimer = FindFirstObjectByType<RaceTimer>();
        if (raceTimer == null)
        {
            Debug.LogError("RaceTimer not found in scene!");
        }

        // Ensure the collider is used as a trigger
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the player
        if (!other.CompareTag("Player") || raceTimer == null) return;

        // Case 1: Start line crossed while race is not running
        if (isStartLine && !raceTimer.IsRunning())
        {
            raceTimer.StartRace();
            lastPassTime = Time.time;
            return;
        }

        // Case 2: Lap or finish line crossed while race is running
        if (raceTimer.IsRunning())
        {
            // Prevent multiple triggers in a short time
            if (Time.time > lastPassTime + minTimeBetweenPasses)
            {
                raceTimer.PassLine();
                lastPassTime = Time.time;
            }
        }
    }

    // Returns the current lap number
    public int GetCurrentLap()
    {
        return raceTimer != null ? raceTimer.GetCurrentLap() : 0;
    }

    // Returns the total number of laps required to finish the race
    public int GetRequiredLaps()
    {
        return raceTimer != null ? raceTimer.GetRequiredLaps() : 0;
    }
}

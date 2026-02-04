using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Place this script on a trigger collider at the finish line
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class FinishLine : MonoBehaviour
{
    [Header("Finish Line Settings")]
    [SerializeField] private bool isStartLine = true; // Also acts as start line
    [SerializeField] private int requiredLaps = 1; // Number of laps to complete the race
    
    [Header("Events")]
    public UnityEvent onRaceStart;
    public UnityEvent<float> onLapComplete; // Passes lap time
    public UnityEvent<float, int> onRaceComplete; // Passes total time and total laps
    
    private bool raceStarted = false;
    private int currentLap = 0;

    void Start()
    {
        // Make sure it's a trigger
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player's car
        if (other.CompareTag("Player"))
        {
            RaceTimer raceTimer = FindFirstObjectByType<RaceTimer>();
            
            if (raceTimer == null)
            {
                Debug.LogError("RaceTimer not found in scene!");
                return;
            }

            // Start the race on first trigger
            if (isStartLine && !raceStarted)
            {
                raceStarted = true;
                raceTimer.StartRace();
                onRaceStart?.Invoke();
                Debug.Log("Race Started!");
                return;
            }

            // Complete a lap
            if (raceStarted)
            {
                currentLap++;
                float lapTime = raceTimer.GetCurrentTime();
                
                onLapComplete?.Invoke(lapTime);
                Debug.Log($"Lap {currentLap} completed! Time: {lapTime:F2}s");

                // Check if race is complete
                if (currentLap >= requiredLaps)
                {
                    float totalTime = raceTimer.GetCurrentTime();
                    raceTimer.StopRace();
                    onRaceComplete?.Invoke(totalTime, currentLap);
                    Debug.Log($"Race Finished! Total time: {totalTime:F2}s - {currentLap} laps");
                    raceStarted = false;
                }
                else
                {
                    // Reset lap timer for next lap
                    raceTimer.ResetLapTimer();
                }
            }
        }
    }

    // Public methods for external access
    public int GetCurrentLap() => currentLap;
    public int GetRequiredLaps() => requiredLaps;
    public bool IsRaceStarted() => raceStarted;

    private void OnDrawGizmos()
    {
        // Visualize finish line in editor
        Gizmos.color = isStartLine ? Color.green : Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
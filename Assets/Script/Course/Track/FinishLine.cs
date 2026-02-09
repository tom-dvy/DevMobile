using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class FinishLine : MonoBehaviour
{
    [Header("Finish Line Settings")]
    [SerializeField] private bool isStartLine = true;
    [SerializeField] private int requiredLaps = 1;
    
    [Header("Events")]
    public UnityEvent onRaceStart;
    public UnityEvent<float> onLapComplete;
    public UnityEvent<float, int> onRaceComplete;
    
    private bool raceStarted = false;
    private int currentLap = 0;

    void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        
        Debug.Log($"FinishLine initialized - isStartLine: {isStartLine}, requiredLaps: {requiredLaps}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name}, tag: {other.tag}");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
            
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
                
                Debug.Log($"Lap {currentLap}/{requiredLaps} completed!");
                onLapComplete?.Invoke(lapTime);

                // Check if race is complete
                if (currentLap >= requiredLaps)
                {
                    float totalTime = raceTimer.GetCurrentTime();
                    raceTimer.StopRace();
                    
                    Debug.Log($"RACE COMPLETE! Invoking onRaceComplete event. Total time: {totalTime:F2}s");
                    Debug.Log($"onRaceComplete listeners: {onRaceComplete.GetPersistentEventCount()}");
                    
                    onRaceComplete?.Invoke(totalTime, currentLap);
                    
                    Debug.Log($"Race Finished! Total time: {totalTime:F2}s - {currentLap} laps");
                    raceStarted = false;
                }
                else
                {
                    raceTimer.ResetLapTimer();
                }
            }
        }
    }

    public int GetCurrentLap() => currentLap;
    public int GetRequiredLaps() => requiredLaps;
    public bool IsRaceStarted() => raceStarted;

    private void OnDrawGizmos()
    {
        Gizmos.color = isStartLine ? Color.green : Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
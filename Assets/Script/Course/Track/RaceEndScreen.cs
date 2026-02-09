using UnityEngine;
using TMPro;

public class RaceEndScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI bestLapText;
    [SerializeField] private TextMeshProUGUI messageText;
    
    private RaceTimer raceTimer;
    private CarController carController;
    private bool isSubscribed = false;
    private int subscriptionAttempts = 0;

    void Start()
    {
        Debug.Log("RaceEndScreen: Start");
        
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
            Debug.Log("End screen panel hidden");
        }
        else
        {
            Debug.LogError("End screen panel is NULL!");
        }
        
        FindReferences();
    }

    void Update()
    {
        if (!isSubscribed && subscriptionAttempts < 100)
        {
            TrySubscribeToFinishLine();
            subscriptionAttempts++;
        }
    }

    private void FindReferences()
    {
        if (raceTimer == null)
        {
            raceTimer = FindFirstObjectByType<RaceTimer>();
            if (raceTimer != null)
            {
                Debug.Log("RaceTimer found!");
            }
            else
            {
                Debug.LogWarning("RaceTimer NOT found!");
            }
        }
        
        if (carController == null)
        {
            carController = FindFirstObjectByType<CarController>();
            if (carController != null)
            {
                Debug.Log("CarController found!");
            }
            else
            {
                Debug.LogWarning("CarController NOT found!");
            }
        }
    }

    private void TrySubscribeToFinishLine()
    {
        FinishLine[] finishLines = FindObjectsByType<FinishLine>(FindObjectsSortMode.None);
        
        if (finishLines.Length > 0)
        {
            Debug.Log($"Found {finishLines.Length} FinishLine(s)");
            
            foreach (FinishLine line in finishLines)
            {
                // Remove any existing listener first to avoid duplicates
                line.onRaceComplete.RemoveListener(OnRaceComplete);
                // Add listener
                line.onRaceComplete.AddListener(OnRaceComplete);
                
                Debug.Log($"Subscribed to FinishLine on {line.gameObject.name}");
            }
            
            isSubscribed = true;
        }
    }

    private void OnRaceComplete(float totalTime, int laps)
    {
        Debug.Log($"OnRaceComplete called! Time: {totalTime:F2}s, Laps: {laps}");
        ShowEndScreen(totalTime, laps);
    }

    private void ShowEndScreen(float totalTime, int laps)
    {
        Debug.Log("ShowEndScreen called");
        
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
            Debug.Log("End screen panel activated!");
        }
        else
        {
            Debug.LogError("Cannot show end screen - panel is NULL!");
        }
        
        if (finalTimeText != null)
        {
            finalTimeText.text = $"Final Time: {RaceTimer.FormatTime(totalTime)}";
        }
        
        if (bestLapText != null && raceTimer != null)
        {
            float bestLap = raceTimer.GetBestLapTime();
            if (bestLap > 0)
            {
                bestLapText.text = $"Best Lap: {RaceTimer.FormatTime(bestLap)}";
            }
        }
        
        if (messageText != null)
        {
            messageText.text = "Race Complete!";
        }
        
        if (carController != null)
        {
            carController.enabled = false;
            Debug.Log("Car disabled");
        }
    }

    public void RestartRace()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void QuitToMenu()
    {
        Debug.Log("Quit to menu");
    }
}

using UnityEngine;
using TMPro;
using System.Collections;

public class RaceCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private bool startOnLoad = true;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownPanel;
    
    [Header("Car Reference (Optional - will auto-find)")]
    [SerializeField] private CarController carController;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip countdownBeep;
    [SerializeField] private AudioClip goSound;
    
    private bool countdownFinished = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Auto-find car controller if not set
        if (carController == null)
        {
            carController = FindFirstObjectByType<CarController>();
        }
        
        if (startOnLoad)
        {
            // Attendre un peu pour laisser le temps au CarSpawner de spawn la voiture
            Invoke(nameof(StartCountdown), 0.5f);
        }
    }

    public void StartCountdown()
    {
        if (countdownFinished) return;
        
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        // Disable car movement during countdown
        if (carController != null)
        {
            carController.enabled = false;
            Debug.Log("Countdown: Car disabled");
        }
        else
        {
            Debug.LogWarning("CarController not found! Car might move during countdown.");
        }
        
        // Show countdown panel
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }
        
        // Countdown: 3, 2, 1
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                countdownText.fontSize = 120;
            }
            
            if (audioSource != null && countdownBeep != null)
            {
                audioSource.PlayOneShot(countdownBeep);
            }
            
            Debug.Log($"Countdown: {i}");
            yield return new WaitForSeconds(1f);
        }
        
        // GO!
        if (countdownText != null)
        {
            countdownText.text = "GO!";
            countdownText.fontSize = 150;
        }
        
        if (audioSource != null && goSound != null)
        {
            audioSource.PlayOneShot(goSound);
        }
        
        Debug.Log("GO!");
        yield return new WaitForSeconds(1f);
        
        // Hide countdown
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
        
        // Enable car movement
        if (carController != null)
        {
            carController.enabled = true;
            Debug.Log("Countdown: Car enabled - GO!");
        }
        
        countdownFinished = true;
    }

    public bool IsCountdownFinished() => countdownFinished;
}

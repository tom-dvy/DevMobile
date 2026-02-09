using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Countdown before race starts
/// </summary>
public class RaceCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private bool startOnLoad = true;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownPanel;
    
    [Header("Car Reference")]
    [SerializeField] private CarController carController; // Reference to car to disable/enable
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip countdownBeep;
    [SerializeField] private AudioClip goSound;
    
    private bool countdownFinished = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (startOnLoad)
        {
            StartCountdown();
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
            
            // Play beep sound
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
        
        // Play GO sound
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
        }
        
        countdownFinished = true;
    }

    public bool IsCountdownFinished() => countdownFinished;
}
using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Displays and manages a countdown before the race starts
/// Disables car control during the countdown, then enables it when finished
/// Supports UI text, panel display, and optional audio feedback
/// </summary>
public class RaceCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    [SerializeField] private float countdownDuration = 3f; // Countdown length in seconds
    [SerializeField] private bool startOnLoad = true;       // Start countdown automatically on scene load

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText; // Text displaying countdown numbers
    [SerializeField] private GameObject countdownPanel;     // Panel containing countdown UI

    [Header("Car Reference")]
    [SerializeField] private CarController carController;   // Car to disable/enable during countdown

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip countdownBeep; // Sound played each second
    [SerializeField] private AudioClip goSound;        // Sound played at "GO!"

    private bool countdownFinished = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (startOnLoad)
        {
            StartCountdown();
        }
    }

    // Starts the countdown if it has not already been completed
    public void StartCountdown()
    {
        if (countdownFinished) return;

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        // Disable car control during countdown
        if (carController != null)
        {
            carController.enabled = false;
        }

        // Show countdown UI
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        // Countdown sequence: 3, 2, 1...
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                countdownText.fontSize = 120;
            }

            // Play countdown beep
            if (audioSource != null && countdownBeep != null)
            {
                audioSource.PlayOneShot(countdownBeep);
            }

            yield return new WaitForSeconds(1f);
        }

        // Display "GO!"
        if (countdownText != null)
        {
            countdownText.text = "GO!";
            countdownText.fontSize = 150;
        }

        // Play start sound
        if (audioSource != null && goSound != null)
        {
            audioSource.PlayOneShot(goSound);
        }

        yield return new WaitForSeconds(1f);

        // Hide countdown UI
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        // Re-enable car control
        if (carController != null)
        {
            carController.enabled = true;
        }

        countdownFinished = true;
    }

    // Returns true once the countdown has fully completed
    public bool IsCountdownFinished()
    {
        return countdownFinished;
    }
}

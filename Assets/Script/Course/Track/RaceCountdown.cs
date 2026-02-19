using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class RaceCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private bool startOnLoad = true;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownPanel;
    
    [Header("Starting Lights (Feux)")]
    [SerializeField] private GameObject lightsPanel;
    [Tooltip("Assigne 6 images : les deux du '3', puis les deux du '2', puis les deux du '1'")]
    [SerializeField] private Image[] raceLights; 
    [SerializeField] private Color colorOff = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color colorRed = Color.red;
    [SerializeField] private Color colorGreen = Color.green;

    [Header("Car Reference (Optional)")]
    [SerializeField] private CarController carController;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip countdownBeep;
    [SerializeField] private AudioClip goSound;
    
    private bool countdownFinished = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (carController == null)
            carController = FindFirstObjectByType<CarController>();

        ResetLights();
        
        if (startOnLoad)
            Invoke(nameof(StartCountdown), 0.5f);
    }

    public void StartCountdown()
    {
        if (countdownFinished) return;
        StartCoroutine(CountdownRoutine());
    }

    private void ResetLights()
    {
        foreach (Image light in raceLights)
        {
            if (light != null) light.color = colorOff;
        }
        if (lightsPanel != null) lightsPanel.SetActive(false);
    }

    private IEnumerator CountdownRoutine()
    {
        if (carController != null) carController.enabled = false;
        
        if (countdownPanel != null) countdownPanel.SetActive(true);
        if (lightsPanel != null) lightsPanel.SetActive(true);
        
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                countdownText.fontSize = 120;
            }

            int pairIndex = (int)countdownDuration - i; 
            int oppositeIndex = (raceLights.Length - 1) - pairIndex;

            if (pairIndex < raceLights.Length && raceLights[pairIndex] != null)
                raceLights[pairIndex].color = colorRed;

            if (oppositeIndex >= 0 && raceLights[oppositeIndex] != null)
                raceLights[oppositeIndex].color = colorRed;
            
            if (audioSource != null && countdownBeep != null)
                audioSource.PlayOneShot(countdownBeep);
            
            yield return new WaitForSeconds(1f);
        }
        
        if (countdownText != null)
        {
            countdownText.text = "GO!";
            countdownText.fontSize = 150;
        }

        foreach (Image light in raceLights)
        {
            if (light != null) light.color = colorGreen;
        }
        
        if (audioSource != null && goSound != null)
            audioSource.PlayOneShot(goSound);
        
        yield return new WaitForSeconds(1f);
        
        if (countdownPanel != null) countdownPanel.SetActive(false);
        if (lightsPanel != null) lightsPanel.SetActive(false);
        
        if (carController != null) carController.enabled = true;
        
        countdownFinished = true;
    }

    public bool IsCountdownFinished() => countdownFinished;
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SplashScreen : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string menuSceneName = "Menu";
    
    [Header("Delay Settings")]
    [SerializeField] private float minimumDisplayTime = 1f;
    [SerializeField] private bool useAutoTransition = false;
    [SerializeField] private float autoTransitionDelay = 5f;
    
    [Header("Fade Settings (Optional)")]
    [SerializeField] private bool useFade = false;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    
    private float displayTimer = 0f;
    private bool isTransitioning = false;
    private bool canTransition = false;

    void Start()
    {
        displayTimer = 0f;
        
        if (useAutoTransition)
        {
            Invoke(nameof(LoadMenuScene), autoTransitionDelay);
        }
    }

    void Update()
    {
        displayTimer += Time.deltaTime;
        
        if (displayTimer >= minimumDisplayTime && !canTransition)
        {
            canTransition = true;
            Debug.Log("Splash screen ready - click or tap to continue");
        }
        
        if (isTransitioning || !canTransition) return;
        
        if (IsInputDetected())
        {
            LoadMenuScene();
        }
    }

    private bool IsInputDetected()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }
        
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            return true;
        }
        
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return true;
        }
        
        return false;
    }

    private void LoadMenuScene()
    {
        if (isTransitioning) return;
        
        isTransitioning = true;
        
        if (useFade && fadeCanvasGroup != null)
        {
            StartCoroutine(FadeAndLoadScene());
        }
        else
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }

    private System.Collections.IEnumerator FadeAndLoadScene()
    {
        float timer = 0f;
        
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        
        SceneManager.LoadScene(menuSceneName);
    }
}
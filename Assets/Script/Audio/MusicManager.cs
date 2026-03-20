using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class MusicManager : MonoBehaviour
{
    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
        public float volume = 0.5f;
        [Tooltip("If true, music continues when switching between scenes with same music")]
        public bool persistent = true;
    }

    [Header("Music Settings")]
    [SerializeField] private List<SceneMusic> sceneMusicList = new List<SceneMusic>();
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Transition Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private bool smoothTransitions = true;

    private AudioSource audioSource;
    private static MusicManager instance;
    private string currentSceneName;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        Debug.Log("MusicManager initialized and persisting across scenes");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        PlayMusicForScene(currentSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;
        Debug.Log($"Scene loaded: {newSceneName}");

        if (newSceneName != currentSceneName)
        {
            currentSceneName = newSceneName;
            PlayMusicForScene(newSceneName);
        }
    }

    private void PlayMusicForScene(string sceneName)
    {
        SceneMusic sceneMusic = sceneMusicList.Find(sm => sm.sceneName == sceneName);

        if (sceneMusic != null)
        {
            if (sceneMusic.musicClip != audioSource.clip)
            {
                ChangeMusic(sceneMusic.musicClip, sceneMusic.volume);
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                Debug.Log($"🎵 Continuing same music for {sceneName}");
            }
        }
        else if (defaultMusic != null)
        {
            if (defaultMusic != audioSource.clip)
            {
                ChangeMusic(defaultMusic, defaultVolume);
            }
            Debug.Log($"Playing default music for {sceneName}");
        }
        else
        {
            Debug.LogWarning($"No music configured for scene: {sceneName}");
        }
    }

    private void ChangeMusic(AudioClip newClip, float targetVolume)
    {
        if (newClip == null)
        {
            Debug.LogWarning("Attempted to change to null music clip");
            return;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (smoothTransitions && audioSource.isPlaying)
        {
            fadeCoroutine = StartCoroutine(FadeTransition(newClip, targetVolume));
        }
        else
        {
            audioSource.clip = newClip;
            audioSource.volume = targetVolume;
            audioSource.Play();
            Debug.Log($"Now playing: {newClip.name}");
        }
    }

    private IEnumerator FadeTransition(AudioClip newClip, float targetVolume)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        audioSource.clip = newClip;
        audioSource.Play();

        Debug.Log($"Fading to: {newClip.name}");

        for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeInDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
        fadeCoroutine = null;
    }


    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public void Mute(bool mute)
    {
        audioSource.mute = mute;
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Resume()
    {
        audioSource.UnPause();
    }

    public void Stop()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        audioSource.Stop();
    }

    public static MusicManager Instance => instance;
}
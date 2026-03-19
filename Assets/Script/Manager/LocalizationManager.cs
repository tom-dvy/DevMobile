using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

/// <summary>
/// Gestionnaire de localisation
/// Permet de changer la langue du jeu dynamiquement
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instance;

    public static LocalizationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<LocalizationManager>();
                if (instance == null)
                {
                    UnityEngine.GameObject obj = new UnityEngine.GameObject("LocalizationManager");
                    instance = obj.AddComponent<LocalizationManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Charger la langue sauvegardee
        LoadSavedLanguage();
    }

    // ============================================================================
    // CHANGER DE LANGUE
    // ============================================================================

    /// <summary>
    /// Change la langue du jeu
    /// </summary>
    /// <param name="localeCode">Code de la langue (ex: "fr", "en", "es")</param>
    public void ChangeLanguage(string localeCode)
    {
        StartCoroutine(ChangeLanguageCoroutine(localeCode));
    }

    private IEnumerator ChangeLanguageCoroutine(string localeCode)
    {
        Debug.Log($"[LOCALIZATION] Changing language to: {localeCode}");

        // Attendre que le systeme de localisation soit pret
        yield return LocalizationSettings.InitializationOperation;

        // Trouver la locale correspondante
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

        if (locale == null)
        {
            Debug.LogError($"[LOCALIZATION] Locale not found: {localeCode}");
            yield break;
        }

        // Changer la locale
        LocalizationSettings.SelectedLocale = locale;

        // Sauvegarder le choix
        PlayerPrefs.SetString("SelectedLanguage", localeCode);
        PlayerPrefs.Save();

        Debug.Log($"[LOCALIZATION] Language changed to: {locale.LocaleName}");
    }

    /// <summary>
    /// Change la langue via index (pour dropdown)
    /// </summary>
    /// <param name="index">Index dans la liste des langues (0=French, 1=English, 2=Spanish)</param>
    public void ChangeLanguageByIndex(int index)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        if (index < 0 || index >= locales.Count)
        {
            Debug.LogError($"[LOCALIZATION] Invalid language index: {index}");
            return;
        }

        var locale = locales[index];
        ChangeLanguage(locale.Identifier.Code);
    }

    // ============================================================================
    // CHARGER LA LANGUE SAUVEGARDEE
    // ============================================================================

    private void LoadSavedLanguage()
    {
        if (PlayerPrefs.HasKey("SelectedLanguage"))
        {
            string savedLanguage = PlayerPrefs.GetString("SelectedLanguage");
            Debug.Log($"[LOCALIZATION] Loading saved language: {savedLanguage}");
            ChangeLanguage(savedLanguage);
        }
        else
        {
            Debug.Log("[LOCALIZATION] No saved language, using system default");
            // Unity va automatiquement utiliser la langue du systeme
        }
    }

    // ============================================================================
    // RECUPERER LA LANGUE ACTUELLE
    // ============================================================================

    /// <summary>
    /// Recupere le code de la langue actuelle (ex: "fr", "en", "es")
    /// </summary>
    public string GetCurrentLanguageCode()
    {
        if (LocalizationSettings.SelectedLocale == null)
            return "en"; // Par defaut

        return LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    /// <summary>
    /// Recupere l'index de la langue actuelle (pour dropdown)
    /// </summary>
    public int GetCurrentLanguageIndex()
    {
        if (LocalizationSettings.SelectedLocale == null)
            return 0;

        var locales = LocalizationSettings.AvailableLocales.Locales;
        return locales.IndexOf(LocalizationSettings.SelectedLocale);
    }

    /// <summary>
    /// Recupere le nom de la langue actuelle (ex: "Francais", "English")
    /// </summary>
    public string GetCurrentLanguageName()
    {
        if (LocalizationSettings.SelectedLocale == null)
            return "English";

        return LocalizationSettings.SelectedLocale.LocaleName;
    }
}

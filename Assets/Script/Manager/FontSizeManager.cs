using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Gestionnaire de taille de police pour l'accessibilite
/// Permet de changer la taille de tous les textes du jeu
/// </summary>
public class FontSizeManager : MonoBehaviour
{
    private static FontSizeManager instance;

    public static FontSizeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<FontSizeManager>();
                if (instance == null)
                {
                    UnityEngine.GameObject obj = new UnityEngine.GameObject("FontSizeManager");
                    instance = obj.AddComponent<FontSizeManager>();
                }
            }
            return instance;
        }
    }

    // Tailles de police disponibles
    public enum FontSize
    {
        Small = 0,    // 85% de la taille normale
        Normal = 1,   // 100%
        Large = 2,    // 125%
        ExtraLarge = 3 // 150%
    }

    private FontSize currentFontSize = FontSize.Normal;
    private List<TextMeshProUGUI> registeredTexts = new List<TextMeshProUGUI>();
    private Dictionary<TextMeshProUGUI, float> originalSizes = new Dictionary<TextMeshProUGUI, float>();

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
        // Charger la taille sauvegardee
        LoadSavedFontSize();
    }

    // ============================================================================
    // ENREGISTRER DES TEXTES
    // ============================================================================

    /// <summary>
    /// Enregistre un texte pour qu'il soit affecte par les changements de taille
    /// Appeler cette methode pour chaque TextMeshProUGUI que tu veux rendre adaptable
    /// </summary>
    public void RegisterText(TextMeshProUGUI text)
    {
        if (text == null) return;

        if (!registeredTexts.Contains(text))
        {
            registeredTexts.Add(text);
            originalSizes[text] = text.fontSize;

            // Appliquer la taille actuelle
            ApplyFontSizeToText(text);
        }
    }

    /// <summary>
    /// Desinscrit un texte (par exemple quand il est detruit)
    /// </summary>
    public void UnregisterText(TextMeshProUGUI text)
    {
        if (text == null) return;

        registeredTexts.Remove(text);
        originalSizes.Remove(text);
    }

    /// <summary>
    /// Enregistre automatiquement tous les TextMeshProUGUI dans la scene
    /// Utile pour enregistrer tous les textes d'un coup
    /// </summary>
    public void RegisterAllTextsInScene()
    {
        var allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);

        foreach (var text in allTexts)
        {
            RegisterText(text);
        }

        Debug.Log($"[FONT SIZE] Registered {allTexts.Length} texts");
    }

    // ============================================================================
    // CHANGER LA TAILLE
    // ============================================================================

    /// <summary>
    /// Change la taille de police globale
    /// </summary>
    public void SetFontSize(FontSize size)
    {
        currentFontSize = size;

        Debug.Log($"[FONT SIZE] Changing to: {size}");

        // Appliquer a tous les textes enregistres
        ApplyFontSizeToAllTexts();

        // Sauvegarder le choix
        PlayerPrefs.SetInt("FontSize", (int)size);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Change la taille via index (pour dropdown)
    /// </summary>
    public void SetFontSizeByIndex(int index)
    {
        if (index < 0 || index > 3)
        {
            Debug.LogError($"[FONT SIZE] Invalid index: {index}");
            return;
        }

        SetFontSize((FontSize)index);
    }

    // ============================================================================
    // APPLIQUER LES CHANGEMENTS
    // ============================================================================

    private void ApplyFontSizeToAllTexts()
    {
        // Nettoyer les textes detruits
        registeredTexts.RemoveAll(text => text == null);

        foreach (var text in registeredTexts)
        {
            ApplyFontSizeToText(text);
        }
    }

    private void ApplyFontSizeToText(TextMeshProUGUI text)
    {
        if (text == null) return;

        if (!originalSizes.ContainsKey(text))
        {
            originalSizes[text] = text.fontSize;
        }

        float originalSize = originalSizes[text];
        float multiplier = GetMultiplier(currentFontSize);

        text.fontSize = originalSize * multiplier;
    }

    private float GetMultiplier(FontSize size)
    {
        switch (size)
        {
            case FontSize.Small:
                return 0.85f;
            case FontSize.Normal:
                return 1.0f;
            case FontSize.Large:
                return 1.25f;
            case FontSize.ExtraLarge:
                return 1.5f;
            default:
                return 1.0f;
        }
    }

    // ============================================================================
    // CHARGER LA TAILLE SAUVEGARDEE
    // ============================================================================

    private void LoadSavedFontSize()
    {
        if (PlayerPrefs.HasKey("FontSize"))
        {
            int savedSize = PlayerPrefs.GetInt("FontSize");
            currentFontSize = (FontSize)savedSize;
            Debug.Log($"[FONT SIZE] Loaded saved size: {currentFontSize}");
        }
        else
        {
            currentFontSize = FontSize.Normal;
            Debug.Log("[FONT SIZE] No saved size, using Normal");
        }
    }

    // ============================================================================
    // RECUPERER LA TAILLE ACTUELLE
    // ============================================================================

    /// <summary>
    /// Recupere la taille de police actuelle
    /// </summary>
    public FontSize GetCurrentFontSize()
    {
        return currentFontSize;
    }

    /// <summary>
    /// Recupere l'index de la taille actuelle (pour dropdown)
    /// </summary>
    public int GetCurrentFontSizeIndex()
    {
        return (int)currentFontSize;
    }

    /// <summary>
    /// Recupere le nom de la taille actuelle
    /// </summary>
    public string GetCurrentFontSizeName()
    {
        switch (currentFontSize)
        {
            case FontSize.Small:
                return "Petit";
            case FontSize.Normal:
                return "Normal";
            case FontSize.Large:
                return "Grand";
            case FontSize.ExtraLarge:
                return "Tres Grand";
            default:
                return "Normal";
        }
    }
}

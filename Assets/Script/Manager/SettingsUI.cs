using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
 
/// <summary>
/// Interface utilisateur pour les Settings
/// Gere langue et taille de police
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Language Settings")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("Font Size Settings")]
    [SerializeField] private TMP_Dropdown fontSizeDropdown;

    [Header("Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        SetupLanguageDropdown();
        SetupFontSizeDropdown();
        SetupButtons();

        // Enregistrer tous les textes de la scene pour la taille de police
        FontSizeManager.Instance.RegisterAllTextsInScene();
    }

    // ============================================================================
    // SETUP DROPDOWNS
    // ============================================================================

    private void SetupLanguageDropdown()
    {
        if (languageDropdown == null) return;

        // Remplir les options
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string>
        {
            "Francais",
            "English",
            "Espanol"
        });

        // Definir la valeur actuelle
        int currentIndex = LocalizationManager.Instance.GetCurrentLanguageIndex();
        languageDropdown.value = currentIndex;
        languageDropdown.RefreshShownValue();

        // Lier l'event
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        Debug.Log($"[SETTINGS] Language dropdown initialized. Current: {currentIndex}");
    }

    private void SetupFontSizeDropdown()
    {
        if (fontSizeDropdown == null) return;

        // Remplir les options
        fontSizeDropdown.ClearOptions();
        fontSizeDropdown.AddOptions(new List<string>
        {
            "Petit",
            "Normal",
            "Grand",
            "Tres Grand"
        });

        // Definir la valeur actuelle
        int currentIndex = FontSizeManager.Instance.GetCurrentFontSizeIndex();
        fontSizeDropdown.value = currentIndex;
        fontSizeDropdown.RefreshShownValue();

        // Lier l'event
        fontSizeDropdown.onValueChanged.AddListener(OnFontSizeChanged);

        Debug.Log($"[SETTINGS] Font size dropdown initialized. Current: {currentIndex}");
    }

    private void SetupButtons()
    {
        if (applyButton != null)
        {
            applyButton.onClick.AddListener(OnApplyButtonClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    // ============================================================================
    // EVENTS CALLBACKS
    // ============================================================================

    private void OnLanguageChanged(int index)
    {
        Debug.Log($"[SETTINGS] Language changed to index: {index}");
        LocalizationManager.Instance.ChangeLanguageByIndex(index);
    }

    private void OnFontSizeChanged(int index)
    {
        Debug.Log($"[SETTINGS] Font size changed to index: {index}");
        FontSizeManager.Instance.SetFontSizeByIndex(index);
    }

    private void OnApplyButtonClicked()
    {
        Debug.Log("[SETTINGS] Apply button clicked");
        // Les changements sont deja appliques en temps reel
        // Ce bouton sert juste a confirmer visuellement
        OnCloseButtonClicked();
    }

    private void OnCloseButtonClicked()
    {
        Debug.Log("[SETTINGS] Close button clicked");
        gameObject.SetActive(false);
    }

    // ============================================================================
    // CLEANUP
    // ============================================================================

    private void OnDestroy()
    {
        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.RemoveAllListeners();
        }

        if (fontSizeDropdown != null)
        {
            fontSizeDropdown.onValueChanged.RemoveAllListeners();
        }

        if (applyButton != null)
        {
            applyButton.onClick.RemoveAllListeners();
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Interface utilisateur pour demander et afficher le pseudo du joueur
/// A utiliser au premier lancement ou dans les settings
/// </summary>
public class PlayerNameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject nameInputPanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Display")]
    [SerializeField] private TextMeshProUGUI displayNameText;

    private void Start()
    {
        // Lier le bouton
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        // Verifier si le joueur a deja un pseudo
        CheckExistingName();
    }

    private async void CheckExistingName()
    {
        // Attendre que l'authentification soit prete
        await System.Threading.Tasks.Task.Delay(1000);

        if (!AuthenticationManager.Instance.IsAuthenticated)
        {
            Debug.LogWarning("[PLAYER NAME UI] Not authenticated yet");
            return;
        }

        // Recuperer le pseudo existant
        string existingName = await AuthenticationManager.Instance.GetPlayerNameAsync();

        if (existingName == "Player" || string.IsNullOrEmpty(existingName))
        {
            // Pas de pseudo -> afficher le panel
            ShowNameInputPanel();
        }
        else
        {
            // Pseudo deja defini -> afficher
            HideNameInputPanel();
            UpdateDisplayName(existingName);
        }
    }

    private async void OnConfirmButtonClicked()
    {
        string inputName = nameInputField.text.Trim();

        // Validation
        if (string.IsNullOrEmpty(inputName))
        {
            ShowFeedback("Le pseudo ne peut pas etre vide !", Color.red);
            return;
        }

        if (inputName.Length < 3)
        {
            ShowFeedback("Le pseudo doit contenir au moins 3 caracteres", Color.red);
            return;
        }

        if (inputName.Length > 20)
        {
            ShowFeedback("Le pseudo ne peut pas depasser 20 caracteres", Color.red);
            return;
        }

        // Sauvegarder le pseudo
        try
        {
            await AuthenticationManager.Instance.SetPlayerNameAsync(inputName);

            ShowFeedback("Pseudo enregistre avec succes !", Color.green);

            // Attendre un peu puis cacher le panel
            await System.Threading.Tasks.Task.Delay(1000);
            HideNameInputPanel();
            UpdateDisplayName(inputName);
        }
        catch (System.Exception e)
        {
            ShowFeedback($"Erreur : {e.Message}", Color.red);
        }
    }

    private void ShowNameInputPanel()
    {
        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(true);
        }
    }

    private void HideNameInputPanel()
    {
        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(false);
        }
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
        }
    }

    private void UpdateDisplayName(string name)
    {
        if (displayNameText != null)
        {
            displayNameText.text = $"Bienvenue, {name} !";
        }
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
        }
    }
}

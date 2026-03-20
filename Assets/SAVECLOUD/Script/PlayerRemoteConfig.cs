using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.UI;
using Unity.RemoteConfig;
public class PlayerRemoteConfig : MonoBehaviour
{
    [Header("Réglages Joueur")]
    public float moveSpeed = 5f; // Valeur par défaut locale

    // Structures requises par l'API Remote Config (peuvent rester vides)
    public struct userAttributes { }
    public struct appAttributes { }

    async void Awake()
    {

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        FetchRemoteData();
    }

    void FetchRemoteData()
    {
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        // Vérifie si la clé existe sur le serveur et récupère la valeur
        // "PlayerSpeed" doit correspondre exactement au nom sur le dashboard
        moveSpeed = RemoteConfigService.Instance.appConfig.GetFloat("PlayerSpeed");

        Debug.Log($"Vitesse mise à jour via Remote Config : {moveSpeed}");
    }
    private void OnDestroy()
    {
        // Nettoyage de l'événement
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;
    }
}
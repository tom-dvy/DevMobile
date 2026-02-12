using UnityEngine;
using System.Collections;

public class AnalyticsInitializer : MonoBehaviour
{
	private IEnumerator Start()
	{
		Debug.Log("[ANALYTICS INITIALIZER] Waiting for Manager...");

		// Attente intelligente : On attend tant que le Manager n'est pas prêt
		// (Timeout de sécurité pour ne pas bloquer indéfiniment si internet est coupé)
		float timeout = 5f;
		while (!AnalyticsManager.Instance.IsInitialized && timeout > 0)
		{
			yield return null; // Attend la frame suivante
			timeout -= Time.deltaTime;
		}

		if (AnalyticsManager.Instance.IsInitialized)
		{
			Debug.Log("[ANALYTICS INITIALIZER] Manager ready! Sending scene_load event.");
			// On envoie un event spécifique à cette scène
			AnalyticsManager.Instance.TrackEvent("scene_loaded_main");
		}
		else
		{
			Debug.LogWarning("[ANALYTICS INITIALIZER] Timed out. Analytics not ready.");
		}
	}
}
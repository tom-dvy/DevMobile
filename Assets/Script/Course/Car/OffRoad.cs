using UnityEngine;
using UnityEngine.EventSystems;


public class OffRoad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerRespawner respawner = other.GetComponent<PlayerRespawner>();
        if (respawner != null)
        {
            respawner.Respawn();
        }
    }
}
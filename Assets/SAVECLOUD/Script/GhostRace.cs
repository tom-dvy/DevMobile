using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using Unity.AI;
using UnityEngine;
using UnityEngine.AI;
using System;
public class GhostRace : MonoBehaviour
{
    public CloudSave cloud;
    public float speed;
    private bool GhostraceStart=false;

    private int currentWaypoint =0;

    void Start()
{
    cloud.OnUserSignedIn += Initialize;

    if (Unity.Services.Core.UnityServices.State == Unity.Services.Core.ServicesInitializationState.Initialized 
        && Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn)
    {
        Debug.Log("Déjà connecté, lancement manuel de l'initialisation du Ghost.");
        Initialize(true);
    }
    
    cloud.LoadData();
}
	private void Initialize(bool success)
    {
        if(success)
            StartCoroutine(isready());
        else
        {
            Debug.LogError("I can't load the race ghosts because the user has got an issue signing in", this);
        }
    }
	void Update()
{
    Debug.Log($"Ghost State: Ready={GhostraceStart}, Waypoints={cloud.Goto.Count}, Current={currentWaypoint}");

    if (!GhostraceStart) return;

    if (cloud.Timer != null && !cloud.Timer.IsRunning()) return;

    if(currentWaypoint >= cloud.Goto.Count) return;

    float distance = Vector3.Distance(cloud.Goto[currentWaypoint].position, transform.position);

    if (distance > 0.1f) 
    {
        transform.position = Vector3.MoveTowards(transform.position, cloud.Goto[currentWaypoint].position, speed * Time.deltaTime);
        Quaternion targetRotation = cloud.Goto[currentWaypoint].rotation * Quaternion.Euler(0, 0, 180);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 500f * Time.deltaTime);
    }
    else
    {
        currentWaypoint++;
    }
}
    IEnumerator isready()
    {
        cloud.LoadData();
        
        float timeout = 5f; 
        float timer = 0f;
        while(cloud.Goto.Count == 0 && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if(cloud.Goto.Count > 0)
        {
            Debug.Log("Fantôme chargé et prêt !");
            GhostraceStart = true;
        }
        else
        {
            Debug.LogWarning("Aucune donnée de fantôme trouvée pour ce seed.");
        }
    }
}

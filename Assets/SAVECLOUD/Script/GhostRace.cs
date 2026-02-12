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
    public float speed=1;
    private bool GhostraceStart=false;

    private int currentWaypoint =0;

    void Start()
    {
        cloud.OnUserSignedIn += Initialize;
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
        if (!GhostraceStart)
        {
            return;
        }

        if(currentWaypoint == cloud.Goto.Count - 1)
        {
            GhostraceStart = false;
            return;
        }

        if (Vector3.Distance(cloud.Goto[currentWaypoint].position, transform.position) > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, cloud.Goto[currentWaypoint].position, speed * Time.deltaTime);
        }
        else
        {
            currentWaypoint++;
        }
    }

    IEnumerator isready()
    {
        cloud.LoadData();
        yield return new WaitUntil(() => cloud.Goto.Count > 0);
		Debug.Log("cloud.Goto.Count est maintenant supérieur à 0, et est donc chargé");
        GhostraceStart = true;
    }
}

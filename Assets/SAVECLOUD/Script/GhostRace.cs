using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using Unity.AI;
using UnityEngine;
using EasyButtons;
using UnityEngine.AI;
using System;
public class GhostRace : MonoBehaviour
{
    public CloudSave cloud;
    private float speed=20;
    void Start()
    {
        cloud.OnUserSignedIn += Initialize;
    }

	private void Initialize(bool success)
    {
        if(success)
            StartCoroutine(followthepath());
        else
        {
            Debug.LogError("I can't load the race ghosts because the user has got an issue signing in", this);
        }
    }

	void Update()
    {
    }
    IEnumerator followthepath()
    {
        cloud.LoadData();
		yield return new WaitUntil(() => cloud.Goto.Count > 0);
		Debug.Log("cloud.Goto.Count est maintenant supérieur à 0, et est donc chargé");
        for (int i = 0; i < cloud.Goto.Count; i++)
        {
            Debug.Log(cloud.Goto[i].position);
            transform.position = Vector3.MoveTowards(transform.position, cloud.Goto[i].position, speed * Time.deltaTime); 
            yield return new WaitForSeconds(5f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;
using EasyButtons;
public class Autosave : MonoBehaviour
{
    public CloudSave cloud;
    public RaceTimer _raceTimer;
    
    [SerializeField] List<Pose> actual=new List<Pose>();
    void Start()
    {
        StartCoroutine(SaveRun());
    }
    void Update()
    {
        
    }
    IEnumerator SaveRun()
    {
        Vector3 Transform = new (transform.position.x, transform.position.y, transform.position.z);
        Debug.Log(Transform);
        Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.position.z,0);
        Debug.Log(rotation);
        yield return new WaitForSeconds(2);
        Pose actualpose = new Pose(Transform,rotation);
        actual.Add(actualpose);
        StartCoroutine(SaveRun());
    }
    [Button]
    public void sendlist()
    {
        cloud.SaveRace(actual);
    }
    public void saveTime()
    {
        
    }
}

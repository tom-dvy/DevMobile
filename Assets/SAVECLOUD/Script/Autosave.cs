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
        Vector3 randomTransform = new (Random.Range(1,10), 0, Random.Range(20,30));
        Debug.Log(randomTransform);
        Quaternion randomrotation = new Quaternion(Random.Range(1, 10), Random.Range(10, 20), Random.Range(20, 30), Random.Range(30,40));
        Debug.Log(randomrotation);
        yield return new WaitForSeconds(5f);
        Pose actualpose = new Pose(randomTransform,randomrotation);
        actual.Add(actualpose);
        StartCoroutine(SaveRun());
    }
    [Button]
    public void sendlist()
    {
        cloud.SaveData(actual);
    }
}

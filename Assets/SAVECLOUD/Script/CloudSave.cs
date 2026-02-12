using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;
public class CloudSave : MonoBehaviour
{
    public GameObject target;
    public List<Pose> Goto;

    public event Action<bool> OnUserSignedIn;


    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        Task operation = AuthenticationService.Instance.SignInAnonymouslyAsync();
        await operation;
        if(operation.IsCompletedSuccessfully)
        {
            Debug.Log("user is successfully signed in");
            OnUserSignedIn?.Invoke(true);
        }
        else
        {
            Debug.LogError("We are not signed in !!");
            OnUserSignedIn?.Invoke(false);
        }
    
    }
    public async void SaveRace(List<Pose> Poses)
    {
        RaceData a = new RaceData("Playertest", Poses);

        string raceJSON = JsonUtility.ToJson(a);
        Dictionary<string, object> raceDictionnary = JsonUtility.FromJson<Dictionary<string, object>>(raceJSON);
        await CloudSaveService.Instance.Data.Player.SaveAsync(raceDictionnary);
        Debug.Log($"Saved data {string.Join(',', raceJSON)}");
    }
    public async void SaveData(List<Pose> Poses)
    {
        var playerData = new Dictionary<string, object>{
          {"firstKeyName", Poses},
          {"secondKeyName", 123}
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        Debug.Log($"Saved data {string.Join(',', playerData)}");
    }
    public async void LoadData()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
          "firstKeyName", "secondKeyName"
        });
        if (playerData.TryGetValue("firstKeyName", out var firstKey))
        {
            Debug.Log(firstKey.Value.GetAs<List<Pose>>());
            Goto = firstKey.Value.GetAs<List<Pose>>();
        }
        if (playerData.TryGetValue("secondKeyName", out var secondKey))
        {
            Debug.Log($"secondKey value: {secondKey.Value.GetAs<int>()}");
        }
    }
}
public class RaceData
{
    public string name;
    public List<Pose> poses;
    public RaceData(string _name, List<Pose> posess)
    {
        name = _name;
        poses = posess;
    }
}
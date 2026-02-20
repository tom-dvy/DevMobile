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
    public RaceData Playerdata;
    public List<Pose> Goto;
    public TrackGenerator seed;
    public RaceTimer Timer;
    public List<RaceData> Listplayerdata;
    public event Action<bool> OnUserSignedIn;
    public List<RaceData>Playerlist = new List<RaceData>();
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
    public void SaveRace(List<Pose> Poses)
    {
        Playerdata = new RaceData("PlayertestV2", Poses,Timer.totalRaceTime);
        SaveData();
    }
    public async void SaveData()
    {
        Playerlist.Add(Playerdata);
        var playerData = new Dictionary<string, object>{
          {""+seed.seed, Playerlist},
          {"secondKeyName", 123}
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        Debug.Log($"Saved data {string.Join(',', playerData)}");
    }
    public async void LoadData()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {""+seed.seed,
          "firstKeyName", "secondKeyName"
        });
        if (playerData.TryGetValue(""+seed.seed, out var Listplayer))
        {
            Listplayerdata = Listplayer.Value.GetAs<List<RaceData>>();
            Goto = Listplayerdata[0].poses;
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
    public float TimerTrack;
    public RaceData(string _name, List<Pose> posess,float timer)
    {
        name = _name;
        poses = posess;
        TimerTrack= timer;
    }
}
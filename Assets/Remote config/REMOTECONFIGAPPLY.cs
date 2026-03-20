using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.RemoteConfig;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.UI;
public class REMOTECONFIGAPPLY : MonoBehaviour
{
    public static REMOTECONFIGAPPLY Instance {get; private set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void Update()
    {
        
    }
    async Task InializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
	void Awake()
	{
	if (!Instance == null)
        {
            Instance = this ;
        }
        else
        {
            Destroy(gameObject);
        }
	}
    public struct userAttributes
    {
        public int speed;
    }
    async void Start()
    {
        await InializeRemoteConfigAsync();
        userAttributes uastruct= new userAttributes();
        uastruct.speed=20;
       
    }
}

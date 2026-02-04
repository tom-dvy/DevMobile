using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{

    [SerializeField] InterstitialAd interstitialAdManager;
    [SerializeField] RewardedAds rewardedAdManager;
    [SerializeField] bool isAdLoading = false;
    void Start()
    {
        Debug.Log("AdsManager Started");
        if(interstitialAdManager == null)
        {
            //Debug.Log("No InterstitialAd.cs");
            interstitialAdManager = gameObject.GetComponent<InterstitialAd>();
        }

        if(rewardedAdManager == null)
        {
            rewardedAdManager = gameObject.GetComponent<RewardedAds>();
        }
    }

    public async void ExampleInterstitialAd()
    {
        if(!isAdLoading)
        {
            isAdLoading =true;
            Debug.Log("Example Timer Started");
            await Awaitable.WaitForSecondsAsync(3f);
            Debug.Log("Example Timer Finished");
            interstitialAdManager.LoadAd();
            interstitialAdManager.ShowAd();
            isAdLoading = false;
        }
    }

    public async void ExampleRewarded()
    {
        Debug.Log("Here in Example");
        if(!isAdLoading)
        {
            Debug.Log("In IT");
            isAdLoading =true;
            Debug.Log("Example Timer Started");
            await Awaitable.WaitForSecondsAsync(3f);
            Debug.Log("Example Timer Finished");
            rewardedAdManager.LoadAd();
            rewardedAdManager.ShowAd();
            isAdLoading = false;
        }
    }
}

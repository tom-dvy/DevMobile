using UnityEngine;
using UnityEngine.Advertisements;

public class Test : MonoBehaviour
{
    AdsManager adsManager;
    async void Start()
    {
        adsManager = AdsManager.instance;
        await Awaitable.WaitForSecondsAsync(5f);
        adsManager.RewardedAd(adsManager.rewardManager.ExampleReward);
    }
}

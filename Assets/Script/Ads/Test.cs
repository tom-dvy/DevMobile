using UnityEngine;
using UnityEngine.Advertisements;

public class Test : MonoBehaviour
{
    AdsManager adsManager;
    async void Start()
    {
        adsManager = AdsManager.instance;
    }

    public void ToggleTest()
    {
        adsManager.RewardedAd(adsManager.rewardManager.ExampleReward);
    }
}

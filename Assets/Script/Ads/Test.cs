using UnityEngine;
using UnityEngine.Advertisements;

public class Test : MonoBehaviour
{
    AdsManager adsManager;
    async void Start()
    {
        adsManager = AdsManager.instance;
    }

    public async void Example()
    {
        await Awaitable.WaitForSecondsAsync(2f);
        adsManager.BannerAd(BannerPosition.TOP_LEFT, 5f);
    }
}

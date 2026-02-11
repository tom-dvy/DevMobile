using UnityEngine;
using UnityEngine.Advertisements;

public class Test : MonoBehaviour
{

    void Start()
    {
        AdsManager adsManager = AdsManager.instance;
        adsManager.BannerAd(BannerPosition.BOTTOM_RIGHT, 10f);
    }
}

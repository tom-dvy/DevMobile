using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;


public class BannerAds : MonoBehaviour
{
    [SerializeField] Button _loadBannerButton;

    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null;

    void Start()
    {
        #if UNITy_IOS
        _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
        #endif

        Advertisement.Banner.SetPosition(_bannerPosition);

        _loadBannerButton.onClick.AddListener(LoadBanner);
        _loadBannerButton.interactable = true;
    }

    public void LoadBanner()
    {
        Debug.Log("LoadBanner Started");
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Debug.Log($"_adUnitId = {_adUnitId} || options = {options}",this);
        Advertisement.Banner.Load(_adUnitId, options);
        //Advertisement.Banner.SetPosition(BannerPosition.CENTER);
        Advertisement.Banner.Show(_adUnitId);
    }

    private void OnBannerLoaded()
    {
        Debug.Log($"Banner Loaded !!");
        
    }

    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
    }

    void OnDestroy()
    {
        _loadBannerButton.onClick.RemoveAllListeners();
    }
}

using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    // Singleton
    public static AdsManager instance {get; private set;}

    [Header("Ads Initialization")]
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    [Header("Banner Ads")]
    //[SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    [SerializeField] string _iOSAdUnitId;
    [SerializeField] string _androidAdUnitId;
    private string _adUnitId;

    void Awake()
    {
        Debug.Log("Awake");
        //Debug 
        if(instance != null)
        {
            Debug.Log("Multiple instances of AdsManager", this);
            Destroy(this);
            return;
        }
        instance = this;
        
    }

    void Start()
    {
        //Initialization must be completed before calling any advertisements !!
        InitializeAds();
    }

    /// <summary>
    /// This function Initialize ads and select the good device ID.
    /// <para> Must be called before calling ads (only needed one time) </para>
    /// </summary>
    void InitializeAds()
    {
        #if UNITY_IOS
        _gameId = _iOSGameId;
        #elif UNITY_ANDROID
        _gameId = _androidGameId;
        #elif UNITY_EDITOR
        _gameId = _androidGameId; //Only for testing the functionality in the Editor
        #endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    async public void BannerAd(BannerPosition bannerPosition, float bannerDurationTime)
    {
        #if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
        #endif

        Advertisement.Banner.SetPosition(bannerPosition);
        LoadBanner();
        await Awaitable.WaitForSecondsAsync(bannerDurationTime);
        Advertisement.Banner.Hide();
    }

    public void LoadBanner()
    {
        //Debug.Log("LoadBanner Started");
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        //Debug.Log($"_adUnitId = {_adUnitId} || options = {options}",this);
        Advertisement.Banner.Load(_adUnitId, options);
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

    public void InterstitialAd()
    {
        #if UNITy_IOS
        _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
        #endif
        LoadAd();
        ShowAd();
    }
    public void RewardedAd()
    {
        #if UNITy_IOS
        _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
        #endif
        LoadAd();
        ShowAd();
    }

    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        Debug.Log("Showing Ad: " + _adUnitId);
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
        Debug.Log("Unity Ads Rewarded Ad Completed");
        // Grant a reward.
        }
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
        // Configure the button to call the ShowAd() method when clicked:
        //_showAdButton.onClick.AddListener(ShowAd);
        // Enable the button for users to click:
        //_showAdButton.interactable = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }

}

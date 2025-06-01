using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

public class AppOpenAdsManager : MonoBehaviour
{
    public static AppOpenAdsManager Instance;

    /// <summary>
    /// Key test: AD_UNIT_ID = "ca-app-pub-3940256099942544/3419835294";
    /// </summary>
#if UNITY_ANDROID
    private const string AD_UNIT_ID = "ca-app-pub-9819920607806935/2527922612";
#elif UNITY_IOS
    private const string AD_UNIT_ID = "ca-app-pub-/";
#else
    private const string AD_UNIT_ID = "ca-app-pub-/";
#endif

    private AppOpenAd ad;
    private bool isShowingAd = false;
    private DateTime switchAppsTime;

    ////Kiểm tra việc loại bỏ show AOA
    ////Nếu quá thời gian hoặc đã vào game
    private bool dropAOA;

    public bool isBackFillInter;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void Setup()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            Debug.LogWarning("AOA_Initialize()");
            if (AdsManager.Ins.IsAOA) LoadAd();
            if (isBackFillInter) AdmobMediationManager.ins.OnStartHandle();
        });
        // MobileAds.RaiseAdEventsOnUnityMainThread = true;
    }

    private Coroutine i_ShowAOA;

    public void ShowAOA()
    {
        if (!AdsManager.Ins.IsAOA) return;
        if (AdsManager.isNoAds || AdsManager.Ins.isMkt) return;
        if (i_ShowAOA != null) StopCoroutine(i_ShowAOA);
        i_ShowAOA = StartCoroutine(ie_ShowAOA());
    }

    public void DropShowAOA()
    {
        if (i_ShowAOA != null) StopCoroutine(i_ShowAOA);
        dropAOA = true;
    }

    IEnumerator ie_ShowAOA()
    {
        Debug.LogWarning("Waiting aoa loaded");
        LoadAd();
        dropAOA = false;
        var waiting = false;
        Timer.Schedule(this, AdsManager.Ins.AOAWaitTime, () => { waiting = true; });
        yield return new WaitUntil(() => IsAdAvailable || waiting);
        Debug.LogWarning("check drop aoa: " + dropAOA);
        if (!dropAOA) ShowAdIfAvailable();
    }

    public bool IsAdAvailable
    {
        get { return ad != null; }
    }

    private bool _isLoading;
    private int _timeLoad;

    private void LoadAd()
    {
        //đã có ads rồi hoặc đang load rồi thì thôi load
        Debug.LogWarning("Check AoA loaded: " + IsAdAvailable + " " + _isLoading);
        if (IsAdAvailable || _isLoading) return;
        _isLoading = true;
        Debug.LogWarning("AOA_LoadAd()");

        AdRequest request = new AdRequest();
        // Load an app open ad for portrait orientation
        AppOpenAd.Load(AD_UNIT_ID, request, ((appOpenAd, error) =>
        {
            _isLoading = false;
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error);
                _timeLoad++;
                double retryDelay = Math.Pow(2, Math.Min(4, _timeLoad));
                Invoke("LoadAd", (float)retryDelay);
                return;
            }

            // App open ad is loaded.
            _timeLoad = 0;
            ad = appOpenAd;
            Debug.LogWarning("AOA_ Load AOA thanh cong");
        }));
    }

    private void ShowAdIfAvailable()
    {
        Debug.LogWarning("AOA_ ShowAdIfAvailable()");
        if (Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.OSXEditor)
        {
            return;
        }

        Debug.LogWarning("AOA_Thoi gian_" + !IsAdAvailable
                                          + "_" + isShowingAd
                                          + "_" + AdsManager.Ins._timeWatchAds
                                          + "_" + AdsManager.Ins._timeWatchAdsInter
                                          + "_" + MaxManager.Ins.showingVideoAds);

        if (!IsAdAvailable
            || AdsManager.Ins._timeWatchAds < 15
            || AdsManager.Ins._timeWatchAdsInter < 15
            || isShowingAd
            || MaxManager.Ins.showingVideoAds)
        {
            Debug.LogWarning("AOA_Loi thoi gian capping");
            return;
        }

        //Ko show AOA quá gần nhau
        //if (GameManager.ins.timeAOA < 15) return;
        ad.OnAdFullScreenContentClosed += HandleAdDidDismissFullScreenContent;
        ad.OnAdFullScreenContentFailed += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdFullScreenContentOpened += HandleAdDidPresentFullScreenContent;
        ad.OnAdImpressionRecorded += HandleAdDidRecordImpression;
        ad.OnAdPaid += HandlePaidEvent;

        // Debug.LogWarning("AOA_ ShowAdIfAvailable() thanh cong");
        // AdsManager.Ins.UnlockControl();
        isShowingAd = true;
        ad.Show();
    }

    private void OnApplicationPause(bool paused)
    {
#if UNITY_EDITOR
        return;
#endif

        // Hiển thị AOA nếu back ra ngoài rồi vào lại > time capping
        if (paused)
        {
            //Lưu lại thời điểm User back ra ngoài
            Debug.LogWarning("AOA_Back ra ngoài");
            switchAppsTime = DateTime.Now;
        }
        else
        {
            //Khi vào lại game
            //Debug.LogWarning("AOA_Khi vào lại game");

            Debug.LogWarning(!MaxManager.Ins.showingVideoAds + " "
                                                             + AdsManager.Ins.AOA_SwitchApps + " "
                                                             + (DateTime.Now - switchAppsTime).TotalSeconds + " "
                                                             + AdsManager.Ins.AOA_SwitchApps_Seconds);

            if (!MaxManager.Ins.showingVideoAds
                && AdsManager.Ins.AOA_SwitchApps
                && (DateTime.Now - switchAppsTime).TotalSeconds >= AdsManager.Ins.AOA_SwitchApps_Seconds)
            {
                Debug.LogWarning("AOA_Bat dau show SwitchApps");
                ShowAOA();
            }

            //Nếu đang dùng MAX
            if (MaxManager.Ins != null && MaxManager.Ins.showingVideoAds)
            {
                MaxManager.Ins.showingVideoAds = false; //Vừa xem Ads xong
                Debug.LogWarning("AOA_Vừa xem Ads xong");
            }

            /*
             * Test thử việc dùng inter thay cho aoa lúc switch app
             */
            //AdsManager.Ins.ShowInterstitial();
        }
    }

    private void HandleAdDidDismissFullScreenContent()
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        LoadAd();
        Timer.Schedule(this, 3f, () => { isShowingAd = false; });
        AdsManager.Ins.UnlockControl();
    }

    private void HandleAdFailedToPresentFullScreenContent(AdError adError)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", adError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        LoadAd();
        isShowingAd = false;
        AdsManager.Ins.UnlockControl();
    }

    private void HandleAdDidPresentFullScreenContent()
    {
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
        AdsManager.Ins.UnlockControl();
    }

    private void HandleAdDidRecordImpression()
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(AdValue adValue)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
            adValue.CurrencyCode, adValue.Value);
    }

    ///////////////// SHOW COLLAPSIBLE BANNER /////////////////////////

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9819920607806935/4317489039";
    // private string _adUnitId = "ca-app-pub-3940256099942544/2014213617"; // key tét 
    // private string _adUnitId = "ca-app-pub-9819920607806935/9129409194"; // key tét 
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    private string _adUnitId = "unused";
#endif

    public BannerView _bannerView;

    private bool isBannerCollapsible;
    private bool isDoneCollaps;
    private bool isLoadFail;

    private int loadFailNum;

    // private void Update()
    // {
    //     Debug.Log("FUXCK " + (_bannerView == null));
    // }

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView(bool isCollapsible)
    {
        Debug.Log("Creating banner view " + isCollapsible);

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        // Create a 320x50 banner at top of the screen
        // _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
        // _bannerView = new BannerView(_adUnitId, isCollapsible ? AdSize.Banner : AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(Screen.width), AdPosition.Bottom);
        _bannerView = new BannerView(_adUnitId, AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);

        // if (isCollapsible) 
        ListenToAdEventsCollapsible();
        // else ListenToAdEvents();
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadCBAd(bool isCollapsible = true)
    {
        // create an instance of a banner view first.
        if (_bannerView == null /*|| (isBannerCollapsible != isCollapsible)*/)
        {
            CreateBannerView(isCollapsible);
        }

        isBannerCollapsible = isCollapsible;

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        if (isCollapsible)
        {
            adRequest.Extras.Add("collapsible", "bottom");
            adRequest.Extras.Add("collapsible_request_id", System.Guid.NewGuid().ToString());
        }

        // send the request to load the ad.
        Debug.Log("Loading banner ad. " + isCollapsible);
        _bannerView.LoadAd(adRequest);
    }

    public void ShowBanner(bool isOn)
    {
        if (isOn) _bannerView.Show();
        else _bannerView.Hide();
    }

    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    // private void ListenToAdEvents()
    // {
    //     // Raised when an ad is loaded into the banner view.
    //     _bannerView.OnBannerAdLoaded += () =>
    //     {
    //         Debug.Log("Banner view loaded an ad with response : "
    //                   + _bannerView.GetResponseInfo());
    //         _bannerView.Show();
    //     };
    //     // Raised when an ad fails to load into the banner view.
    //     _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
    //     {
    //         if (_bannerView == null) return;
    //         Timer.ScheduleSupreme(1, () => LoadCBAd());
    //         Debug.LogError("Banner view failed to load an ad with error : "
    //                        + error);
    //     };
    //     // Raised when the ad is estimated to have earned money.
    //     _bannerView.OnAdPaid += (AdValue adValue) =>
    //     {
    //         Debug.Log(String.Format("Banner view paid {0} {1}.",
    //             adValue.Value,
    //             adValue.CurrencyCode));
    //     };
    //     // Raised when an impression is recorded for an ad.
    //     _bannerView.OnAdImpressionRecorded += () => { Debug.Log("Banner view recorded an impression."); };
    //     // Raised when a click is recorded for an ad.
    //     _bannerView.OnAdClicked += () => { Debug.Log("Banner view was clicked."); };
    //     // Raised when an ad opened full screen content.
    //     _bannerView.OnAdFullScreenContentOpened += () => { Debug.Log("Banner view full screen content opened."); };
    //     // Raised when the ad closed full screen content.
    //     _bannerView.OnAdFullScreenContentClosed += () => { Debug.Log("Banner view full screen content closed."); };
    // }
    // private void ListenToAdEventsCollapsible()
    // {
    //     isDoneCollaps = false;
    //     isLoadFail = true;
    //     // Raised when an ad is loaded into the banner view.
    //     _bannerView.OnBannerAdLoaded += () =>
    //     {
    //         Debug.Log("Collaps Banner view loaded an ad with response : "
    //                   + _bannerView.GetResponseInfo());
    //         _bannerView.Show();
    //         // AdsManager.Ins.SetBlockPanel(true);
    //         isLoadFail = false;
    //         loadFailNum = 0;
    //     };
    //     // Raised when an ad fails to load into the banner view.
    //     _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
    //     {
    //         if (_bannerView == null) return; // ???????????????????????????????????????????????????????? why?
    //         loadFailNum++;
    //         Timer.ScheduleSupreme(Math.Clamp(1 + loadFailNum, 0, 64), () =>
    //         {
    //             if (isLoadFail) LoadCBAd();
    //         });
    //         isLoadFail = true;
    //         Debug.LogError("Collaps Banner view failed to load an ad with error : "
    //                        + error);
    //     };
    //     // Raised when the ad is estimated to have earned money.
    //     _bannerView.OnAdPaid += (AdValue adValue) =>
    //     {
    //         Debug.Log(String.Format("Collaps Banner view paid {0} {1}.",
    //             adValue.Value,
    //             adValue.CurrencyCode));
    //     };
    //     // Raised when an impression is recorded for an ad.
    //     _bannerView.OnAdImpressionRecorded += () => { Debug.Log("Collaps Banner view recorded an impression."); };
    //     // Raised when a click is recorded for an ad.
    //     _bannerView.OnAdClicked += () => { Debug.Log("Collaps Banner view was clicked."); };
    //     // Raised when an ad opened full screen content.
    //     _bannerView.OnAdFullScreenContentOpened += () => { Debug.Log("Collaps Banner view full screen content opened."); };
    //     // Raised when the ad closed full screen content.
    //     _bannerView.OnAdFullScreenContentClosed += () =>
    //     {
    //         isDoneCollaps = true;
    //         AdsManager.Ins.SetBlockPanel(false);
    //         Debug.Log("Collaps Banner view full screen content closed.");
    //     };
    // }
    private void ListenToAdEventsCollapsible()
    {
        isDoneCollaps = false;
        isLoadFail = true;
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Collaps Banner view loaded an ad with response : "
                      + _bannerView.GetResponseInfo());
            _bannerView.Show();
            // AdsManager.Ins.SetBlockPanel(true);
            isLoadFail = false;
            loadFailNum = 0;

            AdsManager.Ins.HideBanner();
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            if (_bannerView == null) return; // ???????????????????????????????????????????????????????? why?
            AdsManager.Ins.ShowBanner();
            isLoadFail = true;
            Debug.LogError("Collaps Banner view failed to load an ad with error : "
                           + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Collaps Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () => { Debug.Log("Collaps Banner view recorded an impression."); };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () => { Debug.Log("Collaps Banner view was clicked."); };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () => { Debug.Log("Collaps Banner view full screen content opened."); };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            isDoneCollaps = true;
            Debug.Log("Collaps Banner view full screen content closed.");
        };
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void ShowNormAdsIfNotCollaps() // hit on play
    {
        // // if (isBannerCollapsible)
        // // {
        // // if (!isDoneCollaps || isLoadFail) LoadCBAd();
        // AdsManager.Ins.SetBlockPanel(false);
        // DestroyBannerView();
        // LoadCBAd();
        // // }
    }
}
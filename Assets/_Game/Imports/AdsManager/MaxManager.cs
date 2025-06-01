using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaxManager : MonoBehaviour
{
    public static MaxManager Ins;

    public Color bannerColor;

    private const string MaxSdkKey = "ZoNyqu_piUmpl33-qkoIfRp6MTZGW9M5xk1mb1ZIWK6FN9EBu0TXSHeprC3LMPQI7S3kTc1-x7DJGSV8S-gvFJ";

    [Header("Android ID")] private const string InterstitialAdUnitId = "bbb61d4c15ad690a";
    private const string RewardedAdUnitId = "bb2f40c4ece963d6";
    private const string BannerAdUnitId = "8277058b15a9eced";
    private const string mrecAdUnitId = "....";

    [Header("IOS ID")]
    // private const string mrecAdUnitId = "fc37d9b8b815b315";
    private const string InterstitialAdUnitId_IOS = "";

    private const string RewardedAdUnitId_IOS = "";
    private const string BannerAdUnitId_IOS = "";


    [Header("Action Event")] public Action OnRewardAds_Finish, OnRewardAds_Fail;
    public Action OnInter_Finish;

    [Header("Status")] [HideInInspector] public bool isBannerShowing;
    [HideInInspector] public bool isMRecShowing;
    [HideInInspector] public bool isVideoLoaded;

    public bool recieveReward = false;
    public bool showingVideoAds = false;
    private const bool isIOS = false;
    public bool isShowMRECsAds = false;

    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;

    #region SETUP

    private void Awake()
    {
        if (Ins != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Ins = this;
        DontDestroyOnLoad(this.gameObject);

#if UNITY_IOS || UNITY_IPHONE
        isIOS = true;
#endif
    }

    public void Setup(Timer.Task task)
    {
        // Time.timeScale = 0;
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
            /*
            InitializeMRecAds();
            */

            /*
            MaxSdk.ShowMediationDebugger();
            */
            task?.Invoke();
            Debug.Log("MAX SDK Initialized");

            // Time.timeScale = 1;
        };

        MaxSdk.SetUserId(AppsFlyer.getAppsFlyerId());
        /*
        MaxSdk.SetSdkKey(MaxSdkKey);
        */
        /*
        MaxSdk.SetCreativeDebuggerEnabled(AdsManager.Ins.IsCreativeDebug || AdsManager.Ins.isForceMaxCreativeDebug);
        */
        MaxSdk.InitializeSdk();
        // MaxSdk.SetCreativeDebuggerEnabled(false);
        //
        // var cmpService = MaxSdk.CmpService;
        //
        // cmpService.ShowCmpForExistingUser(error =>
        // {
        //     if (null == error)
        //     {
        //         // The CMP alert was shown successfully.
        //     }
        // });
    }

    #endregion

    #region Interstitial Ad Methods

    private void InitializeInterstitialAds()
    {
        // Attach callbacks

        //Load inter ads thành công
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;

        //Load inter ads fail
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;

        //Hiển thị fail
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialFailedToDisplayEvent;

        //Ấn nút tắt ads
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;

        //Click vào ads
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;

        //Giá trị của ads
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

        //Ads hiển thị thành công
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;

        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += this.OnAdRevenuePaidEvent;


        // Load the first interstitial
        LoadInterstitial();
    }

    public void LoadInterstitial()
    {
        if (AdsManager.isNoAds) return;
        /*
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        */

        //Cái này mới thêm, cần test lại đã
        if (IsLoadInterstitial()) return;
        MaxSdk.LoadInterstitial(isIOS ? InterstitialAdUnitId_IOS : InterstitialAdUnitId);
    }

    public void ShowInterstitial(string placement = "", Action OnFinish = null)
    {
        try
        {
            //Debug.LogWarning("call inter: " + (AdsManager.Ins._timeWatchAds < AdsManager.Ins.timeWatchAds) + " " + (AdsManager.Ins._timeWatchAdsInter < AdsManager.Ins.timeWatchAdsInter));
            if (AdsManager.isNoAds
                || AdsManager.Ins._timeWatchAds < AdsManager.Ins.timeWatchAds
                || AdsManager.Ins._timeWatchAdsInter < AdsManager.Ins.timeWatchAdsInter)
            {
                if (OnFinish != null) OnFinish?.Invoke();
                return;
            }

            OnInter_Finish = OnFinish;

            if (MaxSdk.IsInterstitialReady(isIOS ? InterstitialAdUnitId_IOS : InterstitialAdUnitId))
            {
                //Debug.Log("show inter");
                AdsManager.Ins.BlockControl();
                //Log Event
                FirebaseManager.Ins.ads_inter_click();
                AppsflyerEventRegister.af_inters_ad_eligible();
                MaxManager.Ins.showingVideoAds = true;
                Dictionary<string, string> value = new Dictionary<string, string>();
                AdsManager.Ins._timeWatchAdsInter = 0;
                FirebaseManager.Ins.ads_inter_show_placement(placement);
                MaxSdk.ShowInterstitial(isIOS ? InterstitialAdUnitId_IOS : InterstitialAdUnitId, placement);
            }
            else if (AdmobMediationManager.ins.ShowInterstitialAd())
            {
                FirebaseManager.Ins.ads_inter_missing(placement);
                //Debug.LogWarning("Lỗi chưa load đc Inter");
                LoadInterstitial();
            }
            else
            {
                FirebaseManager.Ins.ads_inter_missing(placement);
                //Debug.LogWarning("Lỗi chưa load đc Inter");
                LoadInterstitial();
                if (OnFinish != null) OnInter_Finish?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Lỗi Inter: " + e);
            if (OnFinish != null) OnInter_Finish?.Invoke();
        }
    }

    public bool IsLoadInterstitial()
    {
        return MaxSdk.IsInterstitialReady(isIOS ? InterstitialAdUnitId_IOS : InterstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Reset retry attempt
        interstitialRetryAttempt = 0;
        Debug.Log("Interstitial loaded");

        //Log Event
        FirebaseManager.Ins.ads_inter_load();
        AppsflyerEventRegister.af_inters_api_called();
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, interstitialRetryAttempt));
        Invoke("LoadInterstitial", (float)retryDelay);
        Debug.Log("Interstitial failed to load with error code: " + errorInfo.ToString());

        AdsManager.Ins.UnlockControl();

        //Log Event
        FirebaseManager.Ins.ads_inter_fail("Load Fail: " + errorInfo.ToString());
    }

    private void OnInterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        //DebugCustom.Log("Interstitial failed to display with error code: " + errorCode);
        LoadInterstitial();
        if (OnInter_Finish != null) OnInter_Finish?.Invoke();
        Debug.Log("Interstitial failed to display with error code: " + errorInfo.ToString());

        AdsManager.Ins.UnlockControl();
        MaxManager.Ins.showingVideoAds = false;
        //Log Event
        FirebaseManager.Ins.ads_inter_fail("Display Fail: " + errorInfo.ToString());
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        AdsManager.Ins.UnlockControl();

        // Interstitial ad is hidden. Pre-load the next ad
        AdsManager.Ins._timeWatchAdsInter = 0;
        LoadInterstitial();
        if (OnInter_Finish != null) OnInter_Finish?.Invoke();
        Debug.Log("Interstitial dismissed");
        MaxManager.Ins.showingVideoAds = false;
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Reset retry attempt
        interstitialRetryAttempt = 0;
        Debug.Log("Interstitial Displayed");

        AdsManager.Ins.UnlockControl();

        //Log Event
        FirebaseManager.Ins.ads_inter_show(AdsManager.Ins.InterPlacement);
        AppsflyerEventRegister.af_inters_displayed();

        BuyingPackDataFragment.SetInterFlag();

        FirebaseManager.Ins.CheckInterWatched();
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Interstitial revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "interstitial";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.Ins.ADS_RevenuePain(data);
    }

    #endregion

    #region Rewarded Ad Methods

    private string _rewardPlacement;

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        //Reward ads load thành công
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;

        //Reward ads Load thất bại
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;

        //Reward ads show thất bại
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;

        //Reward ads Show thành công
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;

        //Click vào Reward ads 
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;

        //Tắt ads
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;

        //Phần thưởng có thể nhận được (nên dùng event tắt ads)
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        //Doanh thu
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;


        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += this.OnAdRevenuePaidEvent;


        // Load the first RewardedAd
        LoadRewardedAd();
    }

    public bool isRewardedVideoAvailable()
    {
        return MaxSdk.IsRewardedAdReady(isIOS ? RewardedAdUnitId_IOS : RewardedAdUnitId);
    }

    public void LoadRewardedAd()
    {
        /*
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        */

        //Có sẵn r thì ko cần load
        //Phòng trường hợp load liên tục
        if (isRewardedVideoAvailable())
        {
            Debug.LogWarning("Ads Reward already loaded");
            return;
        }

        isVideoLoaded = false;
        MaxSdk.LoadRewardedAd(isIOS ? RewardedAdUnitId_IOS : RewardedAdUnitId);
    }

    public void ShowRewardedAd(string placement = "", Action OnFinish = null, Action OnFail = null)
    {
        Debug.Log("Call reward");
        this._rewardPlacement = placement;
        try
        {
            // if (Application.isEditor)
            // {
            //     if(OnFinish != null) OnFinish?.Invoke();
            //     return true;
            // }

            OnRewardAds_Finish = OnFinish;
            OnRewardAds_Fail = OnFail;

            recieveReward = false;

            if (MaxSdk.IsRewardedAdReady(isIOS ? RewardedAdUnitId_IOS : RewardedAdUnitId))
            {
                Debug.Log("Reward show done");
                AdsManager.Ins.BlockControl();

                //Event Log
                FirebaseManager.Ins.ads_reward_click(_rewardPlacement);
                AppsflyerEventRegister.af_rewarded_ad_eligible();

                showingVideoAds = true;
                Dictionary<string, string> value = new Dictionary<string, string>();
                MaxSdk.ShowRewardedAd(isIOS ? RewardedAdUnitId_IOS : RewardedAdUnitId, placement);
            }
            else if (AdmobMediationManager.ins.ShowRewardedAd())
            {
                AdsManager.Ins.UnlockControl();

                Debug.LogWarning("Lỗi chưa load đc Video Ads");
                LoadRewardedAd();
            }
            else
            {
                AdsManager.Ins.UnlockControl();

                Debug.LogWarning("Lỗi chưa load đc Video Ads");
                LoadRewardedAd();
                if (OnRewardAds_Fail != null) OnRewardAds_Fail?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Lỗi VideoAds: " + e);
            OnRewardAds_Fail?.Invoke();
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        Debug.Log("Rewarded ad loaded");
        isVideoLoaded = true;
        // Reset retry attempt
        rewardedRetryAttempt = 0;

        //Log Event
        AppsflyerEventRegister.af_rewarded_api_called();
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isVideoLoaded = false;
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, rewardedRetryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
        Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.ToString());

        AdsManager.Ins.UnlockControl();

        //Event Log
        FirebaseManager.Ins.ads_reward_fail(_rewardPlacement, "Load Fail: " + errorInfo.ToString());
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        LoadRewardedAd();
        OnRewardAds_Fail?.Invoke();
        OnRewardAds_Fail = null;
        showingVideoAds = false;
        Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.ToString());

        AdsManager.Ins.UnlockControl();

        //Event Log
        FirebaseManager.Ins.ads_reward_fail(_rewardPlacement, "Display Fail: " + errorInfo.ToString());
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        recieveReward = false;
        Debug.Log("Rewarded ad displayed");

        AdsManager.Ins.UnlockControl();

        //Event Log
        FirebaseManager.Ins.ads_reward_show(_rewardPlacement);
        AppsflyerEventRegister.af_rewarded_displayed();

        GrandManager.ins.TriggerPenalty();

        FirebaseManager.Ins.CheckRewardWatched();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        AdsManager.Ins.UnlockControl();

        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
        if (recieveReward)
        {
            recieveReward = false;
            AdsManager.Ins._timeWatchAds = 0;
            showingVideoAds = false;
            OnRewardAds_Finish?.Invoke();
            OnRewardAds_Finish = null;
        }
        else
        {
            OnRewardAds_Fail?.Invoke();
            OnRewardAds_Fail = null;
            AdsManager.Ins._timeWatchAds = 0;
            showingVideoAds = false;
        }

        Debug.Log("Rewarded ad dismissed");
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward
        recieveReward = true;
        Debug.Log("Rewarded ad received reward");

        AdsManager.Ins.UnlockControl();

        //Event Log
        FirebaseManager.Ins.ads_reward_complete(_rewardPlacement, LevelDataFragment.cur.GetFireBaseLevel().ToString());
        AppsflyerEventRegister.af_rewarded_ad_completed();
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad revenue paid. Use this callback to track user revenue.


        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "video_reward";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.Ins.ADS_RevenuePain(data);
        Debug.Log("Rewarded ad revenue paid");
    }

    #endregion

    #region Banner Ad Methods

    private void InitializeBannerAds()
    {
        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        // if (MaxSdkUtils.IsTablet() || ((float)Screen.height / Screen.width) < 1921f / 1080)
        // {
        //     MaxSdk.SetBannerExtraParameter(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId, "adaptive_banner", "false");
        //     MaxSdk.SetBannerWidth(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId, 742);
        // }

        MaxSdk.SetBannerPlacement(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId, "tan cung noi dau");
        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId, bannerColor);
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += (s, info) => { CanvasBannerOff.SetBannerOffButton(MaxSdk.GetBannerLayout(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId).height, MaxSdkUtils.GetScreenDensity()); };
    }

    public void ShowBanner()
    {
#if UNITY_EDITOR
        return;
#endif
        if (AdsManager.isNoAds || isBannerShowing) return;
        MaxSdk.ShowBanner(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId);
        isBannerShowing = true;
    }

    public void HideBanner()
    {
        if (!isBannerShowing) return;
        MaxSdk.HideBanner(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId);
        isBannerShowing = false;
    }

    #endregion

    #region MRECs Ad method

    public void InitializeMRecAds()
    {
        return;
        Debug.Log("vao day r");
        // MRECs are sized to 300x250 on phones and tablets
        MaxSdk.CreateMRec(mrecAdUnitId, MaxSdkBase.AdViewPosition.Centered);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += this.OnAdRevenuePaidEvent;

        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
        MaxSdk.LoadMRec(mrecAdUnitId);
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Loaded mrec");
    }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error)
    {
        Debug.Log("Load mrec fail " + error.Message + "_" + error.MediatedNetworkErrorMessage);
        MaxSdk.LoadMRec(mrecAdUnitId);
    }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad revenue paid. Use this callback to track user revenue.


        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "Banner";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;


        Debug.LogWarning("Banner ad revenue paid");
    }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    public void ShowMRECsAds(MaxSdkBase.AdViewPosition pos = MaxSdkBase.AdViewPosition.Centered)
    {
        return;
        if (isShowMRECsAds) return;
        try
        {
            MaxSdk.UpdateMRecPosition(mrecAdUnitId, pos);
            //if (Application.isEditor) return;
            //if (GameManager.ins.isRemoveAllAds) return;
            Debug.Log("Show mrec");
            MaxSdk.ShowMRec(mrecAdUnitId);
            isShowMRECsAds = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi ShowMRCsAds:" + ex);
        }
    }

    public void HideMRECsAds()
    {
        return;
        if (!isShowMRECsAds) return;
        try
        {
            MaxSdk.HideMRec(mrecAdUnitId);
            isShowMRECsAds = false;
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi HideMRCsAds:" + ex);
        }
    }

    public void ReShowBanner()
    {
        this.HideMRECsAds();
        this.ShowBanner();
    }

    #endregion


    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue1 = adInfo.Revenue;
        var impressionParameters = new[]
        {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue1),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression_abi", impressionParameters);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
}

[System.Serializable]
public class ImpressionData
{
    public string CountryCode;
    public string NetworkName;
    public string AdUnitIdentifier;
    public string Placement;
    public double Revenue;
    public string AdFormat;
}
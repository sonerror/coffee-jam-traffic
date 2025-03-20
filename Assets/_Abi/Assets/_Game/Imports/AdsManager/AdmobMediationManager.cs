using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using AppsFlyerSDK;
using GoogleMobileAds.Api.Mediation;


public class AdmobMediationManager : MonoBehaviour
{
    public static AdmobMediationManager ins;

    private void Awake()
    {
        ins = this;
        DontDestroyOnLoad(gameObject);
    }

    //newkey
    string interID = "ca-app-pub-9819920607806935/6100002009";
    private string rewardID = "ca-app-pub-9819920607806935/1096555944";

    private Coroutine interCor;
    private Coroutine rewardCor;

    //string interID = "ca-app-pub-3940256099942544/1033173712";
    //string interID1 = "ca-app-pub-3940256099942544/1033173712";
    //string interID2 = "ca-app-pub-3940256099942544/1033173712";

    public void OnStartHandle()
    {
        Debug.Log("Init BackHill");
        isAdShowed = true;
        isRewardShowed = true;
        LoadInterstitialAd();
        LoadRewardedAd();
    }

    private InterstitialAd _interstitialAd;
    private bool isAdShowed;
    private int interstitialRetryAttempt = 2;

    public void LoadInterstitialAd()
    {
        if (AdsManager.isNoAds) return;
        if (Application.internetReachability == NetworkReachability.NotReachable) return;

        // Clean up the old ad before loading a new one.
        // Debug.Log("Check " + (_interstitialAd != null) + "  " + isAdShowed);
        if (!isAdShowed) return;
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(interID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
                    interstitialRetryAttempt++;
                    // Invoke(nameof(LoadInterstitialAd), (float)retryDelay);
                    if (interCor != null) Timer.StopTimerSupreme(interCor);
                    interCor = Timer.ScheduleSupreme((float)retryDelay, LoadInterstitialAd);
                    // AdsManager.Ins.UnlockControl();

                    //Log Event
                    FirebaseManager.Ins.ads_inter_fail("Load Fail: " + error);
                    return;
                }

                interstitialRetryAttempt = 2;

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());
                FirebaseManager.Ins.ads_inter_load();
                AppsflyerEventRegister.af_inters_api_called();

                isAdShowed = false;
                _interstitialAd = ad;
                RegisterEventHandlers(ad);
            });
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public bool ShowInterstitialAd() //return if ads show
    {
        if (!AppOpenAdsManager.Instance.isBackFillInter) return false;
        if (AdsManager.isNoAds) return false;
        if (Application.internetReachability == NetworkReachability.NotReachable) return false;
        if (!isAdShowed && _interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad back fill.");
            isAdShowed = true;
            _interstitialAd.Show();

            AdsManager.Ins.BlockControl();
            //Log Event
            FirebaseManager.Ins.ads_inter_click();
            AppsflyerEventRegister.af_inters_ad_eligible();
            MaxManager.Ins.showingVideoAds = true;
            AdsManager.Ins._timeWatchAdsInter = 0;
            return true;
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
            LoadInterstitialAd();
            return false;
        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            // Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
            //     adValue.Value,
            //     adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () => { Debug.Log("Interstitial ad recorded an impression."); };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () => { Debug.Log("Interstitial ad was clicked."); };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");

            Debug.Log("Interstitial Displayed");

            AdsManager.Ins.UnlockControl();

            //Log Event
            FirebaseManager.Ins.ads_inter_show();
            AppsflyerEventRegister.af_inters_displayed();
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            MaxManager.Ins.showingVideoAds = false;
            AdsManager.Ins.UnlockControl();

            // Interstitial ad is hidden. Pre-load the next ad
            AdsManager.Ins._timeWatchAdsInter = 0;
            LoadInterstitialAd();
            if (MaxManager.Ins.OnInter_Finish != null) Timer.ScheduleFrame(() => MaxManager.Ins.OnInter_Finish?.Invoke());
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            MaxManager.Ins.showingVideoAds = false;
            LoadInterstitialAd();
            if (MaxManager.Ins.OnInter_Finish != null) Timer.ScheduleFrame(() => MaxManager.Ins.OnInter_Finish?.Invoke());
            Debug.Log("Interstitial failed to display with error code: " + error);

            AdsManager.Ins.UnlockControl();

            //Log Event
            FirebaseManager.Ins.ads_inter_fail("Display Fail: " + error);
        };
    }

    /////////////////////////////// reward //////////////////////////////////////////////

    private RewardedAd rewardedAd;
    private int rewardedRetryAttempt = 2;
    private bool isRewardShowed;

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadRewardedAd()
    {
        if (AdsManager.isNoAds) return;
        if (Application.internetReachability == NetworkReachability.NotReachable) return;

        if (!isRewardShowed) return;
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            if (rewardedAd.CanShowAd()) return;

            Debug.Log("BACK FILL RW DESTROY");
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the BACK FILL rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(rewardID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an BACK FILL RW ad " +
                                   "with error : " + error);

                    double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
                    rewardedRetryAttempt++;
                    // Invoke(nameof(LoadRewardedAd), (float)retryDelay);
                    if (rewardCor != null) Timer.StopTimerSupreme(rewardCor);
                    rewardCor = Timer.ScheduleSupreme((float)retryDelay, LoadRewardedAd);

                    return;
                }

                rewardedRetryAttempt = 2;

                Debug.Log("BACK FILL Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                isRewardShowed = false;
                rewardedAd = ad;
                RegisterEventHandlers(ad);
            });
    }

    public bool ShowRewardedAd()
    {
        if (!AppOpenAdsManager.Instance.isBackFillInter) return false;
        if (Application.internetReachability == NetworkReachability.NotReachable) return false;

        if (!isRewardShowed && rewardedAd != null && rewardedAd.CanShowAd())
        {
            isRewardShowed = true;
            Debug.Log("SHOW BACK FILL RW ADS");
            rewardedAd.Show((Reward reward) => { MaxManager.Ins.recieveReward = true; });

            return true;
        }
        else
        {
            Debug.Log("CANT SHOW REWARD ADS NOW");
            // LoadRewardedAd();
            return false;
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("BACK FILL Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () => { Debug.Log("BACK FILL Rewarded ad recorded an impression."); };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () => { Debug.Log("BACK FILL Rewarded ad was clicked."); };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("BACK FILL Rewarded ad full screen content opened.");
            MaxManager.Ins.recieveReward = false;
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("BACK FILL Rewarded ad full screen content closed.");
            LoadRewardedAd();

            if (MaxManager.Ins.recieveReward)
            {
                MaxManager.Ins.recieveReward = false;
                AdsManager.Ins._timeWatchAds = 0;
                MaxManager.Ins.showingVideoAds = false;
                Timer.ScheduleFrame(() =>
                {
                    MaxManager.Ins.OnRewardAds_Finish?.Invoke();
                    MaxManager.Ins.OnRewardAds_Finish = null;
                });
            }
            else
            {
                Timer.ScheduleFrame(() =>
                {
                    MaxManager.Ins.OnRewardAds_Fail.Invoke();
                    MaxManager.Ins.OnRewardAds_Fail = null;
                });
                AdsManager.Ins._timeWatchAds = 0;
                MaxManager.Ins.showingVideoAds = false;
            }
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("BACK FILL Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            Timer.ScheduleFrame(() =>
            {
                MaxManager.Ins.OnRewardAds_Fail?.Invoke();
                MaxManager.Ins.OnRewardAds_Fail = null;
            });
            LoadRewardedAd();
        };
    }
}
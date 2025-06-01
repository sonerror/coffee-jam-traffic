using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;
using GoogleMobileAds.Ump.Api;
using UnityEngine.Serialization;

public class AdsManager : MonoBehaviour
{
    #region awake

    public static AdsManager Ins;

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

    #endregion

    [Header("[Other Setup]")]
    //Data save/load
    public static bool isNoAds;

    public const string NO_ADS_KEY = "NoAds";

    public int timeInstall;
    public int timeLastOpen;
    public int daysPlayed;
    public int dayLastOpen;

    public bool isMkt;
    public bool canShowAds;
    public bool isCheckTablet;
    public bool isForceMaxCreativeDebug;

    [Header("[Config Capping Time]")] [Tooltip("Thời gian capping sau khi xem 1 reward")]
    public float timeWatchAds = 15f;

    [Tooltip("Thời gian capping sau khi xem 1 inter")]
    public float timeWatchAdsInter = 15f;

    [Header("[Config AOA]")] [Tooltip("Thời gian đợi aoa load nếu lâu hơn thì bỏ đi")]
    public float AOAWaitTime = 5f;


    [Header("[CDPR_CCPA]")] public bool isCDPR_CCPA;

    [Header("Panel Block ADS")] public GameObject panelBlockControl;

    //[Firebase Remote Param]

    //Có hiển thị AOA ở lần đầu tiên cài game không
    [HideInInspector] public bool AOA_FirstOpen;

    //Có hiển thị AOA khi mở game không
    [HideInInspector] public bool AOA_SessionStart;

    //Có hiển thị AOA khi switch app không
    [HideInInspector] public bool AOA_SwitchApps;

    //Chuyển sang tab khác rồi vào lại app cần bao lâu thì hiện
    [HideInInspector] public long AOA_SwitchApps_Seconds;

    //Thời gian đếm từ lần hiện ads gần nhất
    [Header("Capping Time")] public float _timeWatchAds;
    public float _timeWatchAdsInter;
    private bool _isAdsSetupDone;
    public bool IsAdsSetupDone => _isAdsSetupDone;
    public bool IsMaxInitialize { get; private set; }

    public bool IsAOA;
    public bool IsCreativeDebug;
    public int Ads_Level = 5;
    public int Ads_Capping = 180;
    public int Ads_Penalty = 45;
    public int Babysit_Level = 4;
    public int TreasureLevel = 15;
    public int RatingLevel = 2;
    public int SwapCarTutLevel = 4;
    public int VipBUsTutLevel = 9;
    public int SwapMinionTutLEvel = 14;
    public int WinStreakLevel = 25;

    [HideInInspector] public string InterPlacement = "";
    [HideInInspector] public string RewardPlacement = "";

    IEnumerator Start()
    {
        _isAdsSetupDone = false;
        canShowAds = true;


        //LOAD/SAVE DATA SYSTEM
        {
            //LOAD
            isNoAds = PlayerPrefs.GetInt("NoAds", 0) == 1;
            timeInstall = PlayerPrefs.GetInt("timeInstall", GameHelper.CurrentTimeInSecond);
            timeLastOpen = PlayerPrefs.GetInt("timeLastOpen", GameHelper.CurrentTimeInSecond);
            dayLastOpen = PlayerPrefs.GetInt("dayLastOpen", GameHelper.GetDayNow);
            daysPlayed = PlayerPrefs.GetInt("daysPlayed", 0);
            if (GameHelper.GetDayNow - dayLastOpen > 0)
            {
                daysPlayed += 1;
            }

            //SAVE
            PlayerPrefs.SetInt("NoAds", isNoAds ? 1 : 0);
            PlayerPrefs.SetInt("timeInstall", timeInstall);
            PlayerPrefs.SetInt("timeLastOpen", timeLastOpen);
            PlayerPrefs.SetInt("dayLastOpen", dayLastOpen);
            PlayerPrefs.SetInt("daysPlayed", daysPlayed);
        }

        AOA_FirstOpen = true;
        AOA_SessionStart = true;

        // LeaderBoardController.cur.CallAutoAuthenticate();

        //if (isCDPR_CCPA) CDPR_CCPA.Ins.Setup();

        //Đợi khởi tạo các đối tượng
        yield return new WaitUntil(() =>
            AppOpenAdsManager.Instance != null
            && MaxManager.Ins != null
            && FirebaseManager.Ins != null);


        //Wait Firebase RemoteConfig done
        var check = false;
        Timer.Schedule(this, 2f, () => { check = true; });

        yield return new WaitUntil(() => FirebaseManager.Ins.is_remote_config_done || check);

        //Default Variables
        _timeWatchAds = timeWatchAds;
        _timeWatchAdsInter = timeWatchAdsInter;

        IsMaxInitialize = false;
        // Create a ConsentRequestParameters object.
        ConsentRequestParameters request = new ConsentRequestParameters();

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);

        // IsMaxInitialize = false;
        // MaxManager.Ins.Setup(() =>
        // {
        //     AppOpenAdsManager.Instance.Setup();
        //     IsMaxInitialize = true;
        // });
        Timer.ScheduleSupreme(10.478f, () => IsMaxInitialize = true);
        yield return new WaitUntil(() => IsMaxInitialize);
        Time.timeScale = 1;

        //Setup AOA Supreme
        if (!PlayerPrefs.HasKey("FirstGame"))
        {
            if (AOA_FirstOpen) AppOpenAdsManager.Instance.ShowAOA();
        }
        else if (AOA_SessionStart)
        {
            AppOpenAdsManager.Instance.ShowAOA();
        }

        PlayerPrefs.SetInt("FirstGame", 2710);

        //Setup Max
        //MaxManager.Ins.Setup();

        // if (!isMkt && !isNoAds) ShowBanner();

        //Setup Ironsource

        _isAdsSetupDone = true;
    }

    public bool IsTabletOrSame()
    {
        return isCheckTablet && (float)Screen.height / Screen.width < 1921f / 1080;
    }

    #region MAX UMP

    public void LoadAndShowCmpFlow()
    {
        var cmpService = MaxSdk.CmpService;

        cmpService.ShowCmpForExistingUser(error =>
        {
            if (null == error)
            {
                // The CMP alert was shown successfully.
            }
        });
    }

    public bool IsConsentRequired()
    {
        return MaxSdk.GetSdkConfiguration().ConsentFlowUserGeography == MaxSdkBase.ConsentFlowUserGeography.Gdpr;
    }

    #endregion

    #region Admob consent

    public void ShowPrivacyOptionsForm(GameObject _privacyButton)
    {
        Debug.Log("Showing privacy options form.");

        ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
        {
            if (showError != null)
            {
                Debug.LogError("Error showing privacy options form with error: " + showError.Message);
            }

            // Enable the privacy settings button.
            if (_privacyButton != null)
            {
                _privacyButton.SetActive(IsPrivacyOptionRequired());
            }
        });
    }

    public bool IsPrivacyOptionRequired()
    {
        return ConsentInformation.PrivacyOptionsRequirementStatus ==
               PrivacyOptionsRequirementStatus.Required;
    }

    void OnConsentInfoUpdated(FormError consentError)
    {
        Debug.Log("CONSENT UPDATE");
        if (consentError != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError("CONSENT ERROR" + consentError);
            AppOpenAdsManager.Instance.Setup();
            MaxManager.Ins.Setup(() => IsMaxInitialize = true);
            return;
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        if (!IsMaxInitialize) Time.timeScale = 0.0912f;
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            Debug.Log("CONSENT FORM");
            if (formError != null)
            {
                // Consent gathering failed.
                UnityEngine.Debug.LogError("FORM ERROR " + formError);
                AppOpenAdsManager.Instance.Setup();
                MaxManager.Ins.Setup(() => IsMaxInitialize = true);
                return;
            }

            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
            {
                Debug.Log("CONSENT ADS");
                AppOpenAdsManager.Instance.Setup();
            }

            MaxManager.Ins.Setup(() => IsMaxInitialize = true);
        });
    }

    #endregion

    public void Update()
    {
        if (!_isAdsSetupDone) return;
        _timeWatchAds += Time.deltaTime;
        _timeWatchAdsInter += Time.deltaTime;
    }

    public void ShowInterstitial(string placement = "", Action OnFinish = null)
    {
        InterPlacement = placement;

        if (isNoAds
            || !canShowAds
            || isMkt)
        {
            if (OnFinish != null) OnFinish?.Invoke();
            UnlockControl();
            return;
        }

        //Nếu dùng Max
        MaxManager.Ins.ShowInterstitial(placement, () =>
        {
            if (OnFinish != null) OnFinish.Invoke();
            UnlockControl();
        });

        //Nếu dùng Ironsource
        //....
    }

    public void ShowRewardedAd(string nameEvent = "", Action OnFinish = null, Action OnFail = null)
    {
        RewardPlacement = nameEvent;

        if (isMkt)
        {
            OnFinish?.Invoke();
            return;
        }

        //Nếu dùng Max
        MaxManager.Ins.ShowRewardedAd(nameEvent, () =>
        {
            if (OnFinish != null) OnFinish.Invoke();
            UnlockControl();
        }, () =>
        {
            if (OnFail != null) OnFail.Invoke();
            UnlockControl();
        });

        //Nếu dùng Ironsource
        //....
    }

    public void CheckBanner()
    {
        if (isNoAds || isMkt || VipPassDataFragment.cur.CheckGrandAdsStatePrime()) HideBanner();
        else ShowBanner();
    }

    public void ShowBanner()
    {
        CanvasBannerOff.CheckActive();

        if (isNoAds || isMkt || VipPassDataFragment.cur.IsBlockAds) return;
        MaxManager.Ins.ShowBanner();
    }

    public void HideBanner()
    {
        MaxManager.Ins.HideBanner();
        CanvasBannerOff.CheckActive();
    }

    public void ShowMREC()
    {
        if (isNoAds || isMkt) return;

        MaxManager.Ins.HideBanner();
        MaxManager.Ins.ShowMRECsAds();
    }

    public void ShowAOA()
    {
        AppOpenAdsManager.Instance.ShowAOA();
        //BlockControlAOA(3f);
    }

    public void ShowAOAPro()
    {
        StartCoroutine(ie_Show());
    }

    private IEnumerator ie_Show()
    {
        yield return new WaitUntil(() => GrandManager.ins.IsInternetConnected);
        ShowAOA();
    }

    public void DropAOA()
    {
        AppOpenAdsManager.Instance.DropShowAOA();
    }

    #region Block Control

    public void BlockControlAOA(float time)
    {
        if (panelBlockControl != null) panelBlockControl.SetActive(true);
        GrandManager.ins.PauseGame();
        Timer.SchedulePro(this, time, () =>
        {
            UnlockControl();
            DropAOA();
        });
    }

    public void BlockControl()
    {
        if (panelBlockControl != null) panelBlockControl.SetActive(true);
        // AppOpenAdsManager.Instance.ShowBanner(false);
        // MaxManager.Ins.HideBanner();
        Timer.SchedulePro(this, 2f, UnlockControl);
    }

    public void UnlockControl()
    {
        StopAllCoroutines();
        // if (GrandManager.ins != null) GrandManager.ins.UnpauseGame();
        if (panelBlockControl != null) panelBlockControl.SetActive(false);
        // if (AppOpenAdsManager.Instance != null) AppOpenAdsManager.Instance.ShowBanner(true);
    }

    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Google.Play.Review;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrandManager : SingleTons<GrandManager>
{
    public bool IsInternetConnected { get; set; } = true;
    private bool isNet = true;

    public float playTime { get; set; }
    private bool isCount;

    public bool IsSettingOpen { get; set; }
    public bool IsPause { get; set; }

    private static bool isAOA;
    private static int timeFailed;

    //review
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    [HideInInspector] public bool isReviewing;

    public bool IsHome { get; private set; }
    public bool IsGame { get; private set; }
    public bool IsChallenge { get; private set; }

    public int WinConcurrentNum { get; set; }
    public int LoseConcurrentNum { get; set; }
    public bool FlagIsFirstLose { get; set; }

    public float interTimePoint;

    private void Start()
    {
        GameSetting();
        OnStart();
    }

    private void OnStart()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isNet = false;
            IsInternetConnected = false;
            PauseGame();
            UIManager.ins.OpenUI<CanvasInet>();
        }
        else
            isNet = IsInternetConnected = true;

        Application.lowMemory += OnLowMemory;

        isReviewing = false;

        if (LevelDataFragment.cur.IsBabySitLevel()) interTimePoint = float.MinValue;
    }

    private void Update()
    {
        CheckHandle();
    }

    public bool RequireInter(bool isLose, string placement, Action onDone = null)
    {
        if (VipPassDataFragment.cur.IsBlockAds)
        {
            onDone?.Invoke();
            return false;
        }

        if (LevelDataFragment.cur.gameData.level < AdsManager.Ins.Ads_Level)
        {
            onDone?.Invoke();
            return false;
        }

        if (isLose) LoseConcurrentNum++;
        else
        {
            WinConcurrentNum++;
            ResetFirstLoseFlag();
        }

        if (Time.time - interTimePoint > AdsManager.Ins.Ads_Capping)
        {
            if (FlagIsFirstLose && isLose)
            {
                FlagIsFirstLose = false;
                onDone?.Invoke();
                Debug.Log("REQ INTER FIRST LOSE " + (Time.time - interTimePoint) + "   " + WinConcurrentNum + "   " + LoseConcurrentNum);
                return false;
            }

            AdsManager.Ins.ShowInterstitial(placement, onDone);
            ResetCapping();
            Debug.Log("REQ INTER TIME " + (Time.time - interTimePoint) + "   " + WinConcurrentNum + "   " + LoseConcurrentNum);
            return true;
        }

        if (WinConcurrentNum >= 2)
        {
            AdsManager.Ins.ShowInterstitial(placement, onDone);
            ResetCapping();
            Debug.Log("REQ INTER WIN " + (Time.time - interTimePoint) + "   " + WinConcurrentNum + "   " + LoseConcurrentNum);
            return true;
        }

        if (LoseConcurrentNum >= 3)
        {
            AdsManager.Ins.ShowInterstitial(placement, onDone);
            ResetCapping();
            Debug.Log("REQ INTER LOSE " + (Time.time - interTimePoint) + "   " + WinConcurrentNum + "   " + LoseConcurrentNum);
            return true;
        }

        onDone?.Invoke();
        Debug.Log("REQ INTER FAIL " + (Time.time - interTimePoint) + "   " + WinConcurrentNum + "   " + LoseConcurrentNum);
        return false;
    }

    private void TruncateTimePoint()
    {
        if (Time.time - interTimePoint > AdsManager.Ins.Ads_Capping)
        {
            interTimePoint = Time.time - AdsManager.Ins.Ads_Capping;
        }
    }

    public void TriggerPenalty()
    {
        TruncateTimePoint();
        interTimePoint += AdsManager.Ins.Ads_Penalty;
    }

    public void ResetFirstLoseFlag()
    {
        FlagIsFirstLose = true;
    }

    public void ResetCapping()
    {
        interTimePoint = Time.time;
        WinConcurrentNum = 0;
        LoseConcurrentNum = 0;
    }

    private void CheckHandle()
    {
        // Debug.Log("CHECKKKKKK  " + Application.internetReachability);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (isNet)
            {
                isNet = false;
                IsInternetConnected = false;
                PauseGame();
                UIManager.ins.OpenUI<CanvasInet>();
            }
        }
        else
        {
            if (!isNet)
            {
                isNet = true;
                IsInternetConnected = true;
                UnpauseGame();
                UIManager.ins.CloseUI<CanvasInet>();
            }
        }

        // Debug.Log("TIME POINT " + firstInterTimePoint + " " + Time.time + " " + (Time.time - firstInterTimePoint) + "  " + (Time.time - intoHomeTimePoint));
        // Debug.Log("TIME POINT SUB" + secondInterTimePoint + " " + Time.time + " " + (Time.time - secondInterTimePoint));
    }

    public void ResetFlag()
    {
        isNet = true;
    }

    private void OnLowMemory()
    {
        Resources.UnloadUnusedAssets();
    }

    private static void GameSetting()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        Input.multiTouchEnabled = false;
        // Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // Physics.simulationMode = SimulationMode.Script;
#if !UNITY_EDITOR
        // Debug.unityLogger.logEnabled = false;
#endif
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
    }

    public void InitHome()
    {
        IntoHomeHandle();
    }

    public void InitLevel()
    {
        IntoLevelHandle();
    }

    public void IntoHome()
    {
        // AdsManager.Ins.HideBanner();
        UIManager.ins.OpenUI<CanvasTransit>().MakeTransit(IntoHomeHandle);
    }

    public void IntoHome(Action onDoneTransit)
    {
        // AdsManager.Ins.HideBanner();
        UIManager.ins.OpenUI<CanvasTransit>().MakeTransit(() =>
        {
            IntoHomeHandle();
            onDoneTransit?.Invoke();
        });
    }

    public void InToGame()
    {
        UIManager.ins.OpenUI<CanvasTransit>().MakeTransit(IntoLevelHandle);
    }

    public void InToGame(Action onDoneTransit)
    {
        UIManager.ins.OpenUI<CanvasTransit>().MakeTransit(() =>
        {
            IntoLevelHandle();
            onDoneTransit?.Invoke();
        });
    }

    public void IntoChallenge()
    {
        UIManager.ins.OpenUI<CanvasTransit>().MakeTransit(IntoChallengeHandle);
    }

    public void IntoChallenge(Action onDoneTransit)
    {
        UIManager.ins.OpenUI<CanvasTransit>().MakeTransit(() =>
        {
            IntoChallengeHandle();
            onDoneTransit?.Invoke();
        });
    }

    private void IntoHomeHandle()
    {
        IsHome = true;
        IsGame = false;
        IsChallenge = false;

        AdsManager.Ins.CheckBanner();

        WinstreakDataFragment.cur.SetLoseStreakFlag(false);
        ResourcesDataFragment.cur.ProcessPending();

        TransportCenter.cur.NukeMap();
        JunkPile.ins.RecallAll();
        EmojiPopping.Activate(false);

        CameraCon.ins.SwitchPerspective();
        CameraCon.ins.SetRestPoint(HomeModel.cur.homeCamRestPoint);

        HomeModel.cur.lightObject.SetActive(true);
        HomeModel.cur.gameObject.SetActive(true);
        ParkingLot.cur.lightObject.SetActive(false);

        UIManager.ins.CloseAllUI<CanvasTransit>();
        UIManager.ins.OpenUI<CanvasFloatingStuff>();
        UIManager.ins.OpenUI<CanvasHome>();

        RenderSettings.fog = true;
    }

    private float _startLevelTime;

    private void IntoLevelHandle()
    {
        IsHome = false;
        IsGame = true;
        IsChallenge = false;

        AdsManager.Ins.CheckBanner();

        CanvasWinstreak.HandleJustLose();

        WinstreakDataFragment.cur.SetLoseStreakFlag(true);
        TreasureDataFragment.cur.NukeRecord();
        ResourcesDataFragment.cur.ProcessPending();

        CameraCon.ins.SwitchOrthographic();
        CameraCon.ins.SetRestPoint(ParkingLot.cur.GetCamRestPoint());

        LevelDataFragment.cur.LoadLevel();
        EmojiPopping.Activate(true);

        HomeModel.cur.lightObject.SetActive(false);
        HomeModel.cur.gameObject.SetActive(false);
        ParkingLot.cur.lightObject.SetActive(true);

        UIManager.ins.CloseAllUI<CanvasTransit>();
        UIManager.ins.OpenUI<CanvasFloatingStuff>();
        UIManager.ins.OpenUI<CanvasGamePlay>();

        CanvasFloatingStuff.cur.NukeGoldNoff();

        DataManager.ins.LogStart();

        CanvasSpecialCarTut.TriggerTut();
        
        RenderSettings.fog = false;

        _startLevelTime = Time.time;
    }

    public float GetCompleteDuration()
    {
        return Time.time-_startLevelTime;
    }
    private void IntoChallengeHandle()
    {
        IsHome = false;
        IsGame = false;
        IsChallenge = true;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPause = true;
    }

    public void UnpauseGame()
    {
        if (IsSettingOpen || !IsInternetConnected) return;
        Time.timeScale = 1f;
        IsPause = false;
    }

    public void CallReview()
    {
        isReviewing = true;
        StartCoroutine(Review());

        IEnumerator Review()
        {
            _reviewManager = new ReviewManager();

            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                isReviewing = false;
                yield break;
            }

            _playReviewInfo = requestFlowOperation.GetResult();

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                isReviewing = false;
                yield break;
            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.

            isReviewing = false;
            yield return null;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasHome : UICanvasPrime
{
    public static CanvasHome cur;
    public RectTransform canvasRect;

    public TMP_Text playLevelText;

    public GameObject noAdsButtonObject;
    public GameObject starterPackButtonObject;
    public GameObject halloweenPackButtonObject;
    public GameObject removeAdsBundleButtonObject;
    public GameObject neverGiveUpButtonObject;
    public GameObject obj_buttonSpaceMission;
    public GameObject obj_buttonCarRace;

    public TMP_Text spinNumText;

    public RectTransform[] iconRect;
    [SerializeField] private Vector2[] iconPoses;

    /*
    public RectTransform[] rightIconRect;
    */
    [SerializeField] private Vector2[] rightIconPos;

    public RectTransform playButtonRect;

    public RectTransform profileButtonRect;

    private void Awake()
    {
        cur = this;

        profileButtonRect.SafeAreaAdaption(canvas);
    }

    protected override void OnOpenCanvas()
    {
        // CanvasFloatingStuff canvasFloatingStuff = UIManager.ins.GetUICanvas<CanvasFloatingStuff>();
        // canvasFloatingStuff.goldRect.anchoredPosition = new Vector2(250, -89.2002f);

        base.OnOpenCanvas();

        playLevelText.text = "Level " + (LevelDataFragment.cur.gameData.level + 1);

        StartCoroutine(CheckEventSpaceAndRace());
        CheckStuff();

        HomeTreasureModule.cur.JustBeNormal();
        HomeTreasureModule.cur.CheckTime();
        HomeTreasureModule.cur.HandleTreasure();

        HomeWinstreakModule.cur.CheckTime();
        HomeWinstreakModule.cur.CheckWinStreakPop();

        ProfileDataFragment.cur.CapNhatTen();
    }

    public void CheckEvent()
    {
        StartCoroutine(CheckEventSpaceAndRace());
        CheckStuff();
    }

    public IEnumerator CheckEventSpaceAndRace()
    {
        Debug.Log("CHECKKKKKKK");
        SpaceMissionDataFragment.cur.CalculateTime();
        CarRaceDataFragment.cur.CalculateTime();
        obj_buttonSpaceMission.SetActive(false);
        obj_buttonCarRace.SetActive(false);

        int indexWinSpace = -1;
        if (SpaceMissionDataFragment.cur.timeLeft > 0 && LevelDataFragment.cur.gameData.level >= SpaceMissionDataFragment.ActiveLevel)
        {
            indexWinSpace = SpaceMissionDataFragment.cur.CalculateProgress();
            ButtonSpaceHome.cur.TinhRank();
            obj_buttonSpaceMission.SetActive(true);
        }

        RaceStatus raceStatus = RaceStatus.Racing;
        if (CarRaceDataFragment.cur.timeLeft > 0 && LevelDataFragment.cur.gameData.level >= CarRaceDataFragment.ActiveLevel)
        {
            obj_buttonCarRace.SetActive(true);
            raceStatus = CarRaceDataFragment.cur.CalculateProgress();
            ButtonRaceHome.cur.TinhRank();
        }

        yield return new WaitUntil(() => UnbiasedTime.IsValidTime);
        yield return null;
        //checking pack
        // Debug.Log("WAIT 1");
        var pack = BuyingPackDataFragment.cur;
        if (pack.IsShowingRemoveAdsBundle) yield return new WaitUntil(() => !pack.IsShowingRemoveAdsBundle);
        if (pack.IsShowingNeverGiveUp) yield return new WaitUntil(() => !pack.IsShowingNeverGiveUp);
        if (pack.IsShowingStarter) yield return new WaitUntil(() => !pack.IsShowingStarter);
        if (pack.IsShowingRemoveAds) yield return new WaitUntil(() => !pack.IsShowingRemoveAds);
        // Debug.Log("WAIT 2");
        yield return null;

        yield return new WaitUntil(() => !HomeTreasureModule.cur.IsHandlingTreasure);
        yield return null;
        // Debug.Log("WAIT 3");

        if (CanvasTreasureStart.IsOpen) yield return new WaitUntil(() => !CanvasTreasureStart.IsOpen);
        yield return null;
        // Debug.Log("WAIT 4");

        if (CanvasTreasureShow.IsOpen) yield return new WaitUntil(() => !CanvasTreasureShow.IsOpen);
        yield return null;
        if (CanvasWinstreakStart.IsOpen) yield return new WaitUntil(() => !CanvasWinstreakStart.IsOpen);
        yield return null;
        if (CanvasWinstreak.IsOpen) yield return new WaitUntil(() => !CanvasWinstreak.IsOpen);
        yield return null;
        // Debug.Log("WAIT 5");

        // nếu đang là event space mision
        if (SpaceMissionDataFragment.cur.timeLeft > 0 && LevelDataFragment.cur.gameData.level >= SpaceMissionDataFragment.ActiveLevel)
        {
            obj_buttonSpaceMission.SetActive(true);

            if (indexWinSpace == 0) // player win
            {
                UIManager.ins.OpenUI<CanvasBlockage>();
                yield return Yielders.Get(.82f);
                UIManager.ins.CloseUI<CanvasBlockage>();
                UIManager.ins.OpenUI<CanvasSpaceMission>().Setup(indexWinSpace);
            }
            else if (indexWinSpace > 0) // bot win
            {
                UIManager.ins.OpenUI<CanvasBlockage>();
                yield return Yielders.Get(.82f);
                UIManager.ins.CloseUI<CanvasBlockage>();
                UIManager.ins.OpenUI<CanvasSpaceMission>().Setup(indexWinSpace);
            }
        }

        // nếu đang là event car race
        if (CarRaceDataFragment.cur.timeLeft > 0 && LevelDataFragment.cur.gameData.level >= CarRaceDataFragment.ActiveLevel)
        {
            obj_buttonCarRace.SetActive(true);

            // if (CarRaceDataFragment.cur.gameData.chuaNhanThuong)
            // {
            //     UIManager.ins.OpenUI<CanvasCarGrand>().Setup();
            //     yield break;
            // }


            if (raceStatus == RaceStatus.PlayerWin || raceStatus == RaceStatus.PlayerLose)
            {
                UIManager.ins.OpenUI<CanvasBlockage>();
                yield return Yielders.Get(.82f);
                UIManager.ins.CloseUI<CanvasBlockage>();
                UIManager.ins.OpenUI<CanvasCarGrand>().Setup(raceStatus);
            }
        }
        // else
        // {
        //     if (CarRaceDataFragment.cur.gameData.chuaNhanThuong)
        //     {
        //         UIManager.ins.OpenUI<CanvasCarGrand>().Setup();
        //     }
        // }
    }

    private void CheckStuff()
    {
        const string key = "HOME_POP_DAILY";

        BuyingPackDataFragment.cur.CheckShowOnHome();
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 2710);

            StartCoroutine(ie_Check());
        }

        CheckNoAdsOffer();
        CheckStarterPack();
        CheckHalloweenPack();
        CheckRemoveAdsBundle();
        CheckNeverGiveUpBundle();
        CheckSpinNum();

        return;

        IEnumerator ie_Check()
        {
            var pack = BuyingPackDataFragment.cur;
            if (pack.IsShowingRemoveAdsBundle) yield return new WaitUntil(() => !pack.IsShowingRemoveAdsBundle);
            if (pack.IsShowingNeverGiveUp) yield return new WaitUntil(() => !pack.IsShowingNeverGiveUp);
            if (pack.IsShowingStarter) yield return new WaitUntil(() => !pack.IsShowingStarter);
            if (pack.IsShowingRemoveAds) yield return new WaitUntil(() => !pack.IsShowingRemoveAds);

            yield return Yielders.Get(0.28f);
            OnClickDailyLogin();
        }
    }

    public void OnClickSpin()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasSpin>();
    }

    public void OnClickDailyReward()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasDailyReward>();
    }

    public void OnClickDailyLogin()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasDailyLogin>();
    }

    public void OnClickPlay()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        GrandManager.ins.InToGame();
        //
        // CanvasFloatingStuff canvasFloatingStuff = UIManager.ins.GetUICanvas<CanvasFloatingStuff>();
        // canvasFloatingStuff.goldRect.anchoredPosition = new Vector2(50, -89.2002f);
    }

    public void OnClickIapShop()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasIapShop>();
    }

    public void OnClickNoAdsOffer()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasOfferRemoveAdsPack>().Setup(false);
    }

    public void OnClickStarterOffer()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasOfferStarter>().Setup(false);
    }

    public void OnClickHalloweenOffer()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasOfferHalloween>().Setup(false);
    }

    public void OnClickNoAdsBundle()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasOfferRemoveAdsBundle>().Setup(false);
    }

    public void OnClickNeverGiveUp()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasOfferNeverGiveUp>().Setup(false);
    }

    public void OnClickWinstreak()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasWinstreak>();
    }

    public void OnClickSpaceMission()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasSpaceMission>().Setup();
    }

    public void OnClickCarRace()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasCarGrand>().Setup();
    }

    public static void CheckNoAdsOfferStatic()
    {
        if (cur == null) return;
        cur.CheckNoAdsOffer();
    }

    private void CheckNoAdsOffer()
    {
        noAdsButtonObject.SetActive(!AdsManager.isNoAds);

        CheckIconPos();
    }

    public static void CheckStarterPackStatic()
    {
        if (cur == null) return;
        cur.CheckStarterPack();
    }

    private void CheckStarterPack()
    {
        starterPackButtonObject.SetActive(BuyingPackDataFragment.cur.gameData.isShowStarterPack && !BuyingPackDataFragment.cur.gameData.isStarterPackBought);

        CheckIconPos();
    }

    public static void CheckHalloweenPackStatic()
    {
        if (cur == null) return;
        cur.CheckHalloweenPack();
    }

    private void CheckHalloweenPack()
    {
        if (false)
        {
            halloweenPackButtonObject.SetActive(BuyingPackDataFragment.cur.gameData.isShowHalloween && !BuyingPackDataFragment.cur.gameData.isHalloweenPackBought);
            CheckIconPos();
        }
    }

    public static void CheckRemoveAdsBundleStatic()
    {
        if (cur == null) return;
        cur.CheckRemoveAdsBundle();
    }

    private void CheckRemoveAdsBundle()
    {
        var isOn = BuyingPackDataFragment.cur.gameData.isShowRemoveAdsBundle && !AdsManager.isNoAds;
        removeAdsBundleButtonObject.SetActive(isOn);

        CheckIconPos();
    }

    public static void CheckNeverGiveUpBundleStatic()
    {
        if (cur == null) ;
        cur.CheckNeverGiveUpBundle();
    }

    private void CheckNeverGiveUpBundle()
    {
        var isOn = BuyingPackDataFragment.cur.gameData.isShowNeverGiveUp && BuyingPackDataFragment.cur.gameData.isNeverGiveUpEnable;
        neverGiveUpButtonObject.SetActive(isOn);

        CheckRightIconPos();
    }

    public static void CheckSpinNumStatic()
    {
        if (cur == null) return;
        cur.CheckSpinNum();
    }

    private void CheckSpinNum()
    {
        var require = SpinDataFragment.cur.GetRequiredPassedLevel();
        var passed = Mathf.Min(SpinDataFragment.cur.gameData.levelPassedNum, require);

        spinNumText.text = passed + "/" + require;
    }

    private void CheckIconPos()
    {
        var list = new List<RectTransform>();
        if (!AdsManager.isNoAds) list.Add(iconRect[0]);
        if (BuyingPackDataFragment.cur.gameData.isShowStarterPack && !BuyingPackDataFragment.cur.gameData.isStarterPackBought) list.Add(iconRect[1]);
        if (!AdsManager.isNoAds && BuyingPackDataFragment.cur.gameData.isShowRemoveAdsBundle) list.Add(iconRect[2]);
        if (BuyingPackDataFragment.cur.gameData.isShowHalloween && !BuyingPackDataFragment.cur.gameData.isHalloweenPackBought) list.Add(iconRect[3]);

        for (int i = 0; i < list.Count; i++)
        {
            list[i].anchoredPosition = iconPoses[i];
        }
    }

    public static void CheckRightIconPosStatic()
    {
        if (cur == null) return;
        cur.CheckRightIconPos();
    }

    public void CheckRightIconPos()
    {
        /*var list = new List<RectTransform>();
        var isWinstreakOn = HomeWinstreakModule.cur.iconObject.activeSelf;
        var isNeverGiveUpOn = BuyingPackDataFragment.cur.gameData.isShowNeverGiveUp && BuyingPackDataFragment.cur.gameData.isNeverGiveUpEnable;
        bool isSpaceMission = SpaceMissionDataFragment.cur.timeLeft > 0 && LevelDataFragment.cur.gameData.level >= 25;
        bool isCarRace = CarRaceDataFragment.cur.timeLeft > 0 && LevelDataFragment.cur.gameData.level >= 25;

        if (isWinstreakOn) list.Add(rightIconRect[0]);
        if (isNeverGiveUpOn) list.Add(rightIconRect[1]);
        if (isSpaceMission) list.Add(rightIconRect[2]);
        if (isCarRace) list.Add(rightIconRect[3]);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == rightIconRect[3])
            {
                list[i].anchoredPosition = rightIconPos[i] - Vector2.up * 30;
            }
            else
            {
                list[i].anchoredPosition = rightIconPos[i];
            }
        }*/
    }
}
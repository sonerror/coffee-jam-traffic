using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Buying Pack Fragment", fileName = "Buying Pack Fragment")]
public class BuyingPackDataFragment : DataFragment
{
    public static BuyingPackDataFragment cur;

    public Data gameData;

    private const int TIME_SPACE_EIGHT_MINUTES = 480;
    private const int TIME_SPACE_TWELVE_MINUTES = 720;
    private const int TIME_SPACE_FIFTEEN_MINUTES = 900;
    private const int TIME_SPACE_TEN_MINUTES = 600;
    private const int TIME_SPACE_TWO_HOURS = 7200;
    private const int TIME_SPACE_FIVE_HOURS = 18000;
    private const int TIME_SPACE_EIGHT_HOURS = 28800;

    public bool IsSomethingShowing { get; set; }
    public bool IsShowingRemoveAds { get; set; }
    public bool IsShowingStarter { get; set; }
    public bool IsShowingRemoveAdsBundle { get; set; }
    public bool IsShowingNeverGiveUp { get; set; }
    public bool IsShowingHalloween { get; set; }

    public bool IsJustInter { get; set; }
    public int InterShowedNum { get; set; }

    public int ConsecutiveLoseNum
    {
        get => gameData.consecutiveLoseNum;

        set => gameData.consecutiveLoseNum = value;
    }

    private float staterPackCappingTimePoint;
    private float adsBundleTimePoint;
    private bool isHalloweenShowedSinceOpen;

#if UNITY_EDITOR
    private void OnEnable()
    {
        cur = this;

        ///////////////// cant assign value to vảiable ínide scriptable object --> mút done in âke ỏ smt  ?? or is it

        ResetStuff();
    }

#else
    private void Awake()
    {
        // Debug.Log("LEVE DATA FRAGMENT");
        cur = this;

        ResetStuff();
    }
#endif

    private void ResetStuff()
    {
        staterPackCappingTimePoint = float.MinValue;
        adsBundleTimePoint = float.MinValue;

        IsJustInter = false;
        InterShowedNum = 0;
        ConsecutiveLoseNum = 0;

        isHalloweenShowedSinceOpen = false;
    }

    public override void Load()
    {
        if (!LoadData(ref gameData, key)) ResetData();
        gameData.baseOpenTime = DateTime.FromBinary(gameData.baseOpenTimeLong);
        gameData.neverGiveUpTime = DateTime.FromBinary(gameData.neverGiveUpTimeLong);
        UpdateShowFlag();
    }

    public override void Save()
    {
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
        gameData.neverGiveUpTimeLong = gameData.neverGiveUpTime.ToBinary();
        SaveData(gameData, key);
    }

    public override void ResetData()
    {
        gameData = new Data();
        gameData.baseOpenTime = gameData.neverGiveUpTime = new DateTime(2022, 1, 1, 0, 0, 0);
        gameData.baseOpenTimeLong = gameData.neverGiveUpTimeLong = gameData.baseOpenTime.ToBinary();
    }

    public override void Update()
    {
        // Save();
    }

    public static void SetInterFlag()
    {
        if (cur == null) return;
        cur.InterShowedNum++;
        cur.IsJustInter = true;
    }

    public void UpdateShowFlag()
    {
        var data = LevelDataFragment.cur.gameData;
        var level = data.level;

        var preAdsUnlockState = gameData.isShowRemoveAds;

        gameData.isShowRemoveAds = gameData.isShowRemoveAds || IsJustInter;
        gameData.isShowStarterPack = gameData.isShowStarterPack || level >= 7;
        gameData.isShowRemoveAdsBundle = gameData.isShowRemoveAdsBundle || level >= 11;
        gameData.isShowNeverGiveUp = gameData.isShowNeverGiveUp || level >= 13;
        gameData.isShowHalloween = gameData.isShowHalloween || level >= 10;

        if (!preAdsUnlockState && gameData.isShowRemoveAds) InterShowedNum = 2710;
    }

    public void CheckShowOnHome()
    {
        UpdateShowFlag();

        if (LevelDataFragment.cur.gameData.level == AdsManager.Ins.TreasureLevel && !TreasureDataFragment.cur.gameData.isTreasureOpened) return; // not show on unlock treasure

        IsSomethingShowing = true;

        int showingNum = 0;

        /*
        IsShowingHalloween = CanItShow() && (IsShowHalloween() || NullifyShowNum());
        */
        IsShowingHalloween = false;
        IsShowingRemoveAdsBundle = CanItShow() && (IsShowRemoveAdsBundle() || NullifyShowNum());
        IsShowingNeverGiveUp = CanItShow() && (IsShowNeverGiveUp() || NullifyShowNum());
        IsShowingStarter = CanItShow() && (IsShowStarterPack() || NullifyShowNum());
        IsShowingRemoveAds = !IsShowingRemoveAdsBundle && CanItShow() && (IsShowRemoveAds() || NullifyShowNum());

        IsSomethingShowing = false;
        return;

        bool CanItShow()
        {
            showingNum++;
            return (showingNum <= 2);
        }

        bool NullifyShowNum()
        {
            showingNum--;
            return false;
        }
    }

    //divide packs into 3 main group to check
    private bool IsShowRemoveAds()
    {
        if (AdsManager.isNoAds || VipPassDataFragment.cur.IsBlockAds) return false;
        if (!gameData.isShowRemoveAds) return false;
        if (!IsRemoveAdsCapping()) return false;
        //show ads pack
        UIManager.ins.OpenUI<CanvasOfferRemoveAdsPack>().Setup(true);
        return true;
    }

    private bool IsShowStarterPack()
    {
        if (!gameData.isShowStarterPack) return false;
        if (gameData.isStarterPackBought) return false;
        if (!IsCappingStarter()) return false;
        //show starter
        UIManager.ins.OpenUI<CanvasOfferStarter>().Setup(true);
        return true;
    }

    private bool IsShowHalloween()
    {
        if (!gameData.isShowHalloween) return false;
        if (gameData.isHalloweenPackBought) return false;
        if (!IsCappingHalloween()) return false;
        //show halloween
        if (false)
        {
            UIManager.ins.OpenUI<CanvasOfferHalloween>().Setup(true);
        }

        return true;
    }

    private bool IsShowRemoveAdsBundle()
    {
        if (AdsManager.isNoAds || VipPassDataFragment.cur.IsBlockAds) return false;
        if (!gameData.isShowRemoveAdsBundle) return false;
        if (!IsCappingRemoveAdsBundle()) return false;
        //show ads pack
        UIManager.ins.OpenUI<CanvasOfferRemoveAdsBundle>().Setup(true);
        return true;
    }

    private bool IsShowNeverGiveUp()
    {
        if (!gameData.isShowNeverGiveUp) return false;
        if (IsCappingNeverGiveUp())
        {
            if (!CheckNeverGiveUpAvailable(out _)) return false;
            //show ads pack
            UIManager.ins.OpenUI<CanvasOfferNeverGiveUp>().Setup(true);
            return true;
        }

        return false;
    }

    public bool IsRemoveAdsCapping()
    {
        if (InterShowedNum > 4 && IsJustInter)
        {
            InterShowedNum = 0;
            IsJustInter = false;
            return true;
        }

        return false;
    }

    public bool IsCappingStarter()
    {
        if (Time.time - staterPackCappingTimePoint > TIME_SPACE_EIGHT_MINUTES)
        {
            staterPackCappingTimePoint = Time.time;
            return true;
        }

        return false;
    }

    public bool IsCappingHalloween()
    {
        if (!isHalloweenShowedSinceOpen)
        {
            isHalloweenShowedSinceOpen = true;
            return true;
        }

        return false;
    }

    public bool IsCappingRemoveAdsBundle()
    {
        if (Time.time - adsBundleTimePoint > TIME_SPACE_TWELVE_MINUTES)
        {
            adsBundleTimePoint = Time.time;
            return true;
        }

        return false;
    }

    public bool IsCappingNeverGiveUp()
    {
        return ConsecutiveLoseNum >= 3;
    }

    public void CheatCapping()
    {
        staterPackCappingTimePoint -= 60;
        adsBundleTimePoint -= 60;
    }

    public bool CheckNeverGiveUpAvailable(out TimeSpan timeLeft)
    {
        var curTime = UnbiasedTime.TrueDateTime;
        var diff = curTime.Subtract(gameData.neverGiveUpTime).TotalSeconds;
        if (diff < TIME_SPACE_FIFTEEN_MINUTES)
        {
            timeLeft = TimeSpan.FromSeconds(TIME_SPACE_FIFTEEN_MINUTES - diff);
            gameData.isNeverGiveUpEnable = true;
            return true;
        }

        if (diff < TIME_SPACE_FIFTEEN_MINUTES + (gameData.isBoughtNeverGiveUpLastTime ? TIME_SPACE_EIGHT_HOURS : TIME_SPACE_TWO_HOURS))
        {
            timeLeft = TimeSpan.FromSeconds(0);
            gameData.isNeverGiveUpEnable = false;
            return false;
        }

        timeLeft = TimeSpan.FromSeconds(TIME_SPACE_FIFTEEN_MINUTES);
        gameData.isNeverGiveUpEnable = true;
        gameData.neverGiveUpTime = curTime;
        gameData.isBoughtNeverGiveUpLastTime = false;
        return true;
    }

    public void OnBuyNGUPackTimeHandle()
    {
        var curTime = DateTime.Now;
        gameData.neverGiveUpTime = curTime.Subtract(TimeSpan.FromSeconds(TIME_SPACE_FIFTEEN_MINUTES));

        Debug.Log("ON BUY " + curTime);

        gameData.isBoughtNeverGiveUpLastTime = true;
    }

    [Serializable]
    public class Data
    {
        public DateTime baseOpenTime;
        public long baseOpenTimeLong;

        public DateTime neverGiveUpTime;
        public long neverGiveUpTimeLong;

        public int consecutiveLoseNum;

        //determine offer is unlock or not
        public bool isShowRemoveAds;
        public bool isShowStarterPack;
        public bool isShowRemoveAdsBundle;
        public bool isShowNeverGiveUp;
        public bool isShowHalloween;

        public bool isStarterPackBought;
        public bool isHalloweenPackBought;

        public bool isNeverGiveUpEnable;

        public bool isBoughtNeverGiveUpLastTime;
    }
}
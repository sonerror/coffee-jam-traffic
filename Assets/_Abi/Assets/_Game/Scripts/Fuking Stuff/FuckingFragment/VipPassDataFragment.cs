using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Vip Pass Fragment", fileName = "Vip Pass Fragment")]
public class VipPassDataFragment : DataFragment
{
    public static VipPassDataFragment cur;

    public Data gameData;

    public bool IsBlockAds { get; set; }

    private const int VIP_TICKET_NUM = 125;
    private const int TIME_SPACE_3_DAY = 259200;
    private const int TIME_SPACE_7_DAY = 604800;
    private const int TIME_SPACE_15_DAY = 1296000;

#if UNITY_EDITOR
    private void OnEnable()
    {
        cur = this;
        RefreshStuff();
    }
#else
    private void Awake()
    {
        cur = this;
        RefreshStuff();
    }
#endif

    private void RefreshStuff()
    {
    }

    public override void Load()
    {
        if (!LoadData(ref gameData, key)) ResetData();
        gameData.baseOpenTime = DateTime.FromBinary(gameData.baseOpenTimeLong);
        gameData.baseOpenTime_3 = DateTime.FromBinary(gameData.baseOpenTimeLong_3);
        gameData.baseOpenTime_7 = DateTime.FromBinary(gameData.baseOpenTimeLong_7);
        gameData.baseOpenTime_15 = DateTime.FromBinary(gameData.baseOpenTimeLong_15);

        Timer.ScheduleCondition(() => UnbiasedTime.IsValidTime, () => CheckNewDay());
    }

    public override void Save()
    {
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
        gameData.baseOpenTimeLong_3 = gameData.baseOpenTime_3.ToBinary();
        gameData.baseOpenTimeLong_7 = gameData.baseOpenTime_7.ToBinary();
        gameData.baseOpenTimeLong_15 = gameData.baseOpenTime_15.ToBinary();
        SaveData(gameData, key);
    }

    public override void ResetData()
    {
        gameData = new Data();
        gameData.baseOpenTime = gameData.baseOpenTime_3 = gameData.baseOpenTime_7 = gameData.baseOpenTime_15 = Variables.rootDate;
        gameData.baseOpenTimeLong = gameData.baseOpenTimeLong_3 = gameData.baseOpenTimeLong_7 = gameData.baseOpenTimeLong_15 = gameData.baseOpenTime.ToBinary();
    }

    public bool CheckNewDay()
    {
        TimeSpan diff = UnbiasedTime.TrueDateTime.Subtract(gameData.baseOpenTime);

        bool isNewDay = false;
        while (diff.TotalSeconds >= Variables.TIME_SPACE_DAY)
        {
            isNewDay = true;
            gameData.baseOpenTime = gameData.baseOpenTime.AddDays(Math.Floor(diff.TotalDays));
            diff = UnbiasedTime.TrueDateTime.Subtract(gameData.baseOpenTime);
        }

        if (isNewDay)
        {
            gameData.isVip_3_DailyClaimed = false;
            gameData.isVip_7_DailyClaimed = false;
            gameData.isVip_15_DailyClaimed = false;
            Save();
            return true;
        }

        return false;
    }

    public bool CheckVip_3(out TimeSpan timeLeft)
    {
        var cur = UnbiasedTime.TrueDateTime;
        var diff = cur.Subtract(gameData.baseOpenTime_3).TotalSeconds;
        if (diff > TIME_SPACE_3_DAY)
        {
            timeLeft = TimeSpan.Zero;
            gameData.isVip_3_Active = false;
            UpdateNoAdsState();
            return false;
        }

        timeLeft = TimeSpan.FromSeconds(TIME_SPACE_3_DAY - diff);
        gameData.isVip_3_Active = true;
        UpdateNoAdsState();
        return true;
    }

    public bool CheckVip_7(out TimeSpan timeLeft)
    {
        var cur = UnbiasedTime.TrueDateTime;
        var diff = cur.Subtract(gameData.baseOpenTime_7).TotalSeconds;
        if (diff > TIME_SPACE_7_DAY)
        {
            timeLeft = TimeSpan.Zero;
            gameData.isVip_7_Active = false;
            UpdateNoAdsState();
            return false;
        }

        timeLeft = TimeSpan.FromSeconds(TIME_SPACE_7_DAY - diff);
        gameData.isVip_7_Active = true;
        UpdateNoAdsState();
        return true;
    }

    public bool CheckVip_15(out TimeSpan timeLeft)
    {
        var cur = UnbiasedTime.TrueDateTime;
        var diff = cur.Subtract(gameData.baseOpenTime_15).TotalSeconds;
        if (diff > TIME_SPACE_15_DAY)
        {
            timeLeft = TimeSpan.Zero;
            gameData.isVip_15_Active = false;
            UpdateNoAdsState();
            return false;
        }

        timeLeft = TimeSpan.FromSeconds(TIME_SPACE_15_DAY - diff);
        gameData.isVip_15_Active = true;
        UpdateNoAdsState();
        return true;
    }

    public void SetVip_3()
    {
        gameData.baseOpenTime_3 = UnbiasedTime.TrueDateTime;
        CheckVip_3(out _);
    }

    public void SetVip_7()
    {
        gameData.baseOpenTime_7 = UnbiasedTime.TrueDateTime;
        CheckVip_7(out _);
    }

    public void SetVip_15()
    {
        gameData.baseOpenTime_15 = UnbiasedTime.TrueDateTime;
        CheckVip_15(out _);
    }

    public void ClaimDaily_3()
    {
        gameData.isVip_3_DailyClaimed = true;
        ResourcesDataFragment.cur.AddGold(VIP_TICKET_NUM, "VIP_DAILY_3");
        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);
    }

    public void ClaimDaily_7()
    {
        gameData.isVip_7_DailyClaimed = true;
        ResourcesDataFragment.cur.AddGold(VIP_TICKET_NUM, "VIP_DAILY_7");
        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);
    }

    public void ClaimDaily_15()
    {
        gameData.isVip_15_DailyClaimed = true;
        ResourcesDataFragment.cur.AddGold(VIP_TICKET_NUM, "VIP_DAILY_15");
        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);
    }

    public void UpdateNoAdsState()
    {
        IsBlockAds = gameData.isVip_3_Active || gameData.isVip_7_Active || gameData.isVip_15_Active;
    }

    public bool CheckGrandAdsStatePrime()
    {
        CheckVip_3(out _);
        CheckVip_7(out _);
        CheckVip_15(out _);

        UpdateNoAdsState();

        return IsBlockAds;
    }

    [Serializable]
    public class Data
    {
        public bool isVip_3_Active;
        public bool isVip_7_Active;
        public bool isVip_15_Active;

        public bool isVip_3_DailyClaimed;
        public bool isVip_7_DailyClaimed;
        public bool isVip_15_DailyClaimed;

        public DateTime baseOpenTime;
        public long baseOpenTimeLong;

        public DateTime baseOpenTime_3;
        public long baseOpenTimeLong_3;
        public DateTime baseOpenTime_7;
        public long baseOpenTimeLong_7;
        public DateTime baseOpenTime_15;
        public long baseOpenTimeLong_15;

        public bool isEverBuyLargestPack;
    }
}
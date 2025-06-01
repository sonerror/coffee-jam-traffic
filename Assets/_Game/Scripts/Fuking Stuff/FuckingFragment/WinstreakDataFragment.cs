using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Win Streak Fragment", fileName = "Win Streak Fragment")]
public class WinstreakDataFragment : DataFragment
{
    public static WinstreakDataFragment cur;

    public Data gameData;

    private static TimeSpan firstOffset = new TimeSpan(2, 0, 0, 0);
    private static TimeSpan secondOffset = new TimeSpan(5, 0, 0, 0);
    private static TimeSpan duration = new TimeSpan(4, 0, 0, 0);
    private static TimeSpan fullWeek = new TimeSpan(7, 0, 0, 0);

    public List<RewardBundleList> rewardBundlesList;
    public int[] checkPoints; //5 10 20 50 100

    public bool IsJustIncreaseStreak { get; set; }
    public bool IsJustLose { get; set; }

    public bool IsAvailable { get; set; }

#if UNITY_EDITOR
    private void OnEnable()
    {
        cur = this;

        ResetFlag();
    }

    private void Reset()
    {
        ResetFlag();
    }
#else
    private void Awake()
    {
        cur = this;
        ResetFlag();
    }
#endif

    public void ResetFlag()
    {
        IsJustIncreaseStreak = false;
        IsJustLose = false;
    }

    public override void Load()
    {
        if (!LoadData(ref gameData, key)) ResetData();
        gameData.baseOpenTime = DateTime.FromBinary(gameData.baseOpenTimeLong);

        if (gameData.isMeantToLoseStreak)
        {
            SetLoseStreakFlag(false);
            BackToLastCheckPoint();
        }
    }

    public override void Save()
    {
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
        SaveData(gameData, key);
    }

    public override void ResetData()
    {
        gameData = new Data();
        gameData.baseOpenTime = new DateTime(2022, 1, 1, 0, 0, 0);
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();

        gameData.giftState = new bool[checkPoints.Length];
    }

    public bool IsStreakUnlock()
    {
        gameData.isStreakUnlock = gameData.isStreakUnlock || LevelDataFragment.cur.gameData.level >= AdsManager.Ins.WinStreakLevel;
        return gameData.isStreakUnlock;
    }

    public bool IsTimeForStreak(out TimeSpan timeLeft)
    {
        if (!IsStreakUnlock())
        {
            timeLeft = TimeSpan.Zero;
            IsAvailable = false;
            return false;
        }

        var curTime = UnbiasedTime.TrueDateTime;

        var lastDate = ExtensionMethodUltimate.GetLastBeginOfWeekDate();
        var startPoint = lastDate.Add(firstOffset);
        var endPoint = lastDate.Add(secondOffset);

        var diff = curTime.Subtract(startPoint);
        var subDiff = curTime.Subtract(endPoint);

        if (diff.TotalSeconds < 0)
        {
            // Debug.Log("TIME 1");
            timeLeft = diff.Negate();
            IsAvailable = true;
            return true;
        }

        if (subDiff.TotalSeconds < 0)
        {
            // Debug.Log("TIME 2");
            timeLeft = TimeSpan.Zero;
            IsAvailable = false;
            return false;
        }

        // Debug.Log("TIME 3");
        timeLeft = duration.Subtract(subDiff);
        IsAvailable = true;
        return true;
    }

    public bool IsNeedToNoffLoseStreak()
    {
        return IsAvailable && GetSubProgressAndCost().Item1 > 0 && !IsOutLevel();
    }

    public bool IsNewStreakSession()
    {
        var curDate = ExtensionMethodUltimate.GetCurrentDayOfWeek();
        var isPostHalf = curDate == DayOfWeek.Monday || curDate == DayOfWeek.Tuesday;
        var startPoint = isPostHalf ? ExtensionMethodUltimate.GetLastBeginOfWeekDate().Subtract(duration.Subtract(firstOffset)) : ExtensionMethodUltimate.GetLastBeginOfWeekDate().Add(secondOffset);
        var diff = startPoint.Subtract(gameData.baseOpenTime);

        // Debug.Log("DIFF " + diff.TotalSeconds + "   " + duration.TotalSeconds);

        if (diff.TotalSeconds > duration.TotalSeconds)
        {
            gameData.baseOpenTime = startPoint;

            //reset Stuff

            gameData.streak = 0;
            gameData.giftState = new bool[checkPoints.Length];

            Save();
            return true;
        }

        return false;
    }

    public bool IsOutLevel()
    {
        return gameData.streak >= checkPoints[^1];
    }

    public void InCreaseStreak()
    {
        if (IsOutLevel() || !IsTimeForStreak(out _)) return;
        gameData.streak++;
    }

    public void BackToLastCheckPoint()
    {
        if (CanvasWinstreak.RecordedStreak >= 0) CanvasWinstreak.RecordedStreak = gameData.streak;
        CanvasWinstreak.IsJustLoseStreak = true;
        gameData.streak = GetCurrentCheckPoint();
    }

    public (int, int) GetSubProgressAndCost(int s = -1) //may not use for fake streak properly
    {
        if (IsOutLevel()) return (1, 1);
        if (s < 0) s = gameData.streak;
        var index = GetCurrentCheckPointIndex(s);
        if (!gameData.giftState[index] && index > 0) index--;
        var pre = checkPoints[index];
        var post = checkPoints[index + 1];

        var current = s - pre;
        var cost = post - pre;

        current = Mathf.Min(current, cost);
        return (current, cost);
    }

    public bool IsEnoughStreak()
    {
        var (current, cost) = GetSubProgressAndCost();
        return current >= cost;
    }

    public int GetCurrentCheckPoint(int s = -1)
    {
        if (s < 0) s = gameData.streak;
        for (int i = checkPoints.Length - 1; i >= 0; i--)
        {
            if (s >= checkPoints[i])
            {
                return checkPoints[i];
            }
        }

        return checkPoints[^1];
    }

    public int GetCurrentCheckPointIndex(int s = -1)
    {
        if (s < 0) s = gameData.streak;
        for (int i = checkPoints.Length - 1; i >= 0; i--)
        {
            if (s >= checkPoints[i])
            {
                return i;
            }
        }

        return checkPoints.Length - 1;
    }

    public void HandleRewardBundle(RewardBundle rewardBundle)
    {
        switch (rewardBundle.rewardType)
        {
            case RewardType.Gold:
                ResourcesDataFragment.cur.AddGold(rewardBundle.num, "WIN_STREAK", true);
                break;
            case RewardType.Refresh:
                ResourcesDataFragment.cur.AddSwapCar(rewardBundle.num, "WIN_STREAK", true);
                break;
            case RewardType.Vip:
                ResourcesDataFragment.cur.AddVipBus(rewardBundle.num, "WIN_STREAK", true);
                break;
            case RewardType.Sort:
                ResourcesDataFragment.cur.AddSwapMinion(rewardBundle.num, "WIN_STREAK", true);
                break;
        }
    }

    public void PendingRewardBundle(RewardBundle rewardBundle)
    {
        switch (rewardBundle.rewardType)
        {
            case RewardType.Gold:
                ResourcesDataFragment.cur.PendingGold(rewardBundle.num, "WIN_STREAK");
                break;
            case RewardType.Refresh:
                ResourcesDataFragment.cur.PendingSwapCar(rewardBundle.num, "WIN_STREAK");
                break;
            case RewardType.Vip:
                ResourcesDataFragment.cur.PendingVipBus(rewardBundle.num, "WIN_STREAK");
                break;
            case RewardType.Sort:
                ResourcesDataFragment.cur.PendingSwapMinion(rewardBundle.num, "WIN_STREAK");
                break;
        }
    }

    public int GetClampStreak()
    {
        return Mathf.Min(gameData.streak, checkPoints[^1]);
    }

    public void SetLoseStreakFlag(bool isGonnaLoseStreak)
    {
        gameData.isMeantToLoseStreak = isGonnaLoseStreak;
    }

    [Serializable]
    public class Data
    {
        public DateTime baseOpenTime; // start event date
        public long baseOpenTimeLong;

        public bool isStreakUnlock;

        public bool isMeantToLoseStreak;

        public int streak;
        public bool[] giftState = new bool[10];
    }

    [Serializable]
    public class RewardBundleList
    {
        public List<RewardBundle> list;
    }

    [Serializable]
    public struct RewardBundle
    {
        public RewardType rewardType;
        public int num;
    }

    public enum RewardType
    {
        Gold,
        Refresh,
        Vip,
        Sort,
    }
}
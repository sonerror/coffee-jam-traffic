using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using JetBrains.Annotations;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Treasure Fragment", fileName = "Treasure Fragment")]
public class TreasureDataFragment : DataFragment
{
    public static TreasureDataFragment cur;

    public Data gameData;
    public TreasureSetting treasureSetting;

    private static TimeSpan splitWeekOffset = new TimeSpan(4, 0, 0, 0);
    private static TimeSpan postHalfWeek = new TimeSpan(3, 0, 0, 0);
    private static TimeSpan fullWeek = new TimeSpan(7, 0, 0, 0);

    public TreasureType CurrentTreasureType => gameData.currentTreasureType;
    public int RecordedNum { get; private set; }

#if UNITY_EDITOR
    private void OnEnable()
    {
        cur = this;

        NukeRecord();
    }
#else
    private void Awake()
    {
        // Debug.Log("LEVE DATA FRAGMENT");
        cur = this;

        NukeRecord();
    }
#endif

    private void Reset()
    {
        NukeRecord();
    }

    public override void Load()
    {
        if (!LoadData(ref gameData, key)) ResetData();
        gameData.baseOpenTime = DateTime.FromBinary(gameData.baseOpenTimeLong);
    }

    public override void Save()
    {
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
        SaveData(gameData, key);
    }

    public override void ResetData()
    {
        gameData = new Data();
        CheckTreasureType();

        gameData.lastTreasureType = gameData.currentTreasureType;
        // gameData.baseOpenTime = gameData.currentTreasureType == TreasureType.Minion ? ExtensionMethodUltimate.GetLastBeginOfWeekDate() : ExtensionMethodUltimate.GetLastBeginOfWeekDate().Add(splitWeekOffset);
        gameData.baseOpenTime = Variables.rootDate;
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
    }

    public bool IsTreasureAvailable()
    {
        if (LevelDataFragment.cur.gameData.level >= AdsManager.Ins.TreasureLevel)
        {
            gameData.isTreasureOpened = true;
        }

        return LevelDataFragment.cur.gameData.level >= AdsManager.Ins.TreasureLevel || gameData.isTreasureOpened;
    }

    public void HoldUnclaimedGift()
    {
    }

    public void ResetProgress()
    {
        gameData.currentProgress = 0;
        gameData.currentLevel = 0;
    }

    public bool CheckNewEventTime(out TimeSpan timeLeft)
    {
        var curTime = UnbiasedTime.TrueDateTime;

        var currentType = CurrentTreasureType;
        CheckTreasureType();

        var isNewEvent = IsNewEventSession();
        if (isNewEvent)
        {
            HoldUnclaimedGift();
            ResetProgress();
        }

        switch (CurrentTreasureType)
        {
            case TreasureType.Minion:
            {
                var diff = curTime.Subtract(ExtensionMethodUltimate.GetLastBeginOfWeekDate());
                timeLeft = splitWeekOffset.Subtract(diff);
                break;
            }
            case TreasureType.Bus:
            {
                var diff = curTime.Subtract(ExtensionMethodUltimate.GetLastBeginOfWeekDate().Add(splitWeekOffset));
                timeLeft = postHalfWeek.Subtract(diff);
                break;
            }
            default:
                timeLeft = new TimeSpan();
                break;
        }

        return isNewEvent;

        bool IsNewEventSession()
        {
            var checkDiff = CurrentTreasureType switch
            {
                TreasureType.Minion => gameData.lastTreasureType == TreasureType.Minion ? splitWeekOffset : postHalfWeek,
                TreasureType.Bus => gameData.lastTreasureType == TreasureType.Bus ? postHalfWeek : splitWeekOffset,
            };

            var diff = curTime.Subtract(gameData.baseOpenTime);
            var isNew = diff.Subtract(checkDiff).TotalSeconds > 0;
            if (isNew)
            {
                gameData.lastTreasureType = CurrentTreasureType;
                gameData.baseOpenTime = CurrentTreasureType == TreasureType.Minion ? ExtensionMethodUltimate.GetLastBeginOfWeekDate() : ExtensionMethodUltimate.GetLastBeginOfWeekDate().Add(splitWeekOffset);
            }

            return isNew;
        }
    }

    private void CheckTreasureType()
    {
        var lastDate = ExtensionMethodUltimate.GetLastBeginOfWeekDate();
        var curDate = UnbiasedTime.TrueDateTime;
        var timeLength = curDate.Subtract(lastDate);

        var disToSplit = timeLength.Subtract(splitWeekOffset);

        gameData.currentTreasureType = disToSplit.TotalSeconds > 0 ? TreasureType.Bus : TreasureType.Minion;
    }

    public (int, int) GetCurrentAndRequireProgress()
    {
        var curReqLevel = CurrentTreasureType switch
        {
            TreasureType.Minion => treasureSetting.minionReqPerLevel,
            TreasureType.Bus => treasureSetting.busReqPerLevel,
        };

        var index = IsOutLevel() ? gameData.currentLevel - 1 : gameData.currentLevel;
        var progress = IsOutLevel() ? 9999999 : gameData.currentProgress;

        return (progress, curReqLevel[index]);
    }

    public (int, int) GetCurrentAndRequireTruncateProgress()
    {
        var curReqLevel = CurrentTreasureType switch
        {
            TreasureType.Minion => treasureSetting.minionReqPerLevel,
            TreasureType.Bus => treasureSetting.busReqPerLevel,
        };

        var index = IsOutLevel() ? gameData.currentLevel - 1 : gameData.currentLevel;
        var progress = IsOutLevel() ? 9999999 : gameData.currentProgress;

        var cost = curReqLevel[index];
        var current = Mathf.Min(progress, cost);
        return (current, cost);
    }

    public bool IsFullAtCurrentLevel()
    {
        var (current, cost) = GetCurrentAndRequireProgress();
        return current >= cost;
    }

    public int GetCurrentCost()
    {
        var curReqLevel = CurrentTreasureType switch
        {
            TreasureType.Minion => treasureSetting.minionReqPerLevel,
            TreasureType.Bus => treasureSetting.busReqPerLevel,
        };

        var cost = curReqLevel[gameData.currentLevel];
        return cost;
    }

    public int GetNextCost() // negative if not have next
    {
        var curReqLevel = CurrentTreasureType switch
        {
            TreasureType.Minion => treasureSetting.minionReqPerLevel,
            TreasureType.Bus => treasureSetting.busReqPerLevel,
        };

        var targetIndex = gameData.currentLevel + 1;
        var cost = targetIndex < curReqLevel.Length ? curReqLevel[targetIndex] : -1;
        return cost;
    }

    public bool IncreaseLevel() //return if increase-able
    {
        if (!IsFullAtCurrentLevel() || IsOutLevel()) return false;
        var cost = GetCurrentCost();
        gameData.currentProgress -= cost;
        gameData.currentLevel++;
        return true;
    }

    public bool IsOutLevel()
    {
        var curReqLevel = CurrentTreasureType switch
        {
            TreasureType.Minion => treasureSetting.minionReqPerLevel,
            TreasureType.Bus => treasureSetting.busReqPerLevel,
        };

        return gameData.currentLevel >= curReqLevel.Length;
    }

    public int ProcessRecord()
    {
        if (!IsTreasureAvailable() || IsOutLevel()) return 0;

        gameData.currentProgress += RecordedNum;

        var gained = RecordedNum;
        NukeRecord();

        return gained;
    }

    public void NukeRecord()
    {
        RecordedNum = 0;
    }

    public bool IsProgressOverLoad()
    {
        var cost = GetCurrentCost();
        var nextCost = GetNextCost();

        if (nextCost < 0) return false;
        return gameData.currentProgress >= cost + nextCost;
    }

    public void RecordObject(int amount, TreasureType treasureType)
    {
        if (!IsTreasureAvailable()) return;
        if (CurrentTreasureType != treasureType) return;
        if (BusLevelSO.active == null) return;
        var mul = BusLevelSO.active.busMapHardness switch
        {
            BusMapHardness.Easy => 1,
            BusMapHardness.Hard => 2,
            BusMapHardness.SuperHard => 3,
        };

        RecordedNum += amount * mul;

        // Debug.Log("RECORD " + RecordedNum + "   " + amount + "   " + mul);
    }

    public RewardBundle GetCurrentGift()
    {
        var curRewardList = CurrentTreasureType switch
        {
            TreasureType.Minion => treasureSetting.minionRewardBundles,
            TreasureType.Bus => treasureSetting.busRewardBundles,
        };

        var index = IsOutLevel() ? gameData.currentLevel - 1 : gameData.currentLevel;

        return curRewardList[index];
    }

    public RewardBundle[] GetChestGiftReward(RewardType rewardType)
    {
        return rewardType switch
        {
            RewardType.Chest_1 => treasureSetting.chest_1,
            RewardType.Chest_2 => treasureSetting.chest_2,
            RewardType.Chest_3 => treasureSetting.chest_3,
            RewardType.Gift_1 => treasureSetting.gift_1,
            RewardType.Gift_2 => treasureSetting.gift_2,
            RewardType.Gift_3 => treasureSetting.gift_3,
        };
    }

    public void PendingReward(RewardBundle rewardBundle, string goldPlacement = "")
    {
        switch (rewardBundle.GetRewardType())
        {
            case RewardType.Gold:
                ResourcesDataFragment.cur.PendingGold(rewardBundle.num, goldPlacement);
                break;
            case RewardType.SwapCar:
                // ResourcesDataFragment.cur.PendingSwapCar(rewardBundle.num);
                ResourcesDataFragment.cur.AddSwapCar(rewardBundle.num, goldPlacement);
                break;
            case RewardType.VipBus:
                // ResourcesDataFragment.cur.PendingVipBus(rewardBundle.num);
                ResourcesDataFragment.cur.AddVipBus(rewardBundle.num, goldPlacement);
                break;
            case RewardType.SwapMinion:
                // ResourcesDataFragment.cur.PendingSwapMinion(rewardBundle.num);
                ResourcesDataFragment.cur.AddSwapMinion(rewardBundle.num, goldPlacement);
                break;
            case RewardType.Chest_1:
                var list = GetChestGiftReward(RewardType.Chest_1);
                for (int i = 0; i < list.Length; i++) PendingReward(list[i]);
                break;
            case RewardType.Chest_2:
                var list1 = GetChestGiftReward(RewardType.Chest_2);
                for (int i = 0; i < list1.Length; i++) PendingReward(list1[i]);
                break;
            case RewardType.Chest_3:
                var list2 = GetChestGiftReward(RewardType.Chest_3);
                for (int i = 0; i < list2.Length; i++) PendingReward(list2[i]);
                break;
            case RewardType.Gift_1:
                var list3 = GetChestGiftReward(RewardType.Gift_1);
                for (int i = 0; i < list3.Length; i++) PendingReward(list3[i]);
                break;
            case RewardType.Gift_2:
                var list4 = GetChestGiftReward(RewardType.Chest_2);
                for (int i = 0; i < list4.Length; i++) PendingReward(list4[i]);
                break;
            case RewardType.Gift_3:
                var list5 = GetChestGiftReward(RewardType.Chest_3);
                for (int i = 0; i < list5.Length; i++) PendingReward(list5[i]);
                break;
        }
    }

    [Serializable]
    public class Data
    {
        public DateTime baseOpenTime;
        public long baseOpenTimeLong;

        public TreasureType lastTreasureType;
        public TreasureType currentTreasureType;

        public int currentProgress;
        public int currentLevel;

        public bool isTreasureOpened;
    }

    [Serializable]
    public class TreasureSetting
    {
        public RewardBundle[] minionRewardBundles;
        public RewardBundle[] busRewardBundles;

        public RewardBundle[] chest_1;
        public RewardBundle[] chest_2;
        public RewardBundle[] chest_3;
        public RewardBundle[] gift_1;
        public RewardBundle[] gift_2;
        public RewardBundle[] gift_3;

        public int[] minionReqPerLevel;
        public int[] busReqPerLevel;
    }

    [Serializable]
    public struct RewardBundle
    {
        public int rewardFlag;
        public int num;

        public RewardType GetRewardType()
        {
            return (RewardType)rewardFlag;
        }
    }

    public enum TreasureType
    {
        Minion,
        Bus,
    }

    public enum RewardType
    {
        Gold,
        SwapCar,
        VipBus,
        SwapMinion,
        Chest_1,
        Chest_2,
        Chest_3,
        Gift_1,
        Gift_2,
        Gift_3,
    }
}
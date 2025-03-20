using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Daily Fragment", fileName = "Daily Fragment")]
public class DailyRewardDataFragment : DataFragment
{
    public static DailyRewardDataFragment cur;

    public Data gameData;


#if UNITY_EDITOR
    private void OnEnable()
    {
        cur = this;
    }
#else
    private void Awake()
    {
        cur = this;
    }
#endif

    public override void Load()
    {
        if (!LoadData(ref gameData, key)) ResetData();
        gameData.baseOpenTime = DateTime.FromBinary(gameData.baseOpenTimeLong);

        Timer.ScheduleCondition(() => UnbiasedTime.IsValidTime, () => CheckNewDailyDay(out _));
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
    }

    public bool CheckNewDailyDay(out TimeSpan timeLeft)
    {
        TimeSpan diff = UnbiasedTime.TrueDateTime.Subtract(gameData.baseOpenTime);
        timeLeft = TimeSpan.FromSeconds(Variables.TIME_SPACE_HALF_DAY).Subtract(diff);

        bool isNewCycle = diff.TotalSeconds >= Variables.TIME_SPACE_HALF_DAY;
        if (isNewCycle)
        {
            OnNewDayRefresh();
            Save();
            return true;
        }

        return false;
    }

    public void OnNewDayRefresh()
    {
        for (int i = 0; i < gameData.itemFlags.Count; i++)
        {
            gameData.itemFlags[i] = false;
        }

        // gameData.lastFreeTime = 0;
    }

    public void PrepareCountDown()
    {
        gameData.baseOpenTime = UnbiasedTime.TrueDateTime;
    }

    public bool CheckIfItsTimeToFree(out TimeSpan timeLeft)
    {
        if (gameData.lastFreeTime + Variables.TIME_SPACE_HOUR > (UnbiasedTime.TrueDateTime.Subtract(Variables.rootDate).TotalSeconds))
        {
            timeLeft = TimeSpan.FromSeconds(Variables.TIME_SPACE_HOUR - ((UnbiasedTime.TrueDateTime.Subtract(Variables.rootDate).TotalSeconds) - gameData.lastFreeTime));
            return false;
        }

        timeLeft = TimeSpan.Zero;
        return true;
    }

    [Serializable]
    public class Data
    {
        public DateTime baseOpenTime;
        public long baseOpenTimeLong;

        public double lastFreeTime;

        public List<bool> itemFlags = new List<bool> { false, false, false, false }; // exclude first free hour-ly item
    }
}
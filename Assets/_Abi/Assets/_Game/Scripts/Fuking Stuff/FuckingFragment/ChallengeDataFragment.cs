using System;
using System.Collections.Generic;
using UnityEngine;
using static CalendarCalculator;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Challenge Fragment", fileName = "Challenge Fragment")]
public class ChallengeDataFragment : DataFragment
{
    public static ChallengeDataFragment cur;

    public Data gameData;

#if UNITY_EDITOR
    private void OnEnable()
    {
        cur = this;
    }
#else
    private void Awake()
    {
        // Debug.Log("LEVE DATA FRAGMENT");
        cur = this;
    }
#endif

    public override void Load()
    {
        if (!LoadData(ref gameData, key)) ResetData();
        // var tmp = LoadDataTest(gameData, key);
        // Debug.Log("CHECKKKK  " + (tmp == gameData));
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
        gameData.baseOpenTime = new DateTime(2022, 1, 1, 0, 0, 0);
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();

        CheckNewDay();

        gameData.isChallengeTut = true;
    }

    public bool IsDayTicked(int day)
    {
        return gameData.tickedDay[day];
    }

    public void TickDay(int day)
    {
        gameData.tickedDay[day] = true;
    }

    public int GetTotalTickedDays()
    {
        var list = gameData.tickedDay;
        int num = 0;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i]) num++;
        }

        return num;
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
            if (GetCurrentMonth(true) != gameData.currentMonth)
            {
                gameData.currentMonth = GetCurrentMonth(true);

                var list = gameData.tickedDay;
                for (int i = 0; i < list.Length; i++) list[i] = false;

                gameData.isUnlockAll = false;
            }

            gameData.isChallengeTut = false;

            Save();
            return true;
        }

        return false;
    }

    public bool IsLoadTutChallenge(int day)
    {
        if (gameData.isChallengeTut)
        {
            if (gameData.markTutDay == -1) gameData.markTutDay = day;
            return gameData.markTutDay == day;
        }

        return false;
    }

    [Serializable]
    public class Data
    {
        public DateTime baseOpenTime;
        public long baseOpenTimeLong;

        public bool isUnlockAll;
        public int currentMonth;

        public bool[] rewardState = new[] { false, false, false, false };
        public bool[] tickedDay = new bool[32]; // day from 1 -> 31 , ignore 0

        public bool isChallengeTut = true;
        public bool isTutOpened;
        public bool isTutCamOpened;
        public bool isTutWoodOpened;

        public int markTutDay = -1;

        public Data()
        {
            currentMonth = GetCurrentMonth();
        }
    }
}
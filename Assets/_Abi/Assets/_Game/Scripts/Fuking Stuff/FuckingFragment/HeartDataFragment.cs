using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Heart Fragment", fileName = "Heart Fragment")]
public class HeartDataFragment : DataFragment
{
    public static HeartDataFragment cur;

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

        gameData.isClaimMoreToday = true;
    }

    public bool CheckNewDay(out TimeSpan timeLeft)
    {
        TimeSpan diff = UnbiasedTime.TrueDateTime.Subtract(gameData.baseOpenTime);
        timeLeft = TimeSpan.FromSeconds(86399).Subtract(diff);

        bool isNewDay = false;
        while (diff.TotalSeconds >= Variables.TIME_SPACE_DAY)
        {
            isNewDay = true;
            gameData.baseOpenTime = gameData.baseOpenTime.AddDays(Math.Floor(diff.TotalDays));
            diff = UnbiasedTime.TrueDateTime.Subtract(gameData.baseOpenTime);
            timeLeft = TimeSpan.FromSeconds(86399).Subtract(diff);
        }

        if (isNewDay)
        {
            if (gameData.isClaimedToday) gameData.trackingDayIndex++;
            gameData.isClaimedToday = false;
            gameData.isClaimMoreToday = true;

            if ( /*!isConsecutiveDay ||*/ gameData.trackingDayIndex >= 7)
            {
                // ResetTick();
                gameData.trackingDayIndex = 0;
            }

            Save();
            return true;
        }

        return false;
    }

    public int GetCurrentDayIndex()
    {
        return gameData.trackingDayIndex;
    }

    // public void TickDay(int index)
    // {
    //     gameData.tickList[index] = true;
    // }

    // public void ResetTick()
    // {
    // for (int i = 0; i < gameData.tickList.Length; i++) gameData.tickList[i] = false;
    // }

    [Serializable]
    public class Data
    {
        public DateTime baseOpenTime;
        public long baseOpenTimeLong;

        public int trackingDayIndex;

        public bool isClaimedToday;
        public bool isClaimMoreToday;
        // public bool[] tickList = new bool[7];

        // public Data()
        // {
        //     for (int i = 0; i < tickList.Length; i++) tickList[i] = true;
        // }
    }
}
using System;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Quest Fragment", fileName = "Quest Fragment")]
public class QuestDataFragment : DataFragment
{
    public static QuestDataFragment cur;

    public VoidEventChannelSO onGainWeeklyExp;

    public Data gameData;

    public Quest[] questList;

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

        for (int i = 0; i < cur.questList.Length; i++)
        {
            cur.questList[i].Load();
        }

        CheckNewQuestWeek(out _);
    }

    public override void Save()
    {
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
        SaveData(gameData, key);

        for (int i = 0; i < cur.questList.Length; i++)
        {
            cur.questList[i].Save();
        }
    }

    public override void ResetData()
    {
        gameData = new Data();

        gameData.baseOpenTime = ExtensionMethodUltimate.GetLastBeginOfWeekDate();
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();

        ResetQuestData();
    }

    public bool CheckNewQuestDay(out TimeSpan timeLeft) //call for only one script
    {
        TimeSpan diff = UnbiasedTime.TrueDateTime.Subtract(cur.gameData.baseOpenTime);
        timeLeft = TimeSpan.FromSeconds(86399).Subtract(diff);

        bool isNewDay = false;
        while (diff.TotalSeconds >= 86400)
        {
            isNewDay = true;
            cur.gameData.baseOpenTime = cur.gameData.baseOpenTime.AddDays(Math.Floor(diff.TotalDays));
            diff = UnbiasedTime.TrueDateTime.Subtract(cur.gameData.baseOpenTime);
            timeLeft = TimeSpan.FromSeconds(86399).Subtract(diff);
        }

        if (isNewDay)
        {
            ResetQuestData();
            return true;
        }

        return false;
    }

    public bool CheckNewQuestWeek(out TimeSpan timeLeft) //call for only one script
    {
        TimeSpan diff = UnbiasedTime.TrueDateTime.Subtract(cur.gameData.baseOpenTime);
        timeLeft = TimeSpan.FromSeconds(Variables.TIME_SPACE_WEEK).Subtract(diff);

        // Debug.Log("CHECK " + diff + "  \n" + timeLeft + " \n" + cur.gameData.baseOpenTime + " \n" + DateTime.Now + " \n" + ExtensionMethodUltimate.GetLastBeginOfWeekDate());
        bool isNewWeek = false;
        while (diff.TotalSeconds >= Variables.TIME_SPACE_WEEK)
        {
            isNewWeek = true;
            cur.gameData.baseOpenTime = cur.gameData.baseOpenTime.AddDays(TimeSpan.FromDays(7).Days);
            diff = UnbiasedTime.TrueDateTime.Subtract(cur.gameData.baseOpenTime);
            timeLeft = TimeSpan.FromSeconds(Variables.TIME_SPACE_WEEK).Subtract(diff);
        }

        if (isNewWeek)
        {
            ResetQuestData();
            return true;
        }

        return false;
    }

    public void AddWeeklyEnergy(int amount)
    {
        gameData.weeklyEnergy = Mathf.Max(0, gameData.weeklyEnergy + amount);
        SaveData(gameData, key);

        onGainWeeklyExp.RaiseEvent();
    }

    public void ResetQuestData()
    {
        gameData.weeklyEnergy = 0;
        gameData.rewardState = new[] { false, false, false };
        for (int i = 0; i < questList.Length; i++)
        {
            questList[i].ResetData();
        }
    }

    [Serializable]
    public class Data
    {
        public DateTime baseOpenTime;
        public long baseOpenTimeLong;

        public int weeklyEnergy;
        public bool[] rewardState = new[] { false, false, false };

        // public Data()
        // {
        //     baseOpenTime = ExtensionMethodUltimate.GetLastBeginOfWeekDate();
        //     baseOpenTimeLong = baseOpenTime.ToBinary();
        //
        //     Debug.Log("INIT DATA " + baseOpenTime);
        // }
    }
}
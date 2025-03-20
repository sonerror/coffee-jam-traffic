using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Quest/Quest", fileName = "Quest ")]
public class Quest : ScriptableObject
{
    // public QuestType questType;

    public QuestSubProgress[] questSubProgressList;


    [SerializeField] private string uniqueKey;
    [SerializeField] private QuestData questData;

    // private void Awake()
    // {
    //     // uniqueKey = "Quest" + questType + questSubProgressList.Length;
    //
    //     Load();
    // }

    public bool MakeReward() //return if make reward success
    {
        if (questData.isClaimed) return false;
        questData.isClaimed = true;

        //make reward
        return true;
    }

    public bool CheckFinish()
    {
        bool isDOne = true;
        for (int i = 0; i < questSubProgressList.Length; i++)
        {
            isDOne &= questSubProgressList[i].CheckComplete();
        }

        return isDOne;
    }

    public bool IsClaimed()
    {
        return questData.isClaimed;
    }

    public void PushProgress(QuestIdentify questIdentify, int value)
    {
        for (int i = 0; i < questSubProgressList.Length; i++)
        {
            var quest = questSubProgressList[i];
            if (quest.questIdentify == questIdentify)
            {
                quest.current = Mathf.Min(quest.current + value, quest.goal);
                // Debug.Log("QUEST   " + questType.ToString());
            }
        }
    }

    public void UpdateProgress(QuestIdentify questIdentify, int value)
    {
        for (int i = 0; i < questSubProgressList.Length; i++)
        {
            if (questSubProgressList[i].questIdentify == questIdentify) questSubProgressList[i].current = value;
        }
    }

    public void ResetData()
    {
        questData = New();
        Save();
    }

    public void Load()
    {
        questData = null;
        questData = JsonUtility.FromJson<QuestData>(PlayerPrefs.GetString(uniqueKey)) ?? New();

        for (int i = 0; i < questSubProgressList.Length; i++)
        {
            questSubProgressList[i].current = questData.questProgressDatas[i].current;
        }

        // Debug.Log("QUEST LOAD" + questSubProgressList[0].questIdentify + "  " + CheckFinish() + "  " + questData.questProgressDatas[0].current);
    }

    public void Save()
    {
        for (int i = 0; i < questSubProgressList.Length; i++)
        {
            questData.questProgressDatas[i].current = questSubProgressList[i].current;
        }

        string data = JsonUtility.ToJson(questData);
        PlayerPrefs.SetString(uniqueKey, data);
        // Debug.Log("QUEST SAVE" + questSubProgressList[0].questIdentify + "  " + CheckFinish());
    }

    private QuestData New()
    {
        for (int i = 0; i < questSubProgressList.Length; i++)
        {
            questSubProgressList[i].current = 0;
        }

        QuestData tmp = new QuestData();
        for (int i = 0; i < questSubProgressList.Length; i++)
        {
            tmp.questProgressDatas.Add(new QuestProgressData(questSubProgressList[i].questIdentify));
        }

        return tmp;
    }

    [Serializable]
    public class QuestData
    {
        public bool isClaimed;
        public List<QuestProgressData> questProgressDatas = new List<QuestProgressData>();
    }

    [Serializable]
    public class QuestSubProgress
    {
        public QuestIdentify questIdentify;
        [HideInInspector] public int current;
        public int goal;
        public string targetDescription;

        public bool CheckComplete()
        {
            return current >= goal;
        }
    }


    [Serializable]
    public class QuestProgressData
    {
        public QuestIdentify questIdentify;
        public int current;

        public QuestProgressData(QuestIdentify questIdentify)
        {
            this.questIdentify = questIdentify;
            current = 0;
        }
    }

    public void LogCurrent()
    {
        // Debug.Log("QUEST " + questData.questProgressDatas[0].current + "   " + questSubProgressList[0].goal);
    }
}
// public enum QuestType
// {
//     GetFreeDiamond,
//     WatchAdsForGold,
//     OpenChest,
//     PassLevel,
//     GachaDream,
//     FuckEnemyUp,
//     UseSpin,
//     FinishDaily,
//     CampDestroy,
// }

public enum QuestIdentify // sub quest
{
    CollectHexa,
    UseRefresh,
    UseHammer,
    CollectCoin,
    UseSwap,
    UseSpinWheel,
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHelper
{
    // private static readonly QuestDataFragment GameDataFragment = QuestDataFragment.cur;
    //
    // public static bool CheckNewQuestDay(out System.TimeSpan timeLeft) //call for only one script
    // {
    //     System.TimeSpan diff = System.DateTime.Now.Subtract(GameDataFragment.baseOpenTime);
    //     timeLeft = System.TimeSpan.FromSeconds((double)86399).Subtract(diff);
    //     if (diff.TotalSeconds >= (double)86400)
    //     {
    //         GameDataFragment.baseOpenTime = GameDataFragment.baseOpenTime.AddDays(System.Math.Floor(diff.TotalDays));
    //         diff = System.DateTime.Now.Subtract(GameDataFragment.baseOpenTime);
    //         timeLeft = System.TimeSpan.FromSeconds((double)86399).Subtract(diff);
    //
    //         ResetQuestData();
    //         return true;
    //     }
    //
    //     return false;
    // }
    //
    // public static void ResetQuestData()
    // {
    //     if (Quest.questDict.Count == 0) return;
    //     foreach (var q in Quest.questDict)
    //     {
    //         q.Value.ResetData();
    //     }
    // }

    // public bool CheckNewDailyDay(out System.TimeSpan timeLeft)
    // {
    //     System.TimeSpan diff = System.DateTime.Now.Subtract(gameData.baseDailyLoginOpenTime);
    //     timeLeft = System.TimeSpan.FromSeconds((double)86399).Subtract(diff);
    //     if (diff.TotalSeconds >= (double)86400)
    //     {
    //         gameData.baseDailyLoginOpenTime = gameData.baseDailyLoginOpenTime.AddDays(System.Math.Floor(diff.TotalDays));
    //         diff = System.DateTime.Now.Subtract(gameData.baseDailyLoginOpenTime);
    //         timeLeft = System.TimeSpan.FromSeconds((double)86399).Subtract(diff);
    //         return true;
    //     }
    //
    //     return false;
    // }
    //
    // public bool CheckNewSpinDay()
    // {
    //     System.TimeSpan diff = System.DateTime.Now.Subtract(gameData.baseSpinOpenTIme);
    //     if (diff.TotalSeconds >= (double)86400)
    //     {
    //         gameData.baseSpinOpenTIme = gameData.baseSpinOpenTIme.AddDays(System.Math.Floor(diff.TotalDays));
    //         Debug.Log("SPINNN TRUEEE");
    //         return true;
    //     }
    //
    //     return false;
    // }
    //
    // public void LoadTimeSub()
    // {
    //     gameData.baseOpenTime = System.DateTime.FromBinary(gameData.baseOpenTimeLong);
    //     Debug.Log("Json Load " + gameData.baseSpinOpenTIme + "  " + gameData.baseSpinOpenTimeLong + "  " + gameData.baseSpinOpenTIme.ToBinary());
    //     gameData.baseDailyLoginOpenTime = System.DateTime.FromBinary(gameData.baseDailyLoginOpenTimeLong);
    //     gameData.baseIdleTime = System.DateTime.FromBinary(gameData.baseIdleTimeLong);
    //     gameData.baseSpinOpenTIme = System.DateTime.FromBinary(gameData.baseSpinOpenTimeLong);
    // }
    //
    // public void SaveTimeSub()
    // {
    //     gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
    //     Debug.Log("Json Save " + gameData.baseSpinOpenTIme + "  " + gameData.baseSpinOpenTimeLong + "  " + gameData.baseSpinOpenTIme.ToBinary());
    //     gameData.baseDailyLoginOpenTimeLong = gameData.baseDailyLoginOpenTime.ToBinary();
    //     gameData.baseIdleTimeLong = gameData.baseIdleTime.ToBinary();
    //     gameData.baseSpinOpenTimeLong = gameData.baseSpinOpenTIme.ToBinary();
    //     // string formatString = $"{0} - {1} - {2} ";
    //     // string.Format(formatString,);
    // }
    //
    // public void LoadAchivementData()
    // {
    //     for (int i = 0; i < gameData.achivementDataList.Count; i++)
    //     {
    //         AssetHolder.ins.achivementList[i].current = gameData.achivementDataList[i].current;
    //         AssetHolder.ins.achivementList[i].curIndex = gameData.achivementDataList[i].maxINdex;
    //     }
    // }
    //
    // public void SaveAchivementData()
    // {
    //     for (int i = 0; i < gameData.achivementDataList.Count; i++)
    //     {
    //         gameData.achivementDataList[i].current = AssetHolder.ins.achivementList[i].current;
    //         gameData.achivementDataList[i].maxINdex = AssetHolder.ins.achivementList[i].curIndex;
    //     }
    // }
    //
    // public void ResetQuestClaimSatte()
    // {
    //     for (int i = 0; i < gameData.questRewardClaimState.Count; i++)
    //     {
    //         gameData.questRewardClaimState[i] = false;
    //     }
    // }
    // public void ResetQuestChest()
    // {
    //     for (int i = 0; i < gameData.chestDailyQuestClaimState.Length; i++)
    //     {
    //         gameData.chestDailyQuestClaimState[i] = false;
    //     }
    // }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCaller
{
    public static void CallQuest(QuestIdentify questIdentify, int value, QuestCallType questCallType = QuestCallType.Push)
    {
        // Debug.Log("QUEST CALLER  " + questIdentify.ToString());
        if (QuestDataFragment.cur.questList.Length == 0) return;
        // Debug.Log("QUEST CALLER PRE  " + questIdentify.ToString());
        if (questCallType == QuestCallType.Push)
        {
            foreach (var q in QuestDataFragment.cur.questList)
            {
                if (q.CheckFinish()) continue;
                q.PushProgress(questIdentify, value);
            }
        }
        else
        {
            foreach (var q in QuestDataFragment.cur.questList)
            {
                if (q.CheckFinish()) continue;
                q.UpdateProgress(questIdentify, value);
            }
        }
    }

    // public static void CallAchivement(AchiveIdentify achiveIdentify, int value)
    // {
    //     AssetHolder.ins.achivementList[(int)achiveIdentify].current += value;
    // }
}

public enum QuestCallType
{
    Push,
    Update,
}
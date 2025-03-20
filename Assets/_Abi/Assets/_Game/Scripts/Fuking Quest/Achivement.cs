using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achivement
{
    public AchiveIdentify achiveType;
    public string[] achiveName;
    public int[] achiveGoal;
    [HideInInspector] public int current;
    [HideInInspector] public int curIndex;
    [SerializeField] private int goldReward = 1500;

    public bool GetCurrentAchiveState() //to know if achive is done
    {
        if (curIndex == achiveGoal.Length) return false;
        if (current >= achiveGoal[curIndex + 1])
        {
            curIndex++;
            return true;
        }

        return false;
    }
}

[System.Serializable]
public class AchivementData
{
    public AchiveIdentify achiveIdentify;
    public int current;
    public int maxINdex;

    public AchivementData(AchiveIdentify achiveIdentify)
    {
        this.achiveIdentify = achiveIdentify;
        current = 0;
        maxINdex = -1;
    }
}

public enum AchiveIdentify
{
    KillEnemy,
    GoldDigger,
    CardCArd,
    Diamond,
    Chest,
    SaveToddler,
    UpgradeCard
}
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.Serialization;

[System.Serializable]
public class GameData
{
    public int dataKey;

    public bool isFirstOpen;
    public int itemDictVer;

    public bool isMusicOn;
    public bool isSoundOn;
    public float musicVolume;
    public float soundVolume;
    public bool isVibrateOn;

    public System.DateTime baseOpenTime;
    public long baseOpenTimeLong;

    public int levelCheckpointStart;
    public int levelCheckPointEnd;

    public int dayPlayed;
    public int rewardWatched;
    public int interWatched;
    public float purchasedValue;

    public int firstTryLevelMark;

    public bool isAmbulanceCarTutShowed;
    public bool isPoliceCarTutShowed;
    public bool isFireTruckCarTutShowed;
    public bool isVipCarTutShowed;

    /// ///////////////////////////////////////////////////////////////
    public void INit()
    {
        dataKey = 010320232;
        isFirstOpen = true;

        isMusicOn = true;
        isSoundOn = true;
        musicVolume = 1;
        soundVolume = 1;
        isVibrateOn = true;

        baseOpenTime = new System.DateTime(2022, 1, 1, 0, 0, 0);
        baseOpenTimeLong = baseOpenTime.ToBinary();

        levelCheckpointStart = 1;
        levelCheckPointEnd = 1;

        firstTryLevelMark = -1;

        InitItemDict();
    }

    private void InitItemDict()
    {
    }
}
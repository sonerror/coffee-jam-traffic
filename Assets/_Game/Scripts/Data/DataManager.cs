using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class DataManager : SingleTons<DataManager>
{
    [SerializeField] private string fileName = "GameData";
    private DataConvert dataConvert;
    public GameData gameData;

    //NOTE: Global Game Data
    private const int dataKey = 010320232;
    private const int itemDictVer = 26072024;

    public DataFragment[] dataFragmentList;

    private const string TIME_PLAY = "TimePlayPro";
    private const string FAIL_TIME = "Fail time";

    public bool IsDoneLoadData { get; private set; }

    private IEnumerator Start()
    {
        // yield return new WaitUntil(() => !UnbiasedTime.cur.isChecking);
        yield return null;
        dataConvert = new DataConvert(Application.persistentDataPath, fileName);
        LoadData();

        IsDoneLoadData = true;
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    private void NewGame()
    {
        gameData = new GameData();
        gameData.INit();
    }

    private void LoadData()
    {
        gameData = dataConvert.Load();
        for (int i = 0; i < dataFragmentList.Length; i++) dataFragmentList[i].Load();

        PostLoadingFragment();

        // Debug.Log("LOAD DATA   ");

        if (gameData == null || gameData.dataKey != dataKey) //        if (gameData is not { dataKey: dataKey })
        {
            NewGame();
            UpdateItemDict();
            return;
        }

        LoadTime();
        UpdateItemDict();
    }

    private void UpdateItemDict()
    {
        if (gameData != null && gameData.itemDictVer != itemDictVer)
        {
            gameData.itemDictVer = itemDictVer;

            for (int i = 0; i < dataFragmentList.Length; i++)
            {
                dataFragmentList[i].Update();
            }
        }
    }

    public void SaveData(bool isFragmentSave = true)
    {
        SaveTime();
        dataConvert.Save(gameData);
        if (FirebaseManager.Ins != null) FirebaseManager.Ins.OnSetUserProperty();

        if (!isFragmentSave) return;
        for (int i = 0; i < dataFragmentList.Length; i++) dataFragmentList[i].Save();
    }

    private bool isNeedToSave = true;

    private void OnApplicationPause(bool isPause) // NOTE: for android
    {
        if (isPause)
        {
            if (isNeedToSave) SaveData();
            isNeedToSave = false;
        }
        else
        {
            isNeedToSave = true;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_EDITOR
        return;
#endif
        if (hasFocus)
        {
            isNeedToSave = true;
        }
        else
        {
            if (isNeedToSave) SaveData();
            isNeedToSave = false;
        }
    }

    private void OnApplicationQuit() // NOTE: for window
    {
        SaveData();
    }

#if UNITY_EDITOR
    [ContextMenu("Delete Data")]
    public void DeleteData()
    {
        // dataConvert ??= new DataConvert(Application.persistentDataPath, fileName);
        // dataConvert.DeleteData();

        PlayerPrefs.DeleteAll();
    }

    [ContextMenu("Reset Fragment")]
    public void ResetFragment()
    {
        for (int i = 0; i < dataFragmentList.Length; i++) dataFragmentList[i].ResetData();
    }

    [ContextMenu("Save Cur Data")]
    public void SaveCurData()
    {
        dataConvert ??= new DataConvert(Application.persistentDataPath, fileName);
        SaveData();
    }

    [ContextMenu("LOad Data")]
    public void LoadDataOnEditor()
    {
        dataConvert ??= new DataConvert(Application.persistentDataPath, fileName);
        LoadData();
    }
#endif

    private void PostLoadingFragment()
    {
    }

    public bool CheckNewDay(out TimeSpan timeLeft) //call for only one script
    {
        var diff = UnbiasedTime.TrueDateTime.Subtract(gameData.baseOpenTime);
        timeLeft = TimeSpan.FromSeconds(Variables.TIME_SPACE_DAY).Subtract(diff);

        bool isNewDay = false;
        while (diff.TotalSeconds >= Variables.TIME_SPACE_DAY)
        {
            isNewDay = true;
            gameData.baseOpenTime = gameData.baseOpenTime.AddDays(Math.Floor(diff.TotalDays));
            diff = UnbiasedTime.TrueDateTime.Subtract(gameData.baseOpenTime);
            timeLeft = TimeSpan.FromSeconds(Variables.TIME_SPACE_DAY).Subtract(diff);
        }

        return isNewDay;
    }

    private void LoadTime()
    {
        gameData.baseOpenTime = DateTime.FromBinary(gameData.baseOpenTimeLong);
    }

    private void SaveTime()
    {
        gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
    }

    public void LogStart()
    {
        int level = LevelDataFragment.cur.GetFireBaseLevel();

        if (level >= gameData.levelCheckpointStart)
        {
            gameData.levelCheckpointStart = level + 1;
            FirebaseManager.Ins.checkpoint_level(level.ToString());
        }

        FirebaseManager.Ins.g_game_start(level.ToString());
    }

    public void LogComplete()
    {
        int level = LevelDataFragment.cur.GetFireBaseLevel();

        if (level >= gameData.levelCheckPointEnd)
        {
            gameData.levelCheckPointEnd = level + 1;
            FirebaseManager.Ins.check_point_end(level.ToString());
        }

        FirebaseManager.Ins.g_game_end(level.ToString(), true, IsFirstPlayLevel(),GrandManager.ins.GetCompleteDuration());
    }

    public void LogFail()
    {
        int level = LevelDataFragment.cur.GetFireBaseLevel();

        FirebaseManager.Ins.g_game_end(level.ToString(), false, IsFirstPlayLevel(),GrandManager.ins.GetCompleteDuration());
    }

    public bool IsFirstPlayLevel()
    {
        var curLevel = LevelDataFragment.cur.gameData.level;
        if (gameData.firstTryLevelMark != curLevel)
        {
            gameData.firstTryLevelMark = curLevel;
            return true;
        }
        else
        {
            return false;
        }
    }
}
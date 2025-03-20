using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Bus;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Level Fragment", fileName = "Level Fragment")]
public class LevelDataFragment : DataFragment
{
    public static bool IsMultiLayer = false;
    public static LevelDataFragment cur;

    public BusLevelSO[] busLevelSoList
    {
        get
        {
            if (IsMultiLayer)
            {
                return busLevelSoList2;
            }
            else
            {
                return busLevelSoList1;
            }
        }
    }
    [SerializeField] private bool _isActive;

    public BusLevelSO[] busLevelSoList1;
    public BusLevelSO[] busLevelSoList2;

    
    public Data gameData;

    public CanvasSpecialCarTut.TutData[] tutDatas;

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (_isActive)
        {
            
        cur = this;
        }
    }
#else
    private void Awake()
    {
        // Debug.Log("LEVE DATA FRAGMENT");
        if(_isActive)
        {
            cur = this;

        }
    }
#endif
    public override void Load()
    {
#if UNITY_EDITOR
        cur = this;
#endif
        if (!LoadData(ref gameData, key)) ResetData();
    }

    public override void Save()
    {
        SaveData(gameData, key);
    }

    public override void ResetData()
    {
        gameData = new Data();
    }

    public void LoadLevel()
    {
        var data = busLevelSoList[GetLevelIndex()];

        // Debug.Log("LOAD LEVEL DATA" + (data != null) + "   " + busLevelSoList.Length);

        
        data.LoadLevel();
    }

    public int GetLevelIndex()
    {
        var length = busLevelSoList.Length;
        var index = gameData.level;

        if (index < length) return index;

        var exceedIndex = index - length;
        const int pushForwardNum = 10;
        var remainLength = length - pushForwardNum;
        var loopedIndex = exceedIndex % remainLength;
        return loopedIndex + pushForwardNum;
    }

    public int GetFireBaseLevel()
    {
        return gameData.level + 1;
    }

    public void AscendLevel()
    {
        gameData.level++;

        SpinDataFragment.cur.PassedLevel();

        Save();
    }

    public bool CheckTutLevel(out int flag) //return if need to show tut
    {
        flag = -1;

        if (!gameData.isSwapCarTutShowed && gameData.level == Config.SwapCarTutLevel)
        {
            flag = 0;
            return true;
        }

        if (!gameData.isVipBusTutShowed && gameData.level == Config.VipBusTutLevel)
        {
            flag = 1;
            return true;
        }

        if (!gameData.isSwapMinionTutShowed && gameData.level == Config.SwapMinionTutLevel)
        {
            flag = 2;
            return true;
        }

        return false;
    }

    public bool IsBabySitLevel()
    {
        return gameData.level <= AdsManager.Ins.Babysit_Level;
    }




    [ContextMenu(("Tunnel Level"))]
    public void ButtonLogTunnelLevel()
    {
        var rs = "";
        for (int i = 0; i < busLevelSoList.Length; i++)
        {
            if (busLevelSoList[i].tunnelDataPacks.Count > 0)
            {
                rs += $"{i + 1},";
            }
        }
        Debug.Log(rs);
    }
    
    [ContextMenu(("Hidden Level"))]
    public void ButtonLogHiddenLevel()
    {
        var rs = "";
        for (int i = 0; i < busLevelSoList.Length; i++)
        {
            if (busLevelSoList[i].grayBusIndexes.Count > 0)
            {
                rs += $"{i + 1},";
            }
        }
        Debug.Log(rs);
    }
    
    [Serializable]
    public class Data
    {
        public int level;

        public bool isSwapCarTutShowed;
        public bool isVipBusTutShowed;
        public bool isSwapMinionTutShowed;
    }
}
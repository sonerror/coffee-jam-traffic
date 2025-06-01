using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Resources Fragment", fileName = "Resources Fragment")]
public class ResourcesDataFragment : DataFragment
{
    public static ResourcesDataFragment cur;

    public Data gameData;

    public int Gold => gameData.gold;
    public int SwapCarNum => gameData.swapCarNum;
    public int VipBusNum => gameData.vipBusNum;
    public int SwapMinionNum => gameData.swapMinionNum;

    [SerializeField] private int swapCarPrice;
    [SerializeField] private int vipBusPrice;
    [SerializeField] private int swapMinionPrice;

    public int SwapCarPrice => swapCarPrice;
    public int VipBusPrice => vipBusPrice;
    public int SwapMinionPrice => swapMinionPrice;

    [SerializeField] private string[] boosterNames;
    public string[] BoosterNames => boosterNames;

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
    }

    public override void Save()
    {
        // RefreshGold();
        SaveData(gameData, key);
    }

    public override void ResetData()
    {
        gameData = new Data();
    }

    public void AddGold(int amount, string placement, bool isTruncatePending = false)
    {
        if (isTruncatePending)
        {
            amount = Mathf.Min(amount, gameData.pendingGold);
        }

        gameData.gold += amount;

        CanvasFloatingStuff.UpdateGold();

        if (amount >= 0)
        {
            if (!isTruncatePending)
            {
                FirebaseManager.Ins.g_gameplay_earn_booster("TICKET", placement, amount);
                FirebaseManager.Ins.earn_virtual_currency("ticket", amount, placement);
            }

            if (gameData.pendingGold > 0 && isTruncatePending) gameData.pendingGold = Mathf.Max(0, gameData.pendingGold - amount);
        }
        else
        {
            FirebaseManager.Ins.spend_virtual_currency("ticket", -amount, placement);
        }
    }

    public void AddSwapCar(int amount, string placement, bool isTruncatePending = false)
    {
        if (isTruncatePending)
        {
            amount = Mathf.Min(amount, gameData.pendingSwapCar);
        }

        gameData.swapCarNum += amount;

        CanvasGamePlay.UpdateBoosterNum();

        if (amount < 0)
        {
            var level = LevelDataFragment.cur.GetFireBaseLevel().ToString();
            FirebaseManager.Ins.g_gameplay_booster(level, "REFRESH");
        }
        else
        {
            if (!isTruncatePending) FirebaseManager.Ins.g_gameplay_earn_booster("REFRESH", placement, amount);
        }

        if (gameData.pendingSwapCar > 0 && amount > 0 && isTruncatePending) gameData.pendingSwapCar = Mathf.Max(0, gameData.pendingSwapCar - amount);
    }

    public void AddVipBus(int amount, string placement, bool isTruncatePending = false)
    {
        if (isTruncatePending)
        {
            amount = Mathf.Min(amount, gameData.pendingVipBus);
        }

        gameData.vipBusNum += amount;

        CanvasGamePlay.UpdateBoosterNum();

        if (amount < 0)
        {
            var level = LevelDataFragment.cur.GetFireBaseLevel().ToString();
            FirebaseManager.Ins.g_gameplay_booster(level, "CLEAR");
        }
        else
        {
            if (!isTruncatePending) FirebaseManager.Ins.g_gameplay_earn_booster("CLEAR", placement, amount);
        }

        if (gameData.pendingVipBus > 0 && amount > 0 && isTruncatePending) gameData.pendingVipBus = Mathf.Max(0, gameData.pendingVipBus - amount);
    }

    public void AddSwapMinion(int amount, string placement, bool isTruncatePending = false)
    {
        if (isTruncatePending)
        {
            amount = Mathf.Min(amount, gameData.pendingSwapMinion);
        }

        gameData.swapMinionNum += amount;

        CanvasGamePlay.UpdateBoosterNum();

        if (amount < 0)
        {
            var level = LevelDataFragment.cur.GetFireBaseLevel().ToString();
            FirebaseManager.Ins.g_gameplay_booster(level, "SORT");
        }
        else
        {
            if (!isTruncatePending) FirebaseManager.Ins.g_gameplay_earn_booster("SORT", placement, amount);
        }

        if (gameData.pendingSwapMinion > 0 && amount > 0 && isTruncatePending) gameData.pendingSwapMinion = Mathf.Max(0, gameData.pendingSwapMinion - amount);
    }

    public void PendingGold(int amount, string placement)
    {
        FirebaseManager.Ins.earn_virtual_currency("ticket", amount, placement);
        FirebaseManager.Ins.g_gameplay_earn_booster("TICKET", placement, amount);
        gameData.pendingGold += amount;
    }

    public void PendingSwapCar(int amount, string placement)
    {
        FirebaseManager.Ins.g_gameplay_earn_booster("REFRESH", placement, amount);
        gameData.pendingSwapCar += amount;
    }

    public void PendingVipBus(int amount, string placement)
    {
        FirebaseManager.Ins.g_gameplay_earn_booster("CLEAR", placement, amount);
        gameData.pendingVipBus += amount;
    }

    public void PendingSwapMinion(int amount, string placement)
    {
        FirebaseManager.Ins.g_gameplay_earn_booster("SORT", placement, amount);
        gameData.pendingSwapMinion += amount;
    }

    public void ProcessPending()
    {
        AddGold(gameData.pendingGold, "PENDING RESOURCE", true);
        AddSwapCar(gameData.pendingSwapCar, "PENDING RESOURCE", true);
        AddVipBus(gameData.pendingVipBus, "PENDING RESOURCE", true);
        AddSwapMinion(gameData.pendingSwapMinion, "PENDING RESOURCE", true);
    }

    public void ProcessPendingGold()
    {
        AddGold(gameData.pendingGold, "PENDING RESOURCE", true);
    }

    [Serializable]
    public class Data
    {
        public int gold;

        public int swapCarNum;
        public int vipBusNum;
        public int swapMinionNum;

        public int pendingGold;
        public int pendingSwapCar;
        public int pendingVipBus;
        public int pendingSwapMinion;
    }
}
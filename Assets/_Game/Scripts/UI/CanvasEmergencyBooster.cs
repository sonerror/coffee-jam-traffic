using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CanvasEmergencyBooster : UICanvasPrime
{
    public RectTransform canvasRect;

    public GameObject[] boosterInfoBoards;

    public TMP_Text boosterNameText;
    public TMP_Text priceText;

    private const string priceSuffix = "";
//<sprite=0>
    private int currentFlag;

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public GameObject exitButtonObject;
    public GameObject normBottomObject;
    public GameObject tutBottomObject;

    public RectTransform moneyButtonRect;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        CanvasFloatingStuff.cur.NukePopMoney();
    }

    public void SetupPanel(int flag, bool isTut = false)
    {
        currentFlag = flag;

        for (int i = 0; i < boosterInfoBoards.Length; i++)
        {
            boosterInfoBoards[i].SetActive(i == flag);
        }

        boosterNameText.text = ResourcesDataFragment.cur.BoosterNames[flag];
        priceText.text = GetPrice(flag) + priceSuffix;

        normBottomObject.SetActive(!isTut);
        exitButtonObject.SetActive(!isTut);
        tutBottomObject.SetActive(isTut);

        if (isTut)
        {
            var data = LevelDataFragment.cur.gameData;
            switch (flag)
            {
                case 0:
                    data.isSwapCarTutShowed = true;
                    break;
                case 1:
                    data.isVipBusTutShowed = true;
                    break;
                case 2:
                    data.isSwapMinionTutShowed = true;
                    break;
            }

            switch (currentFlag)
            {
                case 0:
                    ResourcesDataFragment.cur.AddSwapCar(1, "TUT");
                    break;
                case 1:
                    ResourcesDataFragment.cur.AddVipBus(1, "TUT");
                    break;
                case 2:
                    ResourcesDataFragment.cur.AddSwapMinion(1, "TUT");
                    break;
            }
        }
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void OnClickBuyGold()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (currentFlag == 2 && TransportCenter.cur.IsNoCarOnDuty())
        {
            CanvasGamePlay.PopNoCarToSort();
            return;
        }

        var price = GetPrice(currentFlag);

        if (ResourcesDataFragment.cur.Gold >= price)
        {
            ResourcesDataFragment.cur.AddGold(-price, "EMERGENCY_BOOSTER_" + ResourcesDataFragment.cur.BoosterNames[currentFlag]);

            ProcessPurchase("BUY_WITH_TICKET");
        }
        else
        {
            var pos = moneyButtonRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(canvasRect) + Vector2.up * 96;
            CanvasFloatingStuff.cur.PopMoneyShortage(pos);
        }
    }

    public void OnClickBuysAds()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (currentFlag == 2 && TransportCenter.cur.IsNoCarOnDuty())
        {
            CanvasGamePlay.PopNoCarToSort();
            return;
        }

        var tag = currentFlag switch
        {
            0 => "REFRESH",
            1 => "CLEAR",
            2 => "SORT",
        };
        AdsManager.Ins.ShowRewardedAd("BOOSTER_ADS_" + tag, () => ProcessPurchase("EMERGENCY_ADS"));
    }

    public void OnClickClaim()
    {
        CanvasGamePlay.cur.UnlockBooster(currentFlag);
        Close();
    }

    private void ProcessPurchase(string placement)
    {
        var level = LevelDataFragment.cur.GetFireBaseLevel().ToString();
        switch (currentFlag)
        {
            case 0:
                FirebaseManager.Ins.g_gameplay_earn_booster("REFRESH", placement, 1);
                FirebaseManager.Ins.g_gameplay_booster(level, "REFRESH");
                BusStation.cur.BoosterUltraRandomColor();
                break;
            case 1:
                ResourcesDataFragment.cur.AddVipBus(1, placement);
                if (BusStation.cur.IsVipBusAble()) BusStation.cur.BoosterVipBus();
                break;
            case 2:
                BusStation.cur.BoosterMinionsMix();
                FirebaseManager.Ins.g_gameplay_earn_booster("SORT", placement, 1);
                FirebaseManager.Ins.g_gameplay_booster(level, "SORT");
                break;
        }

        Close();
    }

    public void OnClickBoosterPack(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "EMERGENCY_BOOSTER_PACK", Close);
    }

    private int GetPrice(int flag)
    {
        return flag switch
        {
            0 => ResourcesDataFragment.cur.SwapCarPrice,
            1 => ResourcesDataFragment.cur.VipBusPrice,
            2 => ResourcesDataFragment.cur.SwapMinionPrice,
        };
    }
}
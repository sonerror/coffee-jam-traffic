using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CanvasLose : UICanvasPrime
{
    public RectTransform canvasRect;
    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public GameObject firstPanelObject;
    public GameObject secondPanelObject;
    public RectTransform secondPanelRect;

    public TMP_Text reviveGoldText;
    private const string PREFIX = "";

    public GameObject homeButtonObject;
    public RectTransform retryButtonRect;
    [SerializeField] private Vector2 retryRootPos;
    [SerializeField] private Vector2 retryTargetPos;

    public GameObject normalReviveOptionObject;
    public GameObject freeReviveObject;

    public GameObject packObject;

    public RectTransform moneyButtonRect;

    public GameObject normalLoseBoard;
    public GameObject streakLoseBoard;
    public Animation streakLoseAnim;
    private bool isStreakLosePop;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        firstPanelObject.SetActive(true);
        secondPanelObject.SetActive(false);

        normalLoseBoard.SetActive(true);
        streakLoseBoard.SetActive(false);

        isStreakLosePop = WinstreakDataFragment.cur.IsNeedToNoffLoseStreak();

        AudioManager.ins.PlaySound(SoundType.Lose);

        CanvasFloatingStuff.cur.SetHighLightGold(true);

        reviveGoldText.text = PREFIX + Config.cur.goldPerRevive;

        CheckHomeButton();

        var isINLAstBabySitLevel = LevelDataFragment.cur.gameData.level == AdsManager.Ins.Babysit_Level;
        var isOn = !LevelDataFragment.cur.IsBabySitLevel() || isINLAstBabySitLevel;
        packObject.SetActive(isOn);

        UIManager.ins.CloseUI<CanvasIapShop>();
        UIManager.ins.CloseUI<CanvasSetting>();
        CanvasFloatingStuff.cur.SetHighLightGold(true);
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        popTween?.Kill();

        CanvasFloatingStuff.cur.SetHighLightGold(false);
        CanvasFloatingStuff.cur.NukePopMoney();
    }

    private void PopSecond()
    {
        firstPanelObject.SetActive(false);
        secondPanelObject.SetActive(true);

        secondPanelRect.localScale = Vector3.zero;
        secondPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        DataManager.ins.LogFail();
        SpaceMissionDataFragment.cur.gameData.data_of_players[0].score = 0;
        WinstreakDataFragment.cur.IsJustLose = true;
        WinstreakDataFragment.cur.BackToLastCheckPoint();

        BuyingPackDataFragment.cur.ConsecutiveLoseNum++;
    }

    public void CheckHomeButton()
    {
        // const string key = "LOSE_REVIVE_FREE";
        //
        // var isFree = !PlayerPrefs.HasKey(key) && LevelDataFragment.cur.IsBabySitLevel();
        // PlayerPrefs.SetInt(key, 2710);
        //
        // if (isFree)
        // {
        //     normalReviveOptionObject.SetActive(false);
        //     freeReviveObject.SetActive(true);
        // }
        // else
        {
            normalReviveOptionObject.SetActive(true);
            freeReviveObject.SetActive(false);
        }

        if (LevelDataFragment.cur.IsBabySitLevel())
        {
            homeButtonObject.SetActive(false);
            retryButtonRect.anchoredPosition = retryTargetPos;
        }
        else
        {
            homeButtonObject.SetActive(true);
            retryButtonRect.anchoredPosition = retryRootPos;
        }
    }

    public void OnClickFreeRevive()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);

        TransportCenter.cur.ReviveTransport();
        Close();
    }

    public void OnClickReviveGold()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);

        var price = Config.cur.goldPerRevive;
        if (ResourcesDataFragment.cur.Gold < price)
        {
            var pos = moneyButtonRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(canvasRect) + Vector2.up * 96;
            CanvasFloatingStuff.cur.PopMoneyShortage(pos);
            return;
        }

        ResourcesDataFragment.cur.AddGold(-price, "LOSE_REVIVE");
        TransportCenter.cur.ReviveTransport();
        Close();
    }


    public void OnClickRevive()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);

        AdsManager.Ins.ShowRewardedAd("REVIVE", () =>
        {
            TransportCenter.cur.ReviveTransport();
            Close();
        });
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);

        if (isStreakLosePop)
        {
            isStreakLosePop = false;
            streakLoseAnim.Play();
            return;
        }

        PopSecond();
    }

    public void OnClickRevivePack(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "LOSE_CANVAS");
    }

    public void OnClickBack()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        TreasureDataFragment.cur.NukeRecord();
        GrandManager.ins.RequireInter(true, "LOSE_BACK", () => GrandManager.ins.IntoHome());
    }

    public void OnClickRetry()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        TreasureDataFragment.cur.NukeRecord();
        GrandManager.ins.RequireInter(true, "LOSE_RETRY", () => GrandManager.ins.InToGame());
    }
}
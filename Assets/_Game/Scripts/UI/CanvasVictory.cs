using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CanvasVictory : UICanvasPrime
{
    public CanvasGroup canvasGroup;

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public TMP_Text passengerText;
    public TMP_Text rewardText;
    public TMP_Text adsButtonText;

    private const string PREFIX = "<sprite=0> ";

    public GameObject noAdsOfferObject;

    public GameObject nextButtonObject;
    public RectTransform nextButtonRect;
    [SerializeField] private Vector2 nextRootPos;
    [SerializeField] private Vector2 nextTargetPos;
    public GameObject moreRewardButtonObject;
    public RectTransform moreRewardButtonRect;
    [SerializeField] private Vector2 moreRootPos;
    [SerializeField] private Vector2 moreTargetPos;
    private Coroutine waitingCor;

    public TMP_Text rouletteRewardText;
    public TMP_Text roulettePassengerText;
    public GameObject normalRewardBoardObject;
    public GameObject rouletteRewardBoardObject;

    public RectTransform roulettePointerRect;
    [SerializeField] private Vector2 pointerStartPos;
    [SerializeField] private Vector2 pointerEndPos;
    [SerializeField] private float[] roulettePosCheckPoint;
    [SerializeField] private float[] rouletteMultipleList;

    private float rouletteMultiple;
    private Tween rouletteTween;

    public bool isGetAdsReward;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        canvasGroup.interactable = true;

        rouletteTween?.Kill();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        CheckNoAdsOffer();

        DataManager.ins.LogComplete();

        var minionsNum = BusLevelSO.active.totalMinionNum;
        var reward = GetReward();

        passengerText.text = minionsNum + "/" + minionsNum;
        roulettePassengerText.text = minionsNum + "/" + minionsNum;
        rewardText.text = reward.ToString();
        rouletteRewardText.text = reward.ToString();
        adsButtonText.text = /*PREFIX + */"+" + reward;

        CanvasFloatingStuff.cur.SetHighLightGold(true);

        AudioManager.ins.PlaySound(SoundType.Win);

        FirebaseManager.Ins.CheckDayPlayed();
        AppsflyerEventRegister.pass_level();
        LevelDataFragment.cur.AscendLevel();

        WinstreakDataFragment.cur.InCreaseStreak();
        WinstreakDataFragment.cur.IsJustIncreaseStreak = true;
        WinstreakDataFragment.cur.IsJustLose = false;

        CheckBoard();
        CheckNextButton();
        BuyingPackDataFragment.cur.ConsecutiveLoseNum = 0;

        UIManager.ins.CloseUI<CanvasIapShop>();
        UIManager.ins.CloseUI<CanvasOfferRemoveAdsPack>();
        UIManager.ins.CloseUI<CanvasSetting>();
        CanvasFloatingStuff.cur.SetHighLightGold(false);
        SpaceMissionDataFragment.cur.gameData.data_of_players[0].score++;
        CarRaceDataFragment.cur.gameData.data_of_players[0].score++;

        isGetAdsReward = false;
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        rouletteTween?.Kill();
    }

    private int GetReward()
    {
        return BusLevelSO.active.busMapHardness switch
        {
            BusMapHardness.Easy => Config.cur.goldPerWin,
            BusMapHardness.Hard => 20,
            BusMapHardness.SuperHard => 30,
        };
    }

    public void OnClickMoreReward()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        canvasGroup.interactable = false;

        rouletteTween?.Pause();

        AdsManager.Ins.ShowRewardedAd("VICTORY_BONUS", () =>
            {
                StartCoroutine(ie_Handle());
                rouletteTween?.Kill();
                isGetAdsReward = true;
            },
            () =>
            {
                canvasGroup.interactable = true;
                rouletteTween?.Play();
            });

        return;

        IEnumerator ie_Handle()
        {
            var reward = BusLevelSO.active.busMapHardness == BusMapHardness.Easy ? GetReward() * 2 : GetRouletteReward();

            StopWait();

            yield return null;
            yield return null;
            yield return null;
            yield return null;
            ResourcesDataFragment.cur.AddGold(reward, "WIN_MORE");
            CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);

            yield return Yielders.Get(1.32f);
            if (LevelDataFragment.cur.IsBabySitLevel())
            {
                GrandManager.ins.RequireInter(false, "WIN_MORE_RW", () => GrandManager.ins.InToGame());
            }
            else
            {
                GrandManager.ins.RequireInter(false, "WIN_MORE_RW", () => GrandManager.ins.IntoHome());
            }
        }
    }

    public void OnClickNext()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        canvasGroup.interactable = false;

        rouletteTween?.Pause();
        StopWait();

        //make default reward
        ResourcesDataFragment.cur.AddGold(GetReward(), "WIN");
        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);

        Timer.ScheduleSupreme(1.32f, () =>
        {
            if (LevelDataFragment.cur.IsBabySitLevel())
            {
                GrandManager.ins.RequireInter(false, "WIN_NEXT", () => GrandManager.ins.InToGame());
            }
            else
            {
                GrandManager.ins.RequireInter(false, "WIN_NEXT", () => GrandManager.ins.IntoHome());
            }
        });
    }

    public void OnClickRemoveAds(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "VICTORY_NO_ADS", CheckNoAdsOffer);
    }

    private void CheckNoAdsOffer()
    {
        noAdsOfferObject.SetActive(!AdsManager.isNoAds && !LevelDataFragment.cur.IsBabySitLevel());
    }

    public void StartRoulette()
    {
        var hardness = BusLevelSO.active.busMapHardness;
        if (hardness == BusMapHardness.Easy) return;

        MovePointer(0);
        rouletteTween = DOVirtual.Float(0, 1, 1.28f, MovePointer).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void CheckBoard()
    {
        var hardness = BusLevelSO.active.busMapHardness;
        if (hardness == BusMapHardness.Easy)
        {
            normalRewardBoardObject.SetActive(true);
            rouletteRewardBoardObject.SetActive(false);
        }
        else
        {
            normalRewardBoardObject.SetActive(false);
            rouletteRewardBoardObject.SetActive(true);
        }
    }

    private void MovePointer(float per)
    {
        var pos = Vector2.Lerp(pointerStartPos, pointerEndPos, per);

        roulettePointerRect.anchoredPosition = pos;

        UpdateMultiple(pos.x);

        adsButtonText.text = GetRouletteReward().ToString();
    }

    private void UpdateMultiple(float xPos)
    {
        for (int i = 0; i < roulettePosCheckPoint.Length; i++)
        {
            if (xPos < roulettePosCheckPoint[i])
            {
                rouletteMultiple = rouletteMultipleList[i];
                return;
            }
        }

        rouletteMultiple = rouletteMultipleList[^1];
    }

    private int GetRouletteReward()
    {
        return Mathf.RoundToInt(GetReward() * rouletteMultiple);
    }

    public void CheckNextButton()
    {
        if (LevelDataFragment.cur.IsBabySitLevel())
        {
            moreRewardButtonObject.SetActive(false);
            nextButtonObject.SetActive(true);
            nextButtonRect.anchoredPosition = nextTargetPos;
        }
        else
        {
            ShowCaseButton();
            StartRoulette();
        }
    }

    public void ShowCaseButton()
    {
        moreRewardButtonObject.SetActive(true);
        moreRewardButtonRect.anchoredPosition = moreTargetPos;
        nextButtonObject.SetActive(false);

        StartCoroutine(ie_Wait());
        return;

        IEnumerator ie_Wait()
        {
            yield return Yielders.Get(1);
            if (isGetAdsReward) yield break;
            canvasGroup.interactable = false;
            nextButtonObject.SetActive(true);
            nextButtonRect.anchoredPosition = nextTargetPos;

            DOVirtual.Float(0, 1, .32f, (fuk) =>
            {
                moreRewardButtonRect.anchoredPosition = Vector2.Lerp(moreTargetPos, moreRootPos, fuk);
                nextButtonRect.anchoredPosition = Vector2.Lerp(nextTargetPos, nextRootPos, fuk);
            }).SetEase(Ease.Linear).OnComplete(() => canvasGroup.interactable = true);
        }
    }

    private void StopWait()
    {
        if (waitingCor != null) StopCoroutine(waitingCor);
    }
}
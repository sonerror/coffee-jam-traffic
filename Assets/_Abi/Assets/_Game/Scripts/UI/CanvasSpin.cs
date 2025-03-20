using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CanvasSpin : UICanvasPrime
{
    public static CanvasSpin cur;
    public RectTransform canvasRect;

    public CanvasGroup canvasGroup;

    public Transform wheelTrans;

    public Image progressBarImage;

    public float[] prizeAngle;
    [SerializeField] private float prizeOffsetAngle = 20;

    private int curPrizeIndex;

    public TMP_Text passedLevelText;
    public TMP_Text passLevelDescriptionText;

    public GameObject spinButtonObject;
    public GameObject lockSpinButtonObject;
    public GameObject adsSpinButtonObject;

    private bool isSpinable;
    //end stuff

    private Tween spinTween;
    private bool isSpinning;

    public static bool IsSpinning => cur != null && cur.isSpinning;

    private float timer;
    private const float TICK_OFFSET = 45f;
    private float lastAngle;

    private Tween pointerPopTween;
    public RectTransform pointerRect;
    [SerializeField] private Vector3 pointerNormAngle;
    [SerializeField] private Vector3 pointerHitAngle;
    [SerializeField] private float popTime;

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public GameObject timeObject;
    public TMP_Text timeLeftText;
    private Coroutine timeCor;

    // private const string AAA = "Next free in: ";

    private void Awake()
    {
        cur = this;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        timeObject.SetActive(false);

        canvasGroup.interactable = true;

        CheckSpinButtonAndMore();
        SetupWheel();

        isSpinning = false;
        CanvasFloatingStuff.cur.SetHighLightGold(true);
        CanvasFloatingStuff.SetPlusIcon(false);

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        spinTween.Kill();

        CanvasFloatingStuff.cur.SetHighLightGold(false);
        CanvasFloatingStuff.SetPlusIconDefault();
        isSpinning = false;
    }

    private void SetBar()
    {
        var require = SpinDataFragment.cur.GetRequiredPassedLevel();
        var passed = Mathf.Min(SpinDataFragment.cur.gameData.levelPassedNum, require);

        var per = (float)passed / require;
        progressBarImage.fillAmount = per;
    }

    public void OnCLickExit()
    {
        if (isSpinning) return;
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void OnClickSpin()
    {
        if (isSpinning || !isSpinable) return;
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Spinning();

        SpinDataFragment.cur.ConsumePassedLevel();

        CheckSpinButtonAndMore();
    }

    public void OnClickAdsSpin()
    {
        if (isSpinning || !SpinDataFragment.cur.gameData.isAdsAble) return;
        AudioManager.ins.PlaySound(SoundType.UIClick);

        canvasGroup.interactable = false;

        AdsManager.Ins.ShowRewardedAd("SPIN", () =>
        {
            canvasGroup.interactable = true;

            SpinDataFragment.cur.gameData.isAdsAble = false;
            SpinDataFragment.cur.MarkTime();

            Spinning();

            CheckSpinButtonAndMore();
        }, () => { canvasGroup.interactable = true; });
    }

    private void CheckSpinButtonAndMore()
    {
        var require = SpinDataFragment.cur.GetRequiredPassedLevel();
        var passed = Mathf.Min(SpinDataFragment.cur.gameData.levelPassedNum, require);
        passedLevelText.text = passed + "/" + require;
        passLevelDescriptionText.text = "Win " + require + " levels to spin the Wheel";

        isSpinable = passed >= require;
        var isAdsAble = SpinDataFragment.cur.gameData.isAdsAble;

        SetBar();

        spinButtonObject.SetActive(isSpinable);
        adsSpinButtonObject.SetActive(!isSpinable && isAdsAble);
        lockSpinButtonObject.SetActive(!isSpinable && !isAdsAble);

        CheckTimeLeftTillFree();

        CanvasHome.CheckSpinNumStatic();
    }

    private static WaitUntil waitForValidTime = new WaitUntil(() => UnbiasedTime.IsValidTime);

    public void CheckTimeLeftTillFree()
    {
        if (timeCor != null) StopCoroutine(timeCor);
        timeCor = StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            while (!SpinDataFragment.cur.gameData.isAdsAble)
            {
                yield return waitForValidTime;
                if (!SpinDataFragment.cur.CheckNewTimeSegment(out var timeLeft))
                {
                    timeObject.SetActive(true);
                    timeLeftText.text = timeLeft.ToTimeFormat_H_M_S();
                }
                else
                {
                    timeObject.SetActive(false);
                    CheckSpinButtonAndMore();
                    yield break;
                }

                yield return Yielders.Get(1);
            }

            timeLeftText.text = "";
            timeObject.SetActive(false);
        }
    }

    private void SetupWheel()
    {
        wheelTrans.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
    }

    private void Spinning()
    {
        NoffSpin.UpdateNoffStatic();
        isSpinning = true;
        spinTween.Kill();
        curPrizeIndex = GetPossiblityIndex();
        timer = TICK_OFFSET / 2;
        lastAngle = 0;
        spinTween = DOVirtual.Float(wheelTrans.eulerAngles.z, 360f * 10 + prizeAngle[curPrizeIndex] + Random.Range(-prizeOffsetAngle, prizeOffsetAngle), 5.32f, (x) =>
                {
                    wheelTrans.eulerAngles = Vector3Pot.GetVector3(0, 0, x);
                    float diff = Mathf.Max(wheelTrans.eulerAngles.z - lastAngle, 0);
                    lastAngle = wheelTrans.eulerAngles.z;
                    timer += diff;
                    if (timer > TICK_OFFSET)
                    {
                        timer = 0;
                        AudioManager.ins.PlaySound(SoundType.SpinTick);

                        PopPointer();
                    }
                }).SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    SetPrize(curPrizeIndex);
                    isSpinning = false;
                    AudioManager.ins.PlaySound(SoundType.SpinEnd);
                })
            ;
    }

    private void PopPointer()
    {
        pointerPopTween?.Complete();
        pointerPopTween = pointerRect.DORotate(pointerHitAngle, popTime).SetEase(Ease.OutQuad)
            .OnComplete(() => { pointerPopTween = pointerRect.DORotate(pointerNormAngle, popTime).SetEase(Ease.Linear); });
    }

    private int GetPossiblityIndex()
    {
        if (SpinDataFragment.cur.gameData.spinRank == 0)
        {
            return 4;
        }

        float i = Random.Range(0, 100f);
        return i switch
        {
            < 40 => 0,
            < 60 => 4,
            < 70 => 1,
            < 80 => 1,
            < 90 => 3,
            _ => 5
        };
    }

    private void SetPrize(int index)
    {
        // var pos = starRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(canvasRect);
        var pos = Vector2.zero;
        // Debug.Log("POS " + pos);
        switch (index)
        {
            case 0:
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, pos);
                ResourcesDataFragment.cur.AddGold(10, "SPIN");
                break;
            case 1:
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.VipBusSplash, pos);
                ResourcesDataFragment.cur.AddVipBus(1, "SPIN");
                break;
            case 2:
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, pos);
                ResourcesDataFragment.cur.AddGold(125, "SPIN");
                break;
            case 3:
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.SwapMinionSplash, pos);
                ResourcesDataFragment.cur.AddSwapMinion(1, "SPIN");
                break;
            case 4:
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, pos);
                ResourcesDataFragment.cur.AddGold(50, "SPIN");
                break;
            case 5:
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.SwapCarSplash, pos);
                ResourcesDataFragment.cur.AddSwapCar(1, "SPIN");
                break;
        }
    }
}

// 250 Gold 13,5 %
// 500 Gold 13,5 %
// 750 Gold 13,5 %
// 1000 Gold 13,5 %
// 2000 Gold 13,5 %
// 1 Crystal 13,5 %
// 10 Crystal 13,5 %
// 50 Crystal 5,5 %
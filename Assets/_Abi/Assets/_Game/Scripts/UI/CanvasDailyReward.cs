using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasDailyReward : UICanvasPrime
{
    public GameObject[] buttonObjectList;
    public GameObject[] tickObjects;
    public GameObject[] freeTextObjects;
    public Animation[] adsIconAnim;

    public GameObject oneButton;
    public TMP_Text oneCountDownText;

    public TMP_Text timeLeftText;

    private const string PREFIX = "Reset in ";

    public RectTransform mainPanelRect;
    private Tween popTween;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;

    public Image progressBarImage;
    public GameObject[] baseDotObjects;
    public GameObject[] emptyDotObjects;
    public GameObject[] lineTickObjects;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        CanvasFloatingStuff.cur.SetHighLightGold(true);
        CanvasFloatingStuff.SetPlusIcon(false);

        RefreshAll();

        CheckTime();
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        CanvasFloatingStuff.cur.SetHighLightGold(false);
        CanvasFloatingStuff.SetPlusIconDefault();
    }

    private void CheckTime()
    {
        StartCoroutine(ie_CheckTime());
        return;

        IEnumerator ie_CheckTime()
        {
            while (true)
            {
                HandleFirstButton();

                if (DailyRewardDataFragment.cur.gameData.itemFlags[0])
                {
                    //count down
                    if (DailyRewardDataFragment.cur.CheckNewDailyDay(out var timeLeft))
                    {
                        timeLeftText.text = "";
                        RefreshAll();
                    }
                    else
                    {
                        timeLeftText.text = PREFIX + timeLeft.ToTimeFormat_H_M_S_Dynamic_Lower_Text();
                    }
                }
                else
                {
                    timeLeftText.text = "";
                }

                yield return Yielders.Get(1f);
            }
        }
    }

    private void RefreshAll()
    {
        HandleFirstButton();
        for (int i = 0; i < buttonObjectList.Length; i++) HandleOtherButton(i);

        SetupLine();
        NoffDailyReward.UpdateNoffStatic();
    }

    private void HandleFirstButton()
    {
        var isFree = DailyRewardDataFragment.cur.CheckIfItsTimeToFree(out var timeLeft);
        if (!isFree) oneCountDownText.text = timeLeft.ToTimeFormat_M_S();

        oneButton.SetActive(isFree);
    }

    private void HandleOtherButton(int index) // return if claimable
    {
        var data = DailyRewardDataFragment.cur.gameData;
        if (data.itemFlags[index])
        {
            buttonObjectList[index].SetActive(false);
            tickObjects[index].SetActive(true);
            freeTextObjects[index].SetActive(false);
        }
        else
        {
            if (CheckValid(index))
            {
                buttonObjectList[index].SetActive(true);
                // adsIconAnim[index].Play();
                tickObjects[index].SetActive(false);
                freeTextObjects[index].SetActive(false);
            }
            else
            {
                buttonObjectList[index].SetActive(false);
                tickObjects[index].SetActive(false);
                freeTextObjects[index].SetActive(true);
            }
        }
    }

    private bool CheckValid(int index) //return if ads-able
    {
        var data = DailyRewardDataFragment.cur.gameData;
        return index == 0 || data.itemFlags[index - 1];
    }

    private void SetupLine()
    {
        int currentIndex = 0;
        for (int i = 0; i < lineTickObjects.Length; i++)
        {
            Handle(i);
        }

        progressBarImage.fillAmount = currentIndex switch
        {
            0 => 0,
            1 => .33f,
            2 => .66f,
            _ => 1
        };

        return;

        void Handle(int index)
        {
            var data = DailyRewardDataFragment.cur.gameData;
            if (data.itemFlags[index])
            {
                emptyDotObjects[index].SetActive(true);
                lineTickObjects[index].SetActive(true);
                baseDotObjects[index].SetActive(false);
            }
            else
            {
                if (CheckValid(index))
                {
                    emptyDotObjects[index].SetActive(true);
                    currentIndex = index;
                    lineTickObjects[index].SetActive(false);
                    baseDotObjects[index].SetActive(false);
                }
                else
                {
                    emptyDotObjects[index].SetActive(false);
                    lineTickObjects[index].SetActive(false);
                    baseDotObjects[index].SetActive(true);
                }
            }
        }
    }

    public void OnClickFirstButton()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (!DailyRewardDataFragment.cur.CheckIfItsTimeToFree(out _)) return;
        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
        ResourcesDataFragment.cur.AddGold(5, "DAILY_REWARD");
        DailyRewardDataFragment.cur.gameData.lastFreeTime = ExtensionMethodUltimate.GetCurrentTimeSpanToRootDate();
        RefreshAll();
    }

    public void OnClickReward(int index)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (!CheckValid(index)) return;
        AdsManager.Ins.ShowRewardedAd("DAILY_" + (index + 1), () =>
        {
            DailyRewardDataFragment.cur.gameData.itemFlags[index] = true;
            if (index == 0) DailyRewardDataFragment.cur.PrepareCountDown();
            RefreshAll();

            switch (index)
            {
                case 0:
                    ResourcesDataFragment.cur.AddGold(15, "DAILY_REWARD");
                    CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
                    break;
                case 1:
                    ResourcesDataFragment.cur.AddGold(25, "DAILY_REWARD");
                    CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
                    break;
                case 2:
                    ResourcesDataFragment.cur.AddGold(35, "DAILY_REWARD");
                    CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
                    break;
                case 3:
                    ResourcesDataFragment.cur.AddGold(45, "DAILY_REWARD");
                    CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
                    break;
            }
        });
    }

    public void OnCLickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }
}
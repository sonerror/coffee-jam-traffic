using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasDailyLogin : UICanvasPrime
{
    private bool isNeedUpdate = true;

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    // public GameObject comeBackTextObject;
    public GameObject claimButtonObject;
    // public GameObject claimMoreButtonObject;

    public GameObject[] tickObjects;
    public GameObject[] prizeObject;
    public Image[] bgImages;
    [SerializeField] private Sprite claimedSprite;
    [SerializeField] private Sprite currentSprite;
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite wideClaimedSprite;
    [SerializeField] private Sprite wideCurrentSprite;
    [SerializeField] private Sprite wideNeutralSprite;

    // public TMP_Text timeText;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        if (isNeedUpdate || DailyLoginDataFragment.cur.CheckNewDay(out _))
        {
            isNeedUpdate = false;

            SetClaimButton();
            RefreshItem();
        }

        CanvasFloatingStuff.cur.SetHighLightGold(true);
        CanvasFloatingStuff.SetPlusIcon(false);

        CheckTime();
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        CanvasFloatingStuff.cur.SetHighLightGold(false);
        CanvasFloatingStuff.SetPlusIconDefault();
    }

    public void OnCLickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void OnClickClaim()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        DailyLoginDataFragment.cur.gameData.isClaimedToday = true;
        ClaimHandle();
        SetClaimButton();

        NoffDailyLogin.UpdateNoffStatic();
    }

    // public void OnClickClaimMore()
    // {
    //     AudioManager.ins.PlaySound(SoundType.UIClick);
    //     AdsManager.Ins.ShowRewardedAd("DAILY_LOGIN_MORE", () =>
    //     {
    //         DailyLoginDataFragment.cur.gameData.isClaimMoreToday = true;
    //         ClaimHandle();
    //         SetClaimButton();
    //     });
    // }

    private void CheckTime()
    {
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            yield return new WaitUntil(() => UnbiasedTime.IsValidTime);
            while (true)
            {
                if (DailyLoginDataFragment.cur.CheckNewDay(out var timeLeft))
                {
                    SetClaimButton();
                    RefreshItem();
                }

                // timeText.text = timeLeft.ToTimeFormatPro();
                yield return Yielders.Get(1);
            }
        }
    }

    private void ClaimHandle()
    {
        var index = DailyLoginDataFragment.cur.GetCurrentDayIndex();
        // DailyLoginDataFragment.cur.TickDay(index);

        // FirebaseManager.Ins.claim_daily_login(index + 1);

        switch (index)
        {
            case 0:
                ResourcesDataFragment.cur.AddGold(25, "DAILY_LOGIN");
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
                break;
            case 1:
                ResourcesDataFragment.cur.AddSwapCar(1, "DAILY_LOGIN");
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.SwapCarSplash, Vector2.zero, null);
                break;
            case 2:
                ResourcesDataFragment.cur.AddGold(35, "DAILY_LOGIN");
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
                break;
            case 3:
                ResourcesDataFragment.cur.AddVipBus(1, "DAILY_LOGIN");
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.VipBusSplash, Vector2.zero, null);
                break;
            case 4:
                ResourcesDataFragment.cur.AddGold(75, "DAILY_LOGIN");
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
                break;
            case 5:
                ResourcesDataFragment.cur.AddSwapMinion(1, "DAILY_LOGIN");
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.SwapMinionSplash, Vector2.zero, null);
                break;
            case 6:
                ResourcesDataFragment.cur.AddGold(125, "DAILY_LOGIN");
                CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero, null);
                break;
        }

        RefreshItem();
    }

    private void RefreshItem()
    {
        var index = DailyLoginDataFragment.cur.GetCurrentDayIndex();
        var isClaimed = DailyLoginDataFragment.cur.gameData.isClaimedToday && DailyLoginDataFragment.cur.gameData.isClaimMoreToday;

        for (int i = 0; i < 7; i++)
        {
            if (i < index)
            {
                tickObjects[i].SetActive(true);
                prizeObject[i].SetActive(false);
                bgImages[i].sprite = i == 6 ? wideClaimedSprite : claimedSprite;
            }
            else if (i == index)
            {
                tickObjects[i].SetActive(isClaimed);
                prizeObject[i].SetActive(!isClaimed);
                bgImages[i].sprite = i == 6 ? wideCurrentSprite : currentSprite;
            }
            else
            {
                tickObjects[i].SetActive(false);
                prizeObject[i].SetActive(true);
                bgImages[i].sprite = i == 6 ? wideNeutralSprite : neutralSprite;
            }
        }
    }

    private void SetClaimButton()
    {
        if (!DailyLoginDataFragment.cur.gameData.isClaimedToday)
        {
            // comeBackTextObject.SetActive(false);
            claimButtonObject.SetActive(true);
            // claimMoreButtonObject.SetActive(false);
        }
        else if (!DailyLoginDataFragment.cur.gameData.isClaimMoreToday)
        {
            // comeBackTextObject.SetActive(false);
            claimButtonObject.SetActive(false);
            // claimMoreButtonObject.SetActive(true);
        }
        else
        {
            // comeBackTextObject.SetActive(true);
            claimButtonObject.SetActive(false);
            // claimMoreButtonObject.SetActive(false);
        }
    }
}
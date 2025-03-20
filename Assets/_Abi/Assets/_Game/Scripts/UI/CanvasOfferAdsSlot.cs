using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using DG.Tweening;
using UnityEngine;

public class CanvasOfferAdsSlot : UICanvasPrime
{
    public CanvasGroup canvasGroup;

    public RectTransform canvasRect;

    private ParkSlot currentParkSlot;

    private const int price = 125;

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public RectTransform moneyButtonRect;

    public GameObject offerVip_3_Object;
    public GameObject offerVip_7_Object;
    public GameObject offerVip_15_Object;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        canvasGroup.interactable = true;

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.one * .05f;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        CheckOffer();
    }

    public void Fetch(ParkSlot parkSlot)
    {
        currentParkSlot = parkSlot;
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void OnClickUnlockGold()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);

        if (ResourcesDataFragment.cur.Gold >= price)
        {
            ResourcesDataFragment.cur.AddGold(-price, "BUY ADS SLOT");
            if (currentParkSlot != null)
            {
                currentParkSlot.UnlockSlot();
                FirebaseManager.Ins.g_gameplay_booster(LevelDataFragment.cur.GetFireBaseLevel().ToString(), "PARK_SLOT");
            }

            Close();
        }
        else
        {
            var pos = moneyButtonRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(canvasRect) + Vector2.up * 96;
            CanvasFloatingStuff.cur.PopMoneyShortage(pos);
        }
    }

    public void OnClickUnlockAds()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        canvasGroup.interactable = false;

        AdsManager.Ins.ShowRewardedAd("UNLOCK_PARK_SLOT", () =>
            {
                if (currentParkSlot != null)
                {
                    currentParkSlot.UnlockSlot();
                    FirebaseManager.Ins.g_gameplay_booster(LevelDataFragment.cur.GetFireBaseLevel().ToString(), "PARK_SLOT");
                }

                Close();
            }
            , () => canvasGroup.interactable = true);
    }

    public void OnClickBuyOffer(IAPButtonPro buttonPro)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(buttonPro.id, "OFFER_ADS_SLOT", Close);
    }

    public void CheckOffer()
    {
        var vipData = VipPassDataFragment.cur.gameData;

        if (vipData.isEverBuyLargestPack)
        {
            if (!vipData.isVip_15_Active) SetOffer(2);
            else if (!vipData.isVip_7_Active) SetOffer(1);
            else if (!vipData.isVip_3_Active) SetOffer(0);
            else SetOffer(-1);
        }
        else
        {
            if (!vipData.isVip_3_Active) SetOffer(0);
            else if (!vipData.isVip_7_Active) SetOffer(1);
            else if (!vipData.isVip_15_Active) SetOffer(2);
            else SetOffer(-1);
        }

        return;

        void SetOffer(int index)
        {
            offerVip_3_Object.SetActive(index == 0);
            offerVip_7_Object.SetActive(index == 1);
            offerVip_15_Object.SetActive(index == 2);
        }
    }
}
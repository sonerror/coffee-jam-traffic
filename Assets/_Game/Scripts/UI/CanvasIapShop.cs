using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CanvasIapShop : UICanvasPrime
{
    public static CanvasIapShop cur;
    public RectTransform canvasRect;

    public static bool IsOpen { get; set; }

    public RectTransform mainFrameRect;

    public InAppBox[] allInAppBox;
    public InAppBox noAdsPackInAppBox;
    public InAppBox removeAdsBundleInAppBox;
    public InAppBox starterInAppBox;

    private bool isExpanding;
    private bool isTransitAnim;

    public ScrollRect scrollRect;
    public RectTransform contentRect;
    [SerializeField] private float contentOffsetToBottom;

    public ParticleImage[] particleImages;

    public CanvasGroup bgPanelCg;
    public RectTransform totalMainRect;

    private Vector2 rootTotalPos;
    private Tween panelMoveTween;

    private void Awake()
    {
        mainFrameRect.SafeAreaParentAdoption(canvas);

        cur = this;
        rootTotalPos = Vector2.down * canvasRect.rect.height;

        TriggerEffect();
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        scrollRect.verticalNormalizedPosition = 1;

        CheckContentAvailable();
        ResizeContent();

        IsOpen = true;

        UIManager.ins.CloseUI<CanvasSpin>();
        UIManager.ins.CloseUI<CanvasDailyLogin>();
        UIManager.ins.CloseUI<CanvasDailyReward>();

        CanvasFloatingStuff.cur.SetHighLightGold(true);

        if (GrandManager.ins.IsHome)
        {
            bgPanelCg.alpha = 1;
            totalMainRect.anchoredPosition = Vector2.zero;
        }
        else
        {
            bgPanelCg.alpha = 0;
            totalMainRect.anchoredPosition = rootTotalPos;

            panelMoveTween = DOVirtual.Float(0, 1, .32f, (fuk) =>
            {
                bgPanelCg.alpha = fuk;
                totalMainRect.anchoredPosition = rootTotalPos * (1 - fuk);
            }).SetEase(Ease.OutSine);
        }
    }

    public override void Close()
    {
        IsOpen = false;
        if (!UIManager.ins.IsUICanvasOpened<CanvasLose>()) CanvasFloatingStuff.cur.SetHighLightGold(false);

        if (GrandManager.ins.IsHome)
        {
            base.Close();
        }
        else
        {
            panelMoveTween?.Kill();
            panelMoveTween = DOVirtual.Float(1, 0, .24f, (fuk) =>
                {
                    bgPanelCg.alpha = fuk;
                    totalMainRect.anchoredPosition = rootTotalPos * (1 - fuk);
                }).SetEase(Ease.Linear)
                .OnComplete(base.Close);
        }
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void TriggerEffect()
    {
        for (int i = 0; i < particleImages.Length; i++) particleImages[i].Play();
    }

    public static void CheckContent()
    {
        if (cur == null) return;
        cur.CheckContentAvailable();
        cur.ResizeContent();
    }

    public void ResizeContent()
    {
        float height = contentOffsetToBottom; //offset to bottom
        for (int i = 0; i < allInAppBox.Length; i++)
        {
            var b = allInAppBox[i];
            if (b.IsActive) height += b.BoxHeight;
        }

        var size = contentRect.sizeDelta;
        size.y = height;
        contentRect.sizeDelta = size;

        Timer.ScheduleFrame(() => { scrollRect.verticalNormalizedPosition = 1; });
    }

    public void CheckContentAvailable()
    {
        for (int i = 0; i < allInAppBox.Length; i++) allInAppBox[i].Enable(true);

        if (AdsManager.isNoAds)
        {
            noAdsPackInAppBox.Enable(false);
            removeAdsBundleInAppBox.Enable(false);
        }
        else
        {
            removeAdsBundleInAppBox.Enable(BuyingPackDataFragment.cur.gameData.isShowRemoveAdsBundle);
        }

        starterInAppBox.Enable(BuyingPackDataFragment.cur.gameData.isShowStarterPack && !BuyingPackDataFragment.cur.gameData.isStarterPackBought);
        // starterInAppBox.Enable(false);
    }

    public void OnClickNoAdsPack(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickNoAdsBundle(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickStarter(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickProfessional(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickMaster(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickMega(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickGold_1(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickGold_2(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickGold_3(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickGold_4(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickGold_5(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

    public void OnClickGold_6(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "IAP_SHOP", () => { });
    }

#if UNITY_EDITOR
    [ContextMenu("SET HEIGHT")]
    public void SetHeight()
    {
        for (int i = 0; i < allInAppBox.Length; i++) allInAppBox[i].SetHeight();
    }
#endif
}
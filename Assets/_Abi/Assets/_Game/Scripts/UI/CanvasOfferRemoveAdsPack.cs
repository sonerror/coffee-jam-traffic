using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CanvasOfferRemoveAdsPack : UICanvasPrime
{
    public CanvasGroup canvasGroup;

    public GameObject mainPanelObject;
    public RectTransform mainPanelRect;

    private Vector2 rootPos;
    private Vector2 upperPos;
    private Vector2 bottomPos;

    private void Awake()
    {
        rootPos = mainPanelRect.anchoredPosition;
        upperPos = rootPos + Vector2.up * UIManager.ins.UpperOffsetForOffer;
        bottomPos = rootPos - Vector2.up * UIManager.ins.BottomOffsetForOffer;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        mainPanelObject.SetActive(false);
        mainPanelRect.anchoredPosition = upperPos;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        BuyingPackDataFragment.cur.IsShowingRemoveAds = false;
    }

    public override void Close()
    {
        canvasGroup.interactable = false;
        canvasGroup.DOFade(0, .28f);
        mainPanelRect.DOAnchorPos(bottomPos, .28f).SetEase(Ease.Linear)
            .OnComplete(base.Close);
    }

    public void Setup(bool isDelay)
    {
        if (isDelay) StartCoroutine(ie_Pop());
        else Pop();
    }

    private IEnumerator ie_Pop()
    {
        if (BuyingPackDataFragment.cur.IsShowingHalloween) yield return new WaitUntil(() => !BuyingPackDataFragment.cur.IsShowingHalloween);
        if (BuyingPackDataFragment.cur.IsShowingRemoveAdsBundle) yield return new WaitUntil(() => !BuyingPackDataFragment.cur.IsShowingRemoveAdsBundle);
        if (BuyingPackDataFragment.cur.IsShowingNeverGiveUp) yield return new WaitUntil(() => !BuyingPackDataFragment.cur.IsShowingNeverGiveUp);
        if (BuyingPackDataFragment.cur.IsShowingStarter) yield return new WaitUntil(() => !BuyingPackDataFragment.cur.IsShowingStarter);

        yield return null;
        Pop();
    }

    private void Pop()
    {
        canvasGroup.DOFade(1, .28f);

        mainPanelObject.SetActive(true);
        mainPanelRect.DOAnchorPos(rootPos, .36f).SetEase(Ease.OutSine);
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void OnClickIapButton(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "OFFER REMOVE ADS", () => { Close(); });
    }
}
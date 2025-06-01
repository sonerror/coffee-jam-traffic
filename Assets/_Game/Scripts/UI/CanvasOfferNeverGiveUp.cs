using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CanvasOfferNeverGiveUp : UICanvasPrime
{
    public CanvasGroup canvasGroup;

    public GameObject mainPanelObject;
    public RectTransform mainPanelRect;

    private Vector2 rootPos;
    private Vector2 upperPos;
    private Vector2 bottomPos;

    public TMP_Text timeLeftText;

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

        CheckTime();
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        BuyingPackDataFragment.cur.IsShowingNeverGiveUp = false;
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

        Pop();
    }

    private void Pop()
    {
        canvasGroup.DOFade(1, .28f);

        mainPanelObject.SetActive(true);
        mainPanelRect.DOAnchorPos(rootPos, .36f).SetEase(Ease.OutSine);
    }

    private void CheckTime()
    {
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            while (true)
            {
                if (BuyingPackDataFragment.cur.CheckNeverGiveUpAvailable(out var timeLeft))
                {
                    timeLeftText.text = timeLeft.ToTimeFormat_H_M_S_Dynamic_Lower_Text();
                }
                else
                {
                    timeLeftText.text = "0s";
                    yield break;
                }

                yield return Yielders.Get(1f);
            }
        }
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void OnClickIapButton(IAPButtonPro button)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(button.id, "NEVER GIVE UP", () =>
        {
            Close();
            BuyingPackDataFragment.cur.CheckNeverGiveUpAvailable(out _);
            CanvasHome.CheckNeverGiveUpBundleStatic();
        });
    }
}
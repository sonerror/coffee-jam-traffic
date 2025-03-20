using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class CanvasOfferHalloween : UICanvasPrime
{
    public CanvasGroup canvasGroup;

    public GameObject mainPanelObject;
    public RectTransform mainPanelRect;

    private Vector2 rootPos;
    private Vector2 upperPos;
    private Vector2 bottomPos;

    public SkeletonGraphic anim;

    [SpineAnimation] public string bgAnimation;
    [SpineAnimation] public string idleAnimation;
    [SpineAnimation] public string subIdleAnimation;

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

        anim.Clear();
        anim.AnimationState.SetAnimation(0, bgAnimation, true);
        StartIdle();
        return;

        void StartIdle()
        {
            anim.AnimationState.SetAnimation(1, idleAnimation, false).Complete += (entry) => { anim.AnimationState.AddAnimation(1, subIdleAnimation, false, 0).Complete += trackEntry => StartIdle(); };
        }
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        BuyingPackDataFragment.cur.IsShowingHalloween = false;

        anim.AnimationState.ClearTracks();
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
        IAPPrime.ins.ClickPurchase(button.id, "OFFER HALLOWEEN", Close);
    }
}
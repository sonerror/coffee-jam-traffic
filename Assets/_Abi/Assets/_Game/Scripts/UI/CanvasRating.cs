using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasRating : UICanvasPrime
{
    public static bool IsOpen;

    public Image[] starImages;
    public Sprite onSprite;
    public Sprite offSprite;

    public CanvasGroup canvasGroup;

    public CanvasGroup ratingCg;
    public CanvasGroup thanksCg;

    public CanvasGroup noThanksCg;

    private int currentIndex;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        IsOpen = true;
        SetStar(4);

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;

        ratingCg.alpha = 1;
        thanksCg.alpha = 0;

        noThanksCg.interactable = false;
        noThanksCg.alpha = 0;

        canvasGroup.interactable = true;
        canvasGroup.DOFade(1, 0.32f).SetEase(Ease.Linear);

        UIManager.ins.CloseUI<CanvasIapShop>();
        UIManager.ins.CloseUI<CanvasOfferRemoveAdsPack>();
        UIManager.ins.CloseUI<CanvasSetting>();

        Timer.ScheduleSupreme(3.26f, () =>
        {
            noThanksCg.interactable = true;
            noThanksCg.DOFade(1, .42f);
        });
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        IsOpen = false;
    }

    public override void Close()
    {
        canvasGroup.DOFade(0, 0.24f).SetEase(Ease.Linear)
            .OnComplete(() => base.Close());
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    private void SetStar(int index)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].sprite = index >= i ? onSprite : offSprite;
        }
    }

    public void OnClickStar(int index)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        currentIndex = index;
        SetStar(index);
    }

    public void OnClickSubmit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        canvasGroup.interactable = false;

        if (currentIndex >= 3)
        {
            GrandManager.ins.CallReview();
            Timer.ScheduleCondition(() => !GrandManager.ins.isReviewing, () =>
            {
                ratingCg.DOFade(0, 0.28f).SetEase(Ease.Linear);
                thanksCg.DOFade(1, 0.32f).SetEase(Ease.Linear);
                Timer.ScheduleSupreme(0, Close);
            });
        }
        else
        {
            Timer.ScheduleSupreme(0, () =>
            {
                ratingCg.DOFade(0, 0.28f).SetEase(Ease.Linear);
                thanksCg.DOFade(1, 0.32f).SetEase(Ease.Linear);
                Timer.ScheduleSupreme(0, Close);
            });
        }
    }
}
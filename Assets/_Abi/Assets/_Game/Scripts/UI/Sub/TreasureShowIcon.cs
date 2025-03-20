using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TreasureShowIcon : MonoBehaviour
{
    public new GameObject gameObject;
    public RectTransform rect;

    [SerializeField] private bool isGoldItem;
    public bool IsGoldItem => isGoldItem;

    public CanvasGroup canvasGroup;

    public TMP_Text itemNumText;

    public ParticleImage splashEffect;

    public void PopFromChest(Vector2 pos, Vector3 rot)
    {
        rect.localScale = Vector3.zero;
        rect.eulerAngles = Vector3.zero;
        canvasGroup.alpha = 0;

        rect.DOAnchorPos(pos, .38f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            splashEffect.Play();

            AudioManager.ins.PlaySound(SoundType.PopChestGift);
            AudioManager.ins.MakeVibrate();
        });
        rect.DORotate(rot + new Vector3(0, 0, 720), .38f).SetEase(Ease.OutSine);
        rect.DOScale(Vector3.one, .32f);

        canvasGroup.DOFade(1, .18f).SetEase(Ease.Linear);
    }

    public void SetNum(int num) => itemNumText.text = (isGoldItem ? "" : "x") + num;

    public void PopNormal() // for multi
    {
        rect.localScale = Vector3.zero;
        rect.eulerAngles = Vector3.zero;
        canvasGroup.alpha = 1;

        rect.DOScale(Vector3.one, .32f).SetEase(AssetHolder.ins.treasurePopShowOnDisplayCurve);

        canvasGroup.DOFade(1, .18f).SetEase(Ease.Linear);

        AudioManager.ins.PlaySound(SoundType.PopMoney);
        AudioManager.ins.MakeVibrate();
    }

    public void PopDisappear()
    {
        rect.DOScale(Vector3.zero, .36f).SetEase(Ease.Linear);
    }

    public void PopFromShowToPlay()
    {
        var pos = CanvasHome.cur.playButtonRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
        rect.DOAnchorPos(pos, .56f).SetEase(AssetHolder.ins.treasurePopToPlayCurve)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                HomeTreasureModule.cur.PopBlinkEffect();

                AudioManager.ins.PlaySound(SoundType.PopItem);
                AudioManager.ins.MakeVibrate();
            });
        rect.DOScale(Vector3.one * .72f, .48f).SetEase(Ease.InSine);
    }

    public void SetPos(Vector2 pos)
    {
        rect.anchoredPosition = pos;
    }

    public Vector2 GetCanvasPos()
    {
        return rect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
    }

    public void SetEnable(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}
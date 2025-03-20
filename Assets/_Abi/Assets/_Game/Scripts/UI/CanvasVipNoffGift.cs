using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class CanvasVipNoffGift : UICanvasPrime
{
    public static CanvasVipNoffGift cur;

    public CanvasGroup canvasGroup;

    public GameObject[] gifPanel;

    private void Awake()
    {
        cur = this;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, .32f).SetEase(Ease.Linear).OnComplete(() => canvasGroup.interactable = true);
    }

    public void Setup(IapShopVipBox.VipType vipType)
    {
        var flag = (int)vipType;
        for (int i = 0; i < gifPanel.Length; i++)
        {
            gifPanel[i].SetActive(i == flag);
        }
    }

    public void OnClickClose()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }
}
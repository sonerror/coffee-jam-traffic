using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CanvasWinstreakStart : UICanvasPrime
{
    public static CanvasWinstreakStart cur;
    public static bool IsOpen { get; private set; }

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public TMP_Text timeLeftText;

    private void Awake()
    {
        cur = this;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        WinstreakDataFragment.cur.IsTimeForStreak(out var time);
        UpdateTime(time.ToTimeFormat_D_H_M_S_Dynamic_Lower_Text());

        AudioManager.ins.PlaySound(SoundType.TreasureStart);
        IsOpen = true;
    }

    public void OnClickContinue()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasWinstreak>();
        Close();
    }

    public static void UpdateTime(string time)
    {
        if (cur == null) return;
        cur.timeLeftText.text = time;
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();
        IsOpen = false;
    }
}
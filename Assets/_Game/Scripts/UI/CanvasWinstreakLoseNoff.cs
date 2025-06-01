using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CanvasWinstreakLoseNoff : UICanvasPrime
{
    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public GameObject homeButtonObject;
    public GameObject restartButtonObject;

    public GameObject homeTextObject;
    public GameObject restartTextObject;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.one * .05f;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);
    }

    public void Setup(bool isHome)
    {
        homeButtonObject.SetActive(isHome);
        homeTextObject.SetActive(isHome);
        restartButtonObject.SetActive(!isHome);
        restartTextObject.SetActive(!isHome);
    }

    public void OnClickHome()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        CanvasSetting.cur.ToHomeHandle();
    }

    public void OnClickRestart()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        CanvasSetting.cur.OnReplayHandle();
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }
}
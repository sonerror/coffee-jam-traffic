using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CanvasTreasureStart : UICanvasPrime
{
    public static CanvasTreasureStart cur;

    public static bool IsOpen { get; set; }

    public TMP_Text minionTimeText;
    public TMP_Text busTimeText;

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public GameObject minionPanelObject;
    public GameObject busPanelObject;

    private void Awake()
    {
        cur = this;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        IsOpen = true;

        SetTime(HomeTreasureModule.cur.GetTime());

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        var index = (int)TreasureDataFragment.cur.CurrentTreasureType;

        minionPanelObject.SetActive(index == 0);
        busPanelObject.SetActive(index == 1);

        AudioManager.ins.PlaySound(SoundType.TreasureStart);
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        IsOpen = false;
    }

    public static void SetTime(string time)
    {
        if (cur == null) return;
        cur.minionTimeText.text = time;
        cur.busTimeText.text = time;
    }

    public void OnCLickStart()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();

        UIManager.ins.OpenUI<CanvasTreasureShow>();
    }
}
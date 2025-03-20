using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasTreasureShow : UICanvasPrime
{
    public static CanvasTreasureShow cur;

    public static bool IsOpen { get; set; }

    public TMP_Text minionTimeText;
    public TMP_Text busTimeText;

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public GameObject minionPanelObject;
    public GameObject busPanelObject;

    public ProgressBarPro minionProgressBar;
    public ProgressBarPro busProgressBar;

    public TMP_Text minionProgressText;
    public TMP_Text busProgressText;

    public GameObject[] minionRewardObjects; //assign with lock too
    public GameObject[] minionTickObjects;
    public Image[] minionNumbImages;
    public RectTransform[] minionNumbRects;

    public GameObject[] busRewardObjects; //assign with lock too
    public GameObject[] busTickObjects;
    public Image[] busNumbImages;
    public RectTransform[] busNumbRects;

    [SerializeField] private Sprite numbGreenSprite;
    [SerializeField] private Sprite numbGraySprite;

    public RectTransform minionShinyEffectRect;
    public RectTransform busShinyEffectRect;

    public ScrollRect minionScrollRect;
    public ScrollRect busScrollRect;

    [SerializeField] private float itemHeight;
    [SerializeField] private float showBoardHeight;
    [SerializeField] private float minionBoardHeight;
    [SerializeField] private float busBoardHeight;

    public TreasureItem minionPrizeTreasureItem;
    public TreasureItem busPrizeTreasureItem;

    public ParticleImage minionShinyEffect;
    public ParticleImage busShinyEffect;

    private void Awake()
    {
        cur = this;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        IsOpen = true;

        flagIsOpenTreasureShow = true;

        SetTime(HomeTreasureModule.cur.GetTime());

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.one * .08f;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        // mainPanelRect.localScale = Vector3.one;

        Setup();
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        IsOpen = false;

        flagIsOpenTreasureShow = false;
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public static void SetTime(string time)
    {
        if (cur == null) return;
        cur.minionTimeText.text = time;
        cur.busTimeText.text = time;
    }

    public void Setup()
    {
        var index = TreasureDataFragment.cur.gameData.currentLevel;
        var typeIndex = (int)TreasureDataFragment.cur.CurrentTreasureType;

        minionPanelObject.SetActive(typeIndex == 0);
        busPanelObject.SetActive(typeIndex == 1);

        var curProgressBar = typeIndex == 0 ? minionProgressBar : busProgressBar;
        var curProgressText = typeIndex == 0 ? minionProgressText : busProgressText;

        var (current, cost) = TreasureDataFragment.cur.GetCurrentAndRequireTruncateProgress();
        var progressPer = (float)current / cost;

        curProgressBar.FillAmount = progressPer;
        curProgressText.text = current + "/" + cost;

        var curRewardObjects = typeIndex == 0 ? minionRewardObjects : busRewardObjects;
        var curTickObjects = typeIndex == 0 ? minionTickObjects : busTickObjects;
        var curNumbImage = typeIndex == 0 ? minionNumbImages : busNumbImages;
        var curNumbRects = typeIndex == 0 ? minionNumbRects : busNumbRects;
        var curShinyRect = typeIndex == 0 ? minionShinyEffectRect : busShinyEffectRect;
        var currentScrollRect = typeIndex == 0 ? minionScrollRect : busScrollRect;
        var curShinyEffect = typeIndex == 0 ? minionShinyEffect : busShinyEffect;

        curShinyEffect.Stop();
        curShinyEffect.Clear();

        for (int i = 0; i < curRewardObjects.Length; i++)
        {
            if (i < index) //below
            {
                curRewardObjects[i].SetActive(false);
                curTickObjects[i].SetActive(true);
                curNumbImage[i].sprite = numbGreenSprite;
            }
            else if (i == index) //current
            {
                curRewardObjects[i].SetActive(true);
                curTickObjects[i].SetActive(false);
                curNumbImage[i].sprite = numbGraySprite;

                curShinyEffect.Play();
                curShinyRect.anchoredPosition = curNumbRects[i].anchoredPosition;
            }
            else // above
            {
                curRewardObjects[i].SetActive(true);
                curTickObjects[i].SetActive(false);
                curNumbImage[i].sprite = numbGraySprite;
            }
        }

        var curBoardHeight = typeIndex == 0 ? minionBoardHeight : busBoardHeight;
        var checkHeight = curBoardHeight - showBoardHeight;
        var offset = index * itemHeight;
        var per = Mathf.Clamp01(offset / checkHeight);


        currentScrollRect.verticalNormalizedPosition = per;
        // Debug.Log("PERR " + per + "   " + index + "    " + curBoardHeight + "   " + checkHeight + "    " + offset + " " + currentScrollRect.verticalNormalizedPosition);

        var curPrizeItem = typeIndex == 0 ? minionPrizeTreasureItem : busPrizeTreasureItem;
        curPrizeItem.SetupItemIcon(TreasureDataFragment.cur.GetCurrentGift());
    }

    // public float percent;
    //
    // [ContextMenu("SET PER")]
    // public void SetPer()
    // {
    //     var typeIndex = (int)TreasureDataFragment.cur.CurrentTreasureType;
    //     var currentScrollRect = typeIndex == 0 ? minionScrollRect : busScrollRect;
    //     currentScrollRect.verticalNormalizedPosition = percent;
    // }
}
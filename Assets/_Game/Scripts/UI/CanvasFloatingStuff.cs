using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CanvasFloatingStuff : UICanvasPrime
{
    public static CanvasFloatingStuff cur;
    public RectTransform canvasRect;
    public CanvasGroup canvasGroup;

    public TMP_Text goldText;
    public RectTransform goldRect;
    [SerializeField] private Vector2 goldRectNormalSize;
    [SerializeField] private Vector2 goldRectPlusSize;
    [SerializeField] private Vector2 goldHomePos;
    [SerializeField] private Vector2 goldIngamePos;
    public GameObject goldPlusIconObject;
    private bool isGoldPlusOn;

    public GameObject shopIapButtonObject;

    public GameObject goldObject;

    public GameObject settingObject;

    public Canvas goldCanvas;
    public bool IsGoldVisible { get; set; }

    [SerializeField] private ParticleImage[] popParticles;
    [SerializeField] private RectTransform[] popRects;

    public ParticleImage sparkEffect;

    public RectTransform topRect;

    public GameObject popTextObject;
    public RectTransform popTextRect;
    public CanvasGroup popTextCg;
    private Tween popTween;
    private const float popOffset = 6;

    public RectTransform goldNoffRect;
    public ParticleImage golfNoffEffect;
    public ParticleImage goldGainEffect;
    public RectTransform golfNoffTextRect;
    public TMP_Text goldNoffText;
    private Tween goldNoffTextMoveTween;
    [SerializeField] private AnimationCurve goldTextAlphaCurve;
    [SerializeField] private AnimationCurve goldRectMoveCurve;
    [SerializeField] private float goldNoffMoveHeight;

    private int[] goldPerGain = new int[9];
    private int currentGainIndex;

    public ParticleImage vipMoneyEffect;
    public RectTransform vipMoneyEffectRect;

    private void Awake()
    {
        cur = this;

        topRect.SafeAreaParentAdoption(canvas);
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        canvasGroup.alpha = 1;
        SetHighLightGold(false);

        UpdateGold();

        NukeGoldNoff();
        NukePopMoney();

        SetPlusIcon(true);
        cur.shopIapButtonObject.SetActive(GrandManager.ins.IsHome);

        vipMoneyEffect.Clear();

        if (true)
        {
        goldRect.anchoredPosition = GrandManager.ins.IsHome ? goldHomePos : goldIngamePos;
        }
    }

    public static void SetPlusIconDefault()
    {
        SetPlusIcon(true);
    }

    public static void SetPlusIcon(bool isOn)
    {
        if (cur == null) return;
        cur.isGoldPlusOn = isOn;
        if (isOn)
        {
            cur.goldRect.sizeDelta = cur.goldRectPlusSize;
            cur.goldPlusIconObject.SetActive(true);
        }
        else
        {
            cur.goldRect.sizeDelta = cur.goldRectNormalSize;
            cur.goldPlusIconObject.SetActive(false);
        }
    }

    public void PopMoneyShortage(Vector2 pos)
    {
        sparkEffect.Play();

        AudioManager.ins.PlaySound(SoundType.NotEnoughMoney);
        AudioManager.ins.MakeVibrate();

        popTween?.Kill();
        popTextObject.SetActive(true);
        popTextRect.anchoredPosition = pos;
        var end = pos + Vector2.up * popOffset;
        popTextCg.alpha = 0;

        popTween = DOVirtual.Float(0, 1, .38f, (fuk) =>
            {
                popTextRect.anchoredPosition = Vector2.Lerp(pos, end, fuk);
                popTextCg.alpha = fuk;
            })
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                popTween = DOVirtual.Float(0, 0, .84f, (_) => { })
                    .OnComplete(() => { popTween = DOVirtual.Float(1, 0, .32f, (fuk) => { popTextCg.alpha = fuk; }).SetEase(Ease.Linear); });
            });
    }

    public void NukePopMoney()
    {
        popTween?.Kill();
        popTextObject.SetActive(false);
    }

    public void OnClickSetting()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasSetting>();
    }

    public void SetHighLightGold(bool isHighLight)
    {
        goldCanvas.sortingOrder = isHighLight ? 6969 : canvas.sortingOrder;
    }

    public static void UpdateGold()
    {
        if (cur == null) return;

        cur.goldText.text = ResourcesDataFragment.cur.Gold.ToString();
    }

    public void PopItemStuff(FloatingEffectType effectType, Vector2 pos, Timer.Task task = null, float delay = .76f)
    {
        var index = (int)effectType;
        // var rect = popRects[index];
        var par = popParticles[index];

        // rect.anchoredPosition = pos;
        par.Play();

        AudioManager.ins.PlaySound(SoundType.ClaimStuff);

        if (task != null) Timer.ScheduleSupreme(delay, task);
    }

    public void PopGoldNoff(Vector2 pos, int amount)
    {
        NukeGoldNoff();

        AudioManager.ins.PlaySound(SoundType.PopMoney);
        AudioManager.ins.MakeVibrate();

        goldNoffRect.anchoredPosition = pos;
        golfNoffTextRect.anchoredPosition = pos;

        goldNoffText.text = "+" + amount;
        goldNoffText.alpha = 0;

        ResetGoldPerGain();

        var averageGain = (float)amount / 9;
        var gain = Mathf.FloorToInt(averageGain);
        var finalGain = amount - gain * 8;
        for (int i = 0; i < 8; i++)
        {
            goldPerGain[i] = gain;
        }

        goldPerGain[8] = finalGain;

        golfNoffEffect.Play();

        var targetPos = pos + Vector2.up * goldNoffMoveHeight;
        goldNoffTextMoveTween?.Kill();
        goldNoffTextMoveTween = DOVirtual.Float(0, 1, .96f, (fuk) =>
        {
            golfNoffTextRect.anchoredPosition = Vector2.Lerp(pos, targetPos, goldRectMoveCurve.Evaluate(fuk));
            goldNoffText.alpha = goldTextAlphaCurve.Evaluate(fuk);
        }).SetEase(Ease.Linear);
    }

    private void ResetGoldPerGain()
    {
        for (int i = 0; i < goldPerGain.Length; i++) goldPerGain[i] = 0;
        currentGainIndex = 0;
    }

    public void NukeGoldNoff()
    {
        goldNoffTextMoveTween?.Kill();
        goldNoffText.alpha = 0;

        goldGainEffect.Stop();
        goldGainEffect.Clear();

        golfNoffEffect.Stop();
        golfNoffEffect.Clear();
    }

    public void PopGainGoldEffect() // event in golf noff effect
    {
        goldGainEffect.Play();

        AudioManager.ins.PlaySound(SoundType.SpinTick);
        if (currentGainIndex < 9) ResourcesDataFragment.cur.AddGold(goldPerGain[currentGainIndex], "", true);
        currentGainIndex++;
    }

    public void OnClickGoldButton()
    {
        if (CanvasIapShop.IsOpen || !isGoldPlusOn) return;
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasIapShop>();
    }

    public void OnClickShopIap()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasIapShop>();
    }

    public void PopVipMoney(Vector2 pos)
    {
        vipMoneyEffectRect.anchoredPosition = pos;
        vipMoneyEffect.Play();
    }
}

public enum CurrencyDisplayState
{
    OnlyGold,
    OnlySetting,
    GoldSetting,
    NoneAbove,
}

public enum FloatingEffectType
{
    GoldSplash,
    SwapCarSplash,
    VipBusSplash,
    SwapMinionSplash,
}
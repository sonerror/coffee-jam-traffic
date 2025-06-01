using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RewardBundle = WinstreakDataFragment.RewardBundle;

public class HomeWinstreakModule : MonoBehaviour
{
    public static HomeWinstreakModule cur;

    public static bool IsClaimStuff { get; set; }

    private static WaitUntil waitForWinStreakCanvas = new WaitUntil(() => !CanvasWinstreak.IsOpen);

    private bool isWaitForOther;

    [Header("ICON")] public TMP_Text streakNum;
    public RectTransform streakTextRect;

    // public Image iconProgress;
    public ParticleImage increaseStreakEffect;
    public ParticleImage decreaseStreakEffect;

    private int recordedStreak;

    public GameObject iconObject;

    public TMP_Text timeLeftText;

    public TreasureShowIcon swapCarTsi;
    public TreasureShowIcon vipTsi;
    public TreasureShowIcon swapMinionTsi;

    [SerializeField] private AnimationCurve popCurve;

    private void Awake()
    {
        cur = this;
    }

    private void Start()
    {
        CanvasWinstreak.RecordedStreak = WinstreakDataFragment.cur.GetClampStreak();
    }

    private void OnEnable()
    {
        swapCarTsi.SetEnable(false);
        vipTsi.SetEnable(false);
        swapMinionTsi.SetEnable(false);
    }

    public void CheckTime()
    {
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            yield return new WaitUntil(() => UnbiasedTime.IsValidTime);
            yield return null;
            while (true)
            {
                if (WinstreakDataFragment.cur.IsTimeForStreak(out var timeLeft))
                {
                    var timeString = timeLeft.ToTimeFormat_D_H_M_S_Dynamic_Lower_Text();
                    timeLeftText.text = timeString;
                    CanvasWinstreak.UpdateTime(timeString);
                    CanvasWinstreakStart.UpdateTime(timeString);
                    yield return Yielders.Get(1);
                }
                else
                {
                    timeLeftText.text = "";
                    CanvasWinstreak.UpdateTime("");
                    CanvasWinstreakStart.UpdateTime("");
                    StartCoroutine(ie_WaitForClaim());
                    yield break;
                }
            }
        }

        IEnumerator ie_WaitForClaim()
        {
            if (CanvasWinstreak.cur != null) yield return new WaitUntil(() => !CanvasWinstreak.cur.IsClaimReady);
            UIManager.ins.CloseUI<CanvasWinstreak>();
            CheckWinStreakPop();
        }
    }

    public void InstantUpdateStreakIcon()
    {
        recordedStreak = WinstreakDataFragment.cur.GetSubProgressAndCost().Item1;
        streakNum.text = WinstreakDataFragment.cur.IsOutLevel() ? "" : recordedStreak.ToString();
        streakTextRect.localScale = Vector3.one;
        // var (current, cost) = WinstreakDataFragment.cur.GetSubProgressAndCost();
        // var per = (float)current / cost;

        // iconProgress.fillAmount = per;
    }

    public void CheckWinStreakPop()
    {
        var isNewSeason = WinstreakDataFragment.cur.IsTimeForStreak(out _) && WinstreakDataFragment.cur.IsNewStreakSession();
        var isLose = WinstreakDataFragment.cur.IsJustLose;
        var isWin = WinstreakDataFragment.cur.IsJustIncreaseStreak;
        var isUpdateIcon = (isLose || isWin) && !isNewSeason && !WinstreakDataFragment.cur.IsOutLevel();
        var isPopCanvas = false;
        int targetChestIndex = 0;
        List<RewardBundle> targetBundle = null;

        WinstreakDataFragment.cur.ResetFlag();

        // Debug.Log("IS NEW SEASON " + isNewSeason + "   " + WinstreakDataFragment.cur.IsTimeForStreak(out _));
        if (WinstreakDataFragment.cur.IsTimeForStreak(out _))
        {
            //up date icon
            // Debug.Log("ACTIVE " + isNewSeason);
            iconObject.SetActive(!isNewSeason);
            CanvasHome.CheckRightIconPosStatic();
            //check if claimable then open canvas, handle all data immediately on open canvas
            if (WinstreakDataFragment.cur.IsEnoughStreak())
            {
                var index = WinstreakDataFragment.cur.GetCurrentCheckPointIndex();
                if (index > 0 && !WinstreakDataFragment.cur.gameData.giftState[index])
                {
                    WinstreakDataFragment.cur.gameData.giftState[index] = true;
                    var rw = WinstreakDataFragment.cur.rewardBundlesList[index].list;

                    for (int i = 0; i < rw.Count; i++)
                    {
                        var rb = rw[i];
                        WinstreakDataFragment.cur.PendingRewardBundle(rb);
                    }

                    targetChestIndex = index - 1;
                    targetBundle = rw;
                    isPopCanvas = true;
                    IsClaimStuff = true;
                }
            }

            if (!isUpdateIcon) InstantUpdateStreakIcon();

            StartCoroutine(ie_WaitForOther());
        }
        else
        {
            iconObject.SetActive(false);
            CanvasHome.CheckRightIconPosStatic();
        }

        return;

        IEnumerator ie_WaitForOther()
        {
            isWaitForOther = true;
            yield return new WaitUntil(() => UnbiasedTime.IsValidTime);
            yield return null;
            //checking pack
            // Debug.Log("WAIT 1");
            var pack = BuyingPackDataFragment.cur;
            if (pack.IsShowingRemoveAdsBundle) yield return new WaitUntil(() => !pack.IsShowingRemoveAdsBundle);
            if (pack.IsShowingNeverGiveUp) yield return new WaitUntil(() => !pack.IsShowingNeverGiveUp);
            if (pack.IsShowingStarter) yield return new WaitUntil(() => !pack.IsShowingStarter);
            if (pack.IsShowingRemoveAds) yield return new WaitUntil(() => !pack.IsShowingRemoveAds);
            // Debug.Log("WAIT 2");

            yield return new WaitUntil(() => !HomeTreasureModule.cur.IsHandlingTreasure);
            yield return null;
            // Debug.Log("WAIT 3");

            if (CanvasTreasureStart.IsOpen) yield return new WaitUntil(() => !CanvasTreasureStart.IsOpen);
            yield return null;
            // Debug.Log("WAIT 4");

            if (CanvasTreasureShow.IsOpen) yield return new WaitUntil(() => !CanvasTreasureShow.IsOpen);
            yield return null;
            // Debug.Log("WAIT 5");

            UIManager.ins.OpenUI<CanvasBlockage>();

            if (isUpdateIcon && !(isLose && recordedStreak == 0))
            {
                yield return Yielders.Get(0.32f);

                if (isLose)
                {
                    decreaseStreakEffect.Play();
                    AudioManager.ins.PlaySound(SoundType.TextDown);
                }
                else if (isWin)
                {
                    increaseStreakEffect.Play();
                    AudioManager.ins.PlaySound(SoundType.TextUp);
                }

                var root = recordedStreak;
                var target = WinstreakDataFragment.cur.GetSubProgressAndCost().Item1;

                recordedStreak = target;

                if (IsClaimStuff)
                {
                    var curCheckpointIndex = WinstreakDataFragment.cur.GetCurrentCheckPointIndex();
                    var curCheck = WinstreakDataFragment.cur.checkPoints[curCheckpointIndex];
                    var preCheck = WinstreakDataFragment.cur.checkPoints[curCheckpointIndex - 1];
                    target = curCheck - preCheck;
                }

                streakTextRect.DOPunchScale(Vector3.one * .82f, .986f, 1).SetEase(popCurve);
                streakNum.text = target.ToString();
                yield return Yielders.Get(1.26f);
            }

            UIManager.ins.CloseUI<CanvasBlockage>();

            if (isPopCanvas)
            {
                UIManager.ins.OpenUI<CanvasWinstreak>();
                CanvasWinstreak.cur.PopClaim(targetBundle, targetChestIndex);
            }

            if (isNewSeason)
            {
                StartCoroutine(ie_PopNewSeason());
            }

            isWaitForOther = false;
        }

        IEnumerator ie_PopNewSeason()
        {
            if (isWaitForOther) yield return new WaitUntil(() => isWaitForOther);
            if (CanvasWinstreak.IsOpen) yield return waitForWinStreakCanvas;

            UIManager.ins.OpenUI<CanvasWinstreakStart>();
            CheckWinStreakPop();
        }
    }
}
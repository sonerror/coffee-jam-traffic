using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WinstreakDataFragment.RewardType;
using RewardBundle = WinstreakDataFragment.RewardBundle;

public class CanvasWinstreak : UICanvasPrime
{
    public static CanvasWinstreak cur;

    public CanvasGroup canvasGroup;
    public static bool IsOpen { get; private set; }

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public Image subProgressBar;
    public TMP_Text timeLeftText;
    public TMP_Text winLevelText;

    private const string AAA = "Win ";
    private const string BBB = " levels in a row to win!";

    public Image progressBar;

    [SerializeField] private float[] percentPerTiles;
    [SerializeField] private float[] percentCheckpoints;

    [SerializeField] private Vector2 backgroundStartPos;
    [SerializeField] private Vector2 backgroundEndPos;
    [SerializeField] private Vector2 elevatorStartPos;
    [SerializeField] private Vector2 elevatorEndPos;
    [SerializeField] private Vector2 grandChestStartPos;
    [SerializeField] private Vector2 grandChestEndPos;

    public RectTransform backgroundRect;
    public RectTransform subBackgroundRect;
    public RectTransform grandChestRect;
    public RectTransform elevatorRect;

    [SerializeField] private float offsetPercent;

    public GameObject claimPopObject;
    public RectTransform claimPopRect;
    public GameObject[] claimItemObjects;
    public Vector2[] claimPos;
    [SerializeField] private Vector2[] claimPopSize;
    private Tween claimPopTween;
    public bool IsClaimReady { get; private set; }
    private List<RewardBundle> currentRewardBundles;

    public RectTransform[] chestFormation_1;
    public RectTransform[] chestFormation_2;
    public RectTransform[] chestFormation_3;
    public RectTransform[] chestFormation_4;

    public WinstreakChest winstreakChest;
    public WinstreakChest subWinstreakChest;

    public GameObject[] numTextObjects;
    public GameObject[] tickObjects;
    public Image[] blockPointImages;
    public Vector2[] blockPointPoses;
    [SerializeField] private Sprite normalBlockSprite;
    [SerializeField] private Sprite doneBlockSprite;

    public GameObject tutObject;
    public Animation tutAnim;

    public TMP_Text currentStreakText;

    public CanvasGroup bgPanelCg;

    public static int RecordedStreak { get; set; } = -1;
    public static bool IsJustLoseStreak { get; set; }

    public RectTransform subBgRect;
    [SerializeField] private Vector2 subBgRootPos;
    [SerializeField] private Vector2 subBgTargetPos;
    [SerializeField] private Vector2 subBgMiddlePos;

    public ParticleImage streakTextEffect;
    private Tween streakTextTween;
    public RectTransform streakTextRect;

    private bool isChangingBg;

    public ParticleImage confettiEffect;

    private static int PER = Shader.PropertyToID("_per");
    [SerializeField] private Material cloudMat;
    [SerializeField] private Material subCloudMat;

#if UNITY_EDITOR
    [SerializeField] private int testPercent;

    [ContextMenu("SET")]
    public void SET()
    {
        WinstreakDataFragment.cur.gameData.streak = testPercent;
        SetProgress();
    }
#endif

    private void Awake()
    {
        cur = this;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        canvasGroup.interactable = true;

        IsOpen = true;

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.one * .05f;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        claimPopTween?.Kill();
        claimPopObject.SetActive(false);
        IsClaimReady = false;

        tutObject.SetActive(false);
        bgPanelCg.alpha = 1;

        streakTextTween?.Kill();
        streakTextRect.localScale = Vector3.one;

        SetProgress();
        SetSubProgress();

        WinstreakDataFragment.cur.IsTimeForStreak(out var time);
        UpdateTime(time.ToTimeFormat_D_H_M_S_Dynamic_Lower_Text());

        confettiEffect.Clear();
    }

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        IsOpen = false;

        OnClickClaimPopup();
    }

    public override void Close()
    {
        canvasGroup.interactable = false;
        popTween?.Kill();
        popTween = mainPanelRect.DOScale(Vector3.zero, .24f).SetUpdate(true).SetEase(PopCurve);
        bgPanelCg.DOFade(0, .28f).SetEase(Ease.Linear)
            .OnComplete(() => base.Close());
    }

    public static void UpdateTime(string time)
    {
        if (cur == null) return;
        cur.timeLeftText.text = time;
    }

    public static void HandleJustLose()
    {
        if (IsJustLoseStreak)
        {
            RecordedStreak = WinstreakDataFragment.cur.gameData.streak;
        }
    }

    public void PopClaim(List<RewardBundle> rewardBundles, int chestIndex)
    {
        IsClaimReady = true;

        currentRewardBundles = rewardBundles;

        Timer.ScheduleCondition(() => !isChangingBg, () =>
        {
            AudioManager.ins.PlaySound(SoundType.TreasureStart);
            confettiEffect.Play();

            Timer.ScheduleSupreme(0.72f, () =>
            {
                claimPopObject.SetActive(true);
                claimPopRect.localScale = Vector3.one * .05f;
                claimPopTween = claimPopRect.DOScale(Vector3.one * 1.42f, .32f).SetEase(Ease.OutSine);

                for (int i = 0; i < claimItemObjects.Length; i++)
                {
                    claimItemObjects[i].SetActive(i == chestIndex);
                }

                claimPopRect.anchoredPosition = claimPos[rewardBundles.Count];
                claimPopRect.sizeDelta = claimPopSize[rewardBundles.Count];
            });
        });
    }

    public void OnClickExit()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void OnClickTut()
    {
        tutObject.SetActive(true);
        tutAnim.Play();
    }

    public void OnClickExitTut()
    {
        tutObject.SetActive(false);
    }

    public void OnClickClaimPopup()
    {
        if (!IsClaimReady) return;
        IsClaimReady = false;

        Close();

        AudioManager.ins.PlaySound(SoundType.UIClick);
        AudioManager.ins.PlaySound(SoundType.ClaimStuff);

        var formation = currentRewardBundles.Count switch
        {
            1 => chestFormation_1,
            2 => chestFormation_2,
            3 => chestFormation_3,
            4 => chestFormation_4,
            _ => chestFormation_4,
        };

        for (int i = 0; i < currentRewardBundles.Count; i++)
        {
            var rb = currentRewardBundles[i];
            CallEffect(rb, formation[i].anchoredPosition);
        }

        return;

        void CallEffect(RewardBundle rewardBundle, Vector2 pos)
        {
            switch (rewardBundle.rewardType)
            {
                case Gold:
                    CanvasFloatingStuff.cur.PopGoldNoff(pos, rewardBundle.num); //plus money
                    break;
                case Refresh:
                    HomeWinstreakModule.cur.swapCarTsi.SetEnable(true);
                    HomeWinstreakModule.cur.swapCarTsi.SetPos(pos);
                    HomeWinstreakModule.cur.swapCarTsi.SetNum(rewardBundle.num);
                    HomeWinstreakModule.cur.swapCarTsi.PopNormal();
                    Timer.ScheduleSupreme(0.34f, () => HomeWinstreakModule.cur.swapCarTsi.PopFromShowToPlay());
                    WinstreakDataFragment.cur.HandleRewardBundle(rewardBundle);
                    break;
                case Vip:
                    HomeWinstreakModule.cur.vipTsi.SetEnable(true);
                    HomeWinstreakModule.cur.vipTsi.SetPos(pos);
                    HomeWinstreakModule.cur.vipTsi.SetNum(rewardBundle.num);
                    HomeWinstreakModule.cur.vipTsi.PopNormal();
                    Timer.ScheduleSupreme(0.34f, () => HomeWinstreakModule.cur.vipTsi.PopFromShowToPlay());
                    WinstreakDataFragment.cur.HandleRewardBundle(rewardBundle);
                    break;
                case Sort:
                    HomeWinstreakModule.cur.swapMinionTsi.SetEnable(true);
                    HomeWinstreakModule.cur.swapMinionTsi.SetPos(pos);
                    HomeWinstreakModule.cur.swapMinionTsi.SetNum(rewardBundle.num);
                    HomeWinstreakModule.cur.swapMinionTsi.PopNormal();
                    Timer.ScheduleSupreme(0.34f, () => HomeWinstreakModule.cur.swapMinionTsi.PopFromShowToPlay());
                    WinstreakDataFragment.cur.HandleRewardBundle(rewardBundle);
                    break;
            }
        }
    }

    public void SetProgress()
    {
        var checkPointIndex = WinstreakDataFragment.cur.GetCurrentCheckPointIndex();
        var targetPrizeIndex = checkPointIndex + 1 + (HomeWinstreakModule.IsClaimStuff ? -1 : 0);
        var (current, cost) = WinstreakDataFragment.cur.GetSubProgressAndCost();
        var subPer = (float)current / cost;
        var per = percentCheckpoints[checkPointIndex] + (checkPointIndex + 1 < percentPerTiles.Length ? percentPerTiles[checkPointIndex + 1] * subPer : 0);

        // Debug.Log("SASA " + checkPointIndex + "  " + current + "  " + cost + "   " + per + "   " + subPer);

        if (RecordedStreak < 0 || RecordedStreak == WinstreakDataFragment.cur.GetClampStreak())
        {
            RecordedStreak = WinstreakDataFragment.cur.GetClampStreak();
            currentStreakText.text = RecordedStreak.ToString();
            subBgRect.anchoredPosition = WinstreakDataFragment.cur.IsOutLevel() ? subBgMiddlePos : subBgTargetPos;
            MoveBg(per);

            isChangingBg = false;
        }
        else
        {
            isChangingBg = true;

            UIManager.ins.OpenUI<CanvasBlockage>();
            subBgRect.anchoredPosition = subBgRootPos;
            var preCheckPointIndex = WinstreakDataFragment.cur.GetCurrentCheckPointIndex(RecordedStreak);
            var (preCur, preCost) = WinstreakDataFragment.cur.GetSubProgressAndCost(RecordedStreak);
            var preSubPer = (float)preCur / preCost;
            var prePer = percentCheckpoints[preCheckPointIndex] + (preCheckPointIndex + 1 < percentPerTiles.Length ? percentPerTiles[preCheckPointIndex + 1] * preSubPer : 0);
            var diff = Mathf.Abs(per - prePer);
            var recorded = RecordedStreak;
            RecordedStreak = WinstreakDataFragment.cur.GetClampStreak();

            currentStreakText.text = recorded.ToString();
            var currentTextStreak = recorded;
            MoveBg(prePer);

            Debug.Log("PRE PER " + prePer + "   " + per);

            Timer.ScheduleSupreme(0.42f, () =>
            {
                DOVirtual.Float(0, 1, 28.24f * diff, (fuk) =>
                    {
                        var moveBgPer = Mathf.Lerp(prePer, per, fuk);
                        MoveBg(moveBgPer);
                        var targetStreak = Mathf.FloorToInt(Mathf.Lerp(recorded, RecordedStreak, fuk));
                        if (targetStreak > currentTextStreak)
                        {
                            currentTextStreak = targetStreak;
                            currentStreakText.text = currentTextStreak.ToString();
                            PopWinStreakText();
                        }
                        else if (targetStreak < currentTextStreak)
                        {
                            currentTextStreak = targetStreak;
                            currentStreakText.text = currentTextStreak.ToString();

                            AudioManager.ins.PlaySound(SoundType.TextDown);
                            AudioManager.ins.MakeVibrate();
                        }
                    }).SetEase(Ease.InSine)
                    .OnComplete(() =>
                    {
                        UIManager.ins.CloseUI<CanvasBlockage>();
                        subBgRect.DOAnchorPos(WinstreakDataFragment.cur.IsOutLevel() ? subBgMiddlePos : subBgTargetPos, .28f).SetEase(Ease.Linear);
                        isChangingBg = false;
                    });
            });
        }

        if (targetPrizeIndex < WinstreakDataFragment.cur.rewardBundlesList.Count)
        {
            winstreakChest.gameObject.SetActive(true);
            subWinstreakChest.gameObject.SetActive(true);

            winstreakChest.SetPos(blockPointPoses[targetPrizeIndex]);
            winstreakChest.SetRewardBubble(targetPrizeIndex);
            subWinstreakChest.SetRewardBubble(targetPrizeIndex);
        }
        else
        {
            winstreakChest.gameObject.SetActive(false);
            subWinstreakChest.gameObject.SetActive(false);
        }

        for (int i = 0; i < blockPointImages.Length; i++)
        {
            var isDone = i <= checkPointIndex - 1; //skip first block
            blockPointImages[i].sprite = isDone ? doneBlockSprite : normalBlockSprite;
            numTextObjects[i].SetActive(!isDone);
            tickObjects[i].SetActive(isDone);
        }

        if (HomeWinstreakModule.IsClaimStuff)
        {
            HomeWinstreakModule.IsClaimStuff = false;
            HomeWinstreakModule.cur.InstantUpdateStreakIcon();
        }

        return;

        void MoveBg(float percent)
        {
            var elevatorPos = Vector2.Lerp(elevatorStartPos, elevatorEndPos, percent);
            var bgPos = Vector2.Lerp(backgroundStartPos, backgroundEndPos, percent) + Vector2.up * (elevatorPos.y - elevatorStartPos.y);
            var grandChestPos = Vector2.Lerp(grandChestStartPos, grandChestEndPos, percent) + Vector2.up * (elevatorPos.y - elevatorStartPos.y);

            backgroundRect.anchoredPosition = bgPos;
            subBackgroundRect.anchoredPosition = bgPos;
            grandChestRect.anchoredPosition = grandChestPos;
            elevatorRect.anchoredPosition = elevatorPos;

            progressBar.fillAmount = percent;

            cloudMat.SetFloat(PER, percent);
            subCloudMat.SetFloat(PER, percent);
        }

        void PopWinStreakText()
        {
            AudioManager.ins.PlaySound(SoundType.PopItem);
            AudioManager.ins.MakeVibrate();

            streakTextTween?.Complete();
            streakTextTween = streakTextRect.DOPunchScale(Vector3.one * .32f, .28f).SetEase(Ease.OutSine);
            streakTextEffect.Play();
        }
    }

    public void SetSubProgress()
    {
        var (current, cost) = WinstreakDataFragment.cur.GetSubProgressAndCost();
        var per = (float)current / cost;

        subProgressBar.fillAmount = per;

        winLevelText.text = AAA + WinstreakDataFragment.cur.GetSubProgressAndCost().Item2 + BBB;
    }
}
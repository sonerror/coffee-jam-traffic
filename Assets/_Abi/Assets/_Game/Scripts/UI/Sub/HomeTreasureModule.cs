using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HomeTreasureModule : MonoBehaviour // set with own canvas with sorting order higher than floating canvas
{
    public static HomeTreasureModule cur;
    public new GameObject gameObject;

    public ProgressBarPro barPro;
    public TMP_Text progressText;
    public TMP_Text timeLeftText;

    [SerializeField] private AnimationCurve barIncreaseCurve;
    [SerializeField] private AnimationCurve barIncreaseDurationCurve;

    public RectTransform objectiveIconRect;
    public RectTransform itemRestRect;

    public RectTransform singleItemShowRect;
    public RectTransform chestShowRect;

    public GameObject firstDarkObject;
    public Image firstDarkImage;
    public GameObject secondDarkObject;
    public Image secondDarkImage;
    public GameObject thirdDarkObject;
    public Image thirdDarkImage;

    public GameObject blockagePanelObject;

    public TreasureItem objectTreasureItem;
    public TreasureItem itemTreasureItem;

    public Image objectiveImage;
    public Sprite[] objectiveSprites;

    public RectTransform tapToContinueRect;
    private Tween tapToContinueTween;

    public ParticleImage objectiveEffect;

    public bool IsHandlingTreasure { get; set; }

    public ParticleImage splashEffect;

    public TreasureShowIcon[] treasureShowIcons; //currently have 4

    public RectTransform[] chestFormation_1;
    public RectTransform[] chestFormation_2;
    public RectTransform[] chestFormation_3;
    public RectTransform[] chestFormation_4;

    public RectTransform[] multiFormation_1;
    public RectTransform[] multiFormation_2;
    public RectTransform[] multiFormation_3;
    public RectTransform[] multiFormation_4;

    [Header("Treasure Item")] public Sprite[] itemSprites;
    public Vector2[] imageSize;
    public Vector2[] imagePos;

    public GameObject showCaseObject;

    private void Awake()
    {
        cur = this;
    }

    public void OnClickTreasure()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasTreasureShow>();
    }

    private static readonly WaitUntil CheckTimeCondition = new WaitUntil(() => UnbiasedTime.IsValidTime);

    public void JustBeNormal()
    {
        if (TreasureDataFragment.cur.IsTreasureAvailable() && UnbiasedTime.IsValidTime)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            if (TreasureDataFragment.cur.IsTreasureAvailable() && !UnbiasedTime.IsValidTime) StartCoroutine(ie_CheckTimeCondition());
        }

        IsHandlingTreasure = false;
        showCaseObject.SetActive(false);

        firstDarkObject.SetActive(false);
        secondDarkObject.SetActive(false);
        thirdDarkObject.SetActive(false);
        blockagePanelObject.SetActive(false);

        NukeTreasureShowIcon();

        tapToContinueRect.localScale = Vector3.zero;
        tapToContinueTween?.Kill();

        objectTreasureItem.gameObject.SetActive(false);

        var reward = TreasureDataFragment.cur.GetCurrentGift();
        itemTreasureItem.SetAnchorPos(itemRestRect);
        itemTreasureItem.SetupItemIcon(reward);
        itemTreasureItem.SetEffect(false);
        itemTreasureItem.NukeChestAnim();
        itemTreasureItem.SetSize(TreasureItem.ShowPosition.HomeDisplay);

        SetProgressBar();

        objectiveImage.sprite = objectiveSprites[(int)TreasureDataFragment.cur.CurrentTreasureType];

        return;

        IEnumerator ie_CheckTimeCondition()
        {
            Debug.Log("CHECKING TIME MODULE");

            yield return CheckTimeCondition;
            gameObject.SetActive(true);
            if (!IsHandlingTreasure) HandleTreasure();
        }
    }

    public void CheckTime()
    {
        if (!TreasureDataFragment.cur.IsTreasureAvailable()) return;
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            timeLeftText.text = "";
            yield return CheckTimeCondition;
            Debug.Log("CHECK TIME " + UnbiasedTime.IsValidTime);
            while (true)
            {
                // Debug.Log("CHECK TIME");
                if (TreasureDataFragment.cur.CheckNewEventTime(out var timeLeft))
                {
                    const string ENDED = "";
                    timeLeftText.text = ENDED;
                    CanvasTreasureShow.SetTime(ENDED);
                    CanvasTreasureStart.SetTime(ENDED);
                    yield return null;
                    StartCoroutine(ie_WaitForReturnGiftComplete());
                    yield break;
                }

                var timeString = timeLeft.ToTimeFormat_D_H_M_S_Dynamic_Lower_Text();
                timeLeftText.text = timeString;
                CanvasTreasureShow.SetTime(timeString);
                CanvasTreasureStart.SetTime(timeString);

                yield return Yielders.Get(1);
            }
        }

        IEnumerator ie_WaitForReturnGiftComplete()
        {
            //checking pack
            var pack = BuyingPackDataFragment.cur;
            if (pack.IsShowingRemoveAdsBundle) yield return new WaitUntil(() => !pack.IsShowingRemoveAdsBundle);
            if (pack.IsShowingNeverGiveUp) yield return new WaitUntil(() => !pack.IsShowingNeverGiveUp);
            if (pack.IsShowingStarter) yield return new WaitUntil(() => !pack.IsShowingStarter);
            if (pack.IsShowingRemoveAds) yield return new WaitUntil(() => !pack.IsShowingRemoveAds);

            yield return new WaitUntil(() => !IsHandlingTreasure && !UICanvasPrime.flagIsOpenTreasureShow);

            //show new season
            UIManager.ins.OpenUI<CanvasTreasureStart>();
            JustBeNormal();
            CheckTime();
        }
    }

    public string GetTime()
    {
        return timeLeftText.text;
    }

    private static readonly WaitUntil WaitForTap = new WaitUntil(() => Input.GetMouseButtonDown(0));

    public void HandleTreasure()
    {
        if (!TreasureDataFragment.cur.IsTreasureAvailable() || !UnbiasedTime.IsValidTime) return;

        IsHandlingTreasure = true;
        showCaseObject.SetActive(true);

        var rootProgress = TreasureDataFragment.cur.gameData.currentProgress;
        var gainedObjectiveAmount = TreasureDataFragment.cur.ProcessRecord();

        Debug.Log("HANDLE " + gainedObjectiveAmount);

        var treasureType = TreasureDataFragment.cur.CurrentTreasureType;

        var rootCurCostList = new List<(int, int, int)>();
        var rewardList = new List<TreasureDataFragment.RewardBundle>();

        while (!TreasureDataFragment.cur.IsOutLevel() && TreasureDataFragment.cur.IsFullAtCurrentLevel())
        {
            var tmp = TreasureDataFragment.cur.GetCurrentAndRequireTruncateProgress();
            rootCurCostList.Add((rootProgress, tmp.Item1, tmp.Item2));
            var rw = TreasureDataFragment.cur.GetCurrentGift();
            rewardList.Add(rw);
            TreasureDataFragment.cur.PendingReward(rw, "TREASURE_" + treasureType + "_" + (TreasureDataFragment.cur.gameData.currentLevel + 1));

            FirebaseManager.Ins.game_treasure_claim(treasureType.ToString(), TreasureDataFragment.cur.gameData.currentLevel.ToString());

            TreasureDataFragment.cur.IncreaseLevel();
            rootProgress = 0;
        }

        var tmpLast = TreasureDataFragment.cur.GetCurrentAndRequireTruncateProgress();
        var lastCurCost = (TreasureDataFragment.cur.IsOutLevel() ? tmpLast.Item1 : rootProgress, tmpLast.Item1, tmpLast.Item2);
        var lastGift = TreasureDataFragment.cur.GetCurrentGift();

        TreasureDataFragment.cur.Save();

        StartCoroutine(ie_Show());
        return;

        IEnumerator ie_Show()
        {
            if (gainedObjectiveAmount == 0)
            {
                IsHandlingTreasure = false;
                yield break;
            }

            //checking pack
            var pack = BuyingPackDataFragment.cur;
            if (pack.IsShowingRemoveAdsBundle) yield return new WaitUntil(() => !pack.IsShowingRemoveAdsBundle);
            if (pack.IsShowingNeverGiveUp) yield return new WaitUntil(() => !pack.IsShowingNeverGiveUp);
            if (pack.IsShowingStarter) yield return new WaitUntil(() => !pack.IsShowingStarter);
            if (pack.IsShowingRemoveAds) yield return new WaitUntil(() => !pack.IsShowingRemoveAds);

            blockagePanelObject.SetActive(true);

            // firstDarkObject.SetActive(true);
            // firstDarkImage.SetAlpha(0);
            // firstDarkImage.DOFade(253f / 255, .28f).SetEase(Ease.OutSine);

            objectTreasureItem.gameObject.SetActive(true);
            objectTreasureItem.SetupForObject(gainedObjectiveAmount, treasureType);
            objectTreasureItem.SetAnchorPos(singleItemShowRect);
            objectTreasureItem.SetSize(TreasureItem.ShowPosition.HomeShow, true);

            AudioManager.ins.PlaySound(SoundType.GainObjective);
            AudioManager.ins.MakeVibrate();

            yield return Yielders.Get(.78f);
            yield return objectTreasureItem.PopBackToDisPlayForObject();

            if (rewardList.Count > 0)
            {
                var isMultiplePrize = rewardList.Count > 1;
                if (isMultiplePrize)
                {
                    var multiRewardList = new List<TreasureDataFragment.RewardBundle>();
                    for (int i = 0; i < rewardList.Count; i++)
                    {
                        var curReward = rewardList[i];
                        yield return UpdateProgressBar(true, rootCurCostList[i]);

                        if (curReward.rewardFlag >= 4) //is chest
                        {
                            yield return PauseToChest(curReward);
                            itemTreasureItem.SetEffect(false);
                            itemTreasureItem.SetAnchorPos(itemRestRect);
                            yield return itemTreasureItem.PopOutOnlyMorphing(i + 1 < rewardList.Count ? rewardList[i + 1] : lastGift);
                        }
                        else
                        {
                            multiRewardList.Add(curReward);
                            yield return itemTreasureItem.PopMorphing(i + 1 < rewardList.Count ? rewardList[i + 1] : lastGift);
                        }
                    }

                    yield return UpdateProgressBar(true, lastCurCost);
                    yield return ShowMultiPrize(multiRewardList);
                }
                else
                {
                    yield return UpdateProgressBar(true, rootCurCostList[0]);
                    var curReward = rewardList[0];
                    if (curReward.rewardFlag < 4) // not a chest
                    {
                        itemTreasureItem.SetEffect(true);

                        ShowSecondDark();
                        yield return itemTreasureItem.PopFromDisplayToShowMain();

                        ShowTap();
                        yield return WaitForTap;
                        AudioManager.ins.PlaySound(SoundType.UIClick);
                        StopTap();
                        HideSecondDark();

                        if (curReward.GetRewardType() == TreasureDataFragment.RewardType.Gold)
                        {
                            yield return itemTreasureItem.PopDisappear();
                            var pos = itemTreasureItem.rect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
                            CanvasFloatingStuff.cur.PopGoldNoff(pos, curReward.num);
                            yield return Yielders.Get(0.52f);
                        }
                        else
                        {
                            yield return itemTreasureItem.PopFromShowToPlay();
                        }
                    }
                    else
                    {
                        yield return PauseToChest(curReward);
                    }

                    yield return ShowLastItem();
                }
            }

            yield return UpdateProgressBar(false, lastCurCost, true);

            blockagePanelObject.SetActive(false);

            IsHandlingTreasure = false;
            showCaseObject.SetActive(false);
        }

        IEnumerator UpdateProgressBar(bool isWait, (int, int, int) rootCurCost, bool isFinalUpdate = false)
        {
            var (root, current, cost) = rootCurCost;
            current = Mathf.Min(current, cost);
            var per = (float)current / cost;

            var curPer = (float)root / cost;
            var dur = barIncreaseDurationCurve.Evaluate(Mathf.Clamp01(per - curPer)) * .86f;

            DOVirtual.Float(0, 1, dur, Set).SetEase(barIncreaseCurve)
                .OnComplete(() =>
                {
                    Set(1);

                    if (isFinalUpdate) SetProgressBar();
                });

            if (isWait) yield return Yielders.Get(dur);
            yield break;

            void Set(float fuck)
            {
                barPro.FillAmount = Mathf.Lerp(curPer, per, fuck);
                progressText.text = Mathf.Round(Mathf.Lerp(root, current, fuck)) + "/" + cost;
            }
        }

        IEnumerator PauseToChest(TreasureDataFragment.RewardBundle rewardBundle)
        {
            itemTreasureItem.SetEffect(true);
            itemTreasureItem.SetupChestWithAnim(rewardBundle);

            ShowSecondDark();
            yield return itemTreasureItem.PopFromDisplayToShowMainChest();

            ShowTap();
            yield return WaitForTap;
            AudioManager.ins.PlaySound(SoundType.UIClick);
            StopTap();

            itemTreasureItem.SetEffect(false);
            itemTreasureItem.PlayAnim(false);

            //wait for chest to open
            yield return Yielders.Get(rewardBundle.rewardFlag < 7 ? 1.22f : .42f);

            AudioManager.ins.PlaySound(SoundType.PopMoney);

            var chestRewardList = TreasureDataFragment.cur.GetChestGiftReward(rewardBundle.GetRewardType());
            var formation = chestRewardList.Length switch
            {
                1 => chestFormation_1,
                2 => chestFormation_2,
                3 => chestFormation_3,
                4 => chestFormation_4,
            };

            var activeList = new List<TreasureShowIcon>();
            for (int i = 0; i < chestRewardList.Length; i++)
            {
                var tsi = treasureShowIcons[chestRewardList[i].rewardFlag];
                tsi.SetNum(chestRewardList[i].num);
                activeList.Add(tsi);
            }

            var chestPos = itemTreasureItem.rect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);

            for (int i = 0; i < activeList.Count; i++)
            {
                var tsi = activeList[i];
                tsi.SetEnable(true);
                tsi.SetPos(chestPos);

                var targetPos = formation[i].position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);

                tsi.PopFromChest(targetPos, formation[i].eulerAngles);
                yield return Yielders.Get(0.08f);
            }

            yield return Yielders.Get(0.38f);

            ShowTap();
            yield return WaitForTap;
            AudioManager.ins.PlaySound(SoundType.UIClick);
            StopTap();

            HideSecondDark();

            var isChestGold = false;
            for (int i = 0; i < activeList.Count; i++)
            {
                var active = activeList[i];
                var curChestReward = chestRewardList[i];
                if (curChestReward.GetRewardType() == TreasureDataFragment.RewardType.Gold)
                {
                    isChestGold = true;
                    active.PopDisappear();
                    var pos = active.rect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);

                    CanvasFloatingStuff.cur.PopGoldNoff(pos, curChestReward.num);
                }
                else
                {
                    active.PopFromShowToPlay();
                }
            }

            if (isChestGold) yield return Yielders.Get(0.52f);

            yield return Yielders.Get(0.46f);
        }

        IEnumerator ShowLastItem()
        {
            yield return Yielders.Get(.18f);

            itemTreasureItem.SetAnchorPos(singleItemShowRect);
            itemTreasureItem.gameObject.SetActive(true);
            itemTreasureItem.SetupItemIcon(lastGift);

            yield return itemTreasureItem.PopOnShow();
            yield return itemTreasureItem.PopBackToDisPlayForItem();
            itemTreasureItem.SetEffect(false);
        }

        void ShowTap()
        {
            StopTap();
            tapToContinueTween = DOVirtual.Float(0, 0, .28f, (_) => { }).OnComplete(() => tapToContinueTween = tapToContinueRect.DOScale(Vector3.one, .32f).SetEase(AssetHolder.ins.treasurePopShowOnDisplayCurve));
        }

        void StopTap()
        {
            tapToContinueTween?.Kill();
            tapToContinueRect.localScale = Vector3.zero;
        }

        IEnumerator ShowMultiPrize(List<TreasureDataFragment.RewardBundle> rewardBundles)
        {
            var itemNums = new int[4];
            for (int i = 0; i < rewardBundles.Count; i++)
            {
                var rw = rewardBundles[i];
                itemNums[rw.rewardFlag] += rw.num;
            }

            var activeList = new List<TreasureShowIcon>();
            for (int i = 0; i < itemNums.Length; i++)
            {
                if (itemNums[i] == 0) continue;
                var tsi = treasureShowIcons[i];
                tsi.SetNum(itemNums[i]);
                activeList.Add(tsi);
            }

            var formation = activeList.Count switch
            {
                1 => multiFormation_1,
                2 => multiFormation_2,
                3 => multiFormation_3,
                4 => multiFormation_4,
            };

            for (int i = 0; i < activeList.Count; i++)
            {
                var tsi = activeList[i];
                tsi.SetEnable(true);
                var targetPos = formation[i].position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
                tsi.SetPos(targetPos);
                tsi.PopNormal();
            }

            ShowThirdDark();
            yield return Yielders.Get(0.32f);

            ShowTap();
            yield return WaitForTap;
            AudioManager.ins.PlaySound(SoundType.UIClick);
            StopTap();

            HideThirdDark();

            bool isMultiGold = false;
            for (int i = 0; i < activeList.Count; i++)
            {
                var active = activeList[i];
                if (active.IsGoldItem)
                {
                    isMultiGold = true;
                    active.PopDisappear();
                    var pos = active.rect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);

                    CanvasFloatingStuff.cur.PopGoldNoff(pos, itemNums[0]);
                }
                else
                {
                    active.PopFromShowToPlay();
                }
            }

            if (isMultiGold) yield return Yielders.Get(0.52f);

            yield return Yielders.Get(0.16f);
        }

        void ShowSecondDark()
        {
            secondDarkObject.SetActive(true);
            secondDarkImage.SetAlpha(0);
            secondDarkImage.DOFade(253f / 255, .28f).SetEase(Ease.OutSine);
        }

        void HideSecondDark()
        {
            secondDarkImage.DOFade(0, .24f).SetEase(Ease.Linear).OnComplete(() => secondDarkObject.SetActive(false));
        }

        void ShowThirdDark()
        {
            thirdDarkImage.SetAlpha(0);
            thirdDarkImage.DOFade(253f / 255, .28f).SetEase(Ease.OutSine);
            thirdDarkObject.SetActive(true);
        }

        void HideThirdDark()
        {
            thirdDarkImage.DOFade(0, .24f).SetEase(Ease.Linear).OnComplete(() => thirdDarkObject.SetActive(false));
        }
    }

    private void NukeTreasureShowIcon()
    {
        for (int i = 0; i < treasureShowIcons.Length; i++) treasureShowIcons[i].SetEnable(false);
    }

    private void SetProgressBar()
    {
        var (current, cost) = TreasureDataFragment.cur.GetCurrentAndRequireProgress();
        current = Mathf.Min(current, cost);
        var per = (float)current / cost;

        barPro.FillAmount = per;
        progressText.text = current + "/" + cost;
    }

    public void PopBlinkEffect()
    {
        splashEffect.Play();
    }
}
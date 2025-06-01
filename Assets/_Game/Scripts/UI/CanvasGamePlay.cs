using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using AssetKits.ParticleImage;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CanvasGamePlay : UICanvasPrime
{
    public static CanvasGamePlay cur;
    public RectTransform canvasRect;

    public TMP_Text currentLevelText;

    public TMP_Text swapCarNumText;
    public TMP_Text vipBusNumText;
    public TMP_Text swapMinionNumText;

    public GameObject swapCarTextObject;
    public GameObject vipBusTextObject;
    public GameObject swapMinionTextObject;
    public GameObject swapCarPlusObject;
    public GameObject vipBusPlusObject;
    public GameObject swapMinionPlusObject;

    public GameObject hardLevelNoffObject;
    public GameObject superHardLevelNoffObject;
    public Animation hardLevelAnimation;
    public Animation superHardLevelAnimation;

    public GameObject[] lockBoosterButtons;
    public GameObject[] unlockBoosterButtons;

    public GameObject vipBusUsageObject;

    public GameObject outOfSlotNoffObject;
    public RectTransform outOfSlotNoffRect;
    public CanvasGroup outOfSlotCg;
    private Tween outOfSlotTween;
    [SerializeField] private float outOfSlotMoveOffset = 120;
    private Vector2 outOfSlotRootPos;
    public TMP_Text outOfSlotText;
    public const string OUT_OF_SLOT = "OUT OF SLOT";
    public const string ONE_SPACE_LEFT = "ONE SPACE LEFT!";
    public const string NO_CAR_TO_SORT = "NO BOX TO SORT";

    public RectTransform levelTextParentRect;
    public RectTransform levelTextRect;
    public Vector2 normalPos;
    public Vector3 normalScale;
    public Vector2 adaptPos;
    public Vector3 adaptScale;

    public RectTransform boosterParentRect;
    [SerializeField] private float boosterOffset;

    private void Awake()
    {
        cur = this;

        levelTextParentRect.SafeAreaAdaption(canvas);

        var isTablet = AdsManager.Ins.IsTabletOrSame();
        if (isTablet) boosterParentRect.anchoredPosition += Vector2.up * boosterOffset;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        UpdateBoosterNum();
        CheckLevelHardStatus();
        CheckBabyTut();
        CheckBoosterButton();

        currentLevelText.text = "LEVEL " + LevelDataFragment.cur.GetFireBaseLevel();
        vipBusUsageObject.SetActive(false);

        outOfSlotNoffObject.SetActive(false);

        // const string key = "rating_level";
        // if (LevelDataFragment.cur.gameData.level == 2 && !PlayerPrefs.HasKey(key))
        // {
        //     PlayerPrefs.SetInt(key, 2710);
        //
        //     UIManager.ins.OpenUI<CanvasRating>();
        // }
        var pos = ParkingLot.cur.onRoadPoint.position.ToCanvasPosFromWorldPos(canvasRect, true);
        pos.x = 0;
        outOfSlotRootPos = pos;

        if (BusLevelSO.active.mapType == MapType.SingleLine)
        {
            levelTextRect.anchoredPosition = normalPos;
            levelTextRect.localScale = normalScale;
        }
        else
        {
            if (AdsManager.Ins.IsTabletOrSame())
            {
                levelTextRect.anchoredPosition = adaptPos;
                levelTextRect.localScale = adaptScale;
            }
            else
            {
                levelTextRect.anchoredPosition = normalPos;
                levelTextRect.localScale = normalScale;
            }
        }
    }

    public static void UpdateBoosterNum()
    {
        if (cur == null) return;

        cur.swapCarNumText.text = ResourcesDataFragment.cur.SwapCarNum.ToString();
        cur.vipBusNumText.text = ResourcesDataFragment.cur.VipBusNum.ToString();
        cur.swapMinionNumText.text = ResourcesDataFragment.cur.SwapMinionNum.ToString();

        var isSwapCarOn = ResourcesDataFragment.cur.SwapCarNum > 0;
        var isVipBusOn = ResourcesDataFragment.cur.VipBusNum > 0;
        var isSwapMinion = ResourcesDataFragment.cur.SwapMinionNum > 0;

        cur.swapCarTextObject.SetActive(isSwapCarOn);
        cur.swapCarPlusObject.SetActive(!isSwapCarOn);
        cur.vipBusTextObject.SetActive(isVipBusOn);
        cur.vipBusPlusObject.SetActive(!isVipBusOn);
        cur.swapMinionTextObject.SetActive(isSwapMinion);
        cur.swapMinionPlusObject.SetActive(!isSwapMinion);
    }

    public void OnClickSwapCar()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (!TransportCenter.cur.IsTimeForBooster()) return;
        if (ResourcesDataFragment.cur.SwapCarNum > 0)
        {
            ResourcesDataFragment.cur.AddSwapCar(-1, "GAME_PLAY");
            BusStation.cur.BoosterUltraRandomColor();
        }
        else
        {
            UIManager.ins.OpenUI<CanvasEmergencyBooster>().SetupPanel(0);
        }
    }

    public void OnClickVipBus()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (!TransportCenter.cur.IsTimeForBooster()) return;
        if (ResourcesDataFragment.cur.VipBusNum > 0)
        {
            if (!BusStation.cur.IsVipBusAble()) return;
            BusStation.cur.BoosterVipBus();
        }
        else
        {
            UIManager.ins.OpenUI<CanvasEmergencyBooster>().SetupPanel(1);
        }
    }

    public void OnClickSwapMinion()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (!TransportCenter.cur.IsTimeForBooster()) return;
        if (ResourcesDataFragment.cur.SwapMinionNum > 0)
        {
            if (TransportCenter.cur.IsNoCarOnDuty())
            {
                PopNoCarToSort();
                return;
            }

            ResourcesDataFragment.cur.AddSwapMinion(-1, "GAME_PLAY");
            BusStation.cur.BoosterMinionsMix();
        }
        else
        {
            UIManager.ins.OpenUI<CanvasEmergencyBooster>().SetupPanel(2);
        }
    }

    private void CheckLevelHardStatus()
    {
        hardLevelNoffObject.SetActive(false);
        superHardLevelNoffObject.SetActive(false);

        if (BusLevelSO.active.busMapHardness == BusMapHardness.Easy) return;
        UIManager.ins.OpenUI<CanvasBlockage>().FuckingBlockTilTime(0.32f, () =>
        {
            switch (BusLevelSO.active.busMapHardness)
            {
                case BusMapHardness.Easy:
                    break;
                case BusMapHardness.Hard:
                    AudioManager.ins.PlaySound(SoundType.HardLevel);
                    AudioManager.ins.MakeVibrate(HapticTypes.HeavyImpact);
                    hardLevelNoffObject.SetActive(true);
                    hardLevelAnimation.Play();
                    break;
                case BusMapHardness.SuperHard:
                    AudioManager.ins.PlaySound(SoundType.SuperHardLevel);
                    AudioManager.ins.MakeVibrate(HapticTypes.HeavyImpact);
                    superHardLevelNoffObject.SetActive(true);
                    superHardLevelAnimation.Play();
                    break;
            }

            Timer.ScheduleSupreme(2.42f, () =>
            {
                hardLevelNoffObject.SetActive(false);
                superHardLevelNoffObject.SetActive(false);
            });
        });
    }

    public static void CheckBoosterButtonStatic(bool isHide)
    {
        if (cur == null) return;
        cur.CheckBoosterButton(isHide);
        cur.vipBusUsageObject.SetActive(isHide);
    }

    private void CheckBoosterButton(bool isHide = false)
    {
        var data = LevelDataFragment.cur.gameData;
        var isSwapCarUnlock = data.level > Config.SwapCarTutLevel || data.isSwapCarTutShowed;
        var isVipBusUnlock = data.level > Config.VipBusTutLevel || data.isVipBusTutShowed;
        var isSwapMinionUnlock = data.level > Config.SwapMinionTutLevel || data.isSwapMinionTutShowed;

        LevelDataFragment.cur.Save();

        SetBoosterButton(0, isSwapCarUnlock, isHide);
        SetBoosterButton(1, isVipBusUnlock, isHide);
        SetBoosterButton(2, isSwapMinionUnlock, isHide);

        if (LevelDataFragment.cur.CheckTutLevel(out var flag))
        {
            SetBoosterButton(flag, false);
            UIManager.ins.OpenUI<CanvasBlockage>().FuckingBlockTilTime(1.72f, () => { UIManager.ins.OpenUI<CanvasEmergencyBooster>().SetupPanel(flag, true); });
        }
    }

    public void UnlockBooster(int flag)
    {
        //efffect
        SetBoosterButton(flag, true);

        UIManager.ins.OpenUI<CanvasBlockage>().FuckingBlockTilTime(1.32f);
    }

    private void SetBoosterButton(int flag, bool isUnlock, bool isHide = false)
    {
        var isLevelOne = LevelDataFragment.cur.gameData.level == 0;
        lockBoosterButtons[flag].SetActive(!isUnlock && !isLevelOne && !isHide);
        unlockBoosterButtons[flag].SetActive(isUnlock && !isLevelOne && !isHide);
    }

    private void CheckBabyTut()
    {
        if (LevelDataFragment.cur.gameData.level > 0) return;

        var list = BusStation.cur.activeBus;
        for (int i = 0; i < list.Count; i++)
        {
            var bus = list[i];
            bus.IsTouchable = false;
        }

        UIManager.ins.OpenUI<CanvasBlockage>().FuckingBlockTilTime(1.32f, () => StartCoroutine(BabyTut()));
        return;

        IEnumerator BabyTut()
        {
            var targetColor = ParkingLot.cur.MinionsQueue.Peek().Color;
            var targetBus = FindBus(targetColor);
            var canvasTut = UIManager.ins.OpenUI<CanvasBabyTut>();

            var waitTut = new WaitUntil(() => canvasTut.DoneWaiting);

            targetBus.IsTouchable = true;
            canvasTut.Wait(targetBus);
            yield return waitTut;

            canvasTut.ShowTut(1);

            targetColor = ParkingLot.cur.MinionsQueue.Peek().Color;
            targetBus = FindBus(targetColor);
            targetBus.IsTouchable = true;
            canvasTut.Wait(targetBus);
            yield return waitTut;

            canvasTut.ShowTut(2);

            targetColor = ParkingLot.cur.MinionsQueue.Peek().Color;
            targetBus = FindBus(targetColor);
            targetBus.IsTouchable = true;
            canvasTut.Wait(targetBus);
            yield return waitTut;

            canvasTut.ShowTut(3);
 
            targetColor = ParkingLot.cur.MinionsQueue.Peek().Color;
            targetBus = FindBus(targetColor);
            targetBus.IsTouchable = true;
            canvasTut.Wait(targetBus);
            yield return waitTut;

            targetColor = ParkingLot.cur.MinionsQueue.Peek().Color;
            targetBus = FindBus(targetColor);
            targetBus.IsTouchable = true;
            canvasTut.Wait(targetBus);
            yield return waitTut;
            
            canvasTut.FadeOut();
        }

        Bus FindBus(JunkColor color)
        {
            return BusStation.cur.groundBus[0];
            var list = BusStation.cur.activeBus;
            for (int i = 0; i < list.Count; i++)
            {
                var bus = list[i];
                if (bus.color == color) return bus;
            }

            return list[0];
        }
    }

    public static void PopOutOfSlotNoffStatic(string text)
    {
        if (cur == null) return;
        cur.PopOutOfSlotNoff(text);
    }

    private void PopOutOfSlotNoff(string text)
    {
        outOfSlotText.text = text;
        outOfSlotTween?.Kill();
        outOfSlotNoffObject.SetActive(true);
        outOfSlotNoffRect.anchoredPosition = outOfSlotRootPos;
        outOfSlotCg.alpha = 0;
        outOfSlotTween = DOVirtual.Float(0, 1, .38f, (fuk) =>
            {
                outOfSlotNoffRect.anchoredPosition = Vector2.Lerp(outOfSlotRootPos, outOfSlotRootPos + Vector2.up * outOfSlotMoveOffset, fuk);
                outOfSlotCg.alpha = fuk;
            })
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                outOfSlotTween = DOVirtual.Float(0, 0, 1.24f, (_) => { })
                    .OnComplete(() => { outOfSlotTween = DOVirtual.Float(1, 0, .32f, (fuk) => { outOfSlotCg.alpha = fuk; }).SetEase(Ease.Linear); });
            });
    }

    public static void PopNoCarToSort()
    {
        if (cur == null) return;
        cur.PopOutOfSlotNoff(NO_CAR_TO_SORT);
    }
}
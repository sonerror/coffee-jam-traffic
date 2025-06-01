using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using AssetKits.ParticleImage;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TreasureItem : MonoBehaviour
{
    public RectTransform rect;
    public new GameObject gameObject;

    //for item
    public Image itemImage;
    public RectTransform itemImageRect;
    public TMP_Text itemText;

    public GameObject normalItemObject;

    private static Sprite[] ItemSprites => HomeTreasureModule.cur.itemSprites;
    private static Vector2[] ImageSize => HomeTreasureModule.cur.imageSize;
    private static Vector2[] ImagePos => HomeTreasureModule.cur.imagePos;

    public GameObject tagObject;

    private int curIndex;

    public ParticleImage bgEffect;

    public TreasureChestGiftAnim[] treasureChestGiftAnims;
    private TreasureChestGiftAnim currentTreasureChestGiftAnim;

    public ParticleImage popExplodeEffect; // use on morphing

    //end for item

    [Header("OBJECT ONLY")] public TMP_Text objectText;
    public Image objectiveImage;
    public Sprite[] objectiveSprites;

    public void SetupItemIcon(TreasureDataFragment.RewardBundle rewardBundle)
    {
        normalItemObject.SetActive(true);

        var rewardType = rewardBundle.GetRewardType();
        var amount = rewardBundle.num;
        curIndex = (int)rewardType;
        itemImage.sprite = ItemSprites[curIndex];
        itemImageRect.sizeDelta = ImageSize[curIndex];
        itemImageRect.anchoredPosition = ImagePos[curIndex];

        if (curIndex < 4)
        {
            tagObject.SetActive(true);
            itemText.text = (rewardType == TreasureDataFragment.RewardType.Gold ? "" : "x") + amount;
        }
        else
        {
            tagObject.SetActive(false);
        }

        NukeChestAnim();
    }

    public void SetupChestWithAnim(TreasureDataFragment.RewardBundle rewardBundle)
    {
        normalItemObject.SetActive(false);
        tagObject.SetActive(false);

        var index = rewardBundle.GetRewardType() switch
        {
            TreasureDataFragment.RewardType.Chest_1 => 0,
            TreasureDataFragment.RewardType.Chest_2 => 1,
            TreasureDataFragment.RewardType.Chest_3 => 2,
            TreasureDataFragment.RewardType.Gift_1 => 3,
            TreasureDataFragment.RewardType.Gift_2 => 4,
            TreasureDataFragment.RewardType.Gift_3 => 5,
        };

        for (int i = 0; i < treasureChestGiftAnims.Length; i++) treasureChestGiftAnims[i].SetActive(i == index);

        currentTreasureChestGiftAnim = treasureChestGiftAnims[index];
        PlayAnim(true);
    }

    public void SetupChestWithAnim(int index, bool isSpace)
    {
        normalItemObject.SetActive(false);
        tagObject.SetActive(false);

        for (int i = 0; i < treasureChestGiftAnims.Length; i++) treasureChestGiftAnims[i].SetActive(i == index);

        currentTreasureChestGiftAnim = treasureChestGiftAnims[index];
        PlayAnim(true, isSpace);
    }

    public void PlayAnim(bool isIdle, bool isSpace = false)
    {
        currentTreasureChestGiftAnim.PlayAnim(isIdle, isSpace);
    }

    public void NukeChestAnim()
    {
        for (int i = 0; i < treasureChestGiftAnims.Length; i++) treasureChestGiftAnims[i].SetActive(false);
    }

    public void SetSize(ShowPosition showPosition, bool isPop = false)
    {
        var size = showPosition switch
        {
            ShowPosition.HomeDisplay => Config.cur.treasureHomeDisplaySize,
            ShowPosition.HomeShow => Config.cur.treasureHomeShowSize,
        };

        if (isPop)
        {
            rect.localScale = Vector3.zero;
            rect.DOScale(size, .32f).SetEase(Ease.OutSine);
        }
        else
        {
            rect.localScale = size;
        }
    }

    public void SetAnchorPos(RectTransform rectTransform)
    {
        var pos = rectTransform.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
        rect.anchoredPosition = pos;
    }

    public void SetEffect(bool isOn)
    {
        if (isOn) bgEffect.Play();
        else bgEffect.Stop();
    }

    public void SetupForObject(int amount, TreasureDataFragment.TreasureType treasureType) //for object only not item , objective percisely
    {
        objectText.alpha = 1;
        objectText.text = amount.ToString();

        objectiveImage.sprite = objectiveSprites[(int)treasureType];

        rect.localScale = Vector3.one;
    }

    public IEnumerator PopMorphing(TreasureDataFragment.RewardBundle targetRewardBundle)
    {
        rect.DOScale(Vector3.zero, .42f).SetEase(AssetHolder.ins.treasurePopDisplayCurve)
            .OnComplete(() =>
            {
                popExplodeEffect.Play();

                AudioManager.ins.PlaySound(SoundType.ClaimStuff);
                AudioManager.ins.MakeVibrate();

                SetupItemIcon(targetRewardBundle);
                rect.localScale = Vector3.zero;
                rect.DOScale(Config.cur.treasureHomeDisplaySize, .24f).SetEase(AssetHolder.ins.treasurePopShowOnDisplayCurve);
            });

        yield return Yielders.Get(0.48f);
    }

    public IEnumerator PopOutOnlyMorphing(TreasureDataFragment.RewardBundle targetRewardBundle)
    {
        popExplodeEffect.Play();

        SetupItemIcon(targetRewardBundle);
        rect.localScale = Vector3.zero;
        rect.DOScale(Config.cur.treasureHomeDisplaySize, .24f).SetEase(AssetHolder.ins.treasurePopShowOnDisplayCurve);

        yield return Yielders.Get(0.38f);
    }

    public IEnumerator PopBackToDisPlayForObject()
    {
        var pos = HomeTreasureModule.cur.objectiveIconRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
        rect.DOAnchorPos(pos, .68f).SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                HomeTreasureModule.cur.objectiveEffect.Play();
                gameObject.SetActive(false);

                AudioManager.ins.PlaySound(SoundType.SpinEnd);
                AudioManager.ins.MakeVibrate();
            });

        rect.DOScale(Vector3.one * .5f, .62f).SetEase(Ease.InSine);
        objectText.DOFade(0, .38f).SetEase(Ease.Linear);

        yield return Yielders.Get(0.72f);
    }

    public IEnumerator PopOnShow() // pop after pre item go to play
    {
        rect.localScale = Vector3.zero;
        rect.DOScale(Config.cur.treasureHomeShowSize, .36f).SetEase(AssetHolder.ins.treasurePopShowOnDisplayCurve);
        yield return Yielders.Get(0.36f);
    }

    public IEnumerator PopDisappear()
    {
        rect.DOScale(Vector3.zero, .36f).SetEase(Ease.Linear);
        yield return Yielders.Get(0.32f);
    }

    public IEnumerator PopBackToDisPlayForItem()
    {
        var pos = HomeTreasureModule.cur.itemRestRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
        rect.DOJumpAnchorPos(pos, .28f, 1, .56f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                rect.DOPunchScale(new Vector3(0.12f, -.18f, 0), .18f, 1);

                AudioManager.ins.PlaySound(SoundType.PopItem);
                AudioManager.ins.MakeVibrate();
            });
        rect.DOScale(Config.cur.treasureHomeDisplaySize, .56f).SetEase(Ease.InSine);
        yield return Yielders.Get(0.58f);
    }

    public IEnumerator PopFromDisplayToShowMain()
    {
        AudioManager.ins.PlaySound(SoundType.ClaimStuff);
        AudioManager.ins.MakeVibrate();

        var pos = HomeTreasureModule.cur.singleItemShowRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
        rect.DOJumpAnchorPos(pos, .16f, 1, .56f).SetEase(Ease.Linear);
        rect.DOScale(Config.cur.treasureHomeShowSize, .52f).SetEase(Ease.Linear);
        yield return Yielders.Get(0.58f);
    }

    public IEnumerator PopFromDisplayToShowMainChest()
    {
        AudioManager.ins.PlaySound(SoundType.PopMoney);
        AudioManager.ins.MakeVibrate();

        var pos = HomeTreasureModule.cur.chestShowRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
        rect.DOJumpAnchorPos(pos, .16f, 1, .62f).SetEase(Ease.Linear);
        rect.DOScale(Config.cur.treasureHomeShowSize, .56f).SetEase(Ease.Linear);
        yield return Yielders.Get(0.64f);
    }

    public IEnumerator PopFromShowToPlay() // single item
    {
        var pos = CanvasHome.cur.playButtonRect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);
        rect.DOAnchorPos(pos, .56f).SetEase(AssetHolder.ins.treasurePopToPlayCurve)
            .OnComplete(() =>
            {
                AudioManager.ins.PlaySound(SoundType.PopItem);
                AudioManager.ins.MakeVibrate();

                gameObject.SetActive(false);
                HomeTreasureModule.cur.PopBlinkEffect();
                SetEffect(false);
            });
        rect.DOScale(Vector3.one * .72f, .48f).SetEase(Ease.InSine);
        yield return Yielders.Get(.62f);
    }


    public enum ShowPosition
    {
        HomeDisplay, // display mean on the bar at home
        HomeShow, // when show what you got
    }
}
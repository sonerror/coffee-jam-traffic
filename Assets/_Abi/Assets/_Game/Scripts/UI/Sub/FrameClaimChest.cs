using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FrameClaimChest : UICanvasPrime
{
    public Image img_bg;
    public Transform title_trans;
    public TreasureItem itemTreasureItem;
    public RectTransform[] chestFormation_1;
    public RectTransform[] chestFormation_2;
    public RectTransform[] chestFormation_3;
    public RectTransform[] chestFormation_4;
    public TreasureShowIcon[] treasureShowIcons; //currently have 4
    private Tween tapToContinueTween;
    public SkeletonGraphic skeletonGraphic_chest;
    public string[] skins;
    private static readonly WaitUntil WaitForTap = new WaitUntil(() => Input.GetMouseButtonDown(0));
    public RectTransform tapToContinueRect;
    public bool ngoaiHome = false;
    public UnityAction callBack = null;
    public TMP_Text txt_des;

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();
    }

    public void SetReward(RewardChestBundle[] rewardChestBundles, int indexChest, bool _ngoaiHome)
    {
        ngoaiHome = _ngoaiHome;
        StartCoroutine(PauseToChest(rewardChestBundles, indexChest));
    }

    private IEnumerator PauseToChest(RewardChestBundle[] rewardChestBundles, int indexChest)
    {
        for (int i = 0; i < treasureShowIcons.Length; i++)
        {
            treasureShowIcons[i].SetEnable(false);
        }

        img_bg.color = new Color(0,0,0,0);
        img_bg.DOFade(0.98f, 0.5f);
        title_trans.localScale = Vector3.zero;
        itemTreasureItem.gameObject.SetActive(false);
        yield return Yielders.Get(0.5f);
        itemTreasureItem.gameObject.SetActive(true);

        itemTreasureItem.SetEffect(true);
        itemTreasureItem.SetupChestWithAnim(indexChest, true);

        title_trans.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        ShowTap();
        yield return WaitForTap;
        AudioManager.ins.PlaySound(SoundType.UIClick);
        StopTap();

        itemTreasureItem.SetEffect(false);
        itemTreasureItem.PlayAnim(false);

        //wait for chest to open
        yield return Yielders.Get(1.22f);

        AudioManager.ins.PlaySound(SoundType.PopMoney);

        RectTransform[] formation = rewardChestBundles.Length switch
        {
            1 => chestFormation_1,
            2 => chestFormation_2,
            3 => chestFormation_3,
            4 => chestFormation_4
        };

        var activeList = new List<TreasureShowIcon>();
        
        for (int i = 0; i < rewardChestBundles.Length; i++)
        {
            var tsi = treasureShowIcons[(int)rewardChestBundles[i].rewardChestType];
            tsi.SetNum(rewardChestBundles[i].quantity);
            // tsi.SetEnable(true);
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

        title_trans.DOScale(0, 0.3f).SetEase(Ease.InBack);
        img_bg.DOFade(0, 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
        });

        // HideSecondDark();

        var isChestGold = false;
        for (int i = 0; i < activeList.Count; i++)
        {
            var active = activeList[i];
            var curChestReward = rewardChestBundles[i];
            if (curChestReward.rewardChestType == RewardChestType.ticket)
            {
                isChestGold = true;
                active.PopDisappear();
                var pos = active.rect.position.ToFullScreenCanvasPosFromOverLayCanvasPos(CanvasHome.cur.canvasRect);

                CanvasFloatingStuff.cur.PopGoldNoff(pos, curChestReward.quantity);
            }
            else
            {
                if(ngoaiHome)
                {
                    active.PopFromShowToPlay();
                }
                else
                {
                    active.PopDisappear();
                }
            }
        }

        callBack?.Invoke();
        callBack = null;

        if (isChestGold) yield return Yielders.Get(0.52f);

        yield return Yielders.Get(0.46f);
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
}

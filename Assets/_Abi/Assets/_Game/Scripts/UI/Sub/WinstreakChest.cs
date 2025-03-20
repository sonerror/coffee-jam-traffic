using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WinstreakChest : MonoBehaviour
{
    public new GameObject gameObject;
    public RectTransform rect;

    public GameObject popObject;
    public RectTransform popRect;

    private Tween popTween;

    public GameObject[] chestRewardBubbleObjects;

    private void OnEnable()
    {
        popTween?.Kill();
        popObject.SetActive(false);
    }

    public void OnClickChest()
    {
        PopPop();
    }

    private void PopPop()
    {
        if (WinstreakDataFragment.cur.IsOutLevel() || CanvasWinstreak.cur.IsClaimReady) return;

        AudioManager.ins.PlaySound(SoundType.UIClick);

        popTween?.Kill();
        popObject.SetActive(true);
        popRect.localScale = Vector3.zero;
        popTween = popRect.DOScale(Vector3.one, .24f).SetEase(Ease.OutSine);

        Timer.ScheduleCondition(() => Input.GetMouseButtonDown(0), () => // no need to check if is clicked agian due to PopPop execute in button event --> trigger on mouse UP , after this mouse down
        {
            popTween.Kill();
            popTween = popRect.DOScale(Vector3.zero, .24f).SetEase(Ease.OutSine).OnComplete(() => popObject.SetActive(false));
        });
    }

    public void SetPos(Vector2 pos)
    {
        rect.anchoredPosition = pos;
    }

    public void SetRewardBubble(int index)
    {
        index--; // skip first null prize
        for (int i = 0; i < chestRewardBubbleObjects.Length; i++)
        {
            chestRewardBubbleObjects[i].SetActive(i == index);
        }
    }
}
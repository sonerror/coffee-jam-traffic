using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class NoffDailyReward : MonoBehaviour
{
    public static NoffDailyReward cur;

    public GameObject noffObject;
    public TMP_Text noffText;
    public RectTransform noffRect;
    private Tween noffTween;
    private Vector3 noffRootScale;

    private bool isNoffOn;

    private void Awake()
    {
        cur = this;
        noffRootScale = noffRect.localScale;
    }

    private void OnEnable()
    {
        UpdateNoff();

        Check();
    }

    private void OnDisable()
    {
        noffTween?.Kill();
    }

    private void Check()
    {
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            while (true)
            {
                UpdateNoff();

                yield return Yielders.Get(4);
            }
        }
    }

    private void UpdateNoff()
    {
        int dailyNum = 0;
        if (DailyRewardDataFragment.cur.CheckIfItsTimeToFree(out var _)) dailyNum++;
        var dailyData = DailyRewardDataFragment.cur.gameData;
        for (int i = 0; i < 4; i++)
        {
            if (!dailyData.itemFlags[i]) dailyNum++;
        }

        noffObject.SetActive(dailyNum > 0);
        noffText.text = dailyNum.ToString();

        var isOn = dailyNum > 0;
        if (isOn) NoffBubble();
        else noffTween?.Kill();

        isNoffOn = isOn;
    }

    public static void UpdateNoffStatic()
    {
        if (cur == null) return;
        cur.UpdateNoff();
    }

    private void NoffBubble()
    {
        if (isNoffOn) return;
        noffTween?.Kill();
        noffRect.localScale = noffRootScale;
        noffTween = noffRect.DOScale(noffRootScale * 1.12f, .72f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
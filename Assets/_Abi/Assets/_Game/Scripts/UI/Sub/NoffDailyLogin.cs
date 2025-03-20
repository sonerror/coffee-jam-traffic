using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NoffDailyLogin : MonoBehaviour
{
    public static NoffDailyLogin cur;

    public GameObject noffObject;
    public RectTransform noffRect;
    private Tween noffTween;
    private Vector3 noffRootScale;

    private void Awake()
    {
        cur = this;
        noffRootScale = noffRect.localScale;
    }

    private void OnEnable()
    {
        UpdateNoff();

        CheckTime();
    }

    private void OnDisable()
    {
        noffTween?.Kill();
    }

    private void CheckTime()
    {
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            yield return new WaitUntil(() => UnbiasedTime.IsValidTime);
            while (true)
            {
                if (DailyLoginDataFragment.cur.CheckNewDay(out _))
                {
                    UpdateNoff();
                }

                yield return Yielders.Get(1);
            }
        }
    }

    private void UpdateNoff()
    {
        var isOn = !DailyLoginDataFragment.cur.gameData.isClaimedToday;

        noffObject.SetActive(isOn);
        if (isOn) NoffBubble();
        else noffTween?.Kill();
    }

    public static void UpdateNoffStatic()
    {
        if (cur == null) return;
        cur.UpdateNoff();
    }

    private void NoffBubble()
    {
        noffTween?.Kill();
        noffRect.localScale = noffRootScale;
        noffTween = noffRect.DOScale(noffRootScale * 1.12f, .72f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
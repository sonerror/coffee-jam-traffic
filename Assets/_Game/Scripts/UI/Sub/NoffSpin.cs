using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NoffSpin : MonoBehaviour
{
    public static NoffSpin cur;

    public GameObject noffObject;
    public RectTransform noffRect;
    private Tween noffTween;
    private Vector3 noffRootScale;

    private Coroutine timeCor;

    public Image noffIconImage;
    [SerializeField] private Sprite normIconSprite;
    [SerializeField] private Sprite adsIconSprite;

    private void Awake()
    {
        cur = this;
        noffRootScale = noffRect.localScale;
    }

    private void OnEnable()
    {
        UpdateNoff();
    }

    private void OnDisable()
    {
        noffTween?.Kill();
    }

    private void UpdateNoff()
    {
        var require = SpinDataFragment.cur.GetRequiredPassedLevel();
        var passed = Mathf.Min(SpinDataFragment.cur.gameData.levelPassedNum, require);


        var isNorm = passed >= require;
        var isAds = SpinDataFragment.cur.gameData.isAdsAble;
        var isOn = isNorm || isAds;
        if (isOn) NoffBubble();
        else noffTween?.Kill();
        noffObject.SetActive(isOn);

        noffIconImage.sprite = isNorm ? normIconSprite : adsIconSprite;

        CheckTimeLeftTillFree();
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

    private static WaitUntil waitForValidTime = new WaitUntil(() => UnbiasedTime.IsValidTime);

    public void CheckTimeLeftTillFree()
    {
        if (timeCor != null) StopCoroutine(timeCor);
        timeCor = StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            if (SpinDataFragment.cur.gameData.isAdsAble) yield break;
            while (true)
            {
                yield return waitForValidTime;
                if (SpinDataFragment.cur.CheckNewTimeSegment(out _))
                {
                    UpdateNoff();
                    yield break;
                }

                yield return Yielders.Get(1);
            }
        }
    }
}
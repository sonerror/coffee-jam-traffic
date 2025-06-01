using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using DG.Tweening;
using UnityEngine;

public class CanvasBabyTut : UICanvasPrime
{
    public RectTransform canvasRect;
    public RectTransform handRect;
    public GameObject handObject;

    public bool DoneWaiting { get; set; }

    public CanvasGroup canvasGroup;
    private Tween fadeTween;

    public GameObject[] tutBoardObjects;

    public Animation handAnim;

    public RectTransform[] babyTutRects;

    private void Awake()
    {
        var pos = babyTutRects[0].anchoredPosition;
        var refPos = ParkingLot.cur.babyTutRefPoint.position.ToCanvasPosFromWorldPos(canvasRect, true);

        pos.y = refPos.y;
        for (int i = 0; i < babyTutRects.Length; i++)
        {
            babyTutRects[i].anchoredPosition = pos;
        }
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        canvasGroup.alpha = 0;
        fadeTween?.Kill();
        fadeTween = canvasGroup.DOFade(1, 0.32f).SetEase(Ease.Linear);

        ShowTut(0);
    }

    public void Wait(Bus bus)
    {
        DoneWaiting = false;
        StartCoroutine(ie_Wait());
        return;

        IEnumerator ie_Wait()
        {
            var pos = bus.transform.position.ToCanvasPosFromWorldPos(canvasRect, true);
            handRect.anchoredPosition = pos;
            handObject.SetActive(true);
            handAnim.Play();
            yield return new WaitUntil(() => !bus.IsStayOnBusZone);
            handObject.SetActive(false);
            yield return Yielders.Get(0.32f);
            DoneWaiting = true;
        }
    }

    public void FadeOut()
    {
        fadeTween = canvasGroup.DOFade(0, 0.32f).SetEase(Ease.Linear)
            .OnComplete(Close);
    }

    public void ShowTut(int flag)
    {
        for (int i = 0; i < tutBoardObjects.Length; i++)
        {
            tutBoardObjects[i].SetActive(false);
        }
    }
}
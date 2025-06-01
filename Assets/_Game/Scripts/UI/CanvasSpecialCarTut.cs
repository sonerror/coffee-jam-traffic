using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using DG.Tweening;
using UnityEngine;

public class CanvasSpecialCarTut : UICanvasPrime
{
    public static CanvasSpecialCarTut cur;
    public CanvasGroup canvasGroup;
    public RectTransform canvasRect;

    public GameObject[] tutObjects;
    public RectTransform[] tutRects;

    public RectTransform handTutRect;

    private Bus currentBus;

    public Animation handAnim;

    private static TutData[] tutDatas => LevelDataFragment.cur.tutDatas;

    private void Awake()
    {
        cur = this;
    }

    private void Start()
    {
        var pos = ParkingLot.cur.babyTutRefPoint.position.ToCanvasPosFromWorldPos(canvasRect, true);

        for (int i = 0; i < tutRects.Length; i++) tutRects[i].anchoredPosition = pos;
    }

    public static void TriggerTut()
    {
        TutData curTutData = new TutData() { index = -1 };
        for (int i = 0; i < tutDatas.Length; i++)
        {
            var td = tutDatas[i];
            if (td.level == LevelDataFragment.cur.gameData.level)
            {
                curTutData = td;
                break;
            }
        }

        if (curTutData.index < 0) return;
        var bus = BusStation.cur.activeBus[curTutData.index];

        var isTutShowed = bus.color switch
        {
            JunkColor.Ambulance => DataManager.ins.gameData.isAmbulanceCarTutShowed,
            JunkColor.PoliceCar => DataManager.ins.gameData.isPoliceCarTutShowed,
            JunkColor.FireTruck => DataManager.ins.gameData.isFireTruckCarTutShowed,
            JunkColor.VipProMax => DataManager.ins.gameData.isVipCarTutShowed,
            _ => true
        };

        if (isTutShowed) return;

        UIManager.ins.OpenUI<CanvasSpecialCarTut>();
        cur.currentBus = bus;

        switch (bus.color)
        {
            case JunkColor.Ambulance:
                DataManager.ins.gameData.isAmbulanceCarTutShowed = true;
                break;
            case JunkColor.PoliceCar:
                DataManager.ins.gameData.isPoliceCarTutShowed = true;
                break;
            case JunkColor.FireTruck:
                DataManager.ins.gameData.isFireTruckCarTutShowed = true;
                break;
            case JunkColor.VipProMax:
                DataManager.ins.gameData.isVipCarTutShowed = true;
                break;
        }

        var flag = bus.color switch
        {
            JunkColor.Ambulance => 0,
            JunkColor.PoliceCar => 1,
            JunkColor.FireTruck => 2,
            JunkColor.VipProMax => 3,
        };

        for (int i = 0; i < cur.tutObjects.Length; i++)
        {
            cur.tutObjects[i].SetActive(flag == i);
        }

        cur.handTutRect.anchoredPosition = bus.transform.position.ToCanvasPosFromWorldPos(cur.canvasRect, true);
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        canvasGroup.alpha = 0;
        Timer.ScheduleSupreme(0.32f, () => canvasGroup.DOFade(1, .32f).SetEase(Ease.Linear));

        handAnim.Play();
    }

    public override void Close()
    {
        canvasGroup.DOFade(0, .32f).SetEase(Ease.Linear)
            .OnComplete(() => canvasGroup.interactable = false);
    }

    public void OnBusLeave()
    {
        Close();
    }

    [Serializable]
    public struct TutData
    {
        public int level;
        public int index;
    }
}
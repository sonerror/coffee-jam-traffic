using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasBannerOff : MonoBehaviour
{
    public static CanvasBannerOff cur;

    public new GameObject gameObject;

    public Canvas canvas;

    public RectTransform xButtonRect;

    private float dp;
    private float density;

    private void Awake()
    {
        cur = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        Timer.ScheduleCondition(() => AdsManager.Ins.IsAdsSetupDone, CheckActive);
    }

    public static void SetBannerOffButton(float dp, float density)
    {
        Timer.ScheduleCondition(() => cur != null, () =>
        {
            cur.dp = dp;
            cur.density = density;

            cur.CalculateAndSetPos();
        });
    }

    public void CalculateAndSetPos()
    {
        var pos = cur.xButtonRect.anchoredPosition;

        var pixel = dp * density;

        pos.y = pixel / cur.canvas.scaleFactor + cur.canvas.GetSafeAreaBottomLength();
        cur.xButtonRect.anchoredPosition = pos;

        //  Debug.Log("POSSS " + pos + "    " + dp + "   " + density + "    " + pixel + "   \n" + cur.canvas.scaleFactor + "   " + cur.canvas.GetSafeAreaBottomLength());
    }

    public void SetPos(float height)
    {
        var pos = cur.xButtonRect.anchoredPosition;
        pos.y = height;
        cur.xButtonRect.anchoredPosition = pos;
    }

    public static void CheckActive()
    {
        // Debug.Log("SETTUP " + AdsManager.isNoAds + "  " + (cur != null));
        if (cur == null) return;
        cur.CalculateAndSetPos();
        cur.gameObject.SetActive(!AdsManager.isNoAds && !VipPassDataFragment.cur.IsBlockAds);
    }

    public void OnClickXButton(IAPButtonPro buttonPro)
    {
        if (GrandManager.ins.IsGame && !TransportCenter.cur.IsGamePlaying) return;

        AudioManager.ins.PlaySound(SoundType.UIClick);
        // IAPPrime.ins.ClickPurchase(buttonPro.id, "BANNER BUTTON");

        UIManager.ins.OpenUI<CanvasOfferRemoveAdsPack>().Setup(false);
    }
}
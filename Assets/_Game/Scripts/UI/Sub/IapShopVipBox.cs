using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IapShopVipBox : MonoBehaviour
{
    public static IapShopVipBox vip_3;
    public static IapShopVipBox vip_7;
    public static IapShopVipBox vip_15;

    [SerializeField] private VipType vipType;

    public GameObject lockPanelObject;
    public GameObject unlockPanelObject;

    public GameObject claimDailyButtonObject;
    public GameObject dummyClaimButtonObject;

    public TMP_Text timeLeftText;

    public GameObject popObject;
    public RectTransform popRect;

    private Tween popTween;

    private void Awake()
    {
        switch (vipType)
        {
            case VipType.Vip_3:
                vip_3 = this;
                break;
            case VipType.Vip_7:
                vip_7 = this;
                break;
            case VipType.Vip_15:
                vip_15 = this;
                break;
        }
    }

    private void OnEnable()
    {
        Check();

        popTween?.Kill();
        popObject.SetActive(false);
    }

    private void PopPop()
    {
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

    public static void CheckStatic(VipType vipType)
    {
        switch (vipType)
        {
            case VipType.Vip_3:
                if (vip_3 == null) return;
                vip_3.Check();
                break;
            case VipType.Vip_7:
                if (vip_7 == null) return;
                vip_7.Check();
                break;
            case VipType.Vip_15:
                if (vip_15 == null) return;
                vip_15.Check();
                break;
        }
    }

    public void Check()
    {
        if (gameObject.activeInHierarchy) StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            SetPanelState();
            timeLeftText.text = "Not purchased";
            AdsManager.Ins.CheckBanner();

            while (IsUnlock())
            {
                TimeSpan timeLeft;
                Debug.Log("CHECKING " + vipType);
                var isOn = vipType switch
                {
                    VipType.Vip_3 => VipPassDataFragment.cur.CheckVip_3(out timeLeft),
                    VipType.Vip_7 => VipPassDataFragment.cur.CheckVip_7(out timeLeft),
                    VipType.Vip_15 => VipPassDataFragment.cur.CheckVip_15(out timeLeft),
                };

                timeLeftText.text = timeLeft.ToTimeFormat_D_H_M_S_Dynamic_Lower_Text();
                yield return Yielders.Get(1);
            }

            SetPanelState();
            timeLeftText.text = "Not purchased";
            AdsManager.Ins.CheckBanner();
        }
    }

    public void SetPanelState()
    {
        VipPassDataFragment.cur.CheckNewDay();

        var isUnlock = IsUnlock();
        var isClaimed = vipType switch
        {
            VipType.Vip_3 => VipPassDataFragment.cur.gameData.isVip_3_DailyClaimed,
            VipType.Vip_7 => VipPassDataFragment.cur.gameData.isVip_7_DailyClaimed,
            VipType.Vip_15 => VipPassDataFragment.cur.gameData.isVip_15_DailyClaimed,
        };

        lockPanelObject.SetActive(!isUnlock);
        unlockPanelObject.SetActive(isUnlock);
        claimDailyButtonObject.SetActive(!isClaimed);
        dummyClaimButtonObject.SetActive(isClaimed);
    }

    public bool IsUnlock()
    {
        return vipType switch
        {
            VipType.Vip_3 => VipPassDataFragment.cur.gameData.isVip_3_Active,
            VipType.Vip_7 => VipPassDataFragment.cur.gameData.isVip_7_Active,
            VipType.Vip_15 => VipPassDataFragment.cur.gameData.isVip_15_Active,
        };
    }

    public void OnClickBuyPack(IAPButtonPro buttonPro)
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(buttonPro.id, "IAP_SHOP");
    }

    public void OnClaimDaily()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        switch (vipType)
        {
            case VipType.Vip_3:
                VipPassDataFragment.cur.ClaimDaily_3();
                break;
            case VipType.Vip_7:
                VipPassDataFragment.cur.ClaimDaily_7();
                break;
            case VipType.Vip_15:
                VipPassDataFragment.cur.ClaimDaily_15();
                break;
        }

        SetPanelState();
    }

    public void OnClickChest()
    {
        PopPop();
    }

    public enum VipType
    {
        Vip_3,
        Vip_7,
        Vip_15,
    }
}
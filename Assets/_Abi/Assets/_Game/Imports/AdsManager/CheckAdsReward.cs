using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CheckAdsReward : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public Image imgIconAds;

    public Image btnImg;
    public Sprite imgOn;

    public float alphaDisable = 0.8f;
    [HideInInspector] public bool isLoadNeeded;

    private void OnEnable()
    {
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        isLoadNeeded = true;
        isAds = false;
        isAdsOld = true;
        StartCoroutine(ie_Check());
    }

    private bool isAds;
    private bool isAdsOld;
    IEnumerator ie_Check()
    {
        while (true)
        {
            if (imgIconAds == null)
            {
                yield return Yielders.Get(0.5f);
                continue;
            }

            if (!isLoadNeeded)
            {
                Ads();
                yield return Yielders.Get(0.5f);
                continue;
            }

            if (isAds != isAdsOld)
            {
                isAdsOld = isAds;
                if (isAds)
                {
                    Ads();
                }
                else
                {
                    NotAds();
                }
            }
#if UNITY_EDITOR
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                isAds = true;
            }
            else
            {
                isAds = false;
            }
#else
            if (Application.internetReachability != NetworkReachability.NotReachable && MaxManager.Ins.isRewardedVideoAvailable())
            {
                isAds = true;
            }
            else
            {
                if (AdsManager.Ins.isMkt) isAds = true;
                else isAds = false;
            }
#endif
            yield return Yielders.Get(0.5f);
        }
    }

    public void Ads()
    {
        try
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
            // imgIconAds.sprite = GameConfig.ins.sprIconAds;
            imgIconAds.SetNativeSize();
            imgIconAds.transform.DOKill();
            imgIconAds.transform.localRotation = Quaternion.Euler(0, 0, 0);

            if (btnImg != null) btnImg.sprite = imgOn;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error: " + e);
        }
    }

    public void NotAds()
    {
        try
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 1f;
            // imgIconAds.sprite = GameConfig.ins.sprIconAdsLoading;
            imgIconAds.SetNativeSize();
            imgIconAds
                .transform
                .DOLocalRotate(Vector3.back * 360, 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            // if (btnImg != null) btnImg.sprite = GameConfig.ins.sprButtonOff;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error: " + e);
        }
    }

    public void Remove()
    {
        imgIconAds.transform.DOKill();
        imgIconAds.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // imgIconAds.sprite = GameConfig.ins.sprIconAds;
        imgIconAds.SetNativeSize();
        canvasGroup.interactable = true;
        Destroy(canvasGroup);
        Destroy(this);
    }
}

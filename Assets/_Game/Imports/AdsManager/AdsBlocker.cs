using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Disable / làm mờ button ads reward nếu không có mạng hoặc là không có ad reward load sẵn
/// </summary>
public class AdsBlocker : MonoBehaviour
{
//    CanvasGroup b;
//    private void OnEnable()
//    {
//#if UNITY_EDITOR
//        return;
//#endif
//        b = gameObject.GetComponent<CanvasGroup>();
//        if (b == null) b = gameObject.AddComponent<CanvasGroup>();
//        if (Application.internetReachability == NetworkReachability.NotReachable || !IronsourceManager.Ins.isVideoLoaded)
//        {
//            if (b != null) SetStatus(false);
//        }
//        else if (b != null) SetStatus(true);
//    }

//    private void FixedUpdate()
//    {
//#if UNITY_EDITOR
//        return;
//#endif
//        if (Application.internetReachability == NetworkReachability.NotReachable || !IronsourceManager.Ins.isVideoLoaded)
//        {
//            if (b != null) SetStatus(false);
//        }
//        else if (b != null) SetStatus(true);
//    }

//    public void SetStatus(bool bo, float alpha = 0.3f)
//    {
//        b.alpha = bo ? 1 : alpha;
//        b.interactable = bo;
//    }
}

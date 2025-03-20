using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTransit : UICanvasPrime
{
    // public Image transitImage;
    // private Material transitMat;
    // public Vector4 defalutST;


    // public RectTransform transitRect;
    // public SkeletonGraphic transitSke;
    // [SpineAnimation] public string start;
    // [SpineAnimation] public string loop;
    // [SpineAnimation] public string end;

    private void Awake()
    {
        // transitMat = Instantiate(transitImage.material);
        // transitImage.material = transitMat;

        var ratio = ((float)Screen.width / Screen.height) / (1080f / 1920);
        // transitRect.localScale = ratio < 1 ? Vector3.one / ratio : Vector3.one * ratio;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();
        // transitMat.SetFloat(Variables.FLOAT, 0);
        //
        // int num = UCLN(Screen.width, Screen.height);
        // if (num != 0)
        // {
        //     defalutST.x = Mathf.Round((float)Screen.width / num);
        //     defalutST.y = Mathf.Round((float)Screen.height / num);
        //
        //     if (defalutST.x < 7 || defalutST.y < 12) defalutST *= 2;
        //     if (defalutST.x < 7 || defalutST.y < 12) defalutST *= 2;
        //
        //     transitMat.SetVector(Variables.MAIN_TEX_ST, defalutST);
        // }
    }

    private int UCLN(int a, int b)
    {
        int maxNum = 100;
        while (a != b)
        {
            if (a > b)
                a = a - b;
            else
                b = b - a;

            maxNum--;
            if (maxNum < 0) return 0;
        }

        return a; // or return b; a = b
    }

#if UNITY_EDITOR
    [ContextMenu("AAA")]
    public void Trns()
    {
        MakeTransit();
    }
#endif
    public void MakeTransit(Action onDone = null)
    {
        // if (GrandManager.ins.IsGame) AdsManager.Ins.HideBanner();

        // transitSke.AnimationState.ClearTrack(0);
        // transitSke.AnimationState.SetAnimation(0, start, false).End += (entry =>
        // {
        onDone?.Invoke();
        // System.GC.Collect();
        // });
        // Timer.ScheduleSupreme(1.42f, () => { transitSke.AnimationState.AddAnimation(0, end, false, 0).Complete += entry => { Close(); }; });

        Close();
    }
}
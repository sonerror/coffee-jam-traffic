using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CanvasProfile : UICanvasPrime
{
    public static CanvasProfile cur;
    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;
    public UIChooseAvatar[] uIChooseAvatars;
    public Image img_avatar;
    public Image img_frame;
    private UIChooseAvatar choosing;
    public InputField inputField_name;
    private string valueString = "";

    private void Awake() {
        cur = this;
    }

    private void Start()
    {
        
        for (int i = 0; i < uIChooseAvatars.Length; i++)
        {
            uIChooseAvatars[i].id = i;
            uIChooseAvatars[i].idAvatar = i % ProfileDataFragment.cur.spr_avatars.Length;
            uIChooseAvatars[i].idFrame = i % ProfileDataFragment.cur.spr_frames.Length;
            uIChooseAvatars[i].SetUp();
        }

        img_avatar.sprite = ProfileDataFragment.cur.spr_avatars[ProfileDataFragment.cur.gameData.idAvatar];
        img_frame.sprite = ProfileDataFragment.cur.spr_frames[ProfileDataFragment.cur.gameData.idFrame];
        img_frame.SetNativeSize();

        // name
        inputField_name.text = ProfileDataFragment.cur.gameData.userName;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.one * .05f;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        choosing = uIChooseAvatars[ProfileDataFragment.cur.gameData.idChoose];
        choosing.img_selecting.gameObject.SetActive(true);
    }

    public void Btn_close()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        gameObject.SetActive(false);
    }

    public void UpdateAvatar()
    {
        img_avatar.sprite = ProfileDataFragment.cur.spr_avatars[ProfileDataFragment.cur.gameData.idAvatar];
        img_frame.sprite = ProfileDataFragment.cur.spr_frames[ProfileDataFragment.cur.gameData.idFrame];
        img_frame.SetNativeSize();

        choosing.img_selecting.gameObject.SetActive(false);
        choosing = uIChooseAvatars[ProfileDataFragment.cur.gameData.idChoose];
        choosing.img_selecting.gameObject.SetActive(true);
    }

    public void OnChangeName()
    {
        if(inputField_name.text.Length > 0)
        {
            ProfileDataFragment.cur.gameData.userName = inputField_name.text;
        }
        else
        {
            inputField_name.text = ProfileDataFragment.cur.gameData.userName;
        }
    }

    public void OnValueChange()
    {
        if(inputField_name.text.Length > 10)
        {
            inputField_name.text = inputField_name.text.Substring(0, 10);
        }
    }
}
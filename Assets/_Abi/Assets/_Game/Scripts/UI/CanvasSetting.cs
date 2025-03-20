using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CanvasSetting : UICanvasPrime
{
    public static CanvasSetting cur;

    public CanvasGroup canvasGroup;

    public GameObject musicSlashObject;
    public GameObject soundSlashObject;
    public GameObject vibrateSlashObject;

    // public Slider soundSlider;
    // public Slider musicSlider;

    public GameObject consentButton;

    // public RectTransform exitButtonRect;

    public RectTransform bgRect;
    [SerializeField] private Vector2 normalPanelSize;
    [SerializeField] private Vector2 homePanelSize;

    // public GameObject homeButtonObject;

    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public GameObject notHomePanel;

    public GameObject homeButtonObject;
    public RectTransform retryButtonRect;
    [SerializeField] private Vector2 retryRootPos;
    [SerializeField] private Vector2 retryTargetPos;

    private void Awake()
    {
        cur = this;
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        canvasGroup.interactable = true;

        SetupSoundImage();
        SetupMusicImage();
        SetupVibrateImage();

        // consentButton.SetActive(AdsManager.Ins.IsConsentRequired());

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.zero;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        bgRect.sizeDelta = !AdsManager.Ins.IsPrivacyOptionRequired() ? homePanelSize : normalPanelSize;
        notHomePanel.SetActive(!GrandManager.ins.IsHome);
        if (GrandManager.ins.IsHome)
        {
            if (AdsManager.Ins.IsPrivacyOptionRequired())
            {
                consentButton.SetActive(AdsManager.Ins.IsPrivacyOptionRequired());
                consentButton.transform.localPosition = new Vector3(0, -761, 0);
                bgRect.sizeDelta = new Vector2(797, 900);
            }
            else
            {
                bgRect.sizeDelta = new Vector2(797, 740);
                consentButton.SetActive(AdsManager.Ins.IsPrivacyOptionRequired());
            }
        }
        else
        {
            if (AdsManager.Ins.IsPrivacyOptionRequired())
            {
                consentButton.SetActive(AdsManager.Ins.IsPrivacyOptionRequired());
                consentButton.transform.localPosition = new Vector3(0, -935.6145f, 0);
            }
            else
            {
                bgRect.sizeDelta = new Vector2(797, 886);
                consentButton.SetActive(AdsManager.Ins.IsPrivacyOptionRequired());
            }
        }
        CheckHomeButton();
    }

    public override void Close()
    {
        canvasGroup.interactable = false;
        base.Close();
    }

    public void CheckHomeButton()
    {
        if (LevelDataFragment.cur.IsBabySitLevel())
        {
            homeButtonObject.SetActive(false);
            retryButtonRect.anchoredPosition = retryTargetPos;
        }
        else
        {
            homeButtonObject.SetActive(true);
            retryButtonRect.anchoredPosition = retryRootPos;
        }
    }

    public void OnClickSoundButton()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        DataManager.ins.gameData.isSoundOn = !DataManager.ins.gameData.isSoundOn;
        AudioManager.ins.UpdateAudio();
        SetupSoundImage();
    }

    public void OnClickMusicButton()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        DataManager.ins.gameData.isMusicOn = !DataManager.ins.gameData.isMusicOn;
        AudioManager.ins.UpdateAudio();
        if (DataManager.ins.gameData.isMusicOn) AudioManager.ins.MusicAudioSource.Play();
        SetupMusicImage();
    }

    public void OnClickVibrateButton()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        DataManager.ins.gameData.isVibrateOn = !DataManager.ins.gameData.isVibrateOn;
        AudioManager.ins.UpdateAudio();
        SetupVibrateImage();
    }

    public void OnClickExitButton()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        Close();
    }

    public void SetupSoundImage()
    {
        soundSlashObject.SetActive(!DataManager.ins.gameData.isSoundOn);
    }

    public void SetupMusicImage()
    {
        musicSlashObject.SetActive(!DataManager.ins.gameData.isMusicOn);
    }

    public void SetupVibrateImage()
    {
        vibrateSlashObject.SetActive(!DataManager.ins.gameData.isVibrateOn);
    }

    public void OnClickConsent()
    {
        // AdsManager.Ins.LoadAndShowCmpFlow();
        AudioManager.ins.PlaySound(SoundType.UIClick);
        AdsManager.Ins.ShowPrivacyOptionsForm(consentButton);
    }

    public void OnClickHome()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (WinstreakDataFragment.cur.IsNeedToNoffLoseStreak())
        {
            UIManager.ins.OpenUI<CanvasWinstreakLoseNoff>().Setup(true);
            Close();
            return;
        }

        ToHomeHandle();
    }

    public void ToHomeHandle()
    {
        TreasureDataFragment.cur.NukeRecord();
        GrandManager.ins.RequireInter(true, "SETTING_HOME", () =>
        {
            BuyingPackDataFragment.cur.ConsecutiveLoseNum++;
            DataManager.ins.LogFail();
            if (SpaceMissionDataFragment.cur == null) Debug.Log("NULL 1");
            if (SpaceMissionDataFragment.cur.gameData == null) Debug.Log("NULL 2");
            if (SpaceMissionDataFragment.cur.gameData.data_of_players == null) Debug.Log("NULL 3");
            if (SpaceMissionDataFragment.cur.gameData.data_of_players[0] == null) Debug.Log("NULL 4");
            if (SpaceMissionDataFragment.cur.gameData.data_of_players[0].score == null) Debug.Log("NULL 5");
            SpaceMissionDataFragment.cur.gameData.data_of_players[0].score = 0;
            WinstreakDataFragment.cur.IsJustLose = true;
            WinstreakDataFragment.cur.BackToLastCheckPoint();
            GrandManager.ins.IntoHome();
        });
    }

    public void OnClickReplay()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        if (WinstreakDataFragment.cur.IsNeedToNoffLoseStreak())
        {
            UIManager.ins.OpenUI<CanvasWinstreakLoseNoff>().Setup(false);
            Close();
            return;
        }

        OnReplayHandle();
    }

    public void OnReplayHandle()
    {
        TreasureDataFragment.cur.NukeRecord();
        Helicoptar.cur.Reposition();
        GrandManager.ins.RequireInter(true, "SETTING_REPLAY", () =>
        {
            BuyingPackDataFragment.cur.ConsecutiveLoseNum++;
            DataManager.ins.LogFail();
            SpaceMissionDataFragment.cur.gameData.data_of_players[0].score = 0;
            WinstreakDataFragment.cur.IsJustLose = true;
            WinstreakDataFragment.cur.BackToLastCheckPoint();
            GrandManager.ins.InToGame();
        });
    }
}
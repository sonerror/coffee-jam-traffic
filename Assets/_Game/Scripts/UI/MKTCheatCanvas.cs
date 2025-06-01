using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using _Game.Scripts.Bus;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class MKTCheatCanvas : MonoBehaviour
{
    public GameObject cheatPanel;

    public RectTransform canvasRect;

    public bool isAutoHideUi;
    public bool isHideAds;

    public TMP_InputField inputField;
    public TMP_InputField addGoldInputField;
    private float ts = 1f;
    private CanvasGroup uimanagerCG;

    // public GameObject bgCanvasObject;
    // public RectTransform bgRect;
    // public RectTransform canvasRect;
    // public Canvas bgCanvas;
    // public Image bgImage;
    // public Sprite[] bgSprite;

    private CanvasGamePlay canvasGamePlay;

    // private IEnumerator Start()
    // {
    //     // yield return new WaitUntil(() => UIManager.ins != null);
    //     //
    //     // yield return new WaitUntil(() => CameraCon.ins != null);
    //     // yield return null;
    //     // yield return null;
    //     // yield return null;
    //     //
    //     // // bgCanvas.worldCamera = CameraCon.ins.cam;
    // }

    private bool isCheat;
    public RectTransform[] cheatPoints;
    private Coroutine checkCor;

    private const string key = "cheatable canvas";

    public bool isHideGamePlayCanvas;
    public bool isAutoDeleteAds;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        isCheat = PlayerPrefs.HasKey(key);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AdsManager.Ins != null && GrandManager.ins != null && (GrandManager.ins.IsGame || GrandManager.ins.IsHome));
        if (isCheat) Cheat();
    }

    // private void Update()
    // {
    //     Debug.Log("POS " + GetMousePos());
    // }

    public void OnCLickToogle()
    {
        if (isCheat)
        {
            cheatPanel.SetActive(!cheatPanel.activeInHierarchy);
            if (cheatPanel.activeInHierarchy)
            {
                ts = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = ts;
            }
        }
        else
        {
            if (checkCor != null) StopCoroutine(checkCor);
            checkCor = StartCoroutine(ie_Check());
        }

        return;

        IEnumerator ie_Check()
        {
            int index = 0;
            var code = new int[] { 0, 1, 2, 3, 0 };

            Debug.Log("PASS PRE");

            while (Input.GetMouseButton(0) && index < code.Length)
            {
                var checkPoint = cheatPoints[code[index]];
                var checkPos = checkPoint.position.ToFullScreenCanvasPosFromOverLayCanvasPos(canvasRect);
                yield return new WaitUntil(() => Vector2.Distance(checkPos, GetMousePos()) < 280);

                Debug.Log("PASS " + index);

                index++;
                if (index == code.Length && !isCheat)
                {
                    isCheat = true;

                    PlayerPrefs.SetInt(key, 2710);

                    Cheat();
                    OnCLickToogle();
                }
            }
        }
    }

    private Vector2 GetMousePos()
    {
        var position = canvasRect.position;
        Vector2 anchorPos = (Vector2)Input.mousePosition - new Vector2(position.x, position.y);
        var lossyScale = canvasRect.lossyScale;
        anchorPos = new Vector2(anchorPos.x / lossyScale.x, anchorPos.y / lossyScale.y);
        return anchorPos;
    }

    private void Cheat()
    {
        uimanagerCG = UIManager.ins.gameObject.AddComponent<CanvasGroup>();
        uimanagerCG.alpha = 1;

        if (!isHideGamePlayCanvas)
        {
            canvasGamePlay = UIManager.ins.GetUICanvas<CanvasGamePlay>();
            canvasGamePlay.transform.SetParent(null);
        }

        if (isAutoDeleteAds) OnClickDeleteAds();

        AdsManager.Ins.isForceMaxCreativeDebug = true;
    }

    public void OnClickDeleteAds()
    {
        if (!isHideAds) return;
        AdsManager.Ins.isMkt = true;
        AdsManager.Ins.HideBanner();
    }

    public void OnClickToogleUI()
    {
        uimanagerCG.alpha = 1 - uimanagerCG.alpha;
        OnCLickToogle();
    }

    public void OnCLickReload()
    {
        GrandManager.ins.IntoHome();
        OnCLickToogle();
        uimanagerCG.alpha = 1;
    }

    public void OnCLickCheatAll()
    {
        LevelDataFragment.cur.gameData.isSwapCarTutShowed = true;
        LevelDataFragment.cur.gameData.isVipBusTutShowed = true;
        LevelDataFragment.cur.gameData.isSwapMinionTutShowed = true;

        ResourcesDataFragment.cur.gameData.swapCarNum = 2710;
        ResourcesDataFragment.cur.gameData.vipBusNum = 2710;
        ResourcesDataFragment.cur.gameData.swapMinionNum = 2710;

        OnCLickReload();
    }

    public void OnClickBgMusicOn()
    {
        AudioManager.ins.MusicAudioSource.volume = 1;
    }

    public void OnClickBgMusicOff()
    {
        AudioManager.ins.MusicAudioSource.volume = 0;
    }

    public void OnClickJumpToLevel()
    {
        int i;
        int.TryParse(inputField.text, out i);
        i--;
        if (i < 0) i = 0;

        LevelDataFragment.cur.gameData.level = i;

        GrandManager.ins.InToGame();
        OnCLickToogle();
    }

    public void OnClickGainObjective()
    {
        int i;
        int.TryParse(inputField.text, out i);
        if (i < 0) i = 0;

        TreasureDataFragment.cur.RecordObject(i, TreasureDataFragment.cur.CurrentTreasureType);

        OnCLickToogle();
        GrandManager.ins.IntoHome();
    }

    public void OnClickCheatCapping()
    {
        BuyingPackDataFragment.cur.CheatCapping();
    }

    public void OnCheatWinStreak()
    {
        OnCLickToogle();
        WinstreakDataFragment.cur.InCreaseStreak();
        WinstreakDataFragment.cur.IsJustIncreaseStreak = true;

        GrandManager.ins.IntoHome();
    }

    public void OnClickPassLevelFree()
    {
        if (!TransportCenter.cur.IsGamePlaying || !GrandManager.ins.IsGame) return;
        var buses = BusStation.cur.activeBus;

        for (int i = 0; i < buses.Count; i++)
        {
            var bus = buses[i];
            var color = bus.color;

            if (color == JunkColor.Red) TreasureDataFragment.cur.RecordObject(GetSeatNum(bus.Type), TreasureDataFragment.TreasureType.Minion);
            if (color == JunkColor.Blue) TreasureDataFragment.cur.RecordObject(1, TreasureDataFragment.TreasureType.Bus);
        }

        var tunnels = BusStation.cur.activeTunnels;
        for (int i = 0; i < tunnels.Count; i++)
        {
            var tun = tunnels[i];
            for (int j = 0; j < tun.awaitBuses.Count; j++)
            {
                var bus = tun.awaitBuses[j];
                var color = bus.color;

                if (color == JunkColor.Red) TreasureDataFragment.cur.RecordObject(GetSeatNum(bus.busType), TreasureDataFragment.TreasureType.Minion);
                if (color == JunkColor.Blue) TreasureDataFragment.cur.RecordObject(1, TreasureDataFragment.TreasureType.Bus);
            }
        }

        TransportCenter.cur.IsGamePlaying = false;
        UIManager.ins.OpenUI<CanvasVictory>();

        OnCLickToogle();
        return;

        int GetSeatNum(BusType busType)
        {
            return busType switch
            {
                BusType.Car => 4,
                BusType.Van => 6,
                BusType.Bus => 10,
            };
        }
    }

    public void OnClickSetHeight()
    {
        int i;
        int.TryParse(inputField.text, out i);

        CanvasBannerOff.cur.SetPos(i);
    }


    public void ButtonAddGold()
    {
        if (int.TryParse(addGoldInputField.text, out var goldCount))
        {
            ResourcesDataFragment.cur.AddGold(goldCount, "Cheat");
        }
    }
}
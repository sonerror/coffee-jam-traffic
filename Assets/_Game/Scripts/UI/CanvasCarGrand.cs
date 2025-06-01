using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.Serialization;

public class CanvasCarGrand : UICanvasPrime
{
    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    [Header("Popup")] public GameObject obj_popupStart;
    public GameObject obj_popupLaunch;
    public GameObject obj_popupEnded;
    public GameObject obj_popupLose;
    public GameObject obj_canRace;
    public GameObject obj_tut;

    [Header("Reference")] public TMP_Text txt_time;
    public TMP_Text txt_time1;
    public TMP_Text txt_time2;
    public Image img_processRace;
    public RectTransform progressCar;
    public List<LineCarRace> lineCarRaces;
    private string stringTime;
    public GameObject obj_buttonClose;
    public RectTransform img_bg; // cẩn chỉnh lại để phù hợp nếu có banner

    public RectTransform infoButtonRect;
    public RectTransform closeButtonRect;
    public RectTransform closeTutButtonRect;
    public GameObject obj_buttonCheat;
    public GameObject obj_panelCheat;

    private void Awake()
    {
        infoButtonRect.SafeAreaAdaption(canvas);
        closeButtonRect.SafeAreaAdaption(canvas);
        closeTutButtonRect.SafeAreaAdaption(canvas);
    }

    protected override void OnOpenCanvas()
    {
        base.OnOpenCanvas();

        popTween?.Kill();
        mainPanelRect.localScale = Vector3.one * .05f;
        popTween = mainPanelRect.DOScale(Vector3.one, .24f).SetUpdate(true).SetEase(PopCurve);

        // tính chiều cao banner
        float pixel = 0;
        float dp = 0;
        float density = 0;
        if (AdsManager.isNoAds == false) // có banner
        {
            dp = MaxSdkUtils.GetAdaptiveBannerHeight();
            density = MaxSdkUtils.GetScreenDensity();
            pixel = dp * density;

#if UNITY_EDITOR
            density = 170;
#endif
        }

        Vector2 originalOffsetMin = img_bg.offsetMin; // Bottom-left corner
        img_bg.offsetMin = new Vector2(originalOffsetMin.x, pixel); // Thêm 50 vào padding bên dưới

        obj_buttonCheat.SetActive(AdsManager.Ins.isMkt);
    }

    private void Update()
    {
        if (CarRaceDataFragment.cur.timeLeft > 0)
        {
            stringTime = GameHelper.FormatTimeHHMMSS(CarRaceDataFragment.cur.timeLeft);
            txt_time.text = stringTime;
            txt_time1.text = stringTime;
            txt_time2.text = stringTime;
        }
        else
        {
            txt_time.text = "Ended";
            txt_time1.text = "Ended";
            txt_time2.text = "Ended";
        }
    }

    private void SetUpProcess()
    {
        img_processRace.fillAmount = CarRaceDataFragment.cur.gameData.raceIndex / 3f;
        if (CarRaceDataFragment.cur.gameData.raceIndex == 0)
        {
            progressCar.anchoredPosition = new Vector2(11.4f, 0);
        }
        else if (CarRaceDataFragment.cur.gameData.raceIndex == 1)
        {
            progressCar.anchoredPosition = new Vector2(196, 0);
        }
        else if (CarRaceDataFragment.cur.gameData.raceIndex == 2)
        {
            progressCar.anchoredPosition = new Vector2(410.5f, 0);
        }
        else
        {
            progressCar.anchoredPosition = new Vector2(589, 0);
        }
    }

    public void Setup(RaceStatus raceStatus = RaceStatus.Racing)
    {
        if (CarRaceDataFragment.cur.vuaChoiXong && CarRaceDataFragment.cur.gameData.chuaNhanThuong == false)
        {
            CarRaceDataFragment.cur.vuaChoiXong = false;
            CarRaceDataFragment.cur.gameData.startedRace = false;
        }

        if (CarRaceDataFragment.cur.gameData.raceIndex >= 3)
        {
            obj_canRace.SetActive(false);
            obj_popupEnded.SetActive(true);
        }
        else
        {
            obj_canRace.SetActive(true);
            obj_popupEnded.SetActive(false);
        }

        obj_buttonClose.SetActive(true);
        if (CarRaceDataFragment.cur.gameData.chuaNhanThuong)
        {
            obj_buttonClose.SetActive(false);
        }

        SetUpProcess();
        int indexRace = CarRaceDataFragment.cur.gameData.raceIndex;
        if (CarRaceDataFragment.cur.gameData.startedRace == false)
        {
            obj_popupStart.SetActive(true);
            obj_popupLaunch.SetActive(false);
            obj_popupLose.SetActive(false);
        }
        else
        {
            obj_popupStart.SetActive(false);
            obj_popupLaunch.SetActive(true);
        }

        StartCoroutine(ie_waitToSetLine(raceStatus));
    }

    private IEnumerator ie_waitToSetLine(RaceStatus raceStatus)
    {
        // check vị trí có điểm số cao nhất
        int minScore = -1;
        int top1 = -1;
        for (int i = 0; i < 5; i++)
        {
            if (CarRaceDataFragment.cur.gameData.data_of_players[i].score > minScore)
            {
                minScore = CarRaceDataFragment.cur.gameData.data_of_players[i].score;
                top1 = i;
            }
        }

        minScore = -1;
        int top2 = -1;
        for (int i = 0; i < 5; i++)
        {
            if (i != top1)
            {
                if (CarRaceDataFragment.cur.gameData.data_of_players[i].score > minScore)
                {
                    minScore = CarRaceDataFragment.cur.gameData.data_of_players[i].score;
                    top2 = i;
                }
            }
        }

        minScore = -1;
        int top3 = -1;
        for (int i = 0; i < 5; i++)
        {
            if (i != top1 && i != top2)
            {
                if (CarRaceDataFragment.cur.gameData.data_of_players[i].score > minScore)
                {
                    minScore = CarRaceDataFragment.cur.gameData.data_of_players[i].score;
                    top3 = i;
                }
            }
        }

        yield return null;

        // thiết lập đường bay
        for (int i = 0; i < lineCarRaces.Count; i++)
        {
            lineCarRaces[i].SetUpProfile(CarRaceDataFragment.cur.gameData.data_of_players[i]);
            lineCarRaces[i].fx_win.SetActive(false);

            // thiết lập vị trí
            lineCarRaces[i].obj_tops[0].SetActive(i == top1);
            lineCarRaces[i].obj_tops[1].SetActive(i == top2);
            lineCarRaces[i].obj_tops[2].SetActive(i == top3);
            lineCarRaces[i].SetUpPos(CarRaceDataFragment.cur.gameData.data_of_players[i].score, i);
        }

        if (raceStatus == RaceStatus.PlayerWin)
        {
            PlayerWin();
        }
        else if (raceStatus == RaceStatus.PlayerLose)
        {
            BotWin(CarRaceDataFragment.cur.gameData.top3);
        }
    }

    public void Btn_showInfo()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_tut.SetActive(true);
    }

    public void Btn_Start()
    {
        if (ButtonRaceHome.cur != null) ButtonRaceHome.cur.oldScore = 0;
        FirebaseManager.Ins.g_car_race_start();
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_popupStart.SetActive(false);
        obj_popupLaunch.SetActive(true);
        CarRaceDataFragment.cur.gameData.startedRace = true;
        CarRaceDataFragment.cur.gameData.top1 = -1;
        CarRaceDataFragment.cur.gameData.top2 = -1;
        CarRaceDataFragment.cur.gameData.top3 = -1;
        CarRaceDataFragment.cur.gameData.top4 = -1;
        CarRaceDataFragment.cur.gameData.top5 = -1;
        for (int i = 0; i < lineCarRaces.Count; i++)
        {
            lineCarRaces[i].Reset();
        }

        SetUpBotData();
        Setup();
        StartCoroutine(ie_FakeFindUser());

        CarRaceDataFragment.cur.Save();
    }

    private IEnumerator ie_FakeFindUser()
    {
        yield return null;
        for (int i = 0; i < 5; i++)
        {
            lineCarRaces[i].carTrans.transform.position = lineCarRaces[i].startPos.position + Vector3.left * 500;
        }

        for (int i = 0; i < 5; i++)
        {
            lineCarRaces[i].carTrans.transform.DOMoveX(lineCarRaces[i].startPos.position.x, 0.5f);
            yield return Yielders.Get(0.5f);
        }
    }

    public void SetUpBotData() // thiết lập trình độ của bot, để tính toán thời gian
    {
        int indexRace = CarRaceDataFragment.cur.gameData.raceIndex;
        CreateUser(0, RacePlayerLevel.Master, RacePlayerType.Player, 0);

        if (indexRace == 0)
        {
            int ran_index = Random.Range(1, 5);
            for (int i = 1; i < 5; i++)
            {
                if (i == ran_index)
                {
                    CreateUser(i, RacePlayerLevel.Advanced, RacePlayerType.Bot, i);
                }
                else
                {
                    CreateUser(i, (RacePlayerLevel)Random.Range(0, 2), RacePlayerType.Bot, i);
                }
            }
        }
        else if (indexRace == 1)
        {
            int ran_index = Random.Range(1, 5);
            for (int i = 1; i < 5; i++)
            {
                if (i == ran_index)
                {
                    CreateUser(i, RacePlayerLevel.Expert, RacePlayerType.Bot, i);
                }
                else
                {
                    CreateUser(i, (RacePlayerLevel)Random.Range(0, 3), RacePlayerType.Bot, i);
                }
            }
        }
        else if (indexRace == 2)
        {
            int ran_index = Random.Range(1, 5);
            for (int i = 1; i < 5; i++)
            {
                if (i == ran_index)
                {
                    CreateUser(i, RacePlayerLevel.Master, RacePlayerType.Bot, i);
                }
                else
                {
                    CreateUser(i, (RacePlayerLevel)Random.Range(0, 4), RacePlayerType.Bot, i);
                }
            }
        }
    }

    private void CreateUser(int index, RacePlayerLevel level, RacePlayerType racePlayerType, int id)
    {
        string stringNow = UnbiasedTime.TrueDateTime.ToString();
        if (racePlayerType == RacePlayerType.Player)
        {
            CarRaceDataFragment.cur.gameData.data_of_players[index] = new PlayerDataRace
            (
                level,
                stringNow,
                racePlayerType,
                ProfileDataFragment.cur.gameData.idAvatar,
                ProfileDataFragment.cur.gameData.idFrame,
                ProfileDataFragment.cur.gameData.userName,
                id
            );
        }
        else
        {
            string name = Random.Range(0, 10) == 0 ? "User" + Random.Range(99999, 99999999).ToString("00000000") : ProfileDataFragment.cur.names[Random.Range(0, ProfileDataFragment.cur.names.Count)];

            CarRaceDataFragment.cur.gameData.data_of_players[index] = new PlayerDataRace
            (
                level,
                stringNow,
                racePlayerType,
                Random.Range(0, ProfileDataFragment.cur.spr_avatars.Length),
                Random.Range(0, ProfileDataFragment.cur.spr_frames.Length),
                name,
                id
            );
        }
    }

    [ContextMenu("PlayerWin")]
    public void PlayerWin()
    {
        if (!CarRaceDataFragment.cur.gameData.startedRace) return;
        CarRaceDataFragment.cur.gameData.startedRace = false;

        int indexChest = -1;
        if (CarRaceDataFragment.cur.gameData.top1 == 0) indexChest = 0;
        else if (CarRaceDataFragment.cur.gameData.top2 == 0) indexChest = 1;
        else if (CarRaceDataFragment.cur.gameData.top3 == 0) indexChest = 2;
        
        LineCarRace.HandleOnClaim(indexChest);

        StartCoroutine(ie_runWin(0));
        FirebaseManager.Ins.g_car_race_complete();

        // reset luôn event
        // CarRaceDataFragment.cur.gameData.raceIndex = Mathf.Min(CarRaceDataFragment.cur.gameData.raceIndex + 1, 2);
        // CarRaceDataFragment.cur.gameData.startedRace = false;
    }

    [ContextMenu("BotWin")]
    public void BotWin(int indexWin)
    {
        CarRaceDataFragment.cur.gameData.startedRace = false;
        StartCoroutine(ie_runWin(indexWin));

        // reset luôn event
        // CarRaceDataFragment.cur.gameData.startedRace = false;
    }

    private IEnumerator ie_runWin(int indexWin)
    {
        UIManager.ins.OpenUI<CanvasBlockage>();
        if (indexWin != 0)
        {
            RectTransform _btn = lineCarRaces[indexWin].obj_buttonBot[2].GetComponent<RectTransform>();
            lineCarRaces[indexWin].carTrans.position = lineCarRaces[indexWin].endPos.position;
            lineCarRaces[indexWin].carTrans.gameObject.SetActive(true);
            _btn.anchoredPosition = new Vector2(216, -6);
        }
        else
        {
            CarRaceDataFragment.cur.gameData.startedRace = false;
            CarRaceDataFragment.cur.vuaChoiXong = true;
            int indexChest = 0;
            if (CarRaceDataFragment.cur.gameData.top1 == 0) indexChest = 0;
            else if (CarRaceDataFragment.cur.gameData.top2 == 0) indexChest = 1;
            else if (CarRaceDataFragment.cur.gameData.top3 == 0) indexChest = 2;
            RectTransform _btn = lineCarRaces[indexWin].obj_buttonUser[indexChest].GetComponent<RectTransform>();
            lineCarRaces[indexWin].carTrans.position = lineCarRaces[indexWin].endPos.position;
            lineCarRaces[indexWin].carTrans.gameObject.SetActive(true);
            _btn.anchoredPosition = new Vector2(342, -6);
        }

        // CarRaceDataFragment.cur.vuaChoiXong = true;
        yield return Yielders.Get(0.5f);
        lineCarRaces[indexWin].carTrans.DOMoveX(lineCarRaces[indexWin].winPos.position.x, 0.5f);
        yield return Yielders.Get(0.1f);
        lineCarRaces[indexWin].fx_win.SetActive(true);

        if (indexWin == 0)
        {
            int indexChest = 0;
            if (CarRaceDataFragment.cur.gameData.top1 == 0) indexChest = 0;
            else if (CarRaceDataFragment.cur.gameData.top2 == 0) indexChest = 1;
            else if (CarRaceDataFragment.cur.gameData.top3 == 0) indexChest = 2;
            RectTransform _btn = lineCarRaces[indexWin].obj_buttonUser[indexChest].GetComponent<RectTransform>();
            _btn.gameObject.SetActive(true);
            _btn.anchoredPosition = new Vector2(342, -6);
            _btn.DOAnchorPosX(0, 0.3f);
            lineCarRaces[indexWin].obj_tops[0].transform.SetParent(_btn);
            lineCarRaces[indexWin].obj_tops[1].transform.SetParent(_btn);
            lineCarRaces[indexWin].obj_tops[2].transform.SetParent(_btn);
            UIManager.ins.CloseUI<CanvasBlockage>();
        }
        else // này là thua rồi
        {
            CarRaceDataFragment.cur.vuaChoiXong = true;
            RectTransform _btn = lineCarRaces[indexWin].obj_buttonBot[2].GetComponent<RectTransform>();

            _btn.gameObject.SetActive(true);

            _btn.DOAnchorPosX(0, 0.3f);
            lineCarRaces[indexWin].obj_tops[0].transform.SetParent(_btn);
            lineCarRaces[indexWin].obj_tops[1].transform.SetParent(_btn);
            lineCarRaces[indexWin].obj_tops[2].transform.SetParent(_btn);
            UIManager.ins.CloseUI<CanvasBlockage>();
            obj_popupLose.SetActive(true);
        }
    }

    public void Btn_ClosePopupEnded()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_popupEnded.SetActive(false);
    }

    public void Btn_ClosePopupLose()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_popupLose.SetActive(false);
    }

    public void Btn_close()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        gameObject.SetActive(false);

        if (CarRaceDataFragment.cur.vuaChoiXong)
        {
            CarRaceDataFragment.cur.vuaChoiXong = false;
            CarRaceDataFragment.cur.vuaChoiXong = false;
            UIManager.ins.OpenUI<CanvasCarGrand>().Setup();
        }
    }

    public void Btn_closeTut()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_tut.SetActive(false);
    }

    #region Cheat
        public void Btn_openCheat()
        {
            obj_panelCheat.SetActive(true);
        }

        public void Btn_closeCheat()
        {
            obj_panelCheat.SetActive(false);
        }

        public void Btn_cheatTop(int top)
        {
            if(top == 1)
            {
                CarRaceDataFragment.cur.gameData.data_of_players[0].score = 10;
            }
            else if(top == 2)
            {
                CarRaceDataFragment.cur.gameData.data_of_players[1].timeUpdateScore = DateTime.Now.AddHours(-10).ToString();
            }
            else if(top == 3)
            {
                CarRaceDataFragment.cur.gameData.data_of_players[1].timeUpdateScore = DateTime.Now.AddHours(-10).ToString();
                CarRaceDataFragment.cur.gameData.data_of_players[2].timeUpdateScore = DateTime.Now.AddHours(-10).ToString();
            }
            obj_panelCheat.SetActive(false);
        }

        public void Btn_cheatPlayerLose()
        {
            CarRaceDataFragment.cur.gameData.data_of_players[1].timeUpdateScore = DateTime.Now.AddHours(-10).ToString();
            CarRaceDataFragment.cur.gameData.data_of_players[2].timeUpdateScore = DateTime.Now.AddHours(-10).ToString();
            CarRaceDataFragment.cur.gameData.data_of_players[3].timeUpdateScore = DateTime.Now.AddHours(-10).ToString();
            
            obj_panelCheat.SetActive(false);
        }
    #endregion
}
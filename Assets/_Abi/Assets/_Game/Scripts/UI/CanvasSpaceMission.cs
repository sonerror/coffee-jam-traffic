using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CanvasSpaceMission : UICanvasPrime
{
    public RectTransform mainPanelRect;
    private static AnimationCurve PopCurve => UIManager.ins.popCanvasCurve;
    private Tween popTween;

    public RectTransform button_1;
    public RectTransform button_2;
    public RectTransform button_3;

    [Header("Popup")] public GameObject obj_popupStart;
    public GameObject obj_popupLaunch;
    public GameObject obj_popupCanPlay;
    public GameObject obj_popupEnded;
    public GameObject obj_popupLose;
    public GameObject obj_tut;

    [Header("Reference")] public GameObject[] img_themes;
    public TMP_Text txt_title;
    public TMP_Text txt_plane;
    public TMP_Text txt_time;
    public TMP_Text txt_time1;
    public TMP_Text txt_time2;
    public TMP_Text txt_description;
    public TMP_Text txt_lose;
    public LineSpaceMission[] lineSpaceMissions;
    public int numPlayer = 3;
    private string stringTime;
    public Image img_progress_tut;
    public RectTransform img_bg;
    public GameObject[] obj_titles;
    public GameObject obj_cheat;
    public GameObject obj_cheatPanel;

    private void Awake()
    {
        button_1.SafeAreaAdaption(canvas);
        button_2.SafeAreaAdaption(canvas);
        button_3.SafeAreaAdaption(canvas);
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

        obj_cheat.SetActive(AdsManager.Ins.isMkt);
    }

    private void Update()
    {
        if (SpaceMissionDataFragment.cur.timeLeft > 0)
        {
            stringTime = GameHelper.FormatTimeHHMMSS(SpaceMissionDataFragment.cur.timeLeft);
            txt_time.text = stringTime;
            txt_time1.text = stringTime;
            txt_time2.text = stringTime;
        }
        else
        {
            txt_time.text = "End";
            txt_time1.text = "End";
            txt_time2.text = "End";
        }
    }

    public void Setup(int indexWin = -1, bool activeRocket = true)
    {
        int index_mission = SpaceMissionDataFragment.cur.gameData.missionIndex;

        if(index_mission >= 3)
        {
            obj_popupCanPlay.SetActive(false);
            obj_popupEnded.SetActive(true);
            return;
        }
        else
        {
            obj_popupCanPlay.SetActive(true);
            obj_popupEnded.SetActive(false);
        }
        img_progress_tut.fillAmount = (index_mission) / 3f;


        for (int i = 0; i < obj_titles.Length; i++)
        {
            obj_titles[i].SetActive(i == index_mission);
        }

        if (SpaceMissionDataFragment.cur.gameData.startedSpace == false)
        {
            obj_popupStart.SetActive(true);
            obj_popupLaunch.SetActive(false);
            obj_popupLose.SetActive(false);
            obj_tut.SetActive(false);
        }
        else
        {
            obj_popupStart.SetActive(false);
            obj_popupLaunch.SetActive(true);
        }

        for (int i = 0; i < img_themes.Length; i++)
        {
            img_themes[i].SetActive(i == index_mission);
        }

        int numGame = 0;

        if (index_mission == 0)
        {
            txt_title.text = "SPACE MISSION I";
            txt_plane.text = "MOON";
            txt_description.text = "Beat 5 consecutive rounds on your first \ntry before others to complete the mission!";
            txt_lose.text = "You have lost in the 'Space Mission I' event, please try again and strive harder.";
            numPlayer = 3;
            numGame = 5;
        }
        else if (index_mission == 1)
        {
            txt_title.text = "SPACE MISSION II";
            txt_plane.text = "MARS";
            txt_description.text = "Beat 7 consecutive rounds on your first \ntry before others to complete the mission!";
            txt_lose.text = "You have lost in the 'Space Mission II' event, please try again and strive harder.";
            numPlayer = 4;
            numGame = 7;
        }
        else
        {
            txt_title.text = "SPACE MISSION III";
            txt_plane.text = "SATURN";
            txt_description.text = "Beat 9 consecutive rounds on your first \ntry before others to complete the mission!";
            txt_lose.text = "You have lost in the 'Space Mission III' event, please try again and strive harder.";
            numPlayer = 5;
            numGame = 9;
        }

        StartCoroutine(ie_waitToSetLine(index_mission, numGame, indexWin, activeRocket));
    }

    private IEnumerator ie_waitToSetLine(int index_mission, int numGame, int indexWin, bool activeRocket)
    {
        // check vị trí có điểm số cao nhất
        int minScore = 0;
        int top1 = -1;
        for (int i = 0; i < numPlayer; i++)
        {
            if (SpaceMissionDataFragment.cur.gameData.data_of_players[i].score > minScore)
            {
                minScore = SpaceMissionDataFragment.cur.gameData.data_of_players[i].score;
                top1 = i;
            }
        }

        for (int i = 0; i < lineSpaceMissions.Length; i++)
        {
            lineSpaceMissions[i].fx_win.SetActive(false);
            lineSpaceMissions[i].gameObject.SetActive(i < numPlayer);
        }
        yield return Yielders.EndOfFrame;

        // thiết lập đường bay
        for (int i = 0; i < lineSpaceMissions.Length; i++)
        {
            lineSpaceMissions[i].SetUpProfile(SpaceMissionDataFragment.cur.gameData.data_of_players[i]);
            lineSpaceMissions[i].fx_win.SetActive(false);
            lineSpaceMissions[i].gameObject.SetActive(i < numPlayer);
            lineSpaceMissions[i].skeletonRocket.Skeleton.SetSkin("0" + (index_mission + 1));
            lineSpaceMissions[i].skeletonRocket.Skeleton.SetSlotsToSetupPose();
            lineSpaceMissions[i].txt_score.text = SpaceMissionDataFragment.cur.gameData.data_of_players[i].score.ToString();
            lineSpaceMissions[i].obj_frameScore.SetActive(true);
            // thiết lập vị trí
            lineSpaceMissions[i].SetUpPos(numGame, SpaceMissionDataFragment.cur.gameData.data_of_players[i].score);
            lineSpaceMissions[i].obj_top1.SetActive(i == top1);
            if(activeRocket) lineSpaceMissions[i].rocket.gameObject.SetActive(true);

            if (i == 0 && indexWin == 0)
            {
                PlayerWin();
            }
            else if (indexWin == i)
            {
                BotWin(indexWin);
            }
        }
    }

    public void Btn_showInfo()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_tut.SetActive(true);
    }

    public void Btn_close()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        gameObject.SetActive(false);

        if (SpaceMissionDataFragment.cur.vuaChoiXong)
        {
            SpaceMissionDataFragment.cur.vuaChoiXong = false;
            UIManager.ins.OpenUI<CanvasSpaceMission>().Setup();
        }
    }

    public void Btn_Start()
    {
        if (ButtonSpaceHome.cur != null) ButtonSpaceHome.cur.oldScore = 0;
        FirebaseManager.Ins.g_space_mission_start();
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_popupStart.SetActive(false);
        obj_popupLaunch.SetActive(true);
        SpaceMissionDataFragment.cur.gameData.startedSpace = true;
        for (int i = 0; i < numPlayer; i++)
        {
            // lineSpaceMissions[i].obj_frameScore.SetActive(false);
            lineSpaceMissions[i].rocket.gameObject.SetActive(false);
        }


        SetUpBotData();
        Setup(activeRocket: false);
        StartCoroutine(ie_FakeFindUser());

        SpaceMissionDataFragment.cur.Save();
    }

    private IEnumerator ie_FakeFindUser()
    {
        for (int i = 0; i < numPlayer; i++)
        {
            yield return Yielders.Get(Random.Range(0.2f, 0.3f));
            lineSpaceMissions[i].rocket.gameObject.SetActive(true);
            lineSpaceMissions[i].ShowRocket();
        }
    }

    public void SetUpBotData() // thiết lập trình độ của bot, để tính toán thời gian
    {
        int index_mission = SpaceMissionDataFragment.cur.gameData.missionIndex;

        CreateUser(0, SpacePlayerLevel.Master, SpacePlayerType.Player);

        if (index_mission == 0) // có 3 thằng thôi
        {
            int ran_index = Random.Range(1, 3);
            for (int i = 1; i < 3; i++)
            {
                if (i == ran_index)
                {
                    CreateUser(i, SpacePlayerLevel.Advanced, SpacePlayerType.Bot);
                }
                else
                {
                    CreateUser(i, (SpacePlayerLevel)Random.Range(0, 2), SpacePlayerType.Bot);
                }
            }
        }
        else if (index_mission == 1)
        {
            int ran_index = Random.Range(1, 4);
            for (int i = 1; i < 4; i++)
            {
                if (i == ran_index)
                {
                    CreateUser(i, SpacePlayerLevel.Expert, SpacePlayerType.Bot);
                }
                else
                {
                    CreateUser(i, (SpacePlayerLevel)Random.Range(0, 3), SpacePlayerType.Bot);
                }
            }
        }
        else if (index_mission == 2)
        {
            int ran_index = Random.Range(1, 5);
            for (int i = 1; i < 5; i++)
            {
                if (i == ran_index)
                {
                    CreateUser(i, SpacePlayerLevel.Master, SpacePlayerType.Bot);
                }
                else
                {
                    CreateUser(i, (SpacePlayerLevel)Random.Range(0, 4), SpacePlayerType.Bot);
                }
            }
        }
    }

    private void CreateUser(int index, SpacePlayerLevel level, SpacePlayerType spacePlayerType)
    {
        string stringNow = UnbiasedTime.TrueDateTime.ToString();
        if (spacePlayerType == SpacePlayerType.Player)
        {
            SpaceMissionDataFragment.cur.gameData.data_of_players[index] = new PlayerDataSpace
            (
                level,
                stringNow,
                spacePlayerType,
                ProfileDataFragment.cur.gameData.idAvatar,
                ProfileDataFragment.cur.gameData.idFrame,
                ProfileDataFragment.cur.gameData.userName
            );
        }
        else
        {
            string name = Random.Range(0, 10) == 0 ? "User" + Random.Range(99999, 99999999).ToString("00000000") : ProfileDataFragment.cur.names[Random.Range(0, ProfileDataFragment.cur.names.Count)];

            SpaceMissionDataFragment.cur.gameData.data_of_players[index] = new PlayerDataSpace
            (
                level,
                stringNow,
                spacePlayerType,
                Random.Range(0, ProfileDataFragment.cur.spr_avatars.Length),
                Random.Range(0, ProfileDataFragment.cur.spr_frames.Length),
                name
            );
        }
    }

    public void PlayerWin()
    {
        StartCoroutine(ie_runWin(0, SpaceMissionDataFragment.cur.gameData.missionIndex));
        FirebaseManager.Ins.g_space_mission_complete();

        // reset luôn event
        SpaceMissionDataFragment.cur.gameData.missionIndex++;
        SpaceMissionDataFragment.cur.gameData.startedSpace = false;
    }

    public void BotWin(int indexWin)
    {
        StartCoroutine(ie_runWin(indexWin, SpaceMissionDataFragment.cur.gameData.missionIndex));

        // reset luôn event
        SpaceMissionDataFragment.cur.gameData.startedSpace = false;
    }

    private IEnumerator ie_runWin(int indexWin, int missionIndex)
    {
        UIManager.ins.OpenUI<CanvasBlockage>();
        SpaceMissionDataFragment.cur.vuaChoiXong = true;
        yield return Yielders.Get(0.5f);
        lineSpaceMissions[indexWin].rocket.DOMoveY(lineSpaceMissions[indexWin].winPos.position.y, 0.5f);
        yield return Yielders.Get(0.1f);
        lineSpaceMissions[indexWin].fx_win.SetActive(true);

        if (indexWin == 0)
        {
            yield return Yielders.Get(1);
            FrameClaimChest frameClaimChest = UIManager.ins.OpenUI<FrameClaimChest>();
            frameClaimChest.txt_des.text = "You finished first!";
            frameClaimChest.callBack = () =>
            {
                gameObject.SetActive(false);
                if (SpaceMissionDataFragment.cur.vuaChoiXong)
                {
                    SpaceMissionDataFragment.cur.vuaChoiXong = false;
                    UIManager.ins.OpenUI<CanvasSpaceMission>().Setup();
                }
            };
            if (missionIndex == 0)
            {
                ResourcesDataFragment.cur.PendingGold(50, "space_mission");
                // ResourcesDataFragment.cur.AddSwapCar(1, "space_mission");
                frameClaimChest.SetReward(new RewardChestBundle[] { new RewardChestBundle(RewardChestType.ticket, 50)}, 6, false);
            }
            else if (missionIndex == 1)
            {
                ResourcesDataFragment.cur.AddSwapMinion(1, "space_mission");
                // ResourcesDataFragment.cur.AddVipBus(1, "space_mission");
                frameClaimChest.SetReward(new RewardChestBundle[] {new RewardChestBundle(RewardChestType.wapMinion, 1) }, 7, false);
            }
            else if (missionIndex == 2)
            {
                ResourcesDataFragment.cur.AddSwapCar(1, "space_mission");
                ResourcesDataFragment.cur.AddSwapMinion(1, "space_mission");
                // ResourcesDataFragment.cur.AddVipBus(1, "space_mission");
                frameClaimChest.SetReward(new RewardChestBundle[] { new RewardChestBundle(RewardChestType.swapCar, 1), new RewardChestBundle(RewardChestType.wapMinion, 1) }, 8, false);
            }
        }
        else
        {
            obj_popupLose.SetActive(true);
        }

        UIManager.ins.CloseUI<CanvasBlockage>();
    }

    public void Btn_ClosePopupEnd()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        gameObject.SetActive(false);
    }

    public void Btn_ClosePopupLose()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_popupLose.SetActive(false);
    }

    public void Btn_closeTut()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        obj_tut.SetActive(false);
    }

    #region Cheat
        public void Btn_openCheat()
        {
            obj_cheatPanel.SetActive(true);
        }

        public void Btn_closeCheat()
        {
            obj_cheatPanel.SetActive(false);
        }

        public void Btn_cheatPlayerWin()
        {
            int score = 0;
            if(SpaceMissionDataFragment.cur.gameData.missionIndex == 0)score = 5;
            else if(SpaceMissionDataFragment.cur.gameData.missionIndex == 1)score = 7;
            else score = 9;
            SpaceMissionDataFragment.cur.gameData.data_of_players[0].score = score;
            obj_cheatPanel.SetActive(false);
        }

        public void Btn_cheatPlayerLose()
        {
            SpaceMissionDataFragment.cur.gameData.data_of_players[1].timeUpdateScore = DateTime.Now.AddHours(-10).ToString();
            obj_cheatPanel.SetActive(false);
        }
    #endregion
}
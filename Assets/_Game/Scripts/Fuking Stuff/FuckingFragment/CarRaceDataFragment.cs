using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/car race", fileName = "car race")]
public class CarRaceDataFragment : DataFragment
{
    public static CarRaceDataFragment cur;
    public const int ActiveLevel = 20;
    public Data gameData;
    public bool vuaChoiXong = false;
    public const int MAX_SCORE = 10;
    public int timeLeft = 0; // thời gian có thể chơi còn lại
    private bool daDemGio = false;


#if UNITY_EDITOR
    private void OnEnable()
    {
        cur = this;
    }
#else
    private void Awake()
    {
        cur = this;
    }
#endif


    public override void Load()
    {
        if (!LoadData(ref gameData, key)) ResetData();

        daDemGio = false;
        timeLeft = 0;
        // vuaChoiXong = false;
    }

    public override void Save()
    {
        SaveData(gameData, key);
    }

    public override void ResetData()
    {
        gameData = new Data();
    }

    public void CalculateTime()
    {
        if (daDemGio) return;
        // xem hôm nay là ngày lẻ hay chẵn
        // DateTime dateTimeNow = DateTime.Now;
        DateTime dateTimeNow = UnbiasedTime.TrueDateTime;
        if (dateTimeNow.Day % 2 == 0) // ngày chan thì đúng là đc chơi rồi
        {
            // Lấy thời gian đến cuối ngày (23:59:59)
            DateTime endOfDay = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 23, 59, 59);

            // tính xem đã là ngày mới chưa thì reset luôn lại trạng thái
            if (gameData.timeEndEvent != "" && endOfDay > DateTime.Parse(gameData.timeEndEvent))
            {
                ResetRace();
            }

            gameData.timeEndEvent = endOfDay.ToString();

            // Tính khoảng thời gian còn lại
            TimeSpan remainingTime = endOfDay - dateTimeNow;

            // Lấy số giây còn lại
            timeLeft = (int)remainingTime.TotalSeconds;

            // đếm ngược
            DataManager.ins.StartCoroutine(ie_countDownTimeLeft());
        }
    }

    private IEnumerator ie_countDownTimeLeft()
    {
        daDemGio = true;
        while (timeLeft > 0)
        {
            timeLeft--;

            // Debug.Log("TIME " + timeLeft);

            yield return Yielders.Get(1);
        }

        // kết thúc event
        // Debug.Log("TIME q " + timeLeft);
        // reset lại toàn bộ trạng thái
        ResetRace();
        // tính toán lại
        // Debug.Log("TIME w " + timeLeft);
        yield return Yielders.Get(1);
        yield return Yielders.Get(1);
        yield return Yielders.Get(1);
        daDemGio = false;
        UIManager.ins.GetUICanvas<CanvasHome>().CheckEvent();
        // Debug.Log("TIME e " + timeLeft);

        if (UIManager.ins.GetUICanvas<CanvasCarGrand>().IsOpening())
        {
            // Debug.Log("TIME r " + timeLeft);
            UIManager.ins.GetUICanvas<CanvasCarGrand>().gameObject.SetActive(false);
        }
    }

    public void ResetRace()
    {
        gameData.startedRace = false;
        gameData.raceIndex = 0;
        gameData.top1 = -1;
        gameData.top2 = -1;
        gameData.top3 = -1;
        gameData.top4 = -1;
        gameData.top5 = -1;
    }

    public bool IsWinCarRace()
    {
        return gameData.data_of_players[0].score >= MAX_SCORE;
    }

    public void RankForUser(int id)
    {
        if(gameData.top1 == id || gameData.top2 == id || gameData.top3 == id) return;

        if (gameData.top1 == -1)
        {
            gameData.top1 = id;
        }
        else if (gameData.top2 == -1)
        {
            gameData.top2 = id;
        }
        else if (gameData.top3 == -1)
        {
            gameData.top3 = id;
        }
        else
        {
            if (id != 0)
            {
                gameData.data_of_players[id].score = MAX_SCORE - UnityEngine.Random.Range(1, 4);
            }
        }
    }

    public RaceStatus CalculateProgress() // tính toán điểm sau mỗi lần về back về home
    {
        if (gameData.startedRace == false || (gameData.top1 > 0 && gameData.top2 > 0 && gameData.top3 > 0)) return RaceStatus.RaceNotStart;

        bool ngChoiVeDichNgayLucNay = false;
        if (IsWinCarRace() && gameData.top1 != 0 && gameData.top2 != 0 && gameData.top3 != 0)
        {
            ngChoiVeDichNgayLucNay = true;
            RankForUser(0);
        }

        for (int i = 1; i < 5; i++)
        {
            float timeToPassLv = 350;
            if (gameData.data_of_players[i].playerLevel == RacePlayerLevel.Beginner)
            {
                timeToPassLv = 350;
            }
            else if (gameData.data_of_players[i].playerLevel == RacePlayerLevel.Intermediate)
            {
                timeToPassLv = 300;
            }
            else if (gameData.data_of_players[i].playerLevel == RacePlayerLevel.Advanced)
            {
                timeToPassLv = 220;
            }
            else if (gameData.data_of_players[i].playerLevel == RacePlayerLevel.Expert)
            {
                timeToPassLv = 180;
            }
            else if (gameData.data_of_players[i].playerLevel == RacePlayerLevel.Master)
            {
                timeToPassLv = 150;
            }

            // cần tính toán xem thằng này đã về đích chưa, nếu chưa về thì ưu tiên cho user
            bool daVeDich = (gameData.top1 == i || gameData.top2 == i || gameData.top3 == i || gameData.top4 == i || gameData.top5 == i);

            // xem hiện tại đã quá thời gian trên bao nhiêu lần
            int oldScore = gameData.data_of_players[i].score;
            DateTime timUp = UnbiasedTime.TrueDateTime;
            DateTime.TryParse(gameData.data_of_players[i].timeUpdateScore, out timUp);
            double thoiGianDaTroiQua = UnbiasedTime.TrueDateTime.Subtract(timUp).TotalSeconds;
            // điểm hiện tại là bằng số trên 
            gameData.data_of_players[i].score = (int)(thoiGianDaTroiQua / timeToPassLv);

            // nếu chơi chơi win lúc này, mà đồng thời thằng này cx về đích thì trừ điểm thằng này, cho user về nhất
            if (ngChoiVeDichNgayLucNay && daVeDich == false && gameData.data_of_players[i].score >= MAX_SCORE)
            {
                gameData.data_of_players[i].score = Mathf.Max(oldScore, MAX_SCORE - UnityEngine.Random.Range(1, 4));
            }
        }

        List<PlayerDataRace> playerDataRaces = new List<PlayerDataRace>();
        for (int i = 1; i < 5; i++)
        {
            playerDataRaces.Add(gameData.data_of_players[i]);
        }

        playerDataRaces.Sort((a, b) => b.score.CompareTo(a.score));

        for (int i = 0; i < playerDataRaces.Count; i++)
        {
            if (playerDataRaces[i].score >= MAX_SCORE)
            {
                RankForUser(playerDataRaces[i].id);
            }
        }

        // List<>
        // nếu ng chơi về đích lúc này thì hiện luôn popup
        if (ngChoiVeDichNgayLucNay)
        {
            gameData.chuaNhanThuong = true;
            return RaceStatus.PlayerWin;
        }

        if (gameData.top1 > 0 && gameData.top2 > 0 && gameData.top3 > 0)
        {
            return RaceStatus.PlayerLose;
        }

        return RaceStatus.Racing;
    }

    [Serializable]
    public class Data
    {
        public bool startedRace = false; // đã bắt đầu event chưa
        public int raceIndex = 0; // đã bắt đầu event chưa

        public PlayerDataRace[] data_of_players = new PlayerDataRace[]
        {
            new PlayerDataRace(RacePlayerLevel.Beginner, "", RacePlayerType.Bot, 0, 0, "", 0),
            new PlayerDataRace(RacePlayerLevel.Beginner, "", RacePlayerType.Bot, 0, 0, "", 0),
            new PlayerDataRace(RacePlayerLevel.Beginner, "", RacePlayerType.Bot, 0, 0, "", 0),
            new PlayerDataRace(RacePlayerLevel.Beginner, "", RacePlayerType.Bot, 0, 0, "", 0),
            new PlayerDataRace(RacePlayerLevel.Beginner, "", RacePlayerType.Bot, 0, 0, "", 0),
        };

        public int top1 = -1;
        public int top2 = -1;
        public int top3 = -1;
        public int top4 = -1;
        public int top5 = -1;
        public string timeEndEvent = "";
        public bool chuaNhanThuong = false;
    }
}

[Serializable]
public class PlayerDataRace
{
    public int id;
    public RacePlayerType racePlayerType;
    public string timeUpdateScore;
    public int score = 0;
    public RacePlayerLevel playerLevel; // trình độ của thằng này chơi có khá ko
    public int idAvatar = 0;
    public int idFrameAvatar = 0;
    public string userName;

    public PlayerDataRace()
    {

    }

    public PlayerDataRace(RacePlayerLevel _playerLevel, string _timeUpdateScore, RacePlayerType _racePlayerType, int _idAvatar, int _idFrame, string _userName, int _id)
    {
        playerLevel = _playerLevel;
        timeUpdateScore = _timeUpdateScore;
        score = 0;
        racePlayerType = _racePlayerType;
        idAvatar = _idAvatar;
        idFrameAvatar = _idFrame;
        userName = _userName;
        id = _id;
    }
}

[Serializable]
public enum RacePlayerLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert,
    Master
}

[Serializable]
public enum RacePlayerType
{
    Player,
    Bot
}

[Serializable]
public enum RaceStatus
{
    RaceNotStart,
    PlayerWin,
    PlayerLose,
    Racing
}
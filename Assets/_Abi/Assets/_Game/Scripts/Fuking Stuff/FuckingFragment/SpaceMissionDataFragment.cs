using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Space mission", fileName = "Space mission")]
public class SpaceMissionDataFragment : DataFragment
{
    public const int ActiveLevel = 20;
    public static SpaceMissionDataFragment cur;
    public Data gameData;
    public bool vuaChoiXong = false;
    public int timeLeft = 0; // thời gian có thể chơi còn lại
    private bool daDemGio = false;
    public bool waitToShowEnd = false;


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

        vuaChoiXong = false;
        timeLeft = 0; // thời gian có thể chơi còn lại
        daDemGio = false;
        waitToShowEnd = false;
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
        DateTime dateTimeNow = UnbiasedTime.TrueDateTime;
        if (dateTimeNow.Day % 2 == 1) // ngày lẻ thì đúng là đc chơi rồi
        {
            // Lấy thời gian đến cuối ngày (23:59:59)
            DateTime endOfDay = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 23, 59, 59);

            // tính xem đã là ngày mới chưa thì reset luôn lại trạng thái
            if (gameData.timeEndEvent == "" || (gameData.timeEndEvent != "" && endOfDay > DateTime.Parse(gameData.timeEndEvent)))
            {
                ResetSpace();
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

            yield return Yielders.Get(1);
        }

        // kết thúc event
        // reset lại toàn bộ trạng thái
        ResetSpace();

        // tính toán lại
        yield return Yielders.Get(1);
        yield return Yielders.Get(1);
        yield return Yielders.Get(1);
        daDemGio = false;
        waitToShowEnd = true;
        UIManager.ins.GetUICanvas<CanvasHome>().CheckEvent();
        if (UIManager.ins.GetUICanvas<CanvasSpaceMission>().IsOpening())
        {
            UIManager.ins.GetUICanvas<CanvasSpaceMission>().gameObject.SetActive(false);
        }
    }

    public void ResetSpace()
    {
        gameData.startedSpace = false;
        gameData.missionIndex = 0;
    }

    public bool IsWinSpaceMission()
    {
        int target = 5;
        if (gameData.missionIndex == 0) target = 5;
        else if (gameData.missionIndex == 1) target = 7;
        else if (gameData.missionIndex == 2) target = 9;
        return gameData.data_of_players[0].score >= target;
    }

    public int CalculateProgress() // tính toán điểm sau mỗi lần về back về home
    {
        if (gameData.startedSpace == false) return -1;

        int numPlayer = 0;
        int targetScore = 5;
        int indexWin = -1;
        if (gameData.missionIndex == 0)
        {
            numPlayer = 3;
            targetScore = 5;
        }
        else if (gameData.missionIndex == 1)
        {
            numPlayer = 4;
            targetScore = 7;
        }
        else if (gameData.missionIndex == 2)
        {
            numPlayer = 5;
            targetScore = 9;
        }

        bool hasPlayerWin = false;
        if (IsWinSpaceMission())
        {
            hasPlayerWin = true;
            indexWin = 0;
        }

        for (int i = 1; i < numPlayer; i++)
        {
            float timeToPassLv = 350;
            if (gameData.data_of_players[i].playerLevel == SpacePlayerLevel.Beginner)
            {
                timeToPassLv = 350;
            }
            else if (gameData.data_of_players[i].playerLevel == SpacePlayerLevel.Intermediate)
            {
                timeToPassLv = 300;
            }
            else if (gameData.data_of_players[i].playerLevel == SpacePlayerLevel.Advanced)
            {
                timeToPassLv = 220;
            }
            else if (gameData.data_of_players[i].playerLevel == SpacePlayerLevel.Expert)
            {
                timeToPassLv = 180;
            }
            else if (gameData.data_of_players[i].playerLevel == SpacePlayerLevel.Master)
            {
                timeToPassLv = 150;
            }

            // xem hiện tại đã quá thời gian trên bao nhiêu lần
            int oldScore = gameData.data_of_players[i].score;
            DateTime timUp = UnbiasedTime.TrueDateTime;
            DateTime.TryParse(gameData.data_of_players[i].timeUpdateScore, out timUp);
            double thoiGianDaTroiQua = UnbiasedTime.TrueDateTime.Subtract(timUp).TotalSeconds;
            // điểm hiện tại là bằng số trên 
            gameData.data_of_players[i].score = Mathf.Min(targetScore, (int)(thoiGianDaTroiQua / timeToPassLv));

            if (gameData.data_of_players[i].score >= targetScore)
            {
                if (hasPlayerWin)
                {
                    gameData.data_of_players[i].score = Mathf.Max(oldScore, gameData.data_of_players[i].score - UnityEngine.Random.Range(1, 4));
                }
                else
                {
                    hasPlayerWin = true;
                    indexWin = i;
                }
            }
        }

        return indexWin; // trả về giá trị của ng chơi chiến thắng
    }

    [Serializable]
    public class Data
    {
        public bool startedSpace = false; // đã bắt đầu event chưa
        public int missionIndex = 0; // đã bắt đầu event chưa
        public string timeEndEvent = "";

        public PlayerDataSpace[] data_of_players = new PlayerDataSpace[]
        {
            new PlayerDataSpace(SpacePlayerLevel.Beginner, "", SpacePlayerType.Bot, 0, 0, ""),
            new PlayerDataSpace(SpacePlayerLevel.Beginner, "", SpacePlayerType.Bot, 0, 0, ""),
            new PlayerDataSpace(SpacePlayerLevel.Beginner, "", SpacePlayerType.Bot, 0, 0, ""),
            new PlayerDataSpace(SpacePlayerLevel.Beginner, "", SpacePlayerType.Bot, 0, 0, ""),
            new PlayerDataSpace(SpacePlayerLevel.Beginner, "", SpacePlayerType.Bot, 0, 0, ""),
        };
    }
}

[Serializable]
public class PlayerDataSpace
{
    public SpacePlayerType spacePlayerType;
    public string timeUpdateScore;
    public int score = 0;
    public SpacePlayerLevel playerLevel; // trình độ của thằng này chơi có khá ko
    public int idAvatar = 0;
    public int idFrameAvatar = 0;
    public string userName;

    public PlayerDataSpace()
    {

    }

    public PlayerDataSpace(SpacePlayerLevel _playerLevel, string _timeUpdateScore, SpacePlayerType _spacePlayerType, int _idAvatar, int _idFrame, string _userName)
    {
        playerLevel = _playerLevel;
        timeUpdateScore = _timeUpdateScore;
        score = 0;
        spacePlayerType = _spacePlayerType;
        idAvatar = _idAvatar;
        idFrameAvatar = _idFrame;
        userName = _userName;
    }
}

[Serializable]
public enum SpacePlayerLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert,
    Master
}

[Serializable]
public enum SpacePlayerType
{
    Player,
    Bot
}
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using GooglePlayGames.BasicApi;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.SocialPlatforms;
//
// [Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/LeaderBoard Fragment", fileName = "LeaderBoard Fragment")]
// public class LeaderBoardDataFragment : DataFragment
// {
//     public static LeaderBoardDataFragment cur;
//
//     public Data gameData;
//
//     public TimeSpan offsetPreEnd = new TimeSpan(0, 5, 0);
//     public TimeSpan offsetPostEnd = new TimeSpan(0, 5, 0);
//
//     public bool IsDataLoaded { get; set; }
//     public bool isActive;
//
//     public const float PromotionPercent = .3f;
//     public const int MaxRank = 10000;
//
// #if UNITY_EDITOR
//     private void OnEnable()
//     {
//         cur = this;
//     }
// #else
//     private void Awake()
//     {
//         // Debug.Log("LEVE DATA FRAGMENT");
//         cur = this;
//     }
// #endif
//
//     public override void Load()
//     {
//         if (!UnbiasedTime.IsValidTime || !isActive) return;
//         if (!LoadData(ref gameData, key)) ResetData();
//         gameData.baseOpenTime = DateTime.FromBinary(gameData.baseOpenTimeLong);
//
//         IsDataLoaded = true;
//
//         CheckNewWeek();
//
//         Save();
//     }
//
//     public override void Save()
//     {
//         if (!UnbiasedTime.IsValidTime || !isActive) return;
//         gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
//         SaveData(gameData, key);
//     }
//
//     public override void ResetData()
//     {
//         if (!UnbiasedTime.IsValidTime || !isActive) return;
//         gameData = new Data();
//         gameData.baseOpenTime = GetPreEndTime().AddDays(-7);
//         gameData.baseOpenTimeLong = gameData.baseOpenTime.ToBinary();
//
//         CheckNewWeek();
//
//         gameData.isPendingReward = false;
//     }
//
//     public override void Update()
//     {
//         gameData.isEverPostScore = false;
//     }
//
//     public bool CheckNewWeek()
//     {
//         if (!UnbiasedTime.IsValidTime || !isActive) return false; //to fix bug :)))
//         TimeSpan diff = UnbiasedTime.ForceTrueDateTime.Subtract(gameData.baseOpenTime);
//
//         bool isNewWeek = false;
//         while (diff.TotalSeconds >= Variables.TIME_SPACE_WEEK)
//         {
//             isNewWeek = true;
//             gameData.baseOpenTime = gameData.baseOpenTime.AddDays(Math.Floor(diff.TotalDays / 7) * 7);
//             diff = UnbiasedTime.ForceTrueDateTime.Subtract(cur.gameData.baseOpenTime);
//         }
//
//         if (isNewWeek)
//         {
//             // Debug.Log("New Week");
//
//             gameData.isPendingReward = true;
//             gameData.pendingScore = 0;
//             gameData.recordedScore = 0;
//
//             return true;
//         }
//
//         return false;
//     }
//
//     public DateTime GetNextResetTime() // base on GPGS weekly reset time UTC-7 , if not pass reset time + 7 then still the same reset time
//     {
//         TimeSpan defferTime = DateTime.Now - DateTime.UtcNow;
//         var cur = UnbiasedTime.ForceTrueDateTime.AddHours(-7).Date;
//         cur = cur.DayOfWeek switch
//         {
//             DayOfWeek.Monday => cur.AddDays(6),
//             DayOfWeek.Tuesday => cur.AddDays(5),
//             DayOfWeek.Wednesday => cur.AddDays(4),
//             DayOfWeek.Thursday => cur.AddDays(3),
//             DayOfWeek.Friday => cur.AddDays(2),
//             DayOfWeek.Saturday => cur.AddDays(1),
//             DayOfWeek.Sunday => cur.AddDays(7),
//             _ => cur
//         };
//
//         if (cur.DayOfWeek == DayOfWeek.Sunday && cur.Hour < offsetPostEnd.TotalHours) cur = cur.AddDays(-7);
//
//         return cur.AddDays(7).Add(defferTime);
//     }
//
//     public DateTime GetPreEndTime()
//     {
//         return GetNextResetTime().Add(offsetPreEnd);
//     }
//
//     public DateTime GetPostEndTime()
//     {
//         return GetNextResetTime().Add(offsetPostEnd);
//     }
//
//     public string GetLeaderBoardId()
//     {
//         // return gameData.leagueIndex switch
//         // {
//         //     0 => GPGSIds.leaderboard_lb_1,
//         //     1 => GPGSIds.leaderboard_lb_2,
//         //     2 => GPGSIds.leaderboard_lb_3,
//         //     3 => GPGSIds.leaderboard_lb_4,
//         //     4 => GPGSIds.leaderboard_lb_5,
//         //     _ => GPGSIds.leaderboard_test_lb
//         // };
//
//         return GPGSIds.leaderboard_lb_1;
//     }
//
//     public void AddScore(int score)
//     {
//         Debug.Log("ADD SCORE " + IsLeaderboardTime(true));
//         if (!IsLeaderboardTime(true) || !isActive) return;
//         if (!GPGSCore.cur.IsAuthenticated)
//         {
//             gameData.pendingScore += score;
//             return;
//         }
//
//         if (!IsDataLoaded) Load();
//         CheckNewWeek();
//         GPGSCore.cur.PostScore(gameData.recordedScore + gameData.pendingScore + score, GetLeaderBoardId(), (isSuccess) =>
//         {
//             if (!isSuccess) gameData.pendingScore += score; //call only once every add -> not every fail -> no worry
//             else gameData.recordedScore += score;
//             if (CanvasLeaderBoard.cur != null) CanvasLeaderBoard.cur.IsNeedUpdate = true;
//
//             Save();
//         });
//     }
//
//     public bool IsLeaderboardTime(bool isSoftOffset = false)
//     {
//         var toPreTime = UnbiasedTime.ForceTrueDateTime.Subtract(GetPreEndTime());
//         if (toPreTime.TotalMilliseconds < 0) return true;
//         // var toPostTime = UnbiasedTime.TrueDateTime.Subtract(GetPostEndTime().Subtract(TimeSpan.FromHours(isSoftOffset ? 1 : 0)));
//         // if (toPostTime.TotalMilliseconds > 0) return true;
//         return false;
//     }
//
//     // public void SetRecord(LeaderboardScoreData leaderboardScoreData, IScore playerScore)
//     // {
//     //     gameData.recordedScore = Mathf.RoundToInt(playerScore.value);
//     //     gameData.recordedRank = playerScore.rank;
//     //     gameData.recordedNumOfPlayers = Mathf.RoundToInt(leaderboardScoreData.ApproximateCount);
//     //
//     //     Save();
//     // }
//
//     public LeaderBoardResume HandleResult(int rank)
//     {
//         return rank switch
//         {
//             1 => LeaderBoardResume.DiamondStay,
//             <= 10 => LeaderBoardResume.PlatinumStay,
//             <= 100 => LeaderBoardResume.GoldStay,
//             <= 5000 => LeaderBoardResume.SilverStay,
//             _ => LeaderBoardResume.CopperStay,
//         };
//     }
//
//     public int GetCupIndex(LeaderBoardResume resume)
//     {
//         return resume switch
//         {
//             LeaderBoardResume.CopperStay => 0,
//             LeaderBoardResume.SilverStay => 1,
//             LeaderBoardResume.GoldStay => 2,
//             LeaderBoardResume.PlatinumStay => 3,
//             LeaderBoardResume.DiamondStay => 4,
//             _ => 0,
//         };
//     }
//
//     public int GetCupIndex(int rank)
//     {
//         return rank switch
//         {
//             1 => 4,
//             <= 10 => 3,
//             <= 100 => 2,
//             <= 5000 => 1,
//             _ => 0,
//         };
//     }
//
//     [Serializable]
//     public class Data
//     {
//         public DateTime baseOpenTime;
//         public long baseOpenTimeLong;
//
//         public int leagueIndex;
//
//         public int recordedScore; // get fro leaderboard online
//         public int pendingScore;
//         public bool isEverPostScore;
//         public int recordedRank = MaxRank; // get from the leaderboard online , use to calculate post end weekly leadrboard position
//         public int recordedNumOfPlayers = MaxRank;
//         public int lastRank;
//         public int lastNumOfPlayer;
//
//         public bool isPendingReward;
//     }
// }
//
// public enum LeaderBoardResume
// {
//     CopperStay,
//     CopperUp,
//     SilverDown,
//     SilverStay,
//     SilverUp,
//     GoldSown,
//     GoldStay,
//     GoldUp,
//     PlatinumDown,
//     PlatinumStay,
//     PlatinumUp,
//     DiamondDown,
//     DiamondStay
// }
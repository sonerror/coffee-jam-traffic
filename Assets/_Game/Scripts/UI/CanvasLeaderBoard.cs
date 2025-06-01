// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using GooglePlayGames.BasicApi;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.SocialPlatforms.Impl;
// using UnityEngine.UI;
//
// public class CanvasLeaderBoard : UICanvasPrime
// {
//     public static CanvasLeaderBoard cur;
//     public bool IsNeedUpdate { get; set; } = true;
//     private float updateTimePoint;
//
//     public GameObject comingSoonTextObject;
//
//     public RectTransform canvasRect;
//     public RectTransform mainPanelRect;
//
//     private float initPos;
//     private float tarPos;
//     private Tween panelTween;
//
//     public GameObject mainPanelObject;
//     private bool isMainPanelOn = true;
//
//     public LeaderBoardItem[] topThreeScoreItems;
//     public LeaderBoardItem[] middleScoreItems;
//
//     //loading score
//     private Coroutine loadingCor;
//     private bool isLoadSuccess;
//     private bool isDoneLoading;
//     private bool isUpdating;
//     [SerializeField] private float loadingWaitTime = 10;
//
//     public GameObject loadingObject;
//     public GameObject retryObject;
//     public GameObject scorePanelObject;
//
//
//     public RectTransform scorePanelRect;
//     [SerializeField] private AnimationCurve scorePanelCurve;
//
//     public ScrollRect middleScrollRect;
//     public RectTransform middleRect;
//
//     [SerializeField] private float heightPerBlock;
//
//     public TMP_Text timeText;
//
//     private bool flagScoreEnable;
//     public GameObject normalScoreObject; //inside score panel
//     public GameObject scoreEndedObject; //inside score panel
//
//     public string[] failedStrings;
//     public TMP_Text failedText;
//
//     private string exception = "hihi";
//
//     public GameObject goToGPGObject;
//     public GameObject goToGPGProfileButtonObject;
//
//     private int loadFailPlayerNotShareNum;
//
//     private void Awake()
//     {
//         cur = this;
//         initPos = -canvasRect.sizeDelta.x;
//         isDoneLoading = true;
//     }
//
//     private void Start()
//     {
//         flagScoreEnable = true;
//         normalScoreObject.SetActive(flagScoreEnable);
//         scoreEndedObject.SetActive(!flagScoreEnable);
//     }
//
//     protected override void OnOpenCanvas()
//     {
//         base.OnOpenCanvas();
//
//         isDoneLoading = true;
//         prizePanelObject.SetActive(false);
//         detailObject.SetActive(false);
//     }
//
//     public void MakeTransit(bool isShow, bool isInstant)
//     {
//         panelTween?.Kill();
//
//         var pos = isShow ? tarPos : initPos;
//
//         if (isInstant)
//         {
//             mainPanelRect.SetPanelHorizontalOffsetFromAnchor(pos);
//         }
//         else
//         {
//             var p = -mainPanelRect.offsetMin.x;
//             // panelTween = DOVirtual.Float(0, 1, CanvasMainMenu.SWITCH_TIME, (fuck) => mainPanelRect.SetPanelHorizontalOffsetFromAnchor(Mathf.Lerp(p, pos, fuck))).SetEase(Ease.OutSine);
//         }
//     }
//
//     public void Tween(float per) //0 is hide 1 is show
//     {
//         mainPanelRect.SetPanelHorizontalOffsetFromAnchor(Mathf.Lerp(initPos, tarPos, per));
//         EnableMainPanel(per > .000001f);
//     }
//
//     public void TrueOpen()
//     {
//         // Debug.Log("LDB OPEN");
//         prizePanelObject.SetActive(false);
//         detailObject.SetActive(false);
//
//         if (!LeaderBoardDataFragment.cur.isActive)
//         {
//             comingSoonTextObject.SetActive(true);
//
//             loadingObject.SetActive(false);
//             retryObject.SetActive(false);
//             scorePanelObject.SetActive(false);
//
//             return;
//         }
//
//         if ((IsNeedUpdate /* || (Time.time - updateTimePoint) > 60*/) && isDoneLoading)
//         {
//             updateTimePoint = Time.time;
//             IsNeedUpdate = false;
//             UpdateScores();
//             // Debug.Log("LDB UPDATE SCORE");
//         }
//
//         StartCheckTime();
//     }
//
//     public void TrueClose()
//     {
//         // Debug.Log("LDB CLOSE");
//         // StopAllCoroutines();
//     }
//
//     private void EnableMainPanel(bool isOn)
//     {
//         // if (isMainPanelOn == isOn) return;
//         // isMainPanelOn = isOn;
//         // mainPanelObject.SetActive(isOn);
//         // if (isOn) LeaderboardPlayerNoff.cur.Pop();
//     }
//
//     public void OnClickReload()
//     {
//         if (!IsNeedUpdate || !isDoneLoading) return;
//         updateTimePoint = Time.time;
//         IsNeedUpdate = false;
//         // Debug.Log("LDB RELOAD");
//
//         UpdateScores();
//     }
//
//     private void UpdateScores()
//     {
//         //start loading screen
//         SetupLoading(1);
//         ClearAll();
//
//         isLoadSuccess = false;
//         isDoneLoading = false;
//         loadingCor = StartCoroutine(ie_LoadScore());
//         StartCoroutine(ie_Cancel());
//
//         FirebaseManager.Ins.ldb_req();
//         return;
//
//         IEnumerator ie_Cancel()
//         {
//             var timePoint = Time.time + loadingWaitTime;
//             yield return Yielders.Get(1);
//             yield return new WaitUntil(() => timePoint < Time.time || isDoneLoading);
//             if (!isLoadSuccess)
//             {
//                 if (loadingCor != null) StopCoroutine(loadingCor);
//                 // show reload button
//                 SetupLoading(2);
//                 IsNeedUpdate = true;
//                 isDoneLoading = true;
//
//                 FirebaseManager.Ins.ldb_fail(exception);
//             }
//         }
//
//         IEnumerator ie_LoadScore()
//         {
//             failedText.text = failedStrings[1];
//             exception = "out of time";
//             goToGPGObject.SetActive(false);
//             goToGPGProfileButtonObject.SetActive(false);
//
//             //get true time
//             if (!UnbiasedTime.IsValidTime) yield return UnbiasedTime.cur.Check();
//             if (!UnbiasedTime.IsValidTime)
//             {
//                 failedText.text = failedStrings[0];
//                 exception = "cant load time";
//                 isDoneLoading = true;
//                 yield break;
//             }
//
//             Debug.Log("LDB TIME OKAY");
//
//             if (!GPGSCore.cur.IsAuthenticated)
//             {
//                 GPGSCore.cur.CallManualAuthenticate();
//                 yield return new WaitUntil(() => GPGSCore.cur.IsDoneCallManual);
//                 if (!GPGSCore.cur.IsAuthenticated)
//                 {
//                     failedText.text = failedStrings[3];
//                     exception = "not authenticated";
//                     isDoneLoading = true;
//                     goToGPGObject.SetActive(true);
//                     yield break;
//                 }
//
//                 // Debug.Log("LDB AUTH");
//             }
//
//             if (!LeaderBoardDataFragment.cur.IsDataLoaded) LeaderBoardDataFragment.cur.Load();
//             Debug.Log("LDB DATA OKAY");
//
//             yield return new WaitUntil(() => GPGSCore.cur.IsDonePostScore && GPGSCore.cur.IsLeaderboardLoaded);
//
//             //Post remaining score or post new score when season end to prevent error due to not having score yet on new season
//             if (LeaderBoardDataFragment.cur.gameData.pendingScore > 0 || !LeaderBoardDataFragment.cur.gameData.isEverPostScore || LeaderBoardDataFragment.cur.gameData.isPendingReward)
//             {
//                 var isPostPendingSuccess = false;
//                 GPGSCore.cur.PostScore(LeaderBoardDataFragment.cur.gameData.recordedScore + LeaderBoardDataFragment.cur.gameData.pendingScore, LeaderBoardDataFragment.cur.GetLeaderBoardId(), (isSuccess) =>
//                 {
//                     isPostPendingSuccess = isSuccess;
//                     if (isSuccess)
//                     {
//                         LeaderBoardDataFragment.cur.gameData.pendingScore = 0;
//                         LeaderBoardDataFragment.cur.gameData.isEverPostScore = true;
//                     }
//                 });
//
//                 yield return new WaitUntil(() => GPGSCore.cur.IsDonePostScore);
//                 if (!isPostPendingSuccess)
//                 {
//                     Debug.Log("LDB CANCEL POST PENDING SCORE");
//                     exception = "cant post remaining score";
//                     isDoneLoading = true;
//                     yield break;
//                 }
//
//                 Debug.Log("LDB POST SCORE");
//             }
//
//             if (LeaderBoardDataFragment.cur.gameData.isPendingReward)
//             {
//                 // var isPendingLoadSuccess = false;
//                 // GPGSCore.cur.GetScoreData(LeaderBoardDataFragment.cur.GetLeaderBoardId(), LeaderboardStart.TopScores, LeaderboardTimeSpan.Weekly, 3, (isSuccess) => isPendingLoadSuccess = isSuccess);
//                 // yield return new WaitUntil(() => GPGSCore.cur.IsLeaderboardLoaded);
//                 // if (!isPendingLoadSuccess)
//                 // {
//                 //     isDoneLoading = true;
//                 //     exception = "cant rw load score";
//                 //     Debug.Log("LDB CANCEL RW LOAD");
//                 //     yield break;
//                 // }
//                 //
//                 // var ldb = GPGSCore.cur.currentLeaderboardScoreData;
//                 //
//                 // Debug.Log("LDB PENDING RW");
//                 //
//                 // if (ldb.PlayerScore == null)
//                 // {
//                 //     failedText.text = failedStrings[1];
//                 //     Debug.Log("LDB CANCEL RW PLAYER NULL");
//                 //     exception = "rw player null";
//                 //     isDoneLoading = true;
//                 //     yield break;
//                 // }
//                 //
//                 // Debug.Log("LDB PENDING RW 1");
//                 //
//                 // if (ldb.PlayerScore.rank < 0)
//                 // {
//                 //     failedText.text = failedStrings[2];
//                 //     Debug.Log("LDB CANCEL RW PLAYER NOT SHARE " + ldb.PlayerScore.rank);
//                 //     exception = "rw player not share";
//                 //     goToGPGProfileButtonObject.SetActive(true);
//                 //     isDoneLoading = true;
//                 //     yield break;
//                 // }
//                 //
//                 // Debug.Log("LDB PENDING RW 2");
//
//                 var data = LeaderBoardDataFragment.cur.gameData;
//                 data.isPendingReward = false;
//
//                 var rank = data.recordedRank;
//                 var lastNum = data.recordedNumOfPlayers;
//
//                 var resume = LeaderBoardDataFragment.cur.HandleResult(rank);
//                 PopPrize(resume, rank);
//
//                 Debug.Log("LDB POP PRIZE OKAY " + rank + "  \n last num: " + lastNum + " \n ldb id: " + LeaderBoardDataFragment.cur.GetLeaderBoardId());
//                 // var isPostInitScoreSuccess = false;
//                 // GPGSCore.cur.PostScore(0, LeaderBoardDataFragment.cur.GetLeaderBoardId(), (isSuccess) => isPostInitScoreSuccess = isSuccess);
//                 // yield return new WaitUntil(() => GPGSCore.cur.IsDonePostScore);
//                 // if (!isPostInitScoreSuccess)
//                 // {
//                 //     Debug.Log("LDB CANCEL POST INIT SCORE");
//                 //     exception = "rw cant post init";
//                 //     isDoneLoading = true;
//                 //     LeaderBoardDataFragment.cur.gameData.isEverPostScore = false;
//                 //     yield break;
//                 // }
//             }
//
//             //start with top score data
//             var isGetTopScoreSuccess = false;
//             GPGSCore.cur.GetScoreData(LeaderBoardDataFragment.cur.GetLeaderBoardId(), LeaderboardStart.TopScores, LeaderboardTimeSpan.Weekly, 3, (isSuccess) => isGetTopScoreSuccess = isSuccess);
//             yield return new WaitUntil(() => GPGSCore.cur.IsLeaderboardLoaded);
//             if (!isGetTopScoreSuccess)
//             {
//                 Debug.Log("LDB CANCEL TOP");
//                 exception = "cant load top score";
//                 isDoneLoading = true;
//                 yield break;
//             }
//
//             var leaderboard = GPGSCore.cur.currentLeaderboardScoreData;
//             Debug.Log("LDB GET SCORE OKAY " + (leaderboard.Scores != null) + "   " + (leaderboard.PlayerScore != null));
//             var top = leaderboard.Scores;
//             var player = leaderboard.PlayerScore;
//
//             if (player != null) Debug.Log("LDB PLAYER " + player.rank + "    " + player.value);
//
//             exception = "pre load top profile";
//             GPGSCore.cur.LoadUsers(top);
//             yield return new WaitUntil(() => GPGSCore.cur.isProfileLoaded);
//             var topProfile = GPGSCore.cur.currentUserProfiles;
//             Debug.Log("LDB TOP PROFILE " + topProfile.Length);
//
//             // the continue with the rest            
//             var isMiddleScoreSuccess = false;
//             GPGSCore.cur.GetScoreData(LeaderBoardDataFragment.cur.GetLeaderBoardId(), LeaderboardStart.PlayerCentered, LeaderboardTimeSpan.Weekly, 20, (isSuccess) => isMiddleScoreSuccess = isSuccess);
//             yield return new WaitUntil(() => GPGSCore.cur.IsLeaderboardLoaded);
//             if (!isMiddleScoreSuccess)
//             {
//                 Debug.Log("LDB CANCEL MIDDLE");
//                 exception = "cant load middle score";
//                 isDoneLoading = true;
//                 yield break;
//             }
//
//             leaderboard = GPGSCore.cur.currentLeaderboardScoreData;
//             player ??= leaderboard.PlayerScore;
//             if (player == null)
//             {
//                 failedText.text = failedStrings[1];
//                 Debug.Log("LDB CANCEL PLAYER NULL");
//                 exception = "player null";
//                 isDoneLoading = true;
//                 yield break;
//             }
//
//             var playerRank = player.rank;
//
//             if (playerRank < 0)
//             {
//                 loadFailPlayerNotShareNum++;
//                 Debug.Log("LDB CANCEL PLAYER NOT SHARE");
//                 exception = "player not share";
//                 if (loadFailPlayerNotShareNum < 3)
//                 {
//                     failedText.text = failedStrings[2];
//                     goToGPGProfileButtonObject.SetActive(true);
//                 }
//                 else
//                 {
//                     failedText.text = failedStrings[1];
//                 }
//
//                 isDoneLoading = true;
//                 yield break;
//             }
//
//             var isPlayerOnTop = playerRank < 4;
//             var playerId = player.userID;
//             var score = leaderboard.Scores;
//             Debug.Log("LDB GET MORE SCORE OKAY OKAY " + player.rank + "  " + player.value);
//
//             exception = "pre load middle profile";
//             GPGSCore.cur.LoadUsers(score);
//             Debug.Log("LDB MIDDLE PROFILE");
//             yield return new WaitUntil(() => GPGSCore.cur.isProfileLoaded);
//             Debug.Log("LDB SUB MIDDLE PROFILE");
//             var middleProfiles = GPGSCore.cur.currentUserProfiles;
//
//             Debug.Log("LDB SCORE DATA OKAY ");
//
//             isLoadSuccess = true;
//             isDoneLoading = true;
//
//             // pop complete board
//             SetupLoading(0);
//             // Debug.Log("LDB SETUP OKAY ");
//
//             // Debug.Log("LDB SET RECORD " + LeaderBoardDataFragment.cur.gameData.lastRank + "  " + LeaderBoardDataFragment.cur.gameData.lastNumOfPlayer + "  " + LeaderBoardDataFragment.cur.gameData.recordedRank + "  " +
//             //           LeaderBoardDataFragment.cur.gameData.recordedNumOfPlayers);
//             LeaderBoardDataFragment.cur.SetRecord(leaderboard, player);
//             // Debug.Log("LDB SET RECORD " + LeaderBoardDataFragment.cur.gameData.lastRank + "  " + LeaderBoardDataFragment.cur.gameData.lastNumOfPlayer + "  " + LeaderBoardDataFragment.cur.gameData.recordedRank + "  " +
//             //           LeaderBoardDataFragment.cur.gameData.recordedNumOfPlayers);
//
//             var playerIndex = -1;
//
//             // Debug.Log("LDB PRE FINAL " + top.Length + "   " + score.Length + "  ");
//
//             for (int i = 0; i < top.Length; i++)
//             {
//                 // Debug.Log("LDB POST TOP SCORE " + top[i].rank + "   " + top[i].value + "  " + playerRank);
//                 topThreeScoreItems[i].SetupOnTop(top[i], GPGSCore.cur.GetUserProfile(top[i], topProfile), top[i].userID.Equals(playerId));
//             }
//
//             int curItemIndex = 0;
//             for (int i = 0; i < score.Length; i++)
//             {
//                 var current = score[i];
//                 // Debug.Log("LDB POST SCORE " + current.rank + "   " + current.value + "  " + playerRank);
//                 if (current.rank < 4) continue;
//                 var isPlayer = current.userID.Equals(playerId);
//                 middleScoreItems[curItemIndex].SetupOnMiddle(current, GPGSCore.cur.GetUserProfile(score[i], middleProfiles), isPlayer);
//                 curItemIndex++;
//
//                 if (isPlayer)
//                 {
//                     playerIndex = i;
//                 }
//
//                 if (curItemIndex >= middleScoreItems.Length) break;
//             }
//
//             if (curItemIndex > 0) middleScoreItems[curItemIndex - 1].lineObject.SetActive(false);
//
//             // leaderboardTopLeague.SetLeague(LeaderBoardDataFragment.cur.gameData.leagueIndex);
//             SetMiddleRectSizeAndPos(isPlayerOnTop, playerIndex, curItemIndex);
//         }
//     }
//
//     private void ClearAll()
//     {
//         for (int i = 0; i < 3; i++) topThreeScoreItems[i].ClearWhenTop();
//         for (int i = 0; i < middleScoreItems.Length; i++) middleScoreItems[i].ClearWhenMiddle();
//     }
//
//     private void SetupLoading(int flag)
//     {
//         loadingObject.SetActive(flag == 1);
//         retryObject.SetActive(flag == 2);
//         scorePanelObject.SetActive(flag == 0);
//         if (flag == 0)
//         {
//             scorePanelRect.localScale = Vector3.zero;
//             scorePanelRect.DOScale(Vector3.one, .28f).SetEase(scorePanelCurve);
//             AudioManager.ins.PlaySound(SoundType.UIClick);
//         }
//     }
//
//     private void SetMiddleRectSizeAndPos(bool isPlayerOnTop, int playerIndex, int middleCount)
//     {
//         var size = middleRect.sizeDelta;
//         size.y = middleCount * heightPerBlock;
//         middleRect.sizeDelta = size;
//
//         Timer.ScheduleFrame(() =>
//         {
//             if (isPlayerOnTop)
//             {
//                 middleScrollRect.verticalNormalizedPosition = 1;
//             }
//             else
//             {
//                 var refIndex = Mathf.Max(playerIndex - 4, 0);
//                 var refMaxCount = middleCount - 8;
//                 middleScrollRect.verticalNormalizedPosition = playerIndex < 8 ? 1 : 1 - Mathf.Clamp01((float)refIndex / refMaxCount);
//
//                 Debug.Log("LDB RECT " + refIndex + "   " + refMaxCount + "   " + Mathf.Clamp01((float)refIndex / refMaxCount));
//             }
//         });
//     }
//
//     private void StartCheckTime()
//     {
//         StartCoroutine(ie_Check());
//         return;
//
//         IEnumerator ie_Check()
//         {
//             while (true)
//             {
//                 if (isLoadSuccess) CheckTime();
//                 yield return Yielders.Get(1);
//             }
//         }
//     }
//
//     private void CheckTime()
//     {
//         var timeLeftToPre = LeaderBoardDataFragment.cur.GetPreEndTime().Subtract(UnbiasedTime.ForceTrueDateTime);
//         // var timeLeftToPost = LeaderBoardDataFragment.cur.GetPostEndTime().Subtract(UnbiasedTime.TrueDateTime);
//
//         var totalPreMilliSec = timeLeftToPre.TotalMilliseconds;
//
//         if (totalPreMilliSec > 0) // isNotEnd
//         {
//             SetScorePanel(true);
//
//             timeText.text = timeLeftToPre.ToTimeFormat_D_H_M_S_Dynamic_Text();
//         }
//         else // is pending new week
//         {
//             SetScorePanel(false);
//
//             // timeText.text = timeLeftToPost.ToTimeFormatWeeklyQuest();
//             timeText.text = "WAITING <3";
//         }
//
//         if (LeaderBoardDataFragment.cur.CheckNewWeek() && isDoneLoading)
//         {
//             UpdateScores();
//         }
//     }
//
//     private void SetScorePanel(bool isEnableScore)
//     {
//         if (flagScoreEnable == isEnableScore) return;
//         flagScoreEnable = isEnableScore;
//         normalScoreObject.SetActive(isEnableScore);
//         scoreEndedObject.SetActive(!isEnableScore);
//
//         if (!isEnableScore)
//         {
//             for (int i = 0; i < 3; i++) topThreeScoreItems[i].ClearWhenTop();
//         }
//     }
//
//     /// ////////////////////////////////////////////////    PRIZE   ///////////
//     private LeaderBoardResume currentLDBResume;
//
//     public GameObject prizePanelObject;
//
//     public CanvasGroup prizeCanvasGroup;
//
//     public GameObject resultObject;
//     public RectTransform resultRect;
//     public GameObject rewardObject;
//     public RectTransform rewardRect;
//
//     [SerializeField] private AnimationCurve prizePopCurve;
//
//     public TMP_Text rankingText;
//     public Image cupImage;
//     [SerializeField] private Sprite[] cupSprites;
//     public Sprite[] CupSprites => cupSprites;
//     public TMP_Text leagueText;
//     [SerializeField] private string[] leagueStrings;
//
//     public GameObject[] mainPrizeObject;
//     public GameObject[] promotionPrizeObject;
//
//     [SerializeField] private Vector2 normalRewardSize;
//     [SerializeField] private Vector2 shrinkRewardSize;
//
//     public GameObject promotionObject;
//
//     public GameObject detailObject;
//
//     public void PopPrize(LeaderBoardResume resume, int rank)
//     {
//         currentLDBResume = resume;
//
//         prizePanelObject.SetActive(true);
//         prizeCanvasGroup.alpha = 0;
//         prizeCanvasGroup.DOFade(1, .24f);
//
//         PopResult(rank);
//     }
//
//     private void PopResult(int rank)
//     {
//         resultObject.SetActive(true);
//         rewardObject.SetActive(false);
//         resultRect.localScale = Vector3.zero;
//         resultRect.DOScale(Vector3.one, .28f).SetEase(prizePopCurve);
//
//         AudioManager.ins.PlaySound(SoundType.Win);
//
//         rankingText.text = Mathf.Min(LeaderBoardDataFragment.MaxRank, rank).ToString();
//         var targetLeagueIndex = LeaderBoardDataFragment.cur.GetCupIndex(currentLDBResume);
//         cupImage.sprite = cupSprites[targetLeagueIndex];
//         leagueText.text = leagueStrings[targetLeagueIndex];
//     }
//
//     private void PopReward()
//     {
//         resultObject.SetActive(false);
//         rewardObject.SetActive(true);
//         rewardRect.localScale = Vector3.zero;
//         rewardRect.DOScale(Vector3.one, .28f).SetEase(prizePopCurve);
//
//         // AudioManager.ins.PlaySound(SoundType.BoosterClaim);
//
//         switch (currentLDBResume)
//         {
//             case LeaderBoardResume.CopperStay:
//                 SetPrizeIndex(0, 0);
//                 break;
//             case LeaderBoardResume.CopperUp:
//                 SetPrizeIndex(1, 1);
//                 break;
//             case LeaderBoardResume.SilverStay:
//                 SetPrizeIndex(1, 0);
//                 break;
//             case LeaderBoardResume.SilverUp:
//                 SetPrizeIndex(2, 2);
//                 break;
//             case LeaderBoardResume.GoldStay:
//                 SetPrizeIndex(2, 0);
//                 break;
//             case LeaderBoardResume.GoldUp:
//                 SetPrizeIndex(3, 3);
//                 break;
//             case LeaderBoardResume.PlatinumStay:
//                 SetPrizeIndex(3, 0);
//                 break;
//             case LeaderBoardResume.PlatinumUp:
//                 SetPrizeIndex(4, 4);
//                 break;
//             case LeaderBoardResume.DiamondStay:
//                 SetPrizeIndex(4, 0);
//                 break;
//         }
//     }
//
//     private void SetPrizeIndex(int mainIndex, int promotionIndex)
//     {
//         for (int i = 0; i < mainPrizeObject.Length; i++)
//         {
//             mainPrizeObject[i].SetActive(i == mainIndex);
//             promotionPrizeObject[i].SetActive(i == promotionIndex);
//         }
//
//         rewardRect.sizeDelta = promotionIndex == 0 ? shrinkRewardSize : normalRewardSize;
//         promotionObject.SetActive(promotionIndex != 0);
//     }
//
//     public void OnClickStartNextLeague()
//     {
//         AudioManager.ins.PlaySound(SoundType.UIClick);
//         resultObject.SetActive(false);
//         PopReward();
//     }
//
//     public void OnClickClaim()
//     {
//         // AudioManager.ins.PlaySound(SoundType.BoosterUnlock);
//         HandlePrize();
//
//         prizeCanvasGroup.DOFade(0, .32f).OnComplete(() => prizePanelObject.SetActive(false));
//     }
//
//     private void HandlePrize()
//     {
//     }
//
//     public void OnClickDetail()
//     {
//         AudioManager.ins.PlaySound(SoundType.UIClick);
//         detailObject.SetActive(true);
//     }
//
//     public void OnClickGotIt()
//     {
//         AudioManager.ins.PlaySound(SoundType.UIClick);
//         detailObject.SetActive(false);
//     }
//
//     public void OnClickGoToGPG()
//     {
//         // Application.OpenURL("market://details?id=com.google.android.play.games");
//         Application.OpenURL("https://play.google.com/games/profile");
//     }
//
//     public void OnClickGoToGPGProfile()
//     {
//         Application.OpenURL("https://play.google.com/games/profile");
//     }
// }
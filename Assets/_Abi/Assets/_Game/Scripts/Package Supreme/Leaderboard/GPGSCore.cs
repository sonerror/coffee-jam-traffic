// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using Unity.Services.Authentication;
// using Unity.Services.Core;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.SocialPlatforms;
//
// public class GPGSCore : MonoBehaviour
// {
//     public static GPGSCore cur;
//
//     public bool isActive;
//
//     // flag ////////////////////
//     public bool IsAuthenticated { get; private set; }
//     public bool IsDoneCallManual { get; private set; }
//
//     public string Token;
//     public string Error;
//
//     public bool IsForceDataFromServer { get; set; }
//
//     public string privacySettingsUrl = "https://myaccount.google.com/privacycheckup/";
//
//     private void Awake()
//     {
//         cur = this;
//         DontDestroyOnLoad(gameObject);
//
//         if (!isActive) return;
//         // Debug.Log("ACTIVE");
//         // PlayGamesPlatform.DebugLogEnabled = true;
//         // PlayGamesPlatform.Activate();
//         // CallAutoAuthenticate();
//
//         // Debug.Log("TESTTTTT " + new TimeSpan(-3, 0, 0));
//     }
//
//     #region Auth
//
//     public void CallAutoAuthenticate()
//     {
//         if (!isActive) return;
//         PlayGamesPlatform.Activate();
//         PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
//         // Debug.LogWarning("IS AUTHENTICATE " + PlayGamesPlatform.Instance.IsAuthenticated());
//         // Timer.ScheduleSupreme(10, () => GetScoreData(GPGSIds.leaderboard_test_lb, LeaderboardStart.PlayerCentered, LeaderboardTimeSpan.AllTime));
//     }
//
//     public void CallManualAuthenticate()
//     {
//         if (!isActive || IsAuthenticated) return;
//         IsDoneCallManual = false;
//         PlayGamesPlatform.Activate();
//         PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
//         // Debug.LogWarning("IS AUTHENTICATE " + PlayGamesPlatform.Instance.IsAuthenticated());
//     }
//
//     internal void ProcessAuthentication(SignInStatus status)
//     {
//         IsDoneCallManual = true;
//         if (status == SignInStatus.Success)
//         {
//             // Continue with Play Games Services
//             IsAuthenticated = true;
//             // Debug.Log("LEADERBOARD SUCCESS");
//
//             // GetScoreData(GPGSIds.leaderboard_test_lb, LeaderboardStart.PlayerCentered, LeaderboardTimeSpan.AllTime);
//         }
//         else if (status == SignInStatus.Canceled)
//         {
//             IsAuthenticated = false;
//
//             // Debug.Log("LEADERBOARD CANCEL");
//         }
//         else if (status == SignInStatus.InternalError)
//         {
//             IsAuthenticated = false;
//
//             // Debug.Log("LEADERBOARD FAIL");
//             // Disable your integration with Play Games Services or show a login button
//             // to ask users to sign-in. Clicking it should call
//             // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
//         }
//     }
//
//     // async void Start()
//     // {
//     //     if (!isActive) return;
//     //     // await UnityServices.InitializeAsync();
//     //     // await LoginGooglePlayGames();
//     //     // await SignInWithGooglePlayGamesAsync(Token);
//     //     // await Authenticate();
//     // }
//
//     //Fetch the Token / Auth code
//     public Task LoginGooglePlayGames()
//     {
//         var tcs = new TaskCompletionSource<object>();
//         Debug.Log("LOGIN");
//         PlayGamesPlatform.Instance.Authenticate((success) =>
//         {
//             if (success == SignInStatus.Success)
//             {
//                 Debug.Log("Login with Google Play games successful.");
//                 PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
//                 {
//                     Debug.Log("Authorization code: " + code);
//                     Token = code;
//                     // This token serves as an example to be used for SignInWithGooglePlayGames
//                     tcs.SetResult(null);
//                 });
//             }
//             else
//             {
//                 Error = "Failed to retrieve Google play games authorization code";
//                 Debug.Log("Login Unsuccessful");
//                 tcs.SetException(new Exception("Failed " + Error));
//             }
//         });
//         return tcs.Task;
//     }
//
//
//     async Task SignInWithGooglePlayGamesAsync(string authCode)
//     {
//         try
//         {
//             await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
//             Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); //Display the Unity Authentication PlayerID
//             Debug.Log("SignIn is successful.");
//         }
//         catch (AuthenticationException ex)
//         {
//             // Compare error code to AuthenticationErrorCodes
//             // Notify the player with the proper error message
//             Debug.LogException(ex);
//         }
//         catch (RequestFailedException ex)
//         {
//             // Compare error code to CommonErrorCodes
//             // Notify the player with the proper error message
//             Debug.LogException(ex);
//         }
//     }
//
//     public async Task Authenticate()
//     {
//         PlayGamesPlatform.Activate();
//         await UnityServices.InitializeAsync();
//
//         PlayGamesPlatform.Instance.Authenticate(success =>
//         {
//             if (success == SignInStatus.Success)
//             {
//                 // Debug.LogWarning("MAGIC LOGIN SUCCESS");
//                 PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => { Debug.Log($"MAGIC AUTH IS {code}"); });
//             }
//             else
//             {
//                 // Debug.LogWarning("MAGIC AUTH FAIL");
//             }
//         });
//     }
//
//     #endregion
//
//     public bool IsLeaderboardLoaded { get; private set; } = true;
//     public LeaderboardScoreData currentLeaderboardScoreData;
//
//     public bool IsDonePostScore { get; private set; } = true;
//
//     public bool isProfileLoaded;
//     public IUserProfile[] currentUserProfiles;
//
//     public void PostScore(int score, string leaderboardId, Action<bool> onSuccess = null)
//     {
//         IsDonePostScore = false;
//         if (!IsAuthenticated) return;
//
//         PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, (isSuccess) =>
//         {
//             onSuccess?.Invoke(isSuccess);
//             IsDonePostScore = true;
//             Debug.Log("SCORE POSTED " + isSuccess);
//         });
//         // PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, (fuck) => onSuccess?.Invoke(fuck));
//     }
//
//     public void GetScoreData(string leaderboardId, LeaderboardStart leaderboardStart, LeaderboardTimeSpan timeSpan, int rowCount, Action<bool> isSuccess = null)
//     {
//         IsLeaderboardLoaded = false;
//         if (!IsAuthenticated) return;
//
//         PlayGamesPlatform.Instance.LoadScores(
//             leaderboardId,
//             leaderboardStart,
//             rowCount,
//             LeaderboardCollection.Public,
//             timeSpan,
//             (data) =>
//             {
//                 var mStatus = "Leaderboard data valid: " + data.Valid;
//                 mStatus += "\n approx:" + data.ApproximateCount + " have " + data.Scores.Length + "  Score " + data.PlayerScore + "   Status " + data.Status;
//                 Debug.Log("LEADERBOARD LOAD " + mStatus);
//
//                 IsLeaderboardLoaded = true;
//                 if (data.Status == ResponseStatus.Success && data.Valid)
//                 {
//                     currentLeaderboardScoreData = data;
//                     Debug.Log("GET DATA " + (currentLeaderboardScoreData != null));
//                     isSuccess?.Invoke(true);
//                 }
//                 else
//                 {
//                     isSuccess?.Invoke(false);
//                 }
//             });
//     }
//
//     public void LoadUsers(IScore[] scores)
//     {
//         isProfileLoaded = false;
//         List<string> userIds = new List<string>();
//         if (scores.Length == 0) return;
//         for (int i = 0; i < scores.Length; i++)
//         {
//             userIds.Add(scores[i].userID);
//         }
//
//         Debug.Log("PRE LOAD PROFILE " + scores.Length + "  " + userIds);
//
//         PlayGamesPlatform.Instance.LoadUsers(userIds.ToArray(), (users) =>
//         {
//             currentUserProfiles = users;
//             isProfileLoaded = true;
//         });
//     }
//
//     public IUserProfile GetUserProfile(IScore score, IUserProfile[] userProfiles)
//     {
//         var id = score.userID;
//         for (int i = 0; i < userProfiles.Length; i++)
//         {
//             if (id == userProfiles[i].id) return userProfiles[i];
//         }
//
//         return null;
//     }
//
//     public void ShowLDB()
//     {
//         PlayGamesPlatform.Instance.ShowLeaderboardUI();
//     }
//
//     public void TestScore()
//     {
//         LeaderBoardDataFragment.cur.AddScore(27);
//     }
//
//     public void ResetScore()
//     {
//     }
//
//     public void TestUrl()
//     {
//         // Application.OpenURL("market://details?id=com.google.android.play.games");
//         Application.OpenURL(privacySettingsUrl);
//     }
//
//     public void ShowSetting()
//     {
//     }
// }
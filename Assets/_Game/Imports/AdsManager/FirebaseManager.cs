using System.Collections;
using Firebase.Analytics;
using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;
using AppsFlyerSDK;
using Firebase.Crashlytics;

/// <summary>
/// Document Link: https://docs.google.com/spreadsheets/d/1PUjPCuHoE5pRhD8Up4vCrQWRktgS_MFFADkgZppNdEw/edit#gid=0
/// </summary>
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Ins = null;

    //Check khởi tạo firebase
    private bool fireBaseReady = false; //Firebase đã Init thành công
    private bool firebaseIniting = true; //Firebase đang Init

    public bool is_remote_config_done = false; //Quá trình RemoteConfig đã xong
    public bool is_remote_config_success = false; //RemoteConfig thành công

    #region FIREBASE SETUP

    void Awake()
    {
        if (Ins != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Ins = this;
        firebaseIniting = true;
    }

    private IEnumerator Start()
    {
        CheckFireBase();
        yield return new WaitUntil(() => !firebaseIniting);
        if (fireBaseReady)
        {
            Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;

            //Khởi tạo remote config
            fetch((bool is_fetch_result) => { });
        }
        else
        {
            Debug.LogError("Ko khởi tạo đc Firebase");
        }
    }

    private void CheckFireBase()
    {
        try
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                firebaseIniting = false;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    fireBaseReady = true;

                    // // Create and hold a reference to your FirebaseApp,
                    // // where app is a Firebase.FirebaseApp property of your application class.
                    // // Crashlytics will use the DefaultInstance, as well;
                    // // this ensures that Crashlytics is initialized.
                    // Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                    //
                    // // When this property is set to true, Crashlytics will report all
                    // // uncaught exceptions as fatal events. This is the recommended behavior.
                    // Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                }
                else
                {
                    Debug.LogError(System.String.Format("Lỗi dependencies của Firebase: {0}", dependencyStatus));
                }
            });
        }
        catch (System.Exception ex)
        {
            firebaseIniting = false;
            Debug.LogError("Lỗi khởi tạo Firebase:" + ex.ToString());
        }
    }

    #endregion

    #region USER_PROPERTIES

    public void OnSetUserProperty()
    {
        StartCoroutine(ie_OnSetUserProperty());
    }

    //Hàm này gọi 2 lần:
    //1. Khi mở game
    //2. Khi win 1 level (đối với game hyper) hoặc win 1 level ở main game content (với các game mid/puzzle)
    IEnumerator ie_OnSetUserProperty()
    {
        yield return new WaitUntil(() => fireBaseReady && DataManager.ins != null && DataManager.ins.IsDoneLoadData);
        try
        {
            //Nếu là bản DevelopmentBuild hoặc UnityEditor thì ko bắn UserProperty lên
            if (!Debug.isDebugBuild && !Application.isEditor)
            {
                CheckDayPlayed();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase: (Userproperties) Error: " + e);
        }
    }

    public void CheckDayPlayed()
    {
        if (DataManager.ins.CheckNewDay(out _))
        {
            DataManager.ins.gameData.dayPlayed++;
            DataManager.ins.SaveData(false);


            if (DataManager.ins.gameData.dayPlayed == 2)
            {
                g_levelplay_1stday((LevelDataFragment.cur.gameData.level + 1).ToString());
            }
        }

        OnSetProperty("day_played", DataManager.ins.gameData.dayPlayed);
    }

    public void CheckRewardWatched()
    {
        DataManager.ins.gameData.rewardWatched++;
        DataManager.ins.SaveData(false);

        OnSetProperty("watch_ads_reward", DataManager.ins.gameData.rewardWatched);
    }

    public void CheckInterWatched()
    {
        DataManager.ins.gameData.interWatched++;
        DataManager.ins.SaveData(false);

        OnSetProperty("watch_ads_inter", DataManager.ins.gameData.interWatched);
    }

    public void CheckPurchasedValue(float value, string currencyCode)
    {
        DataManager.ins.gameData.purchasedValue += value;
        DataManager.ins.SaveData(false);

        OnSetProperty("total_value_purchase", DataManager.ins.gameData.purchasedValue);
        OnSetProperty("currency_code", currencyCode);
    }

    private void OnSetProperty(string key, object value)
    {
        try
        {
            FirebaseAnalytics.SetUserProperty(key.ToString(), value.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("Lỗi UserProperty của Firebase: " + key + " _ " + e);
        }
    }

    #endregion

    #region Events

    //OPTION: không yêu cầu
    //Mỗi 30s chơi game (tổng thời gian chơi) tính là 1 checkpoint.
    //[name_game_cur]: tên game hiện tại đang chơi
    //[name_game_max]: tên game cao nhất đã chơi (nếu là các game có nhiều trò như octopus, nếu không thì 2 thông số sẽ là level đang chơi)
    public void check_point_time(int check_point, string name_game_cur, string name_game_max)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            if (check_point < 10)
            {
                FirebaseAnalytics.LogEvent("check_point_time_0" + check_point, new Parameter[]
                {
                    new Parameter("name_game_cur", name_game_cur),
                    new Parameter("name_game_max", name_game_max)
                });
            }
            else
            {
                FirebaseAnalytics.LogEvent("check_point_time_" + check_point, new Parameter[]
                {
                    new Parameter("name_game_cur", name_game_cur),
                    new Parameter("name_game_max", name_game_max)
                });
            }
        }
    }

    //Call mỗi khi start level
    //[check_point]: mốc level (level 0 -> mốc 1, level 1 -> mốc 2 ...)
    //[name_game_cur]: tên game hiện tại đang chơi
    public void checkpoint_level(string check_point)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("checkpoint_level", new Parameter[]
            {
                new Parameter("level", check_point)
            });
        }
    }

    //Call mỗi khi end level
    //[check_point]: mốc level (level 0 -> mốc 1, level 1 -> mốc 2 ...)
    //[name_game_cur]: tên game hiện tại đang chơi
    public void check_point_end(string check_point)
    {
        // if (!Debug.isDebugBuild && !Application.isEditor)
        // {
        //     FirebaseAnalytics.LogEvent("check_point_end" + check_point, new Parameter[]
        //     {
        //         new Parameter("level", check_point)
        //     });
        // }
    }

    public void g_game_start(string level)
    {
#if UNITY_EDITOR
        Debug.Log("FIREBASE_LOG_LEVEL_START " + level);
#endif
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("g_game_start", new Parameter[]
            {
                new Parameter("level", level)
            });
        }
    }

    public void g_game_end(string level, bool isWin, bool isFirstTry,float duration)
    {
#if UNITY_EDITOR
        Debug.Log("FIREBASE_LOG_LEVEL_COMPLETE " + level);
#endif
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("g_game_end", new Parameter[]
            {
                new Parameter("level", level),
                new Parameter("result", isWin ? "win" : "lose"),
                new Parameter("isFirstTry", isFirstTry ? "yeah" : "no"),
                new Parameter("duration_complete",duration)
            });
        }
    }

    public void g_gameplay_booster(string level, string booster)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("g_gameplay_booster", new Parameter[]
            {
                new Parameter("level", level),
                new Parameter("booster", booster)
            });
        }
    }

    public void g_gameplay_earn_booster(string booster, string source, float count)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("g_gameplay_earn_booster", new Parameter[]
            {
                new Parameter("booster", booster),
                new Parameter("source", source),
                new Parameter("count", count)
            });
        }
    }

    public void earn_virtual_currency(string virtual_currency_name, int value, string source)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("earn_virtual_currency", new Parameter[]
            {
                new Parameter("virtual_currency_name", virtual_currency_name),
                new Parameter("value", value),
                new Parameter("source", source)
            });
        }
    }

    public void spend_virtual_currency(string virtual_currency_name, int value, string spend_source)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("spend_virtual_currency", new Parameter[]
            {
                new Parameter("virtual_currency_name", virtual_currency_name),
                new Parameter("value", value),
                new Parameter("spend_source", spend_source)
            });
        }
    }

    public void level_revive(string level)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("level_revive", new Parameter[]
            {
                new Parameter("level", level)
            });
        }
    }

    public void g_space_mission_start()
    {
#if UNITY_EDITOR
        Debug.Log("g_space_mission_start ");
#endif
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            string stage = "";
            if(SpaceMissionDataFragment.cur.gameData.missionIndex == 0) stage = "moon";
            else if(SpaceMissionDataFragment.cur.gameData.missionIndex == 1) stage = "mars";
            else stage = "saturn";
            FirebaseAnalytics.LogEvent("g_space_mission_start", new Parameter[]
            {
                new Parameter("stage", stage),
                new Parameter("place", "home")
            });
        }
    }
    public void g_space_mission_complete()
    {
#if UNITY_EDITOR
        Debug.Log("g_space_mission_complete ");
#endif
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            string stage = "";
            if(SpaceMissionDataFragment.cur.gameData.missionIndex == 0) stage = "moon";
            else if(SpaceMissionDataFragment.cur.gameData.missionIndex == 1) stage = "mars";
            else stage = "saturn";
            FirebaseAnalytics.LogEvent("g_space_mission_complete", new Parameter[]
            {
                new Parameter("stage", stage)
            });
        }
    }
    public void g_car_race_start()
    {
#if UNITY_EDITOR
        Debug.Log("g_car_race_start ");
#endif
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            string stage = "";
            if(CarRaceDataFragment.cur.gameData.raceIndex == 0) stage = "race_1";
            else if(CarRaceDataFragment.cur.gameData.raceIndex == 1) stage = "race_2";
            else stage = "race_3";
            FirebaseAnalytics.LogEvent("g_car_race_start", new Parameter[]
            {
                new Parameter("stage", stage),
                new Parameter("place", "home")
            });
        }
    }
    public void g_car_race_complete()
    {
#if UNITY_EDITOR
        Debug.Log("g_car_race_complete ");
#endif
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            string stage = "";
            if(CarRaceDataFragment.cur.gameData.raceIndex == 0) stage = "race_1";
            else if(CarRaceDataFragment.cur.gameData.raceIndex == 1) stage = "race_2";
            else stage = "race_3";
            FirebaseAnalytics.LogEvent("g_car_race_complete", new Parameter[]
            {
                new Parameter("stage", stage)
            });
        }
    }

    #region Reward ADS

    //Khi hiện offer reward (hiển thị nút x2 reward ở endgame, hiển thị popup offer skin free ...)
    //[placement]: vị trí hiển thị (endgame, shop ...)
    //[level]: level hiển thị
    public void ads_reward_offer(string placement, int level)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_reward_offer", new Parameter[]
            {
                new Parameter("placement", placement),
                new Parameter("level", level)
            });
        }
    }

    //Khi click vào button reward
    //(điều kiện là đã load được ads -> nên block - làm mờ nút ads reward nếu không load đc ads hoặc ko có mạng)
    //[placement]: vị trí hiển thị (endgame, shop ...)
    //[level]: level hiển thị
    public void ads_reward_click(string placement)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_reward_click", new Parameter[]
            {
                new Parameter("placement", placement)
            });
        }
    }

    //Khi reward được show lên (hiển thị thành công)
    public void ads_reward_show(string placement)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_reward_show", new Parameter[]
            {
                new Parameter("placement", placement)
            });
        }
    }

    //Khi reward bị lỗi khi hiển thị 
    //[placement]: vị trí hiển thị (endgame, shop ...)
    //[errormsg]: tên lỗi: Error Message: Unknown,Offline,NoFill,InternalError,InvalidRequest,UnableToPrecached
    //[level]: level hiển thị
    public void ads_reward_fail(string placement, string errormsg)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_reward_fail", new Parameter[]
            {
                new Parameter("placement", placement),
                new Parameter("errormsg", errormsg)
            });
        }
    }

    //Khi reward có thể nhận thưởng
    //Call khi close reward ads hoặc event có thể nhận thưởng
    public void ads_reward_complete(string placement, string level = "")
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_reward_complete", new Parameter[]
            {
                new Parameter("placement", placement),
                new Parameter("level", level)
            });
        }
    }

    #endregion

    #region Inter ADS

    //Khi inter ads load thành công
    public void ads_inter_load()
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_load", new Parameter[] { });
        }
    }

    public void ads_inter_missing(string placement)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_missing", new Parameter[]
            {
                new Parameter("placemetn", placement)
            });
        }
    }

    //Khi click vào ads inter
    public void ads_inter_click()
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_click", new Parameter[] { });
        }
    }

    //Khi ads inter hiển thị (trong sự kiện ads trả về -> chắc chắn sẽ hiện)
    public void ads_inter_show(string placement = "")
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_show", new Parameter[]
            {
                new Parameter("placement", placement),
                new Parameter("level", LevelDataFragment.cur.GetFireBaseLevel().ToString())
                // new Parameter("level", LevelDataFragment.cur.GetFirebaseLevel().ToString())
            });
        }
    }

    public void ads_inter_show_placement(string placement)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_show_placement", new Parameter[]
            {
                new Parameter("placement", placement),
                new Parameter("level", LevelDataFragment.cur.GetFireBaseLevel().ToString())
            });
        }
    }

    //Khi ads inter hiển thị lỗi
    //Error Message: FailToLoad, Unavailable
    public void ads_inter_fail(string errormsg)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_fail", new Parameter[]
            {
                new Parameter("errormsg", errormsg)
            });
        }
    }

    #endregion

    public void game_treasure_claim(string treasure_name, string reward_ID)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("game_treasure_claim", new Parameter[]
            {
                new Parameter("treasure_name", treasure_name),
                new Parameter("reward_ID", reward_ID),
            });
        }
    }

    public void g_levelplay_1stday(string level_max)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("g_levelplay_1stday", new Parameter[]
            {
                new Parameter("level_max", level_max),
            });
        }
    }

    public void iapPurchased(string pack_ID, string packName, string location)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("iapPurchased", new Parameter[]
            {
                new Parameter("pack_ID", pack_ID),
                new Parameter("packName", packName),
                new Parameter("location", location)
            });
        }
    }

    public void iapPurchased_confirmed(string pack_ID, string packName, string location)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("iapPurchased_confirmed", new Parameter[]
            {
                new Parameter("pack_ID", pack_ID),
                new Parameter("packName", packName),
                new Parameter("location", location)
            });
        }
    }

    public void iapPurchased_ingame(string pack_ID, string packName)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("iapPurchased_confirmed", new Parameter[]
            {
                new Parameter("pack_ID", pack_ID),
                new Parameter("packName", packName),
            });
        }
    }

    public void first_iap(string pack_ID, string place, string level)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("first_iap", new Parameter[]
            {
                new Parameter("pack_ID", pack_ID),
                new Parameter("place", place),
                new Parameter("level", level)
            });
        }
    }

    public void daily_quest_claim(string quest_ID)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("daily_quest_claim", new Parameter[]
            {
                new Parameter("quest_ID", quest_ID)
            });
        }
    }

    public void daily_reward_milestone(string quest_ID)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("daily_reward_milestone", new Parameter[]
            {
                new Parameter("quest_ID", quest_ID)
            });
        }
    }

    public void start_loading()
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("start_loading", new Parameter[]
            {
            });
        }
    }

    public void end_loading(float duration)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("end_loading", new Parameter[]
            {
                new Parameter("duration", duration)
            });
        }
    }

    public void claim_daily_login(int day)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("claim_daily_login", new Parameter[]
            {
                new Parameter("day", day)
            });
        }
    }

    public void challenge_start(string day, string month)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("challenge_start", new Parameter[]
            {
                new Parameter("day", day),
                new Parameter("month", month)
            });
        }
    }

    public void challenge_win(string day, string month)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("challenge_win", new Parameter[]
            {
                new Parameter("day", day),
                new Parameter("month", month)
            });
        }
    }

    public void challenge_lose(string day, string month)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("challenge_lose", new Parameter[]
            {
                new Parameter("day", day),
                new Parameter("month", month)
            });
        }
    }

    public void challenge_win_tut(string day, string month)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("challenge_win_tut", new Parameter[]
            {
                new Parameter("day", day),
                new Parameter("month", month)
            });
        }
    }

    public void challenge_chest(string index)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("challenge_chest", new Parameter[]
            {
                new Parameter("index", index)
            });
        }
    }

    public void parrot_touched(string level)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("parrot_touched", new Parameter[]
            {
                new Parameter("level", level)
            });
        }
    }

    public void speedy(string level)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("speedy", new Parameter[]
            {
                new Parameter("level", level)
            });
        }
    }

    public void ldb_req()
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ldb_req", new Parameter[]
            {
            });
        }
    }

    public void ldb_fail(string cause)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ldb_fail", new Parameter[]
            {
                new Parameter("cause", cause)
            });
        }
    }

    #region ADS RevenuePain

    /// <summary>
    /// Send theo event của Max Manager (Log doanh thu từ mỗi quảng cáo)
    /// </summary>
    /// <param name="data"></param>
    public void ADS_RevenuePain(ImpressionData data)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            Parameter[] AdParameters =
            {
                new Parameter("ad_platform", "applovin"),
                new Parameter("ad_source", data.NetworkName),
                new Parameter("ad_unit_name", data.AdUnitIdentifier),
                new Parameter("currency", "USD"),
                new Parameter("value", data.Revenue),
                new Parameter("placement", data.Placement),
                new Parameter("country_code", data.CountryCode),
                new Parameter("ad_format", data.AdFormat),
            };
            FirebaseAnalytics.LogEvent("ad_revenue_pain", AdParameters);

            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ad_platform", "applovin");
                dic.Add("ad_source", data.NetworkName);
                dic.Add("ad_unit_name", data.AdUnitIdentifier);
                dic.Add("currency", "USD");
                dic.Add("value", data.Revenue.ToString());
                dic.Add("placement", data.Placement);
                dic.Add("country_code", data.CountryCode);
                dic.Add("ad_format", data.AdFormat);
                AppsFlyerAdRevenue.logAdRevenue(data.NetworkName,
                    AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
                    data.Revenue, "USD", dic);
            }
            catch (System.Exception e)
            {
            }
        }
    }

    #endregion

    #endregion

    #region Remote Config

    /// <summary>
    /// Setup Remote config
    /// </summary>
    /// <param name="completionHandler"></param>
    public void fetch(Action<bool> completionHandler)
    {
        try
        {
            var settings = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ConfigSettings;
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(settings);

            var fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(new TimeSpan(0));

            fetchTask.ContinueWith(task =>
            {
                is_remote_config_done = true;
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogWarning("fetchTask Firebase Fail");
                    is_remote_config_success = false;
                    completionHandler(false);
                }
                else
                {
                    Debug.LogWarning("fetchTask Firebase Commplete");
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    RefrectProperties();

                    completionHandler(true);
                }
            });
        }
        catch (Exception ex)
        {
            is_remote_config_done = true;
            Debug.Log(ex.ToString());
        }
    }

    /// <summary>
    /// Dữ liệu remote config
    /// </summary>
    private void RefrectProperties()
    {
        try
        {
            //AOA Setting Params
            AdsManager.Ins.IsAOA = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("IsAOA").BooleanValue;
            AdsManager.Ins.AOA_FirstOpen = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("AOA_FirstOpen").BooleanValue;
            AdsManager.Ins.AOA_SessionStart = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("AOA_SessionStart").BooleanValue;
            AdsManager.Ins.AOA_SwitchApps = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("AOA_SwitchApps").BooleanValue;
            AdsManager.Ins.AOA_SwitchApps_Seconds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("AOA_SwitchApps_Seconds").LongValue;
            AdsManager.Ins.timeWatchAds = (float)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("timeWatchAds").LongValue;
            AdsManager.Ins.timeWatchAdsInter = (float)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("timeWatchAdsInter").LongValue;
            AdsManager.Ins.IsCreativeDebug = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("IsCreativeDebug").BooleanValue;
            AdsManager.Ins.Ads_Level = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("Ads_Level").LongValue;
            AdsManager.Ins.Ads_Capping = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("Ads_Capping").LongValue;
            AdsManager.Ins.Ads_Penalty = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("Ads_Penalty").LongValue;
            AdsManager.Ins.Babysit_Level = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("Babysit_Level").LongValue;
            AdsManager.Ins.TreasureLevel = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("TreasureLevel").LongValue;
            AdsManager.Ins.RatingLevel = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("RatingLevel").LongValue;
            AdsManager.Ins.SwapCarTutLevel = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("SwapCarTutLevel").LongValue;
            AdsManager.Ins.VipBUsTutLevel = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("VipBUsTutLevel").LongValue;
            AdsManager.Ins.SwapMinionTutLEvel = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("SwapMinionTutLEvel").LongValue;
            AdsManager.Ins.WinStreakLevel = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("WinStreakLevel").LongValue;
            LevelDataFragment.IsMultiLayer=Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue("is_multi_layer_level_set").BooleanValue;
            
            is_remote_config_success = true;
        }
        catch (Exception ex)
        {
            Debug.Log("Error RefrectProperties: " + ex.Message);
        }
    }

    #endregion
}
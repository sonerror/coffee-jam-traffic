using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class NotificationManager : SingletonMonoBehaviour<NotificationManager>
{
    #region Contents

    public const string NOTI_12H = "Return to Packing Rush today";
    public const string NOTI_20H = "Take on the challenge and become the ultimate packing puzzle master";

    public const string NOTI_2D = "Don't miss out! Claim it now";
    public const string NOTI_4D = "Think you are one of them? 🤔 Let's give it a try";
    public const string NOTI_7D = "Do you miss them? Come back and take them home ~";

    public const string TITLE_12H = "Coffee orders are ready!";
    public const string TITLE_20H = "It's time to relax";

    public const string TITLE_2D = "🎁 Daily Reward has been delivered";
    public const string TITLE_4D = "Only 10% can complete this level";
    public const string TITLE_7D = "😻 Coffee orders are ready!";

    [SerializeField] public bool isPauseToShowAds = false;

    #endregion

    #region Init

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        NotificationSupport.RegisterChannel();

        AndroidNotificationCenter.CancelAllNotifications();
        isPauseToShowAds = false;
        //SendDayNotification();
    }

    #endregion

    public void SendShortNoti(int time)
    {
#if UNITY_ANDROID
        Debug.Log("Show notification");
        //if (_shortNotiId != -1) NotificationSupport.CancelScheduleNotification(_shortNotiId);
        //_shortNotiId = NotificationSupport.CreateNotifycation(stringNotis[2], time);
#endif
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SendQuitApplicationNotification();
        }
        else
        {
            AndroidNotificationCenter.CancelAllNotifications();
            isPauseToShowAds = false;
        }
    }

    public void OnApplicationQuit()
    {
        isPauseToShowAds = false;
        SendQuitApplicationNotification();
    }

    public void SendQuitApplicationNotification()
    {
        if (isPauseToShowAds) return;
        SendDayNotification();
    }

    #region Send Day Notification

    public void SendDayNotification()
    {
        for (var i = 0; i < 8; i++)
        {
            NotificationSupport.CreateNotificationDayZ
            (
                NOTI_12H, TITLE_12H,
                i, 12, 00
            );
            NotificationSupport.CreateNotificationDayZ
            (
                NOTI_20H, TITLE_20H,
                i, 20, 00
            );
        }

        NotificationSupport.CreateNotificationDayZ(NOTI_2D, TITLE_2D, 2, 21, 0);
        NotificationSupport.CreateNotificationDayZ(NOTI_4D, TITLE_4D, 4, 21, 0);
        NotificationSupport.CreateNotificationDayZ(NOTI_7D, TITLE_7D, 7, 21, 0);
    }

    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_ANDROID
using Unity.Notifications.Android;

#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationSupport
{
    //Tên của ứng dụng
    const string channel_id = "Bus Jam - Color Car Puzzle";
    const string title_noti = "Bus Jam - Color Car Puzzle";
    const string subject_title = "Bus Jam - Color Car Puzzle";

    public static int CurrentTimeInSecond
    {
        get { return (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds; }
    }

    #region Create Notification

    public static void CreateNotificationDayZ(string content, string title, int day, int hour, int minute)
    {
#if UNITY_ANDROID
        DateTime current = new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(CurrentTimeInSecond);
        DateTime nextDate = current.AddDays(day);
        nextDate = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hour, minute, 0);

        if (DateTime.Compare(nextDate, current) < 0)
        {
            return;
        }

        CreateNotification(content, title, nextDate);
#endif
    }

#if UNITY_ANDROID

    public static void RegisterChannel()
    {
        // if (AndroidNotificationCenter.GetNotificationChannel(channel_id).Name.Length > 0) return;
        AndroidNotificationCenter.DeleteNotificationChannel(channel_id);

        var c = new AndroidNotificationChannel()
        {
            Id = channel_id,
            Name = "someID",
            Importance = Importance.High,
            Description = "Generic notifications",
            EnableLights = true,
            EnableVibration = true,
            LockScreenVisibility = LockScreenVisibility.Public,
            CanBypassDnd = true,
            CanShowBadge = true,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
    }

    public static int CreateNotification(string content, string title, DateTime time_fire)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = content;
        notification.FireTime = time_fire;
        //notification.RepeatInterval = repeatInterval;
        notification.Color = new Color32((byte)255, (byte)70, (byte)0, (byte)255);
        notification.SmallIcon = "small_icon";
        notification.LargeIcon = "large_icon";
        notification.ShowTimestamp = true;
        notification.Style = NotificationStyle.BigTextStyle;
        return AndroidNotificationCenter.SendNotification(notification, channel_id);
    }
#endif

    #endregion
}
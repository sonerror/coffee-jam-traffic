using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;
using System;

public class TimeFromToGoogle : SingletonMonoBehaviour<TimeFromToGoogle>
{
    public static DateTime GoogleTimeGMT;
    public bool isUsingUnbiasedTime = true;

    public bool isHack = false;

    //Dtemp
    bool hasTimeGoogle = false;

    private void Start()
    {
        int check = PlayerPrefs.GetInt("IsUsingUbiasedTime", 0); // 0 = first time conect Google time , 1 = using UbbiaseTime , 2 = using Datetime.Now
        if (check == 0)
        {
            StartCoroutine(NetTime());
        }
        else if (check == 1)
        {
            isUsingUnbiasedTime = true;
        }
        else
        {
            isUsingUnbiasedTime = false;
        }
    }

    public DateTime TimeGoogle()
    {
        DateTime now = DateTime.UtcNow;

        if (hasTimeGoogle)
        {
            now = GoogleTimeGMT.AddSeconds(Time.time);
        }

        return now;
    }

    public DateTime Now()
    {
        DateTime now = new DateTime();
        TimeSpan defferTime = DateTime.UtcNow - DateTime.Now;

        if (isUsingUnbiasedTime)
        {
            now = UnbiasedTime.cur.Now();
        }
        else
        {
            now = DateTime.Now;
        }

        return now;
        //return DateTime.Now;
    }

    public DateTime UtcNow()
    {
        DateTime now = new DateTime();
        TimeSpan defferTime = DateTime.UtcNow - DateTime.Now;

        float checkdefer = PlayerPrefs.GetFloat("DefferTimeGoogleVsUnbiasedTime", 0);
        if (checkdefer != 0)
        {
            now = UnbiasedTime.cur.Now().AddSeconds(-checkdefer);
        }
        else
        {
            now = UnbiasedTime.cur.Now().AddSeconds(defferTime.TotalSeconds);
        }

        return now;

        //return DateTime.UtcNow;
    }

    public bool isUsingUbbiasedTime(DateTime googleTime, DateTime unbiasedTime)
    {
        if (Math.Abs((googleTime - unbiasedTime).TotalSeconds) <= 60)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator NetTime()
    {
        UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("https://www.google.com");
        yield return myHttpWebRequest.SendWebRequest();
        string netTime = myHttpWebRequest.GetResponseHeader("date");
        if (string.IsNullOrEmpty(netTime))
        {
            // Check internet Connection ???
            hasTimeGoogle = false;
        }
        else
        {
            string fomartDatetime = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";
            DateTime.TryParseExact(netTime, fomartDatetime, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out GoogleTimeGMT);
            TimeSpan defferTime = DateTime.UtcNow - DateTime.Now;
            TimeSpan defferTimeGoogleVsUnbiasedTime = UnbiasedTime.cur.Now() - GoogleTimeGMT;
            float x = (float)defferTimeGoogleVsUnbiasedTime.TotalSeconds;
            PlayerPrefs.SetFloat("DefferTimeGoogleVsUnbiasedTime", x);
            DateTime a = UnbiasedTime.cur.Now().AddSeconds(defferTime.TotalSeconds);
            if (isUsingUbbiasedTime(a, GoogleTimeGMT))
            {
                PlayerPrefs.SetInt("IsUsingUbiasedTime", 1);
                isUsingUnbiasedTime = true;
            }
            else
            {
                PlayerPrefs.SetInt("IsUsingUbiasedTime", 2);
                isUsingUnbiasedTime = false;
            }

            hasTimeGoogle = true;
        }
    }
}
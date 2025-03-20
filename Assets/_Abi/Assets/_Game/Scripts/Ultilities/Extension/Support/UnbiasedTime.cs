using UnityEngine;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.Networking;

public class UnbiasedTime : MonoBehaviour
{
    public static UnbiasedTime cur { get; private set; }

    // Estimated difference in seconds between device time and real world time
    // timeOffset = deviceTime - worldTime;
    private static double timeOffset = 0;
    private const string FORMAT_DATETIME = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";

    private static bool isCheatableTime = false; //must be static

    [SerializeField] private bool isCheatTime;

    // public static DateTime TrueDateTime => DateTime.Now.AddSeconds(timeOffset); // replace DateTime.Now
    public static DateTime TrueDateTime => DateTime.Now.AddSeconds(isCheatableTime ? 0 : timeOffset); // replace DateTime.Now
    public static DateTime TrueUtcDateTime => DateTime.UtcNow.AddSeconds(timeOffset);
    public static DateTime ForceTrueDateTime => DateTime.Now.AddSeconds(timeOffset);

    public static bool IsValidTime; //curently only for ldb

    public VoidEventChannelSO onUpdateTime;

    public bool IsChecking { get; private set; } = false;
    private int tryNum = 0;

    void Awake()
    {
        Debug.Log("TIME AWAKE");
        cur = this;
        isCheatableTime = isCheatTime;
    }

    private IEnumerator Start()
    {
        yield return Check();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SessionEnd();
        }
        else
        {
            SessionStart();
        }
    }

    void OnApplicationQuit()
    {
        SessionEnd();
    }

    // Returns estimated DateTime value taking into account possible device time changes
    public DateTime Now()
    {
        return DateTime.Now.AddSeconds(-1.0f * timeOffset);
    }

    private void SessionStart()
    {
        IsValidTime = isCheatableTime;
        StartCoroutine(Check());
    }

    private void SessionEnd()
    {
        IsValidTime = isCheatableTime;
    }

    public IEnumerator Check()
    {
        if (IsChecking)
        {
            yield return new WaitUntil(() => !IsChecking);
            yield break;
        }

        IsChecking = true;
        UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("https://www.google.com");

        Debug.Log("CHECKING UNBIAS TIME");

        var sendTime = Time.time;

        yield return myHttpWebRequest.SendWebRequest();
        string netTime = myHttpWebRequest.GetResponseHeader("date");
        if (string.IsNullOrEmpty(netTime))
        {
            // FAIL
            timeOffset = 0;
            IsValidTime = isCheatableTime;
            tryNum = Mathf.Min(tryNum + 1, 4);

            Debug.Log("CHECK TIME FAIL");

            Timer.ScheduleSupreme(Mathf.Pow(2, tryNum), () => StartCoroutine(Check()));
        }
        else
        {
            DateTime.TryParseExact(netTime, FORMAT_DATETIME, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out var GoogleTimeGMT);

            TimeSpan defferTime = DateTime.UtcNow - DateTime.Now;

            var localTime = GoogleTimeGMT.AddSeconds(-defferTime.TotalSeconds);

            timeOffset = (localTime - DateTime.Now).TotalSeconds - (Time.time - sendTime);
            IsValidTime = true;
            tryNum = 0;

            Debug.Log("TIME LOCAL" + localTime + "   " + timeOffset);

            onUpdateTime.RaiseEvent();
        }

        IsChecking = false;
    }
}
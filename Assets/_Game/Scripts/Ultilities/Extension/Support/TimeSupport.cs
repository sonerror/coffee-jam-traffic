using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSupport
{
    public static int CurrentTimeInSecond
    {
        get
        {
            return (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }

    public static int GetDayCurrent
    {
        get
        {
            return CurrentTimeInSecond / 86400;
        }
    }
}

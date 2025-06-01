using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CalendarCalculator
{
    public static DayOfWeek GetCurrentDayOfWeek()
    {
        return UnbiasedTime.TrueDateTime.DayOfWeek;
    }

    public static DayOfWeek GetFirstDayOfWeekInMonth()
    {
        var curDate = UnbiasedTime.TrueDateTime;
        var month = curDate.Month;
        var year = curDate.Year;

        return new DateTime(year, month, 1).DayOfWeek;
    }

    public static int GetNumOfDaysInMonth()
    {
        var curDate = UnbiasedTime.TrueDateTime;
        var month = curDate.Month;
        var year = curDate.Year;

        return DateTime.DaysInMonth(year, month);
    }

    public static int GetCurrentMonth(bool isMoreMinuteToSafe = false)
    {
        return isMoreMinuteToSafe ? UnbiasedTime.TrueDateTime.AddSeconds(60).Month : UnbiasedTime.TrueDateTime.Month;
    }

    public static int GetCurrentDayInMonth()
    {
        return UnbiasedTime.TrueDateTime.Day;
    }
}
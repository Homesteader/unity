using UnityEngine;
using System.Collections;
using System;

public static class STime
{

    public static long InitServerTime;

    public static long ServerTime;
    public static long GetNowUnixTime()
    {
        return ServerTime;
    }

    /// <summary>
    /// 获取本地时间
    /// </summary>
    /// <returns></returns>
    public static long GetNowUnixLocalTime()
    {
        DateTime time = DateTime.Now;
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
        return t;
    }

    public static DateTime GetNowDateTime()
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(ServerTime + "0000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
}

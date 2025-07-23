using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class JStopWatch
{

    [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
    private static extern bool QueryPerformanceFrequency(out long frequency);

    [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
    private static extern bool QueryPerformanceCounter(out long counter);

    public long frequency;                                        // high-resolution counter의 frequency를 저장하는 변수.
    public long startCount;                                    // 시간을 재기 시작한 순간의 counter 값.
    public long currentCount;
    public double elapsedTime;

    public enum TIME_UNIT
    {
        SECOND,
        MILLISECOND,
        MICROSECOND,
        NANOSECOND
    }

    
    public void start()
    {
        elapsedTime = (currentCount - startCount) / (double)frequency;

        startCount = currentCount;
        QueryPerformanceFrequency(out frequency);
        QueryPerformanceCounter(out startCount);
    }

    public long getTime(TIME_UNIT timeUnit)
    {
        QueryPerformanceCounter(out currentCount);
        elapsedTime = (currentCount - startCount) / (double)frequency;

        // 진행된 시간 반환.
        switch (timeUnit)
        {
            case TIME_UNIT.SECOND: return (long)(elapsedTime * 1);
            case TIME_UNIT.MILLISECOND: return (long)(elapsedTime * 1000);
            case TIME_UNIT.MICROSECOND: return (long)(elapsedTime * 1000000);
            case TIME_UNIT.NANOSECOND: return (long)(elapsedTime * 1000000000);
        }

        return 0;
    }


}

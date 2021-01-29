using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerTime : MonoBehaviour
{
    [Header("Clock")]
    [SerializeField]
    float maxClockPerDay = 3000.0f;

    [SerializeField]
    float clockRate = 3.0f;

    [SerializeField]
    int maxDayPeriod = 6;

    [Header("Year")]
    [SerializeField]
    int startOfYear = 2020;

    [SerializeField]
    int endOfYear = 2021;

    public enum TimePeriod
    {
        Dawn,
        Morning,
        Launch,
        Afternoon,
        Sunset,
        Evenning
    }

    public Action OnStartAdvanceTime;
    public Action OnFinishAdvanceTime;

    public Action<DateTime> OnDayPass;
    public Action<DateTime> OnMonthPass;
    public Action<DateTime> OnYearPass;

    public int MaxDayPeriod => maxDayPeriod;
    public int DayPeriodID { get {
        int value = (int)((innerClock / ((actualMaxClock))) * maxDayPeriod);
        if (value == maxDayPeriod)
            value = 0;
        return value;
    } } 
    public TimePeriod DayPeriod => (TimePeriod)DayPeriodID;

    public float InnerClock => innerClock;
    public float MaxInnerClock => maxClockPerDay;

    public bool IsPause => isPause;
    public bool IsPlayerBusy => isPlayerBusy;

    public DateTime Calendar => currentDate;

    bool isAdvancingTime = false;
    bool isPause = true;
    bool isForcePauseTime;
    bool isPlayerBusy = false;

    float innerClock = 0.0f;
    float actualMaxClock = 1;

    DateTime startDate;
    DateTime currentDate;
    DateTime nextMonthDate;
    DateTime endDate;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        Tick();
    }
    
    void Initialize()
    {
        startDate = new DateTime(startOfYear, 1, 1);
        endDate = new DateTime(endOfYear, 1, 1);

        currentDate = startDate;
        nextMonthDate = currentDate.AddMonths(1);

        actualMaxClock = (int)(maxClockPerDay / clockRate);
    }

    void Tick()
    {
        if (isPlayerBusy)
            return;

        if (isPause)
            return;

        innerClock += (clockRate * Time.deltaTime);

        if (innerClock > actualMaxClock) {
            UpdateDay();
        }

        Debug.Log("Curent period : " + DayPeriod);
    }

    void UpdateDay()
    {
        innerClock = 0.0f;

        currentDate = currentDate.AddDays(1);
        OnDayPass?.Invoke(currentDate);

        Debug.Log("Day : " + currentDate);

        if (currentDate == nextMonthDate) {
            nextMonthDate = nextMonthDate.AddMonths(1);
            OnMonthPass?.Invoke(currentDate);

            Debug.Log("Month : " + currentDate);
        }

        if (currentDate == endDate) {
            OnYearPass?.Invoke(currentDate);
            Debug.Log("Year : " + currentDate);
        }
    }

    IEnumerator AdvanceTimeCallback(TimeSpan timeSpan, float finishDelay)
    {
        Pause(true);
        OnStartAdvanceTime?.Invoke();

        /* Debug.Log("Start time : " + currentDate); */

        yield return new WaitForSeconds(finishDelay);
        currentDate += timeSpan;

        Pause(false);
        OnFinishAdvanceTime?.Invoke();

        /* Debug.Log("End time : " + currentDate); */
    }

    public void ForcePauseTime(bool value)
    {
        isForcePauseTime = value;
    }

    public void Pause(bool value = true)
    {
        if (isForcePauseTime) {
            isPause = true;
            return;
        }

        isPause = value;
    }

    public void AdvanceTime(TimeSpan timeSpan, float finishDelay = 1.2f)
    {
        if (isAdvancingTime) {
            Debug.LogWarning("Time is currently advancing, not advance for this call...");
            return;
        }

        StartCoroutine(AdvanceTimeCallback(timeSpan, finishDelay));
    }

    public void StartClock()
    {
        ResetClock();
        Pause(false);
    }

    public void ResetClock()
    {
        innerClock = 0.0f;
        isPause = true;
        isPlayerBusy = false;
        currentDate = new DateTime(startOfYear, 1, 1);
    }
}


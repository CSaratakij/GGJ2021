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
    int maxDayPeriod = 5;

    [Header("Year")]
    [SerializeField]
    int startOfYear = 2020;

    [SerializeField]
    int endOfYear = 2021;

    public Action OnStartAdvanceTime;
    public Action OnFinishAdvanceTime;

    public int DayPeriod => (int)(maxClockPerDay % maxDayPeriod);
    public float InnerClock => innerClock;
    public float MaxInnerClock => maxClockPerDay;
    public bool IsPause => isPause;
    public DateTime Calendar => currentDate;

    bool isAdvancingTime = false;
    bool isPause = true;

    float innerClock = 0.0f;

    DateTime startDate;
    DateTime currentDate;
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
        currentDate = new DateTime(startOfYear, 1, 1);
        endDate = new DateTime(endOfYear, 1, 1);
    }

    // don't forget to reset time when player start the new game..
    void Tick()
    {

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

    public void Pause(bool value = true)
    {
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

    public void RestartClock()
    {
        innerClock = 0.0f;
        currentDate = new DateTime(1, 1, startOfYear);
    }
}


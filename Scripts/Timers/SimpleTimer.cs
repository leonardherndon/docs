using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimpleTimer
{
    public float TimeRemaining { get; private set; }
    public float TotalTime { get; private set; }
    public int Limit { get; private set; }
    public bool IsRecurring { get; private set; }
    public bool IsActive { get; private set; }
    public int TimesCounted { get; private set; }

    public float TimeElapsed => TotalTime - TimeRemaining;
    public float PercentElapsed => TimeElapsed / TotalTime;
    public float PercentRemaining => TimeRemaining / TotalTime;
    public bool IsCompleted => TimeRemaining <= 0;

    public delegate void TimerLoopHandler();

    /// <summary>
    /// Emits event when timer is completed
    /// </summary>
    public event TimerLoopHandler TimerLoopEvent;
    
    public delegate void TimerCompleteHandler();
    /// <summary>
    /// Emits event when timer is completed
    /// </summary>
    public event TimerCompleteHandler TimerCompleteEvent;

    /// <summary>
    /// Create a new SimpleTimer
    /// Must call Start() to begin timer
    /// </summary>
    /// <param name="time">Timer length (seconds)</param>
    /// <param name="recurring">Is this timer recurring</param>
    public SimpleTimer(float time, int limit, bool recurring = false)
    {
        TotalTime = time;
        Limit = limit;
        IsRecurring = recurring;
        TimeRemaining = TotalTime;
        IsActive = true;
    }

    /// <summary>
    /// Start timer with existing time
    /// </summary>
    public void Start()
    {
        Debug.Log("Timer Start");
        if (IsActive) { TimesCounted++; }
        TimeRemaining = TotalTime;
        IsActive = true;
        if (TimeRemaining <= 0)
        {
            TimerCompleteEvent?.Invoke();
        }
    }

    /// <summary>
    /// Start timer with new time
    /// </summary>
    public void Start(float time)
    {
        TotalTime = time;
        Start();
    }
    
    public void ResetTime(float time)
    {
        Debug.Log("Timer Reset");
        TimeRemaining = time;
        TotalTime = time;
    }

    public virtual void Update(float timeDelta)
    {
        //Debug.Log("Timer Update");
        if (TimeRemaining > 0 && IsActive)
        {
            TimeRemaining -= timeDelta;
            if (TimeRemaining <= 0)
            {
                if (IsRecurring && (TimesCounted < Limit || Limit == -1))
                {
                    TimeRemaining = TotalTime;
                }
                else
                {
                    TimerCompleteEvent?.Invoke();
                    IsRecurring = false;
                    IsActive = false; 
                    TimeRemaining = 0;
                }

                TimerLoopEvent?.Invoke();
                TimesCounted++;
            }
        }
    }

    public void Invoke()
    {
        TimerLoopEvent?.Invoke();
    }
    
    public void Complete()
    {
        TimerLoopEvent?.Invoke();
    }

    public void Pause()
    {
        IsActive = false;
    }

    /// <summary>
    /// Add additional time to timer
    /// </summary>
    public void AddTime(float time)
    {
        TimeRemaining += time;
        TotalTime += time;
    }
    
}
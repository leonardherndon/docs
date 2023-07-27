using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour, IGameTimers
{
    
    public bool timerActive { get; set; }
    public float timerTime { get; set; }

    public Text levelTimerText;
    
    [SerializeField]
    private string timerString;
    public void StartTimer()
    {
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public void SetTimer(float newTime)
    {
        timerTime = newTime;
    }
    public void InitTimer()
    {
        timerTime = 0;
    }

    public void Update()
    {
        if (timerActive == false)
            return;

        timerTime += Time.deltaTime;
        int seconds = (int)(timerTime % 60);
        int minutes = (int)(timerTime / 60) % 60;
        int hours = (int) (timerTime / 3600) % 24;
        
        
        timerString = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
        //Debug.Log("Timer: " + timerString);
        levelTimerText.text = timerString;
    }
}

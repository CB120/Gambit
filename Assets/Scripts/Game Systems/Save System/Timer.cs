using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;

public class Timer : MonoBehaviour
{
    private float startTime;
    private float onGoingTime;
    private bool timerStopped = false;

    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(!timerStopped)
            onGoingTime = Time.time - startTime;

        // Debug.Log("Time in Seconds: " + (int)onGoingTime);
    }

    public void AddTimeToPlayerStats()
    {
        timerStopped = true;
        SavedData.GameData.playtime += onGoingTime;
    }
}

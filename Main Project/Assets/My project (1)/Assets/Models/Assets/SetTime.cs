using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetTime : MonoBehaviour
{
    public const int hoursInDay = 24, minutesInHour = 60;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float dayDuration = 120;
    float totalTime = 0;
    float currentTime = 0;

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        currentTime = totalTime % dayDuration;
    }
    public float GetHour()
    {
        return currentTime * hoursInDay / dayDuration;
    }
    public float GetMinutes()
    {
        return (currentTime * hoursInDay * minutesInHour / dayDuration) % minutesInHour;
    }
    public string MilitaryTime()
    {
        return Mathf.FloorToInt(GetHour()).ToString("00") + ":" + Mathf.FloorToInt(GetMinutes()).ToString("00");
    }
}
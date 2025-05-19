using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitalClock : MonoBehaviour
{
    private TextMeshProUGUI textDisplay;
    string hour, minute;
    DateTime TimeNow;
    void Start()
    {
        textDisplay = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        TimeNow = DateTime.Now;
        hour = TimeNow.Hour.ToString("00");
        minute = TimeNow.Minute.ToString("00");

        textDisplay.text = hour + ":" + minute;
    }
}

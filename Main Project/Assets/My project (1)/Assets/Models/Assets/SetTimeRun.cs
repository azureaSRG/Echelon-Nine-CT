using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTimeRun : MonoBehaviour
{
    SetTime sm;
    Text display;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sm = FindFirstObjectByType<SetTime>();
        display = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        display.text = sm.MilitaryTime();
    }
}

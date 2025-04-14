using UnityEngine;
using TMPro;

public class TimerText : MonoBehaviour
{
    public TMP_Text timerText;  // Assign this in the Inspector
    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        // Checks to see if the code if the script is running and then adds to the timer
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    // A function that literally takes the time and formats it and assigns it to the text box
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Call this to stop the timer
    public void StopTimer()
    {
        isRunning = false;
    }

    // Call this to run it
    public void StartTimer()
    {
        isRunning = true;
    }

    // Call this to set the time back to 0 but keep it running
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay();
    }
}

using UnityEngine;
using TMPro;

public class SpeedrunManager : MonoBehaviour
{
    public TMP_Text currentTimeText; // Reference to TextMeshPro text element to display current time
    public TMP_Text bestTimeText; // Reference to TextMeshPro text element to display best time

    private const string BestTimeKey = "BestTime";

    private void Start()
    {
        // Load the best time from PlayerPrefs
        if (PlayerPrefs.HasKey(BestTimeKey))
        {
            bestTime = PlayerPrefs.GetFloat(BestTimeKey);
            UpdateBestTimeDisplay();
        }
    }

    private float currentTime = 0f; // Current time taken to complete the scene
    private float bestTime = Mathf.Infinity; // Best time taken to complete the scene, initialized as positive infinity

    private void Update()
    {
        // Update the current time while the scene is active
        currentTime += Time.deltaTime;
        UpdateCurrentTimeDisplay();
    }

    public void CompleteSpeedrun()
    {
        // Check if the current time is less than the best time
        if (currentTime < bestTime)
        {
            // Update the best time if the current time is better
            bestTime = currentTime;
            PlayerPrefs.SetFloat(BestTimeKey, bestTime);
            UpdateBestTimeDisplay();
        }

        // Reset the current time for the next speedrun
        currentTime = 0f;
        UpdateCurrentTimeDisplay();
        Debug.Log(bestTime);
    }

    private void UpdateCurrentTimeDisplay()
    {
        // Update the UI text to display the current time
        if (currentTimeText != null)
        {
            currentTimeText.text = FormatTime(currentTime);
        }
    }

    private void UpdateBestTimeDisplay()
    {
        if (bestTime == Mathf.Infinity)
        {
            bestTimeText.text = "Not set yet";
        }
        else if (bestTimeText != null)
        {
            bestTimeText.text = FormatTime(bestTime);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        // Convert the time in seconds to an integer
        int seconds = Mathf.RoundToInt(timeInSeconds);

        // Return the seconds as a string
        return seconds.ToString();
    }
}

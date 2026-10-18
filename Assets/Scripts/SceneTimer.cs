using UnityEngine;
using System.Collections;

public class SceneTimer : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float bestTime = Mathf.Infinity;
    private bool timerRunning = true;
    public string finalFinishTag = "FinalFinish";
    public GameObject bestTimePrefab;
    private bool finalFinishReached = false;

    void Start()
    {
        // Load best time from PlayerPrefs
        bestTime = PlayerPrefs.GetFloat("BestTime", Mathf.Infinity);

        // Start the timer coroutine
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (timerRunning)
        {
            // Update the elapsed time if timer is running
            if (!finalFinishReached)
                elapsedTime += Time.deltaTime;

            // Display the elapsed time

            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided with final finish tag
        if (other.CompareTag(finalFinishTag))
        {
            // Stop the timer
            timerRunning = false;
            finalFinishReached = true;

            // Check if current time is better than best time
            if (elapsedTime < bestTime)
            {
                // Save best time to PlayerPrefs
                bestTime = elapsedTime;
                PlayerPrefs.SetFloat("BestTime", bestTime);
                PlayerPrefs.Save();
            }

            // Display best time
            Debug.Log("Best Time: " + bestTime.ToString("F2") + " seconds");

            // Update best time prefab
            if (bestTimePrefab != null)
            {
                // Assuming the prefab has a Text component to display the time
                var label = bestTimePrefab.GetComponent<UnityEngine.UI.Text>();
                if (label != null) label.text = bestTime.ToString("F2");
            }
        }
    }
}

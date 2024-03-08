using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalFinish : MonoBehaviour
{
    private AudioSource finishSound;
    private bool levelCompleted = false;

    private void Start()
    {
        finishSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted)
        {
            finishSound.Play();
            levelCompleted = true;
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        // Call SaveData from ItemCollector script to save game data
        ItemCollector itemCollector = FindObjectOfType<ItemCollector>();
        if (itemCollector != null)
        {
            itemCollector.SaveData();
        }
        else
        {
            Debug.LogWarning("ItemCollector reference is null. Make sure there is an ItemCollector script in the scene.");
        }

        float totalTime = StopwatchManager.Instance.GetTotalTime();
        GameObject stopwatchDataObj = Instantiate(Resources.Load("StopwatchDataPrefab")) as GameObject;
        StopwatchData stopwatchData = stopwatchDataObj.GetComponent<StopwatchData>();
        stopwatchData.SaveTime(totalTime);

        // Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }
}

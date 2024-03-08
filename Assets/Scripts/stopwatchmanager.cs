using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StopwatchManager : MonoBehaviour
{
    private float totalTime = 0f;
    private bool isTiming = false;
    public TextMeshProUGUI timeText; // Reference to the TextMeshPro component

    private static StopwatchManager instance;

    public static StopwatchManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StopwatchManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(StopwatchManager).Name;
                    instance = obj.AddComponent<StopwatchManager>();
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (isTiming)
        {
            totalTime += Time.deltaTime;
            UpdateTimeText(); // Update the time text every frame
        }
    }

    public void StartTiming()
    {
        isTiming = true;
    }

    public void StopTiming()
    {
        isTiming = false;
    }

    public float GetTotalTime()
    {
        return totalTime;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) // Reset time on level 1
        {
            totalTime = 0f;
            UpdateTimeText(); // Update the time text when the level is reset
        }
    }

    private void UpdateTimeText()
    {
        if (timeText != null)
        {
            timeText.text = "Time: " + Mathf.RoundToInt(totalTime) + "s";
        }
    }
}

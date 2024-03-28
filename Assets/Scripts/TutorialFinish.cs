using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialFinish : MonoBehaviour
{
    ItemCollector itemCollector;
    private AudioSource finishSound;
    private bool levelCompleted = false;
    SpeedrunManager speedrun;


    private void Start()
    {
        finishSound = GetComponent<AudioSource>();
        itemCollector = FindObjectOfType<ItemCollector>();
        speedrun = FindObjectOfType<SpeedrunManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted)
        {
            speedrun.CompleteSpeedrun();
            finishSound.Play();
            levelCompleted = true;
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        // Call SaveData from ItemCollector script to save game data
        itemCollector.ResetScore();
        // Load the next scenes
        SceneManager.LoadScene(11); 
    }
}

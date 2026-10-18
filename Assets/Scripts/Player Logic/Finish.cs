using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private ItemCollector itemCollector;
    private AudioSource finishSound;
    private SpeedrunManager speedrun;
    private bool levelCompleted;

    private void Start()
    {
        finishSound = GetComponent<AudioSource>();
        itemCollector = FindObjectOfType<ItemCollector>();
        speedrun = FindObjectOfType<SpeedrunManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || levelCompleted) return;
        levelCompleted = true;
        if (speedrun != null) speedrun.CompleteSpeedrun();
        if (finishSound != null) finishSound.Play();
        if (itemCollector != null) itemCollector.SaveData();
        GameController.ResetProgress();
        SceneManager.LoadScene("Credits");
    }
}

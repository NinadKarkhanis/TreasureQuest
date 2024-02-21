using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int coin = 0;
    private int enemy = 0;
    private int score = 0;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private AudioSource collectEffect;
    [SerializeField] private AudioSource enemyded;

    private const string COIN_KEY = "Coins";
    private const string SCORE_KEY = "Score";

    private void Start()
    {
        LoadData(); // Load saved data when the game starts
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            collectEffect.Play();
            Destroy(collision.gameObject);
            coin++;
            UpdateScore();
            SaveData(); // Save data whenever a coin is collected
        }
        else if (collision.gameObject.CompareTag("WeakPoint"))
        {
            enemyded.Play();
            Destroy(collision.gameObject);
            enemy++;
            UpdateScore();
            SaveData(); // Save data whenever an enemy is defeated
        }
    }

    private void UpdateScore()
    {
        score = coin * 10 + enemy * 100; // Example scoring logic
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(COIN_KEY, coin);
        PlayerPrefs.SetInt(SCORE_KEY, score);
        PlayerPrefs.Save();
        Debug.Log("Saved");
    }

    public void LoadData()
    {
        coin = PlayerPrefs.GetInt(COIN_KEY, 0);
        score = PlayerPrefs.GetInt(SCORE_KEY, 0);
        Debug.Log("Loaded");
        UpdateScore(); // Update the score text when loading data
    }
}

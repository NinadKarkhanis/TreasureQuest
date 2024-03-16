using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int coin = 0;
    private int storedCoins = 0; // New variable to store collected coins
    private int enemy = 0;
    private int score = 0;
    private int highScore = 0;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText; // Reference to display the high score
    [SerializeField] private AudioSource collectEffect;
    [SerializeField] private AudioSource enemyded;

    private const string COIN_KEY = "Coins";
    private const string STORED_COINS_KEY = "StoredCoins"; // Key for storing collected coins
    private const string SCORE_KEY = "Score";
    private const string HIGHSCORE_KEY = "HighScore"; // Key for saving high score

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
            storedCoins++; // Increment stored coins
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
        coin = storedCoins; // Update total coins collected
        score = coin * 10 + enemy * 100; // Example scoring logic
        if (score > highScore) // Check if current score is higher than previous high score
        {
            highScore = score;
            UpdateHighScoreDisplay(); // Update high score display if new high score achieved
        }
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
            highScoreText.text = highScore.ToString();
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(COIN_KEY, coin);
        PlayerPrefs.SetInt(STORED_COINS_KEY, storedCoins); // Save stored coins
        PlayerPrefs.SetInt(SCORE_KEY, score);
        PlayerPrefs.SetInt(HIGHSCORE_KEY, highScore); // Save high score
        PlayerPrefs.Save();
        Debug.Log("Saved");
    }

    public void LoadData()
    {
        coin = PlayerPrefs.GetInt(COIN_KEY, 0);
        storedCoins = PlayerPrefs.GetInt(STORED_COINS_KEY, 0); // Load stored coins
        score = PlayerPrefs.GetInt(SCORE_KEY, 0);
        highScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0); // Load high score
        UpdateScore(); // Update the score text when loading data
        UpdateHighScoreDisplay(); // Update high score display
    }

    public void ResetScore()
    {
        coin = 0;
        storedCoins = 0; // Reset stored coins
        enemy = 0;
        score = 0;
        UpdateScore();
        SaveData(); // Save the reset score
        Debug.Log("Score Reset");
    }

    public void ReduceScoreOnDeath()
    {
        score -= 100;
        if (score < 0) // Ensure score never goes below zero
        {
            score = 0;
        }
        UpdateScore();
        SaveData(); // Save the reduced score
        Debug.Log("Score reduced: " + score);
    }
}
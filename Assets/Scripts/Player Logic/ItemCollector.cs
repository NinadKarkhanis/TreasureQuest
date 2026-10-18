using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemCollector : MonoBehaviour
{
    private int storedCoins;
    private int defeatedEnemies;
    private int deathPenalty;
    private int score;
    private int highScore;
    private readonly HashSet<string> collectedCoinIds = new HashSet<string>();

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private AudioSource collectEffect;
    [SerializeField] private AudioSource enemyded;

    private const string COIN_KEY = "Coins";
    private const string STORED_COINS_KEY = "StoredCoins";
    private const string SCORE_KEY = "Score";
    private const string HIGHSCORE_KEY = "HighScore";
    private const string ENEMY_KEY = "TreasureQuest.DefeatedEnemies";
    private const string PENALTY_KEY = "TreasureQuest.DeathPenalty";
    private const string COLLECTED_KEY = "TreasureQuest.CollectedCoinIds";

    private void Start()
    {
        LoadData();
        // Retire already collected scene coins so leaving/re-entering a scene cannot farm them.
        foreach (GameObject existingCoin in GameObject.FindGameObjectsWithTag("Coin"))
            if (collectedCoinIds.Contains(CoinId(existingCoin.transform))) Destroy(existingCoin);
    }

    private static string CoinId(Transform coin)
    {
        // Deterministic ID without requiring manual edits to hundreds of old scene objects.
        // Coins placed at identical positions in one scene should be manually given unique IDs.
        Vector3 p = coin.position;
        return SceneManager.GetActiveScene().name + ":" + Mathf.RoundToInt(p.x * 100f) + ":" +
               Mathf.RoundToInt(p.y * 100f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            string id = CoinId(collision.transform);
            if (!collectedCoinIds.Add(id)) return;
            if (collectEffect != null) collectEffect.Play();
            Destroy(collision.gameObject);
            storedCoins++;
            UpdateScore();
            SaveData();
        }
        else if (collision.CompareTag("WeakPoint"))
        {
            if (enemyded != null) enemyded.Play();
            Destroy(collision.gameObject);
            defeatedEnemies++;
            UpdateScore();
            SaveData();
        }
    }

    private void UpdateScore()
    {
        score = GameRules.CalculateScore(storedCoins, defeatedEnemies, deathPenalty);
        if (score > highScore) highScore = score;
        if (scoreText != null) scoreText.text = score.ToString();
        if (highScoreText != null) highScoreText.text = highScore.ToString();
    }

    public void ReduceScoreOnDeath()
    {
        deathPenalty += 100;
        UpdateScore();
        SaveData();
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(COIN_KEY, storedCoins);
        PlayerPrefs.SetInt(STORED_COINS_KEY, storedCoins);
        PlayerPrefs.SetInt(SCORE_KEY, score);
        PlayerPrefs.SetInt(HIGHSCORE_KEY, highScore);
        PlayerPrefs.SetInt(ENEMY_KEY, defeatedEnemies);
        PlayerPrefs.SetInt(PENALTY_KEY, deathPenalty);
        PlayerPrefs.SetString(COLLECTED_KEY, string.Join(";", collectedCoinIds));
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        storedCoins = PlayerPrefs.GetInt(STORED_COINS_KEY, PlayerPrefs.GetInt(COIN_KEY, 0));
        defeatedEnemies = PlayerPrefs.GetInt(ENEMY_KEY, 0);
        deathPenalty = PlayerPrefs.GetInt(PENALTY_KEY, 0);
        highScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        collectedCoinIds.Clear();
        string previouslyCollected = PlayerPrefs.GetString(COLLECTED_KEY, "");
        if (!string.IsNullOrEmpty(previouslyCollected))
            foreach (string id in previouslyCollected.Split(';'))
                if (!string.IsNullOrEmpty(id)) collectedCoinIds.Add(id);
        UpdateScore();
    }

    public void ResetScore()
    {
        storedCoins = 0;
        defeatedEnemies = 0;
        deathPenalty = 0;
        collectedCoinIds.Clear();
        UpdateScore();
        SaveData();
    }
}

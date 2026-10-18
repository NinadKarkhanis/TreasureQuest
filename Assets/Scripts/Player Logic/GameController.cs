using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class GameController : MonoBehaviour
{
    private const int StartingLives = 10;
    private const float TimeLimitSeconds = 600f;
    private const string SaveScene = "TreasureQuest.SaveScene";
    private const string SaveX = "TreasureQuest.CheckpointX";
    private const string SaveY = "TreasureQuest.CheckpointY";
    private const string SaveLives = "TreasureQuest.Lives";

    private Vector2 checkpointPosition;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private ItemCollector collector;
    private Vector3 originalScale;
    private Color originalColor;
    private bool dying;
    private bool ending;
    private int lives = StartingLives;
    private float remainingSeconds = TimeLimitSeconds;
    private int lastDisplayedSeconds = -1;

    [SerializeField] private AudioSource dedEffect;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject deathGameObject;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        collector = FindObjectOfType<ItemCollector>();
        originalScale = transform.localScale;
        originalColor = sprite.color;
    }

    private void Start()
    {
        checkpointPosition = transform.position;
        if (PlayerPrefs.GetString(SaveScene, "") == SceneManager.GetActiveScene().name)
        {
            lives = Mathf.Clamp(PlayerPrefs.GetInt(SaveLives, StartingLives), 1, StartingLives);
            checkpointPosition = new Vector2(PlayerPrefs.GetFloat(SaveX, checkpointPosition.x),
                PlayerPrefs.GetFloat(SaveY, checkpointPosition.y));
            transform.position = checkpointPosition;
        }
        UpdateLivesDisplay();
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (ending) return;
        remainingSeconds = Mathf.Max(0f, remainingSeconds - Time.deltaTime);
        UpdateTimerDisplay();
        if (remainingSeconds <= 0f) RestartGame();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        int seconds = Mathf.CeilToInt(remainingSeconds);
        if (lastDisplayedSeconds == seconds) return;
        lastDisplayedSeconds = seconds;
        timerText.text = string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
    }
    private void UpdateLivesDisplay() { if (livesText != null) livesText.text = lives.ToString(); }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!ending && !dying && (collision.gameObject.CompareTag("Trap") || collision.gameObject.CompareTag("Enemy")))
            StartCoroutine(HandleDeath());
    }

    public void UpdateCheckpoint(Vector2 position)
    {
        if (ending || dying) return;
        checkpointPosition = position;
        SaveCheckpoint();
    }

    private void SaveCheckpoint()
    {
        PlayerPrefs.SetString(SaveScene, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(SaveX, checkpointPosition.x);
        PlayerPrefs.SetFloat(SaveY, checkpointPosition.y);
        PlayerPrefs.SetInt(SaveLives, lives);
        PlayerPrefs.Save();
    }

    private IEnumerator HandleDeath()
    {
        dying = true; // Prevent the same enemy from taking multiple lives per collision/physics frame.
        lives = Mathf.Max(0, lives - 1);
        UpdateLivesDisplay();
        if (collector != null) collector.ReduceScoreOnDeath();
        if (dedEffect != null) dedEffect.Play();
        if (deathGameObject != null) deathGameObject.SetActive(true);
        if (lives == 0)
        {
            yield return new WaitForSeconds(.45f);
            RestartGame();
            yield break;
        }
        SaveCheckpoint();
        body.velocity = Vector2.zero;
        body.simulated = false;
        sprite.color = Color.red;
        transform.localScale = originalScale * .1f;
        yield return new WaitForSeconds(.5f);
        transform.position = checkpointPosition;
        transform.localScale = originalScale;
        sprite.color = originalColor;
        body.velocity = Vector2.zero;
        body.simulated = true;
        if (deathGameObject != null) deathGameObject.SetActive(false);
        // Brief immunity during respawn avoids immediate repeated damage.
        yield return new WaitForSeconds(.6f);
        dying = false;
    }

    private void RestartGame()
    {
        if (ending) return;
        ending = true;
        ResetProgress();
        if (collector != null) collector.ResetScore();
        Time.timeScale = 1f;
        SceneManager.LoadScene("End Screen");
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(SaveScene);
        PlayerPrefs.DeleteKey(SaveX);
        PlayerPrefs.DeleteKey(SaveY);
        PlayerPrefs.DeleteKey(SaveLives);
        PlayerPrefs.Save();
    }

    public void ResetLives()
    {
        lives = StartingLives;
        UpdateLivesDisplay();
        SaveCheckpoint();
    }
}

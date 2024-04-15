using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    Vector2 CheckpointPos;
    Rigidbody2D playerRb;
    SpriteRenderer spriteRenderer;
    ItemCollector itemCollector;

    [SerializeField] private AudioSource dedEffect;
    [SerializeField] private TMP_Text livesText; // Reference to the TextMeshPro text element for displaying the lives
    [SerializeField] private TMP_Text timerText; // Reference to the TextMeshPro text element for displaying the timer
    [SerializeField] private GameObject deathGameObject;

    private int lives = 10; // Starting number of lives
    private float timer = 600f; // Timer set to 600 seconds (10 minutes)
    private bool isTimerRunning = true; // Flag to control timer state

    private void Awake()
    {   
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollector = FindObjectOfType<ItemCollector>();
    }

    private void Start()
    {
        CheckpointPos = transform.position;
        UpdateLivesDisplay();
        UpdateTimerDisplay();
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        while (timer > 0 && isTimerRunning)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
            UpdateTimerDisplay();
        }

        if (timer <= 0)
        {
            // Timer has expired, restart the game
            RestartGame();
        }
    }

    private void UpdateTimerDisplay()
    {
        // Update the UI text to display the current timer value
        if (timerText != null)
        {
            timerText.text =  Mathf.RoundToInt(timer).ToString();
        }
    }

    private void UpdateLivesDisplay()
    {
        // Update the UI text to display the current lives value
        if (livesText != null)
        {
            livesText.text =  lives.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap") || collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    public void UpdateCheckpoint(Vector2 pos)
    {
        CheckpointPos = pos;
    }

    private void Die()
    {
        if (deathGameObject != null)
        {
            StartCoroutine(ActivateAndDeactivateForDuration(deathGameObject, 0.5f));
        }
        //itemCollector.ReduceScoreOnDeath(); // Call ReduceScoreOnDeath method
        if (dedEffect != null)
        {
            dedEffect.Play();
        }

        // Decrement lives
        lives--;

        // Update lives display
        UpdateLivesDisplay();

        if (lives <= 0)
        {
            // Player has no lives left, restart the game
            RestartGame();
        }
        else
        {
            StartCoroutine(Respawn(0.5f));
           
        }
    }

    private IEnumerator ActivateAndDeactivateForDuration(GameObject gameObject, float duration)
{
    gameObject.SetActive(true);
    yield return new WaitForSeconds(duration);
    gameObject.SetActive(false);
}

    private IEnumerator Respawn(float duration)
    {
        playerRb.simulated = false;
        playerRb.velocity = Vector2.zero;
        transform.localScale = Vector3.zero;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(duration);
        transform.position = CheckpointPos;
        transform.localScale = Vector3.one;
        spriteRenderer.color = Color.white;
        playerRb.simulated = true;
    }

    private void RestartGame()
    {
        isTimerRunning = false; // Stop the timer
        itemCollector.ResetScore();
        SceneManager.LoadScene(2); 
    }

    public void ResetLives()
    {
        lives = 10; // Reset lives to starting value
        UpdateLivesDisplay(); // Update lives display
    }
}

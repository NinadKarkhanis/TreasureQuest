using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private ItemCollector itemCollector; // Declare itemCollector variable

    [SerializeField] private AudioSource dedEffect;
    [SerializeField] private float respawnTime = 3f; // Adjust the respawn time as needed
    [SerializeField] private TMP_Text timerText; // Reference to the TextMeshPro text element for displaying the timer
    [SerializeField] private TMP_Text livesText; // Reference to the TextMeshPro text element for displaying the lives

    private float timer; // Variable to hold the countdown timer value
    private int lives = 10; // Starting number of lives
    private const string LivesKey = "PlayerLives"; // Key for PlayerPrefs

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        itemCollector = FindObjectOfType<ItemCollector>(); // Find the ItemCollector script in the scene

        // Load the number of lives from PlayerPrefs
        if (PlayerPrefs.HasKey(LivesKey))
        {
            lives = PlayerPrefs.GetInt(LivesKey);
        }

        // Start the countdown timer
        timer = respawnTime;
        UpdateTimerDisplay();
        UpdateLivesDisplay();
    }

    private void Update()
    {
        // Decrease the timer value every frame
        timer -= Time.deltaTime;

        // Update the timer display
        UpdateTimerDisplay();

        // Check if the timer has reached zero
        if (timer <= 0)
        {
            // Respawn the player
            Restartlevel();
        }
    }

    private void UpdateTimerDisplay()
    {
        // Update the UI text to display the current timer value
        if (timerText != null)
        {
            timerText.text = Mathf.RoundToInt(timer).ToString();
        }
    }

    private void UpdateLivesDisplay()
    {
        // Update the UI text to display the current lives value
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap") || collision.gameObject.CompareTag("Enemy"))
        {
            Die();
            itemCollector.ReduceScoreOnDeath(); // Call ReduceScoreOnDeath method
        }
    }

    private void Die()
    {
        if (dedEffect != null)
        {
            dedEffect.Play();
        }

        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");

        // Decrement lives
        lives--;

        // Save the updated lives count
        PlayerPrefs.SetInt(LivesKey, lives);

        // Update lives display
        UpdateLivesDisplay();

        // Check if player has remaining lives
        if (lives <= 0)
       {
    // Player has no lives left, reset the score and restart the game
       PlayerPrefs.DeleteKey(LivesKey); // Reset lives count
       SceneManager.LoadScene(2); // Load the scene at index 1 (assuming "Scene2" is at index 1)
      itemCollector.ResetScore(); // Reset the score
      Debug.Log("New score time");
       }
    }

    private void Restartlevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetLives()
    {
        lives = 10; // Reset lives to starting value
        UpdateLivesDisplay(); // Update lives display
        PlayerPrefs.SetInt(LivesKey, lives); // Save the reset lives count
    }
}

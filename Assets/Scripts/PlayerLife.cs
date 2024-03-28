using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerLife : MonoBehaviour
{

    Vector2 CheckpointPOS;
    private Rigidbody2D rb;
    private Animator anim;
    private ItemCollector itemCollector; // Declare itemCollector variable
    //private CheckpointManager checkpointManager; // Reference to the checkpoint manager

    [SerializeField] private AudioSource dedEffect;
    [SerializeField] private float respawnTime = 3f; // Adjust the respawn time as needed
    [SerializeField] private TMP_Text timerText; // Reference to the TextMeshPro text element for displaying the timer
    [SerializeField] private TMP_Text livesText; // Reference to the TextMeshPro text element for displaying the lives

    private float timer; // Variable to hold the countdown timer value
    private int lives = 10; // Starting number of lives
    private const string LivesKey = "PlayerLives"; // Key for PlayerPrefs

    private bool isRespawning = false; // Flag to prevent multiple respawns simultaneously

    private void Start()
    {

        CheckpointPOS = transform.position;  
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        itemCollector = FindObjectOfType<ItemCollector>(); // Find the ItemCollector script in the scene
       // checkpointManager = FindObjectOfType<CheckpointManager>(); // Find the CheckpointManager script in the scene

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
        // Only update the timer if respawning
        if (isRespawning)
        {
            // Decrease the timer value every frame while respawning
            timer -= Time.deltaTime;

            // Update the timer display
            UpdateTimerDisplay();

            // Check if the timer has reached zero
            if (timer <= 0)
            {
                // Reset player state after respawnTime seconds
                ResetPlayerState();
            }
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
        if (!isRespawning && (collision.gameObject.CompareTag("Trap") || collision.gameObject.CompareTag("Enemy")))
        {
            Die();
            //itemCollector.ReduceScoreOnDeath(); // Call ReduceScoreOnDeath method
        }
    }

    private void Die()
{
    if (dedEffect != null)
    {
        dedEffect.Play();
    }

    rb.bodyType = RigidbodyType2D.Static; // Temporarily set to Static to prevent physics interaction during respawn
    anim.SetTrigger("death");

    // Store the current position as respawn position
    //checkpointManager.SetRespawnPosition(transform.position);

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
        itemCollector.ResetScore(); // Reset the score
        Debug.Log("New score time");
    }
    else
    {
        // Reset player state after respawnTime seconds
        isRespawning = true;
        timer = respawnTime;
        Invoke("ResetPlayerState", respawnTime); // Call ResetPlayerState after respawnTime seconds
    }
}


    private void ResetPlayerState()
{
    // Reset player state here
    rb.bodyType = RigidbodyType2D.Dynamic;
    anim.ResetTrigger("death");
    anim.Play("Idle"); // Assuming "Idle" is the name of the idle animation

    // Respawn at the saved position
    //transform.position = checkpointManager.GetRespawnPosition(); 

    //Debug.Log("Player respawned at: " + checkpointManager.GetRespawnPosition());

    isRespawning = false; // Reset the respawn flag
}


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            // Update the respawn position to the checkpoint position
            //checkpointManager.SetRespawnPosition(other.transform.position);
           // Debug.Log("Checkpoint reached. Respawn position updated to: " + checkpointManager.GetRespawnPosition());
        }
    }

    public void ResetLives()
    {
        lives = 10; // Reset lives to starting value
        UpdateLivesDisplay(); // Update lives display
        PlayerPrefs.SetInt(LivesKey, lives); // Save the reset lives count
    }
}

using UnityEngine;
using UnityEngine.UI; // Need to include this for UI elements
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private AudioSource dedEffect;
    [SerializeField] private float respawnTime = 300f; // Adjust the respawn time as needed
    [SerializeField] private TMP_Text timerText; // Reference to the TextMeshPro text element for displaying the timer

    private float timer; // Variable to hold the countdown timer value

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        // Start the countdown timer
        timer = respawnTime;
        UpdateTimerDisplay();
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
            timerText.text =  Mathf.RoundToInt(timer).ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap") || collision.gameObject.CompareTag("Enemy"))
        {
            Die();
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

        // Start the countdown timer again after death
        timer = respawnTime;
    }

    private void Restartlevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

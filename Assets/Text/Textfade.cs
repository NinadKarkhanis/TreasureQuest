using UnityEngine;
using TMPro;

public class FadeTextOnTrigger : MonoBehaviour
{
    public TextMeshProUGUI textMeshProText; // Reference to the TextMeshPro text element

    public float fadeDuration = 1.0f; // Duration of the fade effect in seconds
    public AnimationCurve fadeCurve; // Curve to control the fade effect

    private bool isTriggered = false; // Flag to track if the trigger has been activated
    private float timer = 0f; // Timer to track the elapsed time during the fade

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the trigger has been activated and if the collider is tagged as the trigger
        if (!isTriggered && other.CompareTag("Player"))
        {
            Debug.Log("Trigger entered.");
            isTriggered = true;
            timer = 0f;
        }
    }

    private void Update()
    {
        // If the trigger has been activated, start the fade effect
        if (isTriggered)
        {
            // Increment the timer
            timer += Time.deltaTime;

            // Calculate the alpha value based on the curve
            float alpha = fadeCurve.Evaluate(timer / fadeDuration);

            // Update the alpha value of the TextMeshPro text element
            Color textColor = textMeshProText.color;
            textColor.a = alpha;
            textMeshProText.color = textColor;

            // Check if the fade effect has finished
            if (timer >= fadeDuration)
            {
                Debug.Log("Fade completed.");
                // Disable the trigger and reset the timer
                isTriggered = false;
                timer = 0f;
            }
        }
    }
}

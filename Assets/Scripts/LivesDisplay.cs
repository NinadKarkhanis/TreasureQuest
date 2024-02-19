using UnityEngine;
using TMPro;

public class LivesDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        // Get the TextMeshPro component from the children of the GameObject
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

        // Update the initial text
        UpdateLivesDisplay(0); // Update with initial value of 0 or whatever initial lives count you prefer
    }

    // Update the text to display the remaining lives count
    public void UpdateLivesDisplay(int remainingLives)
    {
        textMeshPro.text = "Lives: " + remainingLives.ToString();
    }
}

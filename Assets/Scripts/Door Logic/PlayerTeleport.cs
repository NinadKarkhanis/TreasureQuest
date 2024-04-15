using UnityEngine;
using UnityEngine.UI;

public class PlayerTeleport : MonoBehaviour
{
    private GameObject currentTeleporter;
    public Button teleportButton; // Reference to your UI Button

    void Start()
    {
        // Assuming you've assigned the Button reference in the Inspector, you can add the onClick event listener here
        if (teleportButton != null)
        {
            teleportButton.onClick.AddListener(TeleportPlayer);
        }
    }

    void Update()
    {
        // Check for keyboard input (E key) to teleport
        if (Input.GetKeyDown(KeyCode.E))
        {
            TeleportPlayer();
        }
    }

    public void TeleportPlayer()
    {
        if (currentTeleporter != null)
        {
            transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            currentTeleporter = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            if (collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
    }
}

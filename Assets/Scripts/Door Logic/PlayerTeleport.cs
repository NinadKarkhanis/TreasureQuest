using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerTeleport : MonoBehaviour
{
    private GameObject currentTeleporter;
    [SerializeField] private Button teleportButton;

    private void OnEnable()
    {
        if (teleportButton != null) teleportButton.onClick.AddListener(TeleportPlayer);
    }
    private void OnDisable()
    {
        if (teleportButton != null) teleportButton.onClick.RemoveListener(TeleportPlayer);
    }
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            TeleportPlayer();
    }
    public void TeleportPlayer()
    {
        if (currentTeleporter == null) return;
        Teleporter teleporter = currentTeleporter.GetComponent<Teleporter>();
        if (teleporter == null || teleporter.GetDestination() == null) return;
        transform.position = teleporter.GetDestination().position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Teleporter")) currentTeleporter = other.gameObject;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentTeleporter) currentTeleporter = null;
    }
}

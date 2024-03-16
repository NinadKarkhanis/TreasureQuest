using UnityEngine;

public class DisablePanelOnTrigger : MonoBehaviour
{
    public GameObject panelObject; // Reference to the GameObject representing the panel

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            panelObject.SetActive(false); // Disable the panel
        }
    }
}

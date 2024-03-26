using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    public GameObject Gate;
    public GameObject Button;
    public GameObject ButtonPressed; // Reference to the GameObject representing the panel

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Gate.SetActive(false); // Disable the panel
            Button.SetActive(false);
            ButtonPressed.SetActive(true);
        }
    }
}

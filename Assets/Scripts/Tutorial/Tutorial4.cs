using UnityEngine;
using UnityEngine.UI;

public class Tutorial4 : MonoBehaviour
{

    public GameObject Tutorial;
    
    public GameObject Tutnew;
    public GameObject Square;


    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if colliding with object tagged as 'tut1'
        if (other.CompareTag("Player"))
        {
            // Enable the button and disable the GameObject
           Tutorial.SetActive(false);
           Tutnew.SetActive(true);
           Square.SetActive(false);
        }
    }

}

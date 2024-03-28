using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject jumpbutton;
    public GameObject Rightbutton;
    public GameObject Leftbutton;

    public GameObject Pausebutton;

    public GameObject tut1;
    public GameObject tut2;
    public GameObject Square;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if colliding with object tagged as 'tut1'
        if (other.CompareTag("Player"))
        {
            // Enable the button and disable the GameObject
            Leftbutton.SetActive(true);
            Square.SetActive(true);
            tut1.SetActive(false);
            tut2.SetActive(true);
        }
    }

}

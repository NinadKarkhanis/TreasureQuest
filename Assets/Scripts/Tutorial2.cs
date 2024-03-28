using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : MonoBehaviour
{
    public GameObject jumpbutton;
    public GameObject Rightbutton;
    public GameObject Leftbutton;

    public GameObject Pausebutton;

    public GameObject tut2;
    public GameObject tut3;
    public GameObject tut4;
    public GameObject Square;
    public GameObject Square2;
    public GameObject gate;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if colliding with object tagged as 'tut1'
        if (other.CompareTag("Player"))
        {
            // Enable the button and disable the GameObject
           tut4.SetActive(true);
           tut3.SetActive(false);
           Pausebutton.SetActive(true);
           Square.SetActive(false);
           Square2.SetActive(false);
           gate.SetActive(false);
        }
    }

}

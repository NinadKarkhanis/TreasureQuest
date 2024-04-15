using UnityEngine;
using UnityEngine.UI;

public class Tutorial1 : MonoBehaviour
{
    public GameObject jumpbutton;
    public GameObject Rightbutton;
    public GameObject Leftbutton;

    public GameObject Pausebutton;

    public GameObject tut2;
    public GameObject tut3;
    public GameObject Square;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if colliding with object tagged as 'tut1'
        if (other.CompareTag("Player"))
        {
            // Enable the button and disable the GameObject
            jumpbutton.SetActive(true);
            tut2.SetActive(false);
            tut3.SetActive(true);
            Square.SetActive(true);
            
        }
    }

}

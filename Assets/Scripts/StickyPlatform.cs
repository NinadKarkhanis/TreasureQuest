using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{

    [SerializeField] private Vector3 scale = new Vector3(1.500174f, 0.703326f, 0.78f);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name =="Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    } 

    void Update()
    {
        transform.localScale = scale;
    }


}

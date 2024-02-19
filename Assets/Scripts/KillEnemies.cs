using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemies : MonoBehaviour
{

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("WeakPoint"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Enemy Killed" );
            
        }
    }
}

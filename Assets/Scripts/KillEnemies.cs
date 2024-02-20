using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillEnemies : MonoBehaviour
{
    private int enemy = 0;

    [SerializeField] private TMP_Text enemyText;
    [SerializeField] private AudioSource enemyded;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("WeakPoint"))
        {
            enemyded.Play();
            Destroy(collision.gameObject);
            enemy++;
            Debug.Log("Enemy Killed" );
            enemyText.text = enemy.ToString(); // Convert integer to string before assigning
            
        }
    }
}

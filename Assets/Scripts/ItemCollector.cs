using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int coin = 0;
    private int enemy = 0;
    private int score = 0;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private AudioSource collectEffect;
    [SerializeField] private AudioSource enemyded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            collectEffect.Play();
            Destroy(collision.gameObject);
            coin ++;
            Debug.Log("Coin:" + coin);
            UpdateScore();
            
        }
        else if(collision.gameObject.CompareTag("WeakPoint"))
        {
            enemyded.Play();
            Destroy(collision.gameObject);
            enemy++;
            Debug.Log("Enemy Killed" );
            UpdateScore();
        }
    }

    private void UpdateScore()
    {
        score = coin * 10 + enemy * 100; // Example scoring logic
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
}


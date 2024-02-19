using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int coin = 0;

    [SerializeField] private TMP_Text coinText;
    [SerializeField] private AudioSource collectEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            collectEffect.Play();
            Destroy(collision.gameObject);
            coin++;  //coin = coin + 1;
            Debug.Log("Coin:" + coin);
            coinText.text = coin.ToString(); // Convert integer to string before assigning
        }
    }
}


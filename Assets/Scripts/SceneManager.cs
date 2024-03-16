using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level1 : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject MobileUI;
    public GameObject Leaderboardscore;
    public GameObject MoreLeaderboard;
    ItemCollector itemCollector;

    public void Start()
    {
        itemCollector = FindObjectOfType<ItemCollector>();
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene(5);
        Time.timeScale=1; 
    }

    public void GameStart()
    {
        itemCollector.ResetScore();
        SceneManager.LoadScene(8); 
        Time.timeScale=1;
    }

    public void Timeboard()
    {
        SceneManager.LoadScene(7);
        Time.timeScale=1; 
    }

    public void LevelOne()
    {
        SceneManager.LoadScene(2);
        Time.timeScale=1; 
    }

    public void LevelTwo()
    {
        SceneManager.LoadScene(3);
        Time.timeScale=1; 
    }

    public void LevelThree()
    {
        SceneManager.LoadScene(4);
        Time.timeScale=1; 
    }

    public void LevelSelection()
    {
        SceneManager.LoadScene(1); 
        Time.timeScale=1;
    }

    public void StartScreen()
    {
        SceneManager.LoadScene(0); 
        Time.timeScale=1;
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        MobileUI.SetActive(false);
        Time.timeScale=0;
    }

    public void MoreScore()
    {
        Leaderboardscore.SetActive(false);
        MoreLeaderboard.SetActive(true);
    }

    public void NoMoreScore()
    {
        Leaderboardscore.SetActive(true);
        MoreLeaderboard.SetActive(false);
    }


    public void Continue()
    {
        PausePanel.SetActive(false);
        MobileUI.SetActive(true);
        Time.timeScale=1;
    }
} 
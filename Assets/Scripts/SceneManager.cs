using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level1 : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject MobileUI;


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

    public void Continue()
    {
        PausePanel.SetActive(false);
        MobileUI.SetActive(true);
        Time.timeScale=1;
    }
} 
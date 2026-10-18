using UnityEngine;
using UnityEngine.SceneManagement;

// Legacy class name is retained so existing Unity scene bindings remain intact.
public class level1 : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject MobileUI;
    public GameObject Leaderboardscore;
    public GameObject MoreLeaderboard;

    public void Leaderboard() { Load("Leaderboard"); }
    public void cutscene() { Load("VideoStart"); }
    public void GameStart() { Load("CompleteLevel"); }
    public void Timeboard() { Load("TimeBoard"); }
    public void StartScreen() { Load("Start_Screen"); }
    public void TutorialScreen() { Load("Tutorial"); }

    private static void Load(string scene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }

    public void Pause()
    {
        if (PausePanel != null) PausePanel.SetActive(true);
        if (MobileUI != null) MobileUI.SetActive(false);
        Time.timeScale = 0f;
    }
    public void Continue()
    {
        if (PausePanel != null) PausePanel.SetActive(false);
        if (MobileUI != null) MobileUI.SetActive(true);
        Time.timeScale = 1f;
    }
    public void MoreScore()
    {
        if (Leaderboardscore != null) Leaderboardscore.SetActive(false);
        if (MoreLeaderboard != null) MoreLeaderboard.SetActive(true);
    }
    public void NoMoreScore()
    {
        if (Leaderboardscore != null) Leaderboardscore.SetActive(true);
        if (MoreLeaderboard != null) MoreLeaderboard.SetActive(false);
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inputScore;
    [SerializeField] private TMP_InputField inputName;
    public UnityEvent<string, int> submitScoreEvent;

    public void SubmitScore()
    {
        if (inputScore == null || inputName == null)
        {
            Debug.LogWarning("Leaderboard UI has not been configured.");
            return;
        }
        int score;
        if (!GameRules.ValidLeaderboardEntry(inputName.text, inputScore.text, out score))
        {
            Debug.LogWarning("Leaderboard needs a name of 2–18 characters and a valid nonnegative score.");
            return;
        }
        if (submitScoreEvent != null) submitScoreEvent.Invoke(inputName.text.Trim(), score);
    }
}

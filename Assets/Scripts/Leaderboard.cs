using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{

    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> scores;
    

    private string publicLeaderboardKey =
    "91858f230c299cc0503399b9d6597be1177dba82666641fee48362fd3fa8f014";

    public void Start(){
        GetLeaderboard();
    }

    public void GetLeaderboard(){
            LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg)=> {
                int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
                for (int i =0; i < loopLength; ++i){
                    names[i].text =msg[i].Username;
                    scores[i].text =msg[i].Score.ToString();
                }

            }));
    }

    public void SetLeaderboardEntry(string username,int score){
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey,username, score, ((msg)=>{
            GetLeaderboard();
        }));
    }

}

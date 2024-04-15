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
    "92460860617ed27b4e42d28fad9b38bc000438f65f4158a188ce3f321c277eb2";

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

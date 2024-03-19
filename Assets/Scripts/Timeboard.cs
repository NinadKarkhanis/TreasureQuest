using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Timeboard : MonoBehaviour
{

    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> times;
    

    private string publicLeaderboardKey =
    "1e11dd19cf130553c66f27e92c3e0edb16e3ed094c576c5237aab70c105b54dc";

    public void Start(){
        GetLeaderboard();
    }

    public void GetLeaderboard(){
            LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg)=> {
                int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
                for (int i =0; i < loopLength; ++i){
                    names[i].text =msg[i].Username;
                    times[i].text =msg[i].Score.ToString();
                }

            }));
    }

    public void SetLeaderboardEntry(string username,int score){
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey,username, score, ((msg)=>{
            GetLeaderboard();
        }));
    }

}
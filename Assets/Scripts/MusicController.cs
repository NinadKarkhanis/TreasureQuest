using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour

{

    public GameObject Bgmusic;
    private MusicController musicController;



    // Function to turn music on
    public void TurnMusicOn()
    {
       Bgmusic.SetActive(true);
    }

    // Function to turn music off
    public void TurnMusicOff()
    {
       Bgmusic.SetActive(false);
    }
}
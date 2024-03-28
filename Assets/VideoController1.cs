using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoController1 : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component

    void Start()
    {
        videoPlayer.loopPointReached += EndReached; // Subscribe to the video end event
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        // Load the next scene when the video ends
        SceneManager.LoadScene(0);
    }
}

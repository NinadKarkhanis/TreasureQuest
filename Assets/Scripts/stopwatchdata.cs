using UnityEngine;

public class StopwatchData : MonoBehaviour
{
    public float totalTime;

    public void SaveTime(float time)
    {
        totalTime = time;
    }
}

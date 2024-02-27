using UnityEngine;

public class KeepScreenOn : MonoBehaviour
{
    void Start()
    {
        // Prevent the screen from sleeping
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}

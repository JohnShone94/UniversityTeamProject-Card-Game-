using UnityEngine;
using System.Collections;

public class s_PauseMenu : MonoBehaviour
{
    bool paused = false;
    public void Paused()
    {
        if (paused)
        {
            Time.timeScale = 1;
            paused = false;
        }
        else
        {
            Time.timeScale = 0;
            paused = true;
        }

    }
}

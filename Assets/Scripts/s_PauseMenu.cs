using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_PauseMenu : MonoBehaviour
{
    public bool paused = false;


    void Start ()
    {
    }

    void Update()
    {
        if (paused == true)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

    }


    public void PauseGame()
    {
        if (paused == false)
        {
            paused = true;
        }
        else
        {
            paused = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_Scoreboard : MonoBehaviour
{
	public Text[] highScores;

	int [] highScoreValues;

	void Start ()
    {
		highScoreValues = new int[highScores.Length];

		for (int x = 0; x < highScores.Length; x++)
        {
			highScoreValues [x] = PlayerPrefs.GetInt ("highScoreValues" + x);
		
		}
		drawScores ();
	}

	void SaveScores()
    {
		for (int x = 0; x < highScores.Length; x++)
        {
			PlayerPrefs.SetInt ("highScoreValues" + x,highScoreValues [x]);
		}


	}

	public void checkForHighScore(int _value)
    {
		for (int x = 0; x < highScores.Length; x++)
        {
			if (_value > highScoreValues [x])
            {
				for (int y = highScores.Length - 1; y > x; y--)
                {
					highScoreValues [y] = highScoreValues [y - 1];

				}

				highScoreValues [x] = _value;
				drawScores ();
				SaveScores ();
				break;
			}
		
		}

	}

	void drawScores()
    {
		for (int x = 0; x < highScores.Length; x++)
        {
			highScores [x].text = highScoreValues [x].ToString ();
		}


	}
	// Update is called once per frame
	void Update () {
		
	}
}

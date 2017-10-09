using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_QuitOnClick : MonoBehaviour {

	public void Quit()
	{
        Application.LoadLevel(0);
	}
}

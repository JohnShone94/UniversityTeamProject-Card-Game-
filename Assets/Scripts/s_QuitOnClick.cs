using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_QuitOnClick : MonoBehaviour {

	public void Quit()
	{
		UnityEditor.EditorApplication.isPlaying = false;
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class s_LoadSceneOnClick : MonoBehaviour {

	public void LoadByIndex(int sceneIndex)
	{
		SceneManager.LoadScene (sceneIndex);
	}
}

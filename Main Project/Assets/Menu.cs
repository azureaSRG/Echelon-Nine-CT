using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void pressedPlayButton()
	{
		SceneManager.LoadScene(1);
	}
	
	public void pressedQuitButton()
	{
		Debug.Log("Game Closed");
		Application.Quit();
	}
}

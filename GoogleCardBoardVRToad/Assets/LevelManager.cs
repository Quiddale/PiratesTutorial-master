using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {

	public void LoadnextScene()

	{
	int currentIndex = SceneManager.GetActiveScene().buildIndex;
	SceneManager.LoadScene(currentIndex + 1);
	}

	public void Quit()
	{
		print("Game will quiet.");
		Application.Quit();
	}
}

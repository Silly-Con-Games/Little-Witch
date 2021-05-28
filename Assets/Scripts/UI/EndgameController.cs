using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameController : MonoBehaviour
{
	public PauseController pauseController;

	public GameObject winMenu;
	public GameObject loseMenu;

    public void Win() {
		pauseController.PauseGame();
		winMenu.SetActive(true);
	}

	public void Lose() {
		pauseController.PauseGame();
		loseMenu.SetActive(true);
	}

	public void Restart() {
		pauseController.UnpauseGame();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	public void ReturnToMain() {
		pauseController.QuitGame();
	}
}

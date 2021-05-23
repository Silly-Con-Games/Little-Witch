using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
	public GameObject menu;

	private bool paused = false;

	public bool IsPaused() {
		return paused;
	}

	public void PauseGame() {
		if (paused) {
			ResumeGame();
		} else {
			Time.timeScale = 0f;
			menu.SetActive(true);
			paused = true;
		}
	}

	public void ResumeGame() {
		Time.timeScale = 1f;
		menu.SetActive(false);
		paused = false;
	}

	public void QuitGame() {
		SceneManager.LoadScene(0);
	}
}

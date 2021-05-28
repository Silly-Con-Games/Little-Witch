using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
	public GameObject menu;
	public GameObject controls;

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

	public void UnpauseGame() {
		Time.timeScale = 1f;
		paused = false;
	}

	public void ResumeGame() {
		UnpauseGame();
		menu.SetActive(false);
	}

	public void ShowControls() {
		controls.SetActive(true);
	}

	public void QuitGame() {
		UnpauseGame();
		SceneManager.LoadScene(0);
	}
}

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
	private bool pauseLocked = false;

	public bool IsPaused() {
		return paused;
	}

	public bool CanResume() {
		return !pauseLocked;
	}

	public void LockPause() {
		pauseLocked = true;
	}

	public void UnlockPause() {
		pauseLocked = false;
	}
 
	public void PauseGame() {
		Time.timeScale = 0f;
		menu.SetActive(true);
		paused = true;
	}

	public void UnpauseGame() {
		Time.timeScale = 1f;
		paused = false;
	}

	public void ResumeGame() {
		if (!pauseLocked) {
			UnpauseGame();
			menu.SetActive(false);
		}
	}

	public void ShowControls() {
		LockPause();
		controls.SetActive(true);
	}

	public void QuitGame() {
		UnpauseGame();
		SceneManager.LoadScene(0);
	}
}

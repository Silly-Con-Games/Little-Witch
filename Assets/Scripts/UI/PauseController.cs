using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
	public GameObject menu;
	public GameObject controls;

	public Button firstButton;
	private Button lastSelectedButton;

	[HideInInspector] public PlayerController playerController;

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
		if (playerController.gamepadActive) lastSelectedButton.Select();
	}
 
	public void PauseGame() {
		Time.timeScale = 0f;
		menu.SetActive(true);
		paused = true;
		playerController.canBeControlled = false;
		if (playerController.gamepadActive) firstButton.Select();
	}

	public void UnpauseGame() {
		Time.timeScale = 1f;
		paused = false;
	}

	public void ResumeGame() {
		if (!pauseLocked) {
			UnpauseGame();
			menu.SetActive(false);
			playerController.canBeControlled = true;
		}
	}

	public void ShowControls() {
		LockPause();
		lastSelectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
		controls.SetActive(true);
	}

	public void QuitGame() {
		UnpauseGame();
		GameController.GameState = EGameState.GameOver;
		SceneManager.LoadScene(0);
	}
}

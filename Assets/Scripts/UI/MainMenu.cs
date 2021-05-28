using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject controls;
	public GameObject settins;
	public GameObject credits;

    public void StartGame() {
		SceneManager.LoadScene(1);
	}

	public void ShowControls() {
		controls.SetActive(true);
	}

	public void ShowSettings() {
		settins.SetActive(true);
	}

	public void ShowCredits() {
		credits.SetActive(true);
	}

	public void QuitGame() {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}

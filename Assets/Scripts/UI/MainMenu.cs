using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public GameObject controls;
	public GameObject settins;
	public GameObject credits;

	public Button continueButton;

	void Start() {
		if (!File.Exists(Application.persistentDataPath + "/gamesave.save")) {
			continueButton.interactable = false;
		}
	}

	public void NewGame() {
		string filepath = Application.persistentDataPath + "/gamesave.save";
		if (File.Exists(filepath)) {
			File.Delete(filepath);
		}
		SceneManager.LoadScene(1);
	}

	public void ContinueGame() {
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

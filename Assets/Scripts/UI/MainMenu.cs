using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public GameObject mainMenu;

	public GameObject enterName;
	public GameObject controls;
	public GameObject controlsBeforeStart;
	public GameObject settings;
	public GameObject credits;
	public GameObject beforeQuit;

	public Button continueToMainButton;
	public Button continueButton;

	public TMPro.TextMeshProUGUI playerNameText;

	void Start() {
		if (!File.Exists(Application.persistentDataPath + "/gamesave.save")) {
			continueButton.interactable = false;
		}
        if (PlayerPrefs.HasKey("player_name")) {
			Debug.Log("player name exists");
			ShowMainMenu();
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

	public void ShowMainMenu()
	{
		mainMenu.SetActive(true);
		enterName.SetActive(false);
	}

	public void EnableContinueToMain(TMPro.TMP_InputField playerName)
	{
		if (string.IsNullOrEmpty(playerName.text))
		{
			Debug.Log("no name entered");
			return;
		}
		PlayerPrefs.SetString("player_name", playerName.text);
		Debug.Log("player name saved");
		PlayerPrefs.Save();
		continueToMainButton.interactable = true;
    }

	public void ShowControls() {
		controls.SetActive(true);
	}

	public void ShowControlsBeforeStart()
	{
		controlsBeforeStart.SetActive(true);
	}

	public void ShowSettings() {
		settings.SetActive(true);
	}

	public void ShowCredits() {
		credits.SetActive(true);
	}

	public void ShowBeforeQuit()
	{
		if (!PlayerPrefs.HasKey("player_name"))
		{
			// this should never happen
			QuitGame();
		}
		playerNameText.text = PlayerPrefs.GetString("player_name");
		mainMenu.SetActive(false);
		beforeQuit.SetActive(true);
	}

	public void QuitGame() {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
	public GameObject mainMenu;

	public PopupMenu enterName;
	public PopupMenu controls;
	public PopupMenu controlsBeforeStart;
	public PopupMenu settings;
	public PopupMenu credits;
	public PopupMenu beforeQuit;
	public HighlightController controlsTutorial;

	public Button firstButton;
	public Button continueToMainButton;
	public Button continueButton;
	private Button lastSelectedButton;

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

	public void TutorialScene()
    {
		SceneManager.LoadScene("TutorialScene");
    }

	public void ShowMainMenu()
	{
		mainMenu.SetActive(true);
		enterName.gameObject.SetActive(false);

		if (Gamepad.current != null) firstButton.Select();
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
		controls.Open(EventSystem.current.currentSelectedGameObject);
	}

	public void ShowControlsBeforeStart()
	{
		controlsBeforeStart.Open(EventSystem.current.currentSelectedGameObject);
		controlsTutorial.StartTutorial();
	}

	public void ShowSettings() {
		settings.Open(EventSystem.current.currentSelectedGameObject);
	}

	public void ShowCredits() {
		credits.Open(EventSystem.current.currentSelectedGameObject);
	}

	// for questionnaire purposes
	public void ShowBeforeQuit()
	{
		QuitGame();

		return;

		if (!PlayerPrefs.HasKey("player_name"))
		{
			// this should never happen
			QuitGame();
		}
		playerNameText.text = PlayerPrefs.GetString("player_name");
		mainMenu.SetActive(false);
		beforeQuit.Open(EventSystem.current.currentSelectedGameObject);
	}

	public void QuitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}

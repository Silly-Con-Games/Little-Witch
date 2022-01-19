using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
	public GameObject mainMenu;

	public GameObject enterName;
	public GameObject controls;
	public GameObject controlsBeforeStart;
	public GameObject settings;
	public GameObject credits;
	public GameObject beforeQuit;
	public HighlightController controlsTutorial;

	public Button continueToMainButton;
	public Button continueButton;

	public TMPro.TextMeshProUGUI playerNameText;

	[SerializeField] private GameObject[] allPopUpMenus;
	private GameObject currentOpenMenu = null;

	private bool gamepadActive = false;

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
		if (gamepadActive) SelectFirstButton(mainMenu);
        foreach (var menu in allPopUpMenus)
		{
			menu.SetActive(false);
		}
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

	public void ShowControls()
	{
		controls.SetActive(true);
		if (gamepadActive) SelectFirstButton(controls);
	}

	public void ShowControlsBeforeStart()
	{
		controlsBeforeStart.SetActive(true);
		controlsTutorial.StartTutorial();
	}

	public void ShowSettings()
	{
		settings.SetActive(true);
		if (gamepadActive) SelectFirstButton(settings);
	}

	public void ShowCredits() {
		credits.SetActive(true);
		if (gamepadActive) SelectFirstButton(credits);
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

	private void OnCancel(InputValue inputValue)
	{
		ShowMainMenu();
	}

	private void SelectFirstButton(GameObject menu)
    {
		menu.GetComponentInChildren<Button>().Select();
	}

	public void OnControlsChanged(PlayerInput pi)
	{
		Debug.Log("controls changed");
		gamepadActive = pi.currentControlScheme.Equals("Gamepad");
		if (gamepadActive) SelectFirstButton(mainMenu);
	}
}

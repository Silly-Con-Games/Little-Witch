using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class EndgameController : MonoBehaviour
{
	public PauseController pauseController;

	public GameObject winMenu;
	public GameObject loseMenu;
	public TextMeshProUGUI text;
    public void Win() {
		pauseController.PauseGame();
		pauseController.LockPause();
		winMenu.SetActive(true);
	}

	public void Lose() {
		var enController = FindObjectOfType<EnemiesController>();
		if (enController != null)
		{
			int wavecnt = enController.GetWaveCounter() - 1;
			int remaining = enController.GetRemainingWaves() + 1;
			string word = "wave";
			if (wavecnt > 1)
				word += "s";

			text.text = $"Congratulations, You have beaten {wavecnt} {word}! {remaining} more to go. Try again from last Save point?";
		}
		pauseController.PauseGame();
		pauseController.LockPause();
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

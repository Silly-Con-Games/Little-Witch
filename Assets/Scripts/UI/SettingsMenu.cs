using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class SettingsMenu : PopupMenu
{
	public Slider musicSlider;
	public Slider soundsSlider;
	
	public MainMenuSoundController controller;

	void Start() {
		musicSlider.value = PlayerPrefs.GetFloat("music_volume", 1f) * 10f;
		soundsSlider.value = PlayerPrefs.GetFloat("sounds_volume", 1f) * 10f;
	}

	public new void Close() {
		PlayerPrefs.Save();
		base.Close();
	}

	public void OnMusicChange() {
		PlayerPrefs.SetFloat("music_volume", musicSlider.value / 10f);
		controller.SetMusicVolume(musicSlider.value / 10f);
	}

	public void OnSoundsChange() {
		PlayerPrefs.SetFloat("sounds_volume", soundsSlider.value / 10f);
		controller.SetSoundsVolume(soundsSlider.value / 10f);
		RuntimeManager.PlayOneShot("event:/menu_click");
	}
}

using UnityEngine;
using FMOD.Studio;
using Config;

public class MainMenuSoundController : MonoBehaviour
{
	public AudioSource musicSource;

	private SoundConfig soundConfig;
	private VCA soundsVCA;

	void Start() {
		soundConfig = GlobalConfigManager.GetGlobalConfig().soundConfig;
		soundsVCA = FMODUnity.RuntimeManager.GetVCA("vca:/GameVCA");

		SetMusicVolume(PlayerPrefs.GetFloat("music_volume", 1f));
		SetSoundsVolume(PlayerPrefs.GetFloat("sounds_volume", 1f));
	}

	public void SetMusicVolume(float multiplier) {
		musicSource.volume = multiplier * soundConfig.musicVolume;
	}

	public void SetSoundsVolume(float multiplier) {
		soundsVCA.setVolume(multiplier * soundConfig.sfxVolume);
	}
}

using System.Collections;
using Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using Assets.Scripts.GameEvents;

public class GameController : MonoBehaviour
{
    public PlayerController witchPrefab;

    public CinemachineVirtualCamera cmCamera;

    public Transform spawnPoint;

    public HUDController hud;

	public PauseController pauseController;

	public EndgameController endgameController;

    public MapController mapController;

    public EnemiesController enemiesController;

    public MusicController musicController;

	public GameObject energySphere;

    public static EGameState GameState
    {
        get => internalGS;
        internal set
        {
            if(internalGS != value)
            {
               Debug.Log($"Changing gamestate from {internalGS} to {value}");
               GameEventQueue.QueueEvent(new GameStateChangedEvent(value, internalGS));
               internalGS = value;
               onGameStateChanged.Invoke(value);
            }
        }
    }

    private static EGameState internalGS = EGameState.GameOver;
    public static UnityEvent<EGameState> onGameStateChanged = new UnityEvent<EGameState>();

    private PlayerController currentWitch;
    private GlobalConfig conf;

	private Save saveToBeLoaded;

    // Start is called before the first frame update
    void Start()
    {
        GameState = EGameState.WaitingForNextWave;
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
        StartCoroutine(SpawnWithDelay(0.1f));

		saveToBeLoaded = null;
		LoadGame();

		enemiesController.onWaveEnd.AddListener(OnWaveEnd);
        StartCoroutine(WaitAndStartWave(enemiesController.GetCurrentPreperationTime()));

		hud.ShowGameGoal();
	}

    void ApplyConfig()
    {
        conf = GlobalConfigManager.GetGlobalConfig();
        FMOD.Studio.VCA vca = FMODUnity.RuntimeManager.GetVCA("vca:/GameVCA");
        vca.setVolume(PlayerPrefs.GetFloat("sounds_volume", 1f) * conf.soundConfig.sfxVolume);
        musicController.SetVolume(PlayerPrefs.GetFloat("music_volume", 1f) * conf.soundConfig.musicVolume);
    }

    private IEnumerator SpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentWitch = Instantiate(witchPrefab, spawnPoint.position, spawnPoint.rotation);
        currentWitch.onDeathEvent.AddListener(OnWitchDeath);
        currentWitch.mapController = mapController;
        currentWitch.hudController = hud;
		currentWitch.pauseController = pauseController;

		currentWitch.Initialize();

		if (saveToBeLoaded != null) {
			currentWitch.energy.Set(saveToBeLoaded.energy);
			currentWitch.health.Set(saveToBeLoaded.health);
			currentWitch.gameObject.transform.position = saveToBeLoaded.witchPosition.Get();
		}

        cmCamera.LookAt = currentWitch.transform;
        cmCamera.Follow = currentWitch.transform;
    }

	[Button("DeleteSave", "Delete save", false)] public string input1;
	public void DeleteSave() {
		string filename = Application.persistentDataPath + "/gamesave.save";
		if (File.Exists(filename)) {
			File.Delete(filename);
		}
	}

	private void SaveGame() {
		Save save = new Save();
		save.wave = EnemiesController.GetWaveCounter();
		save.energy = currentWitch.energy.Energy;
		save.health = currentWitch.health.Health;
		save.witchPosition = new SerializableVector(currentWitch.gameObject.transform.position + Vector3.up * 0.2f);
		save.tiles = mapController.GetTiles();
		List<SerializableVector> energySpheres = new List<SerializableVector>();
		foreach (Object o in FindObjectsOfType<Energy>()) {
			energySpheres.Add(new SerializableVector(((Energy)o).gameObject.transform.position));
		}
		save.energySpheres = energySpheres;

		string filename = Application.persistentDataPath + "/gamesave.save";
		if (File.Exists(filename)) {
			File.Delete(filename);
		}

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(filename);
		bf.Serialize(file, save);
		file.Close();
	}

	private void LoadGame() {
		string filename = Application.persistentDataPath + "/gamesave.save";
		if (File.Exists(filename)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(filename, FileMode.Open);
			Save save = null;

			try {
				save = (Save)bf.Deserialize(file);
			} catch (System.Exception e) {
				Debug.LogError("Exception during load deserialization, deleting save file.");
				file.Close();
				File.Delete(filename);
				return;
			}

			file.Close();

			enemiesController.SetWave(save.wave);
			mapController.SetTiles(save.tiles);

			foreach (SerializableVector v in save.energySpheres) {
				Instantiate(energySphere, v.Get(), Quaternion.identity);
			}
			saveToBeLoaded = save;
		}
	}

    private void OnWitchDeath()
    {
        currentWitch.onDeathEvent.RemoveListener(OnWitchDeath);
        currentWitch = null;
        StartCoroutine(WaitAndLoseCouroutine());
    }

    private void OnWaveEnd()
    {
        if (enemiesController.WasLastWave())
        {
            GameState = EGameState.GameWon;
            FMODUnity.RuntimeManager.PlayOneShot("event:/game/game_won");
			StartCoroutine(WaitAndWinCouroutine());
		}
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/game/wave_end");
            GameState = EGameState.WaitingForNextWave;
			SaveGame();
            StartCoroutine(WaitAndStartWave(enemiesController.GetCurrentPreperationTime()));
            hud.ShowWaveDefeated();
        }
    }

    private IEnumerator WaitAndStartWave(float duration)
    {
        StartCoroutine(hud.ShowTimeTillNextWave(duration, EnemiesController.GetWaveCounter()));
        yield return new WaitForSeconds(duration);
        GameState = EGameState.FightingWave;
        FMODUnity.RuntimeManager.PlayOneShot("event:/game/wave_start");
        enemiesController.SpawnNextWave();
    }

    private IEnumerator WaitAndLoseCouroutine()
    {
        GameState = EGameState.GameOver;
        yield return new WaitForSeconds(conf.respawnTime);

		endgameController.Lose();
    }

	private IEnumerator WaitAndWinCouroutine() {
		GameState = EGameState.GameWon;
		yield return new WaitForSeconds(conf.respawnTime);

		endgameController.Win();
	}
}

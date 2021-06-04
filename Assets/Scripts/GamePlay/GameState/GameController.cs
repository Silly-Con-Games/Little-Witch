using System.Collections;
using Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;

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

    public AudioSource audioSource;

    public static EGameState GameState 
    { 
        get => internalGS; 
        internal set 
        {
            if(internalGS != value)
            {
               Debug.Log($"Changing gamestate from {internalGS} to {value}");
               internalGS = value;
               onGameStateChanged.Invoke(value);
            }
        } 
    }

    private static EGameState internalGS = EGameState.WaitingForNextWave;
    public static UnityEvent<EGameState> onGameStateChanged = new UnityEvent<EGameState>();

    private PlayerController currentWitch;
    private GlobalConfig conf;

    // Start is called before the first frame update
    void Start()
    {

        internalGS = EGameState.WaitingForNextWave;
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
        StartCoroutine(SpawnWithDelay(0.1f));

        enemiesController.onWaveEnd.AddListener(OnWaveEnd);
        hud.ShowGameGoal();
        StartCoroutine(WaitAndStartWave(enemiesController.GetCurrentPreperationTime()));
    }

    void ApplyConfig()
    {
        conf = GlobalConfigManager.GetGlobalConfig();
        FMOD.Studio.VCA vca = FMODUnity.RuntimeManager.GetVCA("vca:/GameVCA"); 
        vca.setVolume(conf.soundConfig.sfxVolume);
        audioSource.volume = conf.soundConfig.musicVolume;
    }

    private IEnumerator SpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentWitch = Instantiate(witchPrefab, spawnPoint.position, spawnPoint.rotation);
        currentWitch.onDeathEvent.AddListener(OnWitchDeath);
        currentWitch.mapController = mapController;
        currentWitch.hudController = hud;
		currentWitch.pauseController = pauseController;

        cmCamera.LookAt = currentWitch.transform;
        cmCamera.Follow = currentWitch.transform;
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
            StartCoroutine(WaitAndStartWave(enemiesController.GetCurrentPreperationTime()));
            hud.ShowWaveDefeated();
        }
    }

    private IEnumerator WaitAndStartWave(float duration)
    {
        StartCoroutine(hud.ShowTimeTillNextWave(duration, enemiesController.GetWaveCounter()));
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
